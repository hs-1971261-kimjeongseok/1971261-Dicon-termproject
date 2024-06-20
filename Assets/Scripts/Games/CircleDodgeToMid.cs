using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDodgeToMid : Games
{
    public GameObject circlePrefab;
    public Transform[] spawnPoints;

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        // 플레이어를 랜덤한 스폰 위치로 이동
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        manager.player.transform.position = randomSpawnPoint.position;

        // 게임 시작 시 중앙에 서로 다른 크기의 원 3개 생성
        SpawnCircles();

        // 게임 코루틴 시작
        gameCoroutine = StartCoroutine(SpawnCircles());
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 원 제거
        foreach (var circle in FindObjectsOfType<Circle>())
        {
            Destroy(circle.gameObject);
        }
    }

    IEnumerator SpawnCircles()
    {
        Vector3 centralPosition = manager.planePositions[manager.currentPlayerIndex].transform.position;

        // 서로 다른 크기의 원 3개 생성
        for (int i = 0; i < 2; i++)
        {
            GameObject circle = Instantiate(circlePrefab, centralPosition, Quaternion.Euler(90,0,0));
            circle.transform.localScale = new Vector3(1f - (0.6f * i), 1f - (0.6f * i), 1f - (0.6f * i));
            Circle circleScript = circle.GetComponent<Circle>();
            circleScript.hard = true;
            circleScript.dodge = false;
            circleScript.rotationDirection = new Vector3(0, 0, Random.Range(0, 90f)).normalized;
            circleScript.rotationSpeed = Random.Range(3f*7* (i* 2+1), 9f*7*(i*2+1)); // 랜덤한 회전 속도 설정
        }
        GameObject circle2 = Instantiate(circlePrefab, centralPosition, Quaternion.Euler(90, 0, 0));
        GameObject circle3 = Instantiate(circlePrefab, centralPosition, Quaternion.Euler(90, 0, 0));
        circle2.transform.localScale = new Vector3(3f, 3f, 3f);
        circle2.GetComponent<Circle>().hard = true;
        circle2.GetComponent<Circle>().spd = 0.37f;
        circle2.GetComponent<Circle>().dodge = true;
        circle3.transform.localScale = new Vector3(3f, 3f, 3f);
        circle3.transform.rotation = Quaternion.Euler(90, 0, 180);
        circle3.GetComponent<Circle>().hard = true;
        circle3.GetComponent<Circle>().spd = 0.37f;
        circle3.GetComponent<Circle>().dodge = true;
        yield return new WaitForSeconds(6.6f); // 0.5초마다 총알 발사
    }
}
