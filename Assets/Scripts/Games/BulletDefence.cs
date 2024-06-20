using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDefence : Games
{
    public GameObject bulletPrefab;
    public Transform[] spawnPoints; // 총알 발사 위치
    public Transform playerTransform; // 플레이어의 위치를 가져오기 위한 참조

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(SpawnBullets());
        manager.player.GetComponent<Player>().barrier = true; // 플레이어의 barrier를 true로 설정
        manager.player.GetComponent<Player>().barrier1.SetActive(true);
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 총알 제거
        foreach (var bullet in FindObjectsOfType<Bullet>())
        {
            Destroy(bullet.gameObject);
        }
        manager.player.GetComponent<Player>().barrier1.SetActive(false);
    }

    IEnumerator SpawnBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            int spawnIndex = Random.Range(0, 4); // 0~3 범위의 랜덤 인덱스 선택
            Transform nextSpawnPoint = spawnPoints[spawnIndex];
            GameObject bullet = Instantiate(bulletPrefab, nextSpawnPoint.position, Quaternion.Euler(90,0,0));
            bullet.GetComponent<Bullet>().speed /= 1.2f;
            yield return new WaitForSeconds(0.1f); // 0.5초마다 총알 발사
        }
    }
}
