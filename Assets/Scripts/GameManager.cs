using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MapArray
{
    public GameObject[] map;
    public void setArray(GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = map[i];
        }
    }
}

public class GameManager : MonoBehaviour
{
    public MapArray[] allCubes; // 0 시작 1 서 2 동 3 북 4 남 5 아래
    public GameObject[] currentCubes = new GameObject[16];
    public AudioClip[] musics; // 0은 항상 플레이됨, 1~4 중 하나와 5~8 중 하나가 플레이됨
    public Transform[] planePositions = new Transform[16];
    public GameObject player;
    private AudioSource audioSource;
    public AudioSource[] tmp;
    public CamMovement camMovement;

    private int currentPlayerIndex;
    private int[][] cubeMusicIndices;
    private int nextPlayerIndex;
    private bool isChoosingDirection = false;

    public GameObject BulletDodgeGamePrefab; // 피하기 게임 프리팹

    private Games currentGame;

    void Start()
    {
        allCubes[0].setArray(currentCubes);
        audioSource = GetComponent<AudioSource>();
        cubeMusicIndices = new int[16][]; // 각 큐브 위치별 음악 조합 배열
        InitializeCubeMusicIndices(); // 큐브 위치별 음악 조합을 초기화
        currentPlayerIndex = 0;
        nextPlayerIndex = currentPlayerIndex;
        StartGame();
    }

    void InitializeCubeMusicIndices()
    {
        for (int i = 0; i < 16; i++)
        {
            cubeMusicIndices[i] = new int[] { 0, Random.Range(1, 5), Random.Range(5, 9) };
        }
    }

    void StartGame()
    {
        PlayMusic(cubeMusicIndices[currentPlayerIndex]);
        SetPlayerPosition(currentPlayerIndex);

        StartCoroutine(GameRoutine());
    }

    void PlayMusic(int[] indices)
    {
        audioSource.Stop();
        audioSource.clip = musics[indices[0]];
        audioSource.Play();
        audioSource.volume = 0.1f;

        tmp[0].Stop();
        tmp[0].clip = musics[indices[1]];
        tmp[0].Play();
        tmp[0].volume = 0.1f;

        tmp[1].Stop();
        tmp[1].clip = musics[indices[2]];
        tmp[1].Play();
        tmp[1].volume = 0.1f;
    }

    void SetPlayerPosition(int index)
    {
        player.transform.position = planePositions[index].position;
        player.GetComponent<Player>().currentCube = currentCubes[index];

        camMovement.targetPosition = currentCubes[index].transform;
        camMovement.StartMoving();

        // 이전에 실행 중이던 게임을 중지하고 제거
        if (currentGame != null)
        {
            currentGame.GameStop();
            Destroy(currentGame.gameObject);
        }

        // 새로운 게임을 현재 큐브에 인스턴스화하고 시작
        GameObject gameInstance = Instantiate(BulletDodgeGamePrefab);
        gameInstance.transform.localScale = Vector3.one;
        gameInstance.transform.position = new Vector3(planePositions[index].position.x, 0, planePositions[index].position.z);
        gameInstance.transform.parent = this.transform;
        currentGame = gameInstance.GetComponent<Games>();
        currentGame.GameStart();
    }

    int[] GetAdjacent(int index)
    {
        List<int> adjacent = new List<int>();

        int row = index / 4;
        int col = index % 4;

        if (row > 0) adjacent.Add(index - 4); // 위쪽
        if (row < 3) adjacent.Add(index + 4); // 아래쪽
        if (col > 0) adjacent.Add(index - 1); // 왼쪽
        if (col < 3) adjacent.Add(index + 1); // 오른쪽

        return adjacent.ToArray();
    }

    IEnumerator GameRoutine()
    {
        while (true)
        {
            // 6.666초 동안 게임 진행
            yield return new WaitForSeconds(6.666f);

            if (currentGame != null)
            {
                currentGame.GameStop();
            }

            // 2.333초 동안 방향키 입력 대기
            isChoosingDirection = true;
            float elapsedTime = 0f;
            while (elapsedTime < 2.333f)
            {
                HandleDirectionInput();
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            isChoosingDirection = false;

            // 1초 동안 플레이어 위치 변경
            yield return new WaitForSeconds(1f);
            currentPlayerIndex = nextPlayerIndex;
            SetPlayerPosition(currentPlayerIndex);

            // 음악 재생
            PlayMusic(cubeMusicIndices[currentPlayerIndex]);

        }
    }

    void HandleDirectionInput()
    {
        if (!isChoosingDirection) return;

        int[] adjacent = GetAdjacent(currentPlayerIndex);

        if (Input.GetKeyDown(KeyCode.UpArrow) && adjacent.Contains(currentPlayerIndex - 4))
        {
            nextPlayerIndex = currentPlayerIndex - 4;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && adjacent.Contains(currentPlayerIndex + 4))
        {
            nextPlayerIndex = currentPlayerIndex + 4;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && adjacent.Contains(currentPlayerIndex - 1))
        {
            nextPlayerIndex = currentPlayerIndex - 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && adjacent.Contains(currentPlayerIndex + 1))
        {
            nextPlayerIndex = currentPlayerIndex + 1;
        }
    }
}
