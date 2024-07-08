using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTower : Games
{
    public GameObject cubePrefab;
    public float spawnHeight = 5f; // 스폰 높이
    public float spawnRange = 4.75f; // 스폰 범위

    private Coroutine gameCoroutine;

    public GameObject[] spawnpoints;

    public override void GameStart()
    {
        manager.player.GetComponent<Player>().wallSet.SetActive(true);
        gameCoroutine = StartCoroutine(SpawnBullets());
        manager.player.GetComponent<Player>().canjump = true;
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 총알 제거
        foreach (var bullet in FindObjectsOfType<Cube>())
        {
            Destroy(bullet.gameObject);
        }

        GameObject[] towerupperside = GameObject.FindGameObjectsWithTag("tower");
        foreach (var bullet in towerupperside)
        {
            Destroy(bullet.gameObject);
        }
        manager.player.GetComponent<Player>().wallSet.SetActive(false);
        manager.player.GetComponent<Player>().canjump = false;
    }

    IEnumerator SpawnBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.8f); // 0.5초마다 총알 발사

            // 현재 오브젝트의 위치를 기준으로 x, z 범위 내에서 랜덤 위치 결정
            Vector3 spawnPosition = spawnpoints[Random.Range(0, 4)].transform.position;

            // 총알 생성
            GameObject bullet = Instantiate(cubePrefab, spawnPosition, Quaternion.Euler(0, 0, 0));


        }
    }
}