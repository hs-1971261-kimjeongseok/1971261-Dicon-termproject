using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterDodge : Games
{
    public GameObject bulletPrefab;

    public Transform[] spawnPoints; // 총알 발사 위치

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        GameObject.Find("Player").transform.position = spawnPoints[0].position;
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

            Transform nextSpawnPoint = this.transform;
            GameObject bullet = Instantiate(bulletPrefab, nextSpawnPoint.position, Quaternion.Euler(90, 0, 0));
            bullet.GetComponent<Bullet>().speed = 5f;
            bullet.GetComponent<Bullet>().centerdodgegame = true;
            bullet.GetComponent<Bullet>().bulletdodgegame = false;
            yield return new WaitForSeconds(0.2f); // 0.5초마다 총알 발사
        }
    }
}
