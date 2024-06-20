using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBulletDefence : Games
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
        manager.player.GetComponent<Player>().barrier2.SetActive(true);
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
        manager.player.GetComponent<Player>().barrier2.SetActive(false);
        manager.player.GetComponent<Player>().barrier = false; // 플레이어의 barrier를 true로 설정
    }

    IEnumerator SpawnBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.12f); // 더 빠른 간격으로 총알 발사
            int spawnIndex = Random.Range(0, spawnPoints.Length); // 랜덤 스폰 포인트 선택
            Transform nextSpawnPoint = spawnPoints[spawnIndex];
            GameObject bullet = Instantiate(bulletPrefab, nextSpawnPoint.position, Quaternion.Euler(90, 0, 0));
            bullet.GetComponent<Bullet>().speed /= 2f;
            bullet.GetComponent<Bullet>().centerdodgegame = true;
            bullet.GetComponent<Bullet>().bulletdodgegame = false;
            //bullet.GetComponent<Bullet>().direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        }
    }
}
