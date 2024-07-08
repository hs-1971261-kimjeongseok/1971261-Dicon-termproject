using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpBulletDodge : Games
{
    public GameObject ballPrefab;
    public float spawnHeight = 5f; // 스폰 높이
    public float spawnRange = 4.75f; // 스폰 범위

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        manager.player.GetComponent<Player>().wallSet.SetActive(true);
        gameCoroutine = StartCoroutine(SpawnBullets());
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 총알 제거
        foreach (var bullet in FindObjectsOfType<Ball>())
        {
            Destroy(bullet.gameObject);
        }
        manager.player.GetComponent<Player>().wallSet.SetActive(false);
    }

    IEnumerator SpawnBullets()
    {
        while (true)
        {
            // 현재 오브젝트의 위치를 기준으로 x, z 범위 내에서 랜덤 위치 결정
            Vector3 spawnPosition = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y + spawnHeight,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );

            // 총알 생성
            GameObject bullet = Instantiate(ballPrefab, spawnPosition, Quaternion.Euler(90, 0, 0));

            yield return new WaitForSeconds(0.3f); // 0.5초마다 총알 발사
        }
    }
}