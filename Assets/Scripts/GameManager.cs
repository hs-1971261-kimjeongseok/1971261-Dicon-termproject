using System;
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
    public GameObject CubeMap; // 전체 큐브들의 중심 위치
    public MapArray[] allCubes; // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
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
    public bool shouldRotateCube = false; // 큐브 회전 여부

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
            cubeMusicIndices[i] = new int[] { 0, UnityEngine.Random.Range(1, 5), UnityEngine.Random.Range(5, 9) };
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
        player.GetComponent<Player>().currentPosition = planePositions[index];

        if (!shouldRotateCube)
        {
            camMovement.targetPosition = planePositions[index];
            camMovement.StartMoving(false);
        }
        

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

    IEnumerator RotateCubeRoutine(Vector3 targetRotation)
    {
        camMovement.gameObject.transform.parent = CubeMap.transform;
        player.gameObject.transform.parent = CubeMap.transform;
        RotateAroundPivot cubeRotate = CubeMap.GetComponent<RotateAroundPivot>();

        Quaternion startRotation = CubeMap.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float duration = 0.5f;
        float elapsed = 0f;

        cubeRotate.rotationAngle = targetRotation;
        cubeRotate.StartRotation();

        while (elapsed < duration && !cubeRotate.rotateEnd)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        CubeMap.transform.rotation = Quaternion.Euler(0,0,0);

        camMovement.gameObject.transform.parent = null;
        player.gameObject.transform.parent= null;
        player.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        currentPlayerIndex = nextPlayerIndex;
        camMovement.targetPosition = planePositions[currentPlayerIndex];
        camMovement.StartMoving(true);
    }



    int nextSide = 0;
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

            player.GetComponent<Player>().canMove = false;
            
            if (shouldRotateCube)
            {
                // 0.5초 동안 카메라 움직임
                yield return new WaitForSeconds(0.5f);
                

                // 큐브 회전
                Vector3 targetRotation = GetCubeRotation();
                yield return RotateCubeRoutine(targetRotation);
                //allCubes[nextSide].setArray(currentCubes);
                


                // 0.5초 동안 플레이어 위치 변경
                yield return new WaitForSeconds(0.5f);
                SetPlayerPosition(currentPlayerIndex);
                
            }
            else
            {
                // 1초 동안 플레이어 위치 변경
                yield return new WaitForSeconds(1f);
                currentPlayerIndex = nextPlayerIndex;
                camMovement.targetPosition = planePositions[currentPlayerIndex];
                camMovement.StartMoving(false);
                SetPlayerPosition(currentPlayerIndex);
                
            }

            player.GetComponent<Player>().canMove = true;


            // 음악 재생
            PlayMusic(cubeMusicIndices[currentPlayerIndex]);
            shouldRotateCube = false; // 큐브 회전 플래그 초기화
        }
    }

    Vector3 GetCubeRotation()
    {
        // 새로운 면으로 큐브 전체 회전을 설정합니다.
        Vector3 targetRotation = Vector3.zero;

        if (nextPlayerIndex == currentPlayerIndex + 12) // 위에서 아래로 이동
        {
            // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
            targetRotation = new Vector3(90, 0, 0); // 남쪽 면으로 회전
        }
        else if (nextPlayerIndex == currentPlayerIndex - 12) // 아래에서 위로 이동
        {
            targetRotation = new Vector3(-90, 0, 0); // 북쪽 면으로 회전
        }
        else if (nextPlayerIndex == currentPlayerIndex + 3) // 왼쪽에서 오른쪽으로 이동
        {
            targetRotation = new Vector3(0, 0, 90); // 동쪽 면으로 회전
        }
        else if (nextPlayerIndex == currentPlayerIndex - 3) // 오른쪽에서 왼쪽으로 이동
        {
            targetRotation = new Vector3(0, 0, -90); // 서쪽 면으로 회전
        }

        return targetRotation;
    }

    void HandleDirectionInput()
    {
        if (!isChoosingDirection) return;

        int[] adjacent = GetAdjacent(currentPlayerIndex);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            
            if (adjacent.Contains(currentPlayerIndex - 4))
            {
                Debug.Log("up");
                nextPlayerIndex = currentPlayerIndex - 4;
            }
            else
            {
                Debug.Log("upa");
                nextPlayerIndex = currentPlayerIndex + 12;
                shouldRotateCube = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (adjacent.Contains(currentPlayerIndex + 4))
            {
                nextPlayerIndex = currentPlayerIndex + 4;
            }
            else
            {
                nextPlayerIndex = currentPlayerIndex - 12;
                shouldRotateCube = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (adjacent.Contains(currentPlayerIndex - 1))
            {
                nextPlayerIndex = currentPlayerIndex - 1;
            }
            else
            {
                nextPlayerIndex = currentPlayerIndex + 3;
                shouldRotateCube = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (adjacent.Contains(currentPlayerIndex + 1))
            {
                nextPlayerIndex = currentPlayerIndex + 1;
            }
            else
            {
                nextPlayerIndex = currentPlayerIndex - 3;
                shouldRotateCube = true;
            }
        }
    }
}
