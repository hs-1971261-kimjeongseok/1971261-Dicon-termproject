using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RainDodge : Games
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
            float randomX = Random.Range(-4.75f, 4.75f);
            Vector3 spawnPosition = new Vector3(randomX + spawnPoints[1].position.x, spawnPoints[1].position.y, spawnPoints[1].position.z);
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.Euler(90,0,0));
            bullet.GetComponent<Bullet>().bulletdodgegame = false;
            bullet.GetComponent<Bullet>().speed = 5f;
            yield return new WaitForSeconds(1f / 3f); // 0.5초마다 총알 발사
        }
    }
}
