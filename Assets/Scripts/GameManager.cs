using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
[System.Serializable]
public class MusicOffset
{
    public int[] offsets;
}

[System.Serializable]
public class Map
{
    public int[] map;
}

public class GameManager : MonoBehaviour
{
    public int currentStage = 0;
    public GameObject Environment;//외부 배경
    public GameObject CubeMap; // 전체 큐브들의 중심 위치
    public MapArray[] allCubes; // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
    public GameObject[] currentCubes = new GameObject[16];
    public AudioClip[] musics; // 0은 항상 플레이됨, 1~4 중 하나와 5~8 중 하나가 플레이됨, 9와 10이 번갈아 플레이됨
    public AudioClip[] bossmusics; // 정해진 순서에 따라 반복됨
    public Transform[] planePositions = new Transform[16];
    public MapArray[] spawnPoints;//플레이어 스폰 위치
    public GameObject player;
    private AudioSource audioSource;
    public AudioSource[] tmp;
    public CamMovement camMovement;
    public GameObject[] postProcessings;

    public int currentPlayerIndex;
    private int[][] cubeMusicIndices;
    public MusicOffset[] musicoffset;
    private int nextPlayerIndex;
    private bool isChoosingDirection = false;
    public bool shouldRotateCube = false; // 큐브 회전 여부

    public Material[] textures;

    public GameObject[] games;
    // 1 총알피하기 2 원 피하기

    private Games currentGame;

    private int nextSpawnpoint = 0;
    int nextDir = 0;

    public int nokori = 2;
    public Map[] maps; // 0 위 1 좌 2 우 3 아래

    public Map[] midPoints;
    public GameObject[] flags; // 0red 1yellow 2green
    public MapArray[] arrows; //0red 1yellow 2green //0up 1left 2right 3down
    public Map[] bossMusicCycle;
    public int bossMusicIdx = 0;

    public GameObject bossimg;

    private Coroutine gameRoutineCoroutine;


    void SetPlayerPosition(int index, bool first = false)
    {

        if (!first)
        {
            player.transform.position = spawnPoints[index].map[nextSpawnpoint].transform.position;
        }
        else
        {
            player.transform.position = planePositions[index].position;
        }
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
        GameObject gameInstance = Instantiate(games[7]);
        gameInstance.transform.localScale = Vector3.one;
        gameInstance.transform.position = new Vector3(planePositions[index].position.x, 0, planePositions[index].position.z);
        gameInstance.transform.parent = this.transform;
        currentGame = gameInstance.GetComponent<Games>();
        currentGame.manager = this;
        player.GetComponent<Player>().setInvincible(1f); // 플레이어 1초 무적 시간 부여
        currentGame.GameStart();
    }

