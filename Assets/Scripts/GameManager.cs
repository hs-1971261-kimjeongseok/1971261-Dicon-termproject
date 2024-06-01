using System.Collections;
using System.Collections.Generic;
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
    private int[][] cubeMusics;

    void Start()
    {
        allCubes[0].setArray(currentCubes);
        audioSource = GetComponent<AudioSource>();
        currentPlayerIndex = 6;
        cubeMusics = new int[16][];
        initializeCubeMusics();
        StartGame();
    }
    void initializeCubeMusics()
    {
        for (int i = 0; i < 16; i++)
        {
            cubeMusics[i] = new int[] { 0, Random.Range(1, 5), Random.Range(5, 9) };
        }
    }
    void StartGame()
    {
        PlayMusic(new int[] { 0, 3, 7 });
        SetPlayerPosition(currentPlayerIndex);

        StartCoroutine(ChangePlayerPositionRoutine());
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
    }

    int[] getAdjacent(int index)
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

    IEnumerator ChangePlayerPositionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(9f);

            // 인접한 큐브 중 하나를 선택하여 이동
            int[] adjacent = getAdjacent(currentPlayerIndex);
            int newCube = Random.Range(0, adjacent.Length);
            currentPlayerIndex = adjacent[newCube];
            SetPlayerPosition(currentPlayerIndex);

            yield return new WaitForSeconds(1f);
            PlayMusic(cubeMusics[currentPlayerIndex]);
        }
    }
}
