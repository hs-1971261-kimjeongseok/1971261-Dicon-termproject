using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletDodge : Games
{
    public GameObject bulletPrefab;
    public Transform[] spawnPoints; // 총알 발사 위치

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(SpawnBullets());
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
    }

    IEnumerator SpawnBullets()
    {
        while (true)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject bullet = Instantiate(bulletPrefab, randomSpawnPoint.position, Quaternion.Euler(90, 0, 0));
            
            yield return new WaitForSeconds(0.5f); // 0.5초마다 총알 발사
        }
    }
}
