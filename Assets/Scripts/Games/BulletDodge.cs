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
    public int closest;
    public int nextspawn;
    IEnumerator SpawnBullets()
    {
        while (true)
        {// 상 좌 우 하
            closest = manager.closestDir;
            nextspawn = 0;
            switch (closest)
            {
                case 0:
                    nextspawn = Random.Range(0, 5) + 3;
                    break;
                case 1:
                    nextspawn = Random.Range(0, 8);
                    while (nextspawn>=2 && nextspawn <= 4) { nextspawn = Random.Range(0, 8); }
                    break;
                case 3:
                    nextspawn = Random.Range(0, 8);
                    while (nextspawn >= 4 && nextspawn <= 6) { nextspawn = Random.Range(0, 8); }
                    break;
                case 2:
                    nextspawn = Random.Range(0, 8);
                    while (nextspawn == 6 || nextspawn == 7 || nextspawn == 0) { nextspawn = Random.Range(0, 8); }
                    break;
            }
            Transform nextSpawnPoint = spawnPoints[nextspawn];
            GameObject bullet = Instantiate(bulletPrefab, nextSpawnPoint.position, Quaternion.Euler(90, 0, 0));
            
            yield return new WaitForSeconds(0.5f); // 0.5초마다 총알 발사
        }
    }
}