    public bool boss;
    void PlayMusic(int[] indices, bool boss = false)
    {

        audioSource.Stop();
        if (boss) { audioSource.clip = bossmusics[bossMusicCycle[bossMusicIdx].map[0] + musicoffset[currentStage].offsets[0]]; }
        else { audioSource.clip = musics[indices[0] + musicoffset[currentStage].offsets[0]]; }
        audioSource.Play();
        audioSource.volume = 0.08f;
        if (musicoffset[currentStage].offsets[0] > 0) { audioSource.volume = 0.05f; }

        tmp[0].Stop();
        if (boss) { tmp[0].clip = bossmusics[bossMusicCycle[bossMusicIdx].map[1] + musicoffset[currentStage].offsets[1]]; }
        else { tmp[0].clip = musics[indices[1] + musicoffset[currentStage].offsets[1]]; }
        tmp[0].Play();
        tmp[0].volume = 0.08f;
        if (musicoffset[currentStage].offsets[1] > 0) { tmp[0].volume = 0.05f; }

        tmp[1].Stop();
        if (boss) { tmp[1].clip = bossmusics[bossMusicCycle[bossMusicIdx].map[2] + musicoffset[currentStage].offsets[2]]; }
        else { tmp[1].clip = musics[indices[1] + musicoffset[currentStage].offsets[2]]; }
        tmp[1].Play();
        tmp[1].volume = 0.08f;
        if (musicoffset[currentStage].offsets[2] > 0) { tmp[1].volume = 0.05f; }

        tmp[2].Stop();
        if (!boss)
        {
            if (music10playing)
            {
                tmp[2].clip = musics[10 + musicoffset[currentStage].offsets[3]];
                music10playing = false;
            }
            else
            {
                tmp[2].clip = musics[9 + musicoffset[currentStage].offsets[3]];
                music10playing = true;
            }
            tmp[2].Play();
            tmp[2].volume = 0.08f;
            if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }
        }
        if (boss) { bossMusicIdx++; }
        if (bossMusicIdx > 7) { bossMusicIdx = 0; }
    }

    void Start()
    {
        Time.timeScale = 1f;
        bossimg.SetActive(false);
        setCubetextures();
        allCubes[0].setArray(currentCubes);
        audioSource = GetComponent<AudioSource>();
        cubeMusicIndices = new int[16][]; // 각 큐브 위치별 음악 조합 배열
        InitializeCubeMusicIndices(); // 큐브 위치별 음악 조합을 초기화

        setMidPoints(0);
        setMidPointArrow(0);

        nextPlayerIndex = currentPlayerIndex;
        StartGame();
    }
    
    void setMidPoints(int curStage)
    {
        
        int stage = UnityEngine.Random.Range(0, 5);
        while(stage == curStage) { stage = UnityEngine.Random.Range(0, 5); }
        midPoints[0].map[0] = stage;
        midPoints[0].map[1] = UnityEngine.Random.Range(0, 16);

        if (!boss)
        {
            stage = UnityEngine.Random.Range(0, 5);
            while (stage == curStage || stage == midPoints[0].map[0]) { stage = UnityEngine.Random.Range(0, 5); }
            midPoints[1].map[0] = stage;
            midPoints[1].map[1] = UnityEngine.Random.Range(0, 16);
        }
        else
        {
            midPoints[1].map[0] = -1;
            midPoints[1].map[1] = -1;
        }
    }
    void setMidPointArrow(int curstage)
    {
        for(int i = 0; i < 3; i++)
        {
            flags[i].SetActive(false);
            for(int j = 0; j < 4; j++)
            {
                arrows[i].map[j].SetActive(false);
            }
        }
        if (!boss)
        {
            if (curstage == midPoints[0].map[0])
            {
                flags[0].transform.position = planePositions[midPoints[0].map[1]].position;
                flags[0].SetActive(true);
            }
            if (curstage == midPoints[1].map[0])
            {
                flags[1].transform.position = planePositions[midPoints[1].map[1]].position;
                flags[1].SetActive(true);
            }
            for (int i = 0; i < 4; i++)
            {
                if (maps[curstage].map[i] == midPoints[0].map[0])
                {
                    arrows[0].map[i].SetActive(true);
                }
                if (maps[curstage].map[i] == midPoints[1].map[0])
                {
                    arrows[1].map[i].SetActive(true);
                }
            }
        }
        else
        {
            if (curstage == midPoints[0].map[0])
            {
                flags[2].transform.position = planePositions[midPoints[0].map[1]].position;
                flags[2].SetActive(true);
            }
            for (int i = 0; i < 4; i++)
            {
                if (maps[curstage].map[i] == midPoints[0].map[0])
                {
                    arrows[2].map[i].SetActive(true);
                }
            }
        }
    }
    
    void setCubetextures(int stage = 0)
    {
        for (int k = 0; k < allCubes.Length; k++) 
        {

            for(int i=0;i< allCubes[k].map.Length;i++)
            {
                int rotx = UnityEngine.Random.Range(0, 2);
                int roty = UnityEngine.Random.Range(0, 2);
                int rotz = UnityEngine.Random.Range(0, 2);
                allCubes[k].map[i].transform.rotation = Quaternion.Euler(90 * rotx, 90 * roty, 90 * rotz);
                MeshRenderer renderer = allCubes[k].map[i].GetComponent<MeshRenderer>();
                renderer.material = textures[stage];
            }
        }
    }
    int decideStage(int curstage)
    {
        
        switch (curstage)
        {
            case 0:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        nextSpawnpoint = 1;
                        return 3;
                        break;
                    case 0:
                        nextSpawnpoint = 1;
                        return 4;
                        break;
                    case 2:
                        nextSpawnpoint = 1;
                        return 1;
                        break;
                    case 1:
                        nextSpawnpoint = 1;
                        return 2;
                        break;
                }
                break;
            case 1:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        return 0;
                        break;
                    case 0:
                        return 5;
                        break;
                    case 2:
                        return 3;
                        break;
                    case 1:
                        return 4;
                        break;
                }
                break;
            case 2:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        nextSpawnpoint = 1;
                        return 0;
                        break;
                    case 0:
                        nextSpawnpoint = 3;
                        return 5;
                        break;
                    case 2:
                        nextSpawnpoint = 3;
                        return 4;
                        break;
                    case 1:
                        nextSpawnpoint = 1;
                        return 3;
                        break;
                }
                break;
            case 3:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        return 0;
                        break;
                    case 0:
                        return 5;
                        break;
                    case 2:
                        return 2;
                        break;
                    case 1:
                        return 1;
                        break;
                }
                break;
            case 4:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        return 0;
                        break;
                    case 0:
                        return 5;
                        break;
                    case 2:
                        return 1;
                        break;
                    case 1:
                        return 2;
                        break;
                }
                break;
            case 5:// 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
                switch (nextSpawnpoint)//위3 아래0 좌2 우1
                {
                    case 3:
                        return 4;
                        break;
                    case 0:
                        return 3;
                        break;
                    case 2:
                        return 1;
                        break;
                    case 1:
                        return 2;
                        break;
                }
                break;
        }
        return 0;
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
        SetPlayerPosition(currentPlayerIndex,true);

        gameRoutineCoroutine = StartCoroutine(GameRoutine());
    }

    bool music10playing = false;

   
    

    

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
        Environment.transform.parent = CubeMap.transform;
        Quaternion startEnvRotation = Environment.transform.rotation;

        RotateAroundPivot cubeRotate = CubeMap.GetComponent<RotateAroundPivot>();

        Quaternion startRotation = CubeMap.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float duration = 0.5f;
        float elapsed = 0f;

        cubeRotate.rotationAngle = targetRotation;
        cubeRotate.StartRotation();

        int nextStage = decideStage(currentStage);

        while (elapsed < duration && !cubeRotate.rotateEnd)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        
        switch (nextStage)
        {
            case 0:
                Environment.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 1:
                Environment.transform.rotation = Quaternion.Euler(0, -90, -90);
                break;
            case 2:
                Environment.transform.rotation = Quaternion.Euler(0, 90, 90);
                break;
            case 3:
                Environment.transform.rotation = Quaternion.Euler(-90, 0, 180);
                break;
            case 4:
                Environment.transform.rotation = Quaternion.Euler(90, 0, 0);
                break;
            case 5:
                Environment.transform.rotation = Quaternion.Euler(180, 0, 0);
                break;
        }
        currentStage = nextStage;

        setPostProcessing();
        setCubetextures(nextStage);



        setMidPointArrow(nextStage);

        Environment.transform.parent = null;

        CubeMap.transform.rotation = Quaternion.Euler(0,0,0);

        camMovement.gameObject.transform.parent = null;
        player.gameObject.transform.parent= null;
        
        player.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        currentPlayerIndex = nextPlayerIndex;
        camMovement.targetPosition = planePositions[currentPlayerIndex];
        camMovement.StartMoving(true);
    }
    void setPostProcessing()
    {
        for (int i = 0; i < 5; i++)
        {
            postProcessings[i].SetActive(false);
        }
        postProcessings[currentStage].SetActive(true);
    }

    int GetClosestDirection()
    {
        Vector3 playerPosition = player.transform.position;
        Vector3[] midPoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            midPoints[i] = spawnPoints[currentPlayerIndex].map[i].transform.position;
        }

        float minDistance = float.MaxValue;
        int closestDir = -1;

        for (int i = 0; i < 4; i++)
        {
            float distance = Vector3.Distance(playerPosition, midPoints[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestDir = i;
            }
        }

        return closestDir;
    } // 상 좌 우 하

    public int closestDir;
    private void Update()
    {
        closestDir = GetClosestDirection();
        if (boss)
        {
            for (int i = 0; i < 4; i++)
            {
                arrows[3].map[i].SetActive(false);
            }
            arrows[3].map[closestDir].SetActive(true);

        }
    }

    public bool gameover = false;
    IEnumerator PlayTransitionCutscene()
    {
        // 카메라를 위로 올리는 애니메이션
        Vector3 originalCamPos = camMovement.transform.position;
        Vector3 middle = new Vector3(0, camMovement.transform.position.y, 20);
        Vector3 targetCamPos = middle + new Vector3(0, 20, 0); // 카메라를 20 유닛 위로 올림
        float duration = 5.2f; // 애니메이션 지속 시간
        float elapsedTime = 0f;


       
        // 움직일 이미지를 활성화하고 떨림 효과와 함께 올리는 애니메이션
        bossimg.SetActive(true);
        Vector3 originalImagePos = bossimg.transform.position;
        Vector3 targetImagePos;
        if (gameover) { targetImagePos = originalImagePos + new Vector3(0, 0, -30); } // 이미지를 30 유닛 위로 올림
        else { targetImagePos = originalImagePos + new Vector3(0, 0, 30);} // 이미지를 30 유닛 위로 올림


        // 이미지를 위로 올림
        while (elapsedTime < duration)
        {
            camMovement.transform.position = Vector3.Lerp(middle, targetCamPos, elapsedTime / duration);
            float randomOffset = UnityEngine.Random.Range(-0.5f, 0.5f); // 좌우로 불연속적으로 흔들리게 함
            bossimg.transform.position = Vector3.Lerp(originalImagePos, targetImagePos, elapsedTime / duration) + new Vector3(randomOffset, 0, 0); // 떨림 효과 추가
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        bossimg.transform.position = targetImagePos;
        camMovement.transform.position = targetCamPos;


       

        // 움직일 이미지를 원래 위치로 되돌리고 비활성화
        //bossimg.transform.position = originalImagePos;
        //bossimg.SetActive(false);

        // 카메라를 원래 위치로 되돌리는 애니메이션
        elapsedTime = 0f;
        duration = 0.5f; // 애니메이션 지속 시간
        while (elapsedTime < duration)
        {
            camMovement.transform.position = Vector3.Lerp(targetCamPos, originalCamPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camMovement.transform.position = originalCamPos;
        // 일정 시간 대기 (음악 재생 시간)
        yield return new WaitForSeconds(0.014285714285715f); // 이미지를 일정 시간 동안 유지
    }


    int nextSide = 0;
    int tmpDir = -1;
    bool transition = false;
    IEnumerator GameRoutine()
    {
        while (true)
        {
            if (gameover) { yield break; }
            if (transition)
            {
                if (currentGame != null)
                {
                    currentGame.GameStop();
                    
                }
                player.GetComponent<Player>().canMove = false;

                tmp[2].Stop();
                tmp[2].clip = bossmusics[0 + musicoffset[currentStage].offsets[0]];
                tmp[2].Play();
                tmp[2].volume = 0.08f;
                if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }


                // 컷신 시작
                yield return StartCoroutine(PlayTransitionCutscene());

                
                //yield return new WaitForSeconds(5.714285714285715f);

                tmp[2].Stop();
                tmp[2].clip = bossmusics[1 + musicoffset[currentStage].offsets[0]];
                tmp[2].Play();
                tmp[2].volume = 0.08f;
                if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }

                yield return new WaitForSeconds((5.714285714285715f / 2f));
                boss = true;
                // 음악 재생
                PlayMusic(cubeMusicIndices[currentPlayerIndex], true);
                player.GetComponent<Player>().canMove = true;
                SetPlayerPosition(currentPlayerIndex, true);

                setMidPoints(currentStage);
                setMidPointArrow(currentStage);

                transition = false;
                

            }
            if (boss)
            {
                // Boss 모드: 4.6137142857초 동안 게임 진행
                yield return new WaitForSeconds(4.6137142857f);

                if (currentGame != null)
                {
                    currentGame.GameStop();
                }
                player.GetComponent<Player>().increaseHP();
                player.GetComponent<Player>().decideRotation(player.GetComponent<Player>().hp);

                // 플레이어 방향 선택 대기: 0.1초 동안 방향키 입력 대기
                player.GetComponent<Player>().canMove = false;
                player.GetComponent<Rigidbody>().isKinematic = true;
                tmpDir = GetClosestDirection();
                switch (tmpDir)
                {
                    case 0:
                        tmpDir = 0;
                        break;
                    case 1:
                        tmpDir = 2;
                        break;
                    case 2:
                        tmpDir = 3;
                        break;
                    case 3:
                        tmpDir = 1;
                        break;
                }


                player.transform.position = spawnPoints[currentPlayerIndex].map[tmpDir].transform.position;
                isChoosingDirection = true;
                float elapsedTime = 0f;
                while (elapsedTime < 0.01f)
                {
                    HandleDirectionInput();
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                isChoosingDirection = false;
                player.GetComponent<Rigidbody>().isKinematic = false;

                if (shouldRotateCube)
                {
                    // 큐브 회전: 1초 동안 플레이어 위치 변경
                    yield return new WaitForSeconds(1f);

                    if (currentStage == midPoints[0].map[0] && currentPlayerIndex == midPoints[0].map[1]) // 게임 종료
                    {
                        tmp[2].Stop();
                        tmp[2].clip = bossmusics[7 + musicoffset[currentStage].offsets[0]];
                        tmp[2].Play();
                        tmp[2].volume = 0.08f;
                        if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }

                        gameover = true;

                        // 컷신 시작
                        yield return StartCoroutine(PlayTransitionCutscene());

                        boss = false;
                        showGameOverUI(true);
                        yield break;
                    }

                    // 큐브 회전
                    Vector3 targetRotation = GetCubeRotation();
                    yield return RotateCubeRoutine(targetRotation);

                    SetPlayerPosition(currentPlayerIndex, true);
                }
                else
                {
                    Environment.transform.parent = CubeMap.transform;
                    switch (currentStage)
                    {
                        case 0:
                            Environment.transform.rotation = Quaternion.Euler(0, 0, 0);
                            break;
                        case 1:
                            Environment.transform.rotation = Quaternion.Euler(0, -90, -90);
                            break;
                        case 2:
                            Environment.transform.rotation = Quaternion.Euler(0, 90, 90);
                            break;
                        case 3:
                            Environment.transform.rotation = Quaternion.Euler(-90, 0, 180);
                            break;
                        case 4:
                            Environment.transform.rotation = Quaternion.Euler(90, 0, 0);
                            break;
                        case 5:
                            Environment.transform.rotation = Quaternion.Euler(180, 0, 0);
                            break;
                    }
                    Environment.transform.parent = null;

                    

                    // 플레이어 위치 변경
                    yield return new WaitForSeconds(1f);

                    if (currentStage == midPoints[0].map[0] && currentPlayerIndex == midPoints[0].map[1]) // 게임 종료
                    {
                        tmp[2].Stop();
                        tmp[2].clip = bossmusics[7 + musicoffset[currentStage].offsets[0]];
                        tmp[2].Play();
                        tmp[2].volume = 0.08f;
                        if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }

                        gameover = true;

                        // 컷신 시작
                        yield return StartCoroutine(PlayTransitionCutscene());
                        boss = false;
                        showGameOverUI(true);
                        yield break;
                    }


                    currentPlayerIndex = nextPlayerIndex;
                    camMovement.targetPosition = planePositions[currentPlayerIndex];
                    camMovement.StartMoving(false);
                    SetPlayerPosition(currentPlayerIndex);
                }

                

                player.GetComponent<Player>().canMove = true;

                // 음악 재생
                PlayMusic(cubeMusicIndices[currentPlayerIndex], true);
                shouldRotateCube = false; // 큐브 회전 플래그 초기화
            }
            else
            {
                // 일반 모드: 6.666초 동안 게임 진행
                yield return new WaitForSeconds(6.666f);

                if (currentGame != null)
                {
                    currentGame.GameStop();
                }
                player.GetComponent<Player>().increaseHP();
                player.GetComponent<Player>().decideRotation(player.GetComponent<Player>().hp);

                // 2.333초 동안 방향키 입력 대기
                player.GetComponent<Player>().canMove = false;
                tmpDir = UnityEngine.Random.Range(0, 4);
                player.transform.position = spawnPoints[currentPlayerIndex].map[tmpDir].transform.position;
                player.GetComponent<Rigidbody>().isKinematic = true;
                isChoosingDirection = true;
                float elapsedTime = 0f;
                while (elapsedTime < 2.333f)
                {
                    HandleDirectionInput();
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                isChoosingDirection = false;
                player.GetComponent<Rigidbody>().isKinematic = false;

                if (shouldRotateCube)
                {
                    // 0.5초 동안 카메라 움직임
                    yield return new WaitForSeconds(0.5f);

                    // 큐브 회전
                    Vector3 targetRotation = GetCubeRotation();
                    yield return RotateCubeRoutine(targetRotation);

                    SetPlayerPosition(currentPlayerIndex, true);

                    // 0.5초 동안 플레이어 위치 변경
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    Environment.transform.parent = CubeMap.transform;
                    switch (currentStage)
                    {
                        case 0:
                            Environment.transform.rotation = Quaternion.Euler(0, 0, 0);
                            break;
                        case 1:
                            Environment.transform.rotation = Quaternion.Euler(0, -90, -90);
                            break;
                        case 2:
                            Environment.transform.rotation = Quaternion.Euler(0, 90, 90);
                            break;
                        case 3:
                            Environment.transform.rotation = Quaternion.Euler(-90, 0, 180);
                            break;
                        case 4:
                            Environment.transform.rotation = Quaternion.Euler(90, 0, 0);
                            break;
                        case 5:
                            Environment.transform.rotation = Quaternion.Euler(180, 0, 0);
                            break;
                    }
                    Environment.transform.parent = null;

                    // 1초 동안 플레이어 위치 변경
                    yield return new WaitForSeconds(1f);

                    if (currentStage == midPoints[0].map[0] && currentPlayerIndex == midPoints[0].map[1])
                    {
                        //피
                        player.GetComponent<Player>().maxHP++;
                        setMidPoints(currentStage);
                        setMidPointArrow(currentStage);
                        nokori--;
                    }
                    if (currentStage == midPoints[1].map[0] && currentPlayerIndex == midPoints[1].map[1])
                    {
                        //뭐로하지
                        player.GetComponent<Player>().maxinvincible += 0.5f;
                        setMidPoints(currentStage);
                        setMidPointArrow(currentStage);
                        nokori--;
                    }

                    currentPlayerIndex = nextPlayerIndex;
                    camMovement.targetPosition = planePositions[currentPlayerIndex];
                    camMovement.StartMoving(false);
                    SetPlayerPosition(currentPlayerIndex);
                }

                

                player.GetComponent<Player>().canMove = true;


                
                shouldRotateCube = false; // 큐브 회전 플래그 초기화
                //transition = true;//for test
                if (nokori == 0) { transition = true; }
                if (!transition)
                {
                    // 음악 재생
                    PlayMusic(cubeMusicIndices[currentPlayerIndex]);
                }
                
            }

        }
    }

    

    void HandleDirectionInput() // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
    {
        if (!isChoosingDirection) return;

        int[] adjacent = GetAdjacent(currentPlayerIndex);

        if (Input.GetKeyDown(KeyCode.UpArrow) || tmpDir == 0)
        {
            if(false)
            {
                tmpDir = 1;
            }
            else
            {
                tmpDir = -1;
                if (adjacent.Contains(currentPlayerIndex - 4))
                {
                    Debug.Log("up");
                    shouldRotateCube = false;
                    nextPlayerIndex = currentPlayerIndex - 4;
                }
                else
                {
                    Debug.Log("upa");
                    nextDir = 0;
                    nextPlayerIndex = decideNextpos(currentPlayerIndex, nextDir);
                    shouldRotateCube = true;
                }
                player.transform.position = spawnPoints[currentPlayerIndex].map[0].transform.position;
                
                nextSpawnpoint = 3;
                Animator animator = player.GetComponent<Animator>();
                animator.SetTrigger("Hit");

            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || tmpDir == 1)
        {
            if(currentStage != 0 && !adjacent.Contains(currentPlayerIndex + 4))
            {
                tmpDir = 0;
            }
            else
            {
                tmpDir = -1;
                if (adjacent.Contains(currentPlayerIndex + 4))
                {
                    shouldRotateCube = false;
                    nextPlayerIndex = currentPlayerIndex + 4;
                }
                else
                {
                    nextDir = 1;
                    nextPlayerIndex = decideNextpos(currentPlayerIndex, nextDir);
                    shouldRotateCube = true;
                }
                player.transform.position = spawnPoints[currentPlayerIndex].map[3].transform.position;
                nextSpawnpoint = 0;
                Animator animator = player.GetComponent<Animator>();
                animator.SetTrigger("Select");
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || tmpDir == 2)
        {
            if(false)
            {
                tmpDir = 3;
            }
            else
            {
                tmpDir = -1;
                if (adjacent.Contains(currentPlayerIndex - 1))
                {

                    nextPlayerIndex = currentPlayerIndex - 1;
                    shouldRotateCube = false;
                }
                else
                {
                    nextDir = 2;
                    nextPlayerIndex = decideNextpos(currentPlayerIndex, nextDir);
                    shouldRotateCube = true;
                }
                player.transform.position = spawnPoints[currentPlayerIndex].map[1].transform.position;
                nextSpawnpoint = 2;
                Animator animator = player.GetComponent<Animator>();
                animator.SetTrigger("Select");
            }
           
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || tmpDir == 3)
        {
            if(false)
            {
                tmpDir = 2;
            }
            else
            {
                tmpDir = -1;
                if (adjacent.Contains(currentPlayerIndex + 1))
                {
                    nextPlayerIndex = currentPlayerIndex + 1;
                    shouldRotateCube = false;
                }
                else
                {
                    nextDir = 3;
                    nextPlayerIndex = decideNextpos(currentPlayerIndex, nextDir);
                    shouldRotateCube = true;
                }
                player.transform.position = spawnPoints[currentPlayerIndex].map[2].transform.position;
                nextSpawnpoint = 1;
                Animator animator = player.GetComponent<Animator>();
                animator.SetTrigger("Select");
            }
            
        }
        
    }
    int decideNextpos(int curPlayerIdx, int dir)
    {
        //dir -> 0 up / 1 down / 2 left / 3 right
        switch (currentStage)
        {
            case 0://위
                switch (dir)
                {
                    case 0://위
                        return 3 - curPlayerIdx;
                        break;
                    case 1://아래
                        return curPlayerIdx - 12;
                        break;
                    case 2://왼
                        return curPlayerIdx / 4;
                        break;
                    case 3://오
                        return (curPlayerIdx + 1) / 4 - 1;
                        break;
                }
                break;
            case 1://서
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx * 4;
                        break;
                    case 1://아래
                        return curPlayerIdx;
                        break;
                    case 2://왼
                        return curPlayerIdx + 3;
                        break;
                    case 3://오
                        return curPlayerIdx - 3;
                        break;
                }
                break;
            case 2://동
                switch (dir)
                {
                    case 0://위
                        return (3 - curPlayerIdx) * 4 + 3;
                        break;
                    case 1://아래
                        return curPlayerIdx;
                        break;
                    case 2://왼
                        return curPlayerIdx - 3;
                        break;
                    case 3://오
                        return curPlayerIdx + 3;
                        break;
                }
                break;
            case 3://북
                switch (dir)
                {
                    case 0://위
                        return (3 - curPlayerIdx);
                        break;
                    case 1://아래
                        return curPlayerIdx;
                        break;
                    case 2://왼
                        return curPlayerIdx + 3;
                        break;
                    case 3://오
                        return curPlayerIdx - 3;
                        break;
                }
                break;
            case 4://남
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx + 12;
                        break;
                    case 1://아래
                        return curPlayerIdx;
                        break;
                    case 2://왼
                        return curPlayerIdx + 3;
                        break;
                    case 3://오
                        return curPlayerIdx - 3;
                        break;
                }
                break;
        }
        return 0;
    }

    //old one
    int decideNextposition(int curPlayerIdx, int dir) // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
    {
        //dir -> 0 up / 1 down / 2 left / 3 right
        switch (currentStage)
        {
            case 0://위
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx + 12;
                        break;
                    case 1://아래
                        return curPlayerIdx - 12;
                        break;
                    case 2://왼
                        return curPlayerIdx + 3;
                        break;
                    case 3://오
                        return curPlayerIdx - 3;
                        break;
                }
                break;
            case 1://서
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx * 4;
                        break;
                    case 1://아래
                        return (15 - curPlayerIdx) * 4;
                        break;
                    case 2://왼
                        return curPlayerIdx;
                        break;
                    case 3://오
                        return curPlayerIdx - 3;
                        break;
                }
                break;
            case 2://동
                switch (dir)
                {
                    case 0://위
                        return (4 - curPlayerIdx) * 4 - 1;
                        break;
                    case 1://아래
                        return (curPlayerIdx - 11) * 4 - 1;
                        break;
                    case 2://왼
                        return curPlayerIdx + 3;
                        break;
                    case 3://오
                        return curPlayerIdx;
                        break;
                }
                break;
            case 3://북
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx;
                        break;
                    case 1://아래
                        return curPlayerIdx - 12;
                        break;
                    case 2://왼
                        return curPlayerIdx / 4;
                        break;
                    case 3://오
                        return (15 - curPlayerIdx) / 4;
                        break;
                }
                break;
            case 4://남
                switch (dir)
                {
                    case 0://위
                        return curPlayerIdx + 12;
                        break;
                    case 1://아래
                        return curPlayerIdx;
                        break;
                    case 2://왼
                        return (12 - curPlayerIdx) / 4 + 12;
                        break;
                    case 3://오
                        return 12 + (curPlayerIdx / 4);
                        break;
                }
                break;
        }
        return 0;
    }
    Vector3 GetCubeRotation()
    {
        // 새로운 면으로 큐브 전체 회전을 설정합니다.
        Vector3 targetRotation = Vector3.zero;

        if (nextDir == 1) // 위에서 아래로 이동
        {
            // 0 위, 1 서, 2 동, 3 북, 4 남, 5 아래
            targetRotation = new Vector3(90, 0, 0); // 남쪽 면으로 회전
        }
        else if (nextDir == 0) // 아래에서 위로 이동
        {
            targetRotation = new Vector3(-90, 0, 0); // 북쪽 면으로 회전
        }
        else if (nextDir == 3) // 왼쪽에서 오른쪽으로 이동
        {
            targetRotation = new Vector3(0, 0, 90); // 동쪽 면으로 회전
        }
        else if (nextDir == 2) // 오른쪽에서 왼쪽으로 이동
        {
            targetRotation = new Vector3(0, 0, -90); // 서쪽 면으로 회전
        }

        return targetRotation;
    }

    public GameObject gameoverset;
    public TMP_Text gameovertext;
    public void showGameOverUI(bool clear)
    {
        audioSource.Stop();
        tmp[0].Stop();
        tmp[1].Stop();
        tmp[2].Stop();

        if (!clear)
        {
            tmp[2].Stop();
            tmp[2].clip = bossmusics[7 + musicoffset[currentStage].offsets[0]];
            tmp[2].Play();
            tmp[2].volume = 0.08f;
            if (musicoffset[currentStage].offsets[3] > 0) { tmp[2].volume = 0.05f; }
        }

        if (gameRoutineCoroutine != null)
        {
            StopCoroutine(gameRoutineCoroutine);
            gameRoutineCoroutine = null;
            Time.timeScale = 0f;
        }

        gameoverset.SetActive(true);
        if (clear)
        {
            gameovertext.text = "Game Clear!";
        }
        else
        {
            gameovertext.text = "Game Over..";
        }
        
    }
    public void goMain()
    {
        SceneManager.LoadScene("StartScene");
    }
}
