using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDodge : Games
{
    public GameObject circlePrefab;

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(SpawnCircles());
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 총알 제거
        foreach (var circle in FindObjectsOfType<Circle>())
        {
            Destroy(circle.gameObject);
        }
    }
    IEnumerator SpawnCircles()
    {
        while (true)
        {
            float randomrot = Random.Range(0f, 360f);

            Vector3 spawnPosition = manager.planePositions[manager.currentPlayerIndex].transform.position;

            GameObject circle = Instantiate(circlePrefab, spawnPosition, Quaternion.Euler(90, 0, randomrot));

            yield return new WaitForSeconds(1f); // 0.5초마다 총알 발사
        }
    }
}
