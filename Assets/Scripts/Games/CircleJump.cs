using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleJump : Games
{
    public GameObject circlePrefab;

    private Coroutine gameCoroutine;

    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(SpawnCircles());
        manager.player.GetComponent<Player>().canjump = true;
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
        manager.player.GetComponent<Player>().canjump = false;
    }
    IEnumerator SpawnCircles()
    {
        while (true)
        {
            float randomrot = Random.Range(0f, 360f);

            Vector3 spawnPosition = manager.planePositions[manager.currentPlayerIndex].transform.position;
            spawnPosition.x += Random.Range(-2f, 2f);
            spawnPosition.z += Random.Range(-2f, 2f);

            GameObject circle2 = Instantiate(circlePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
            GameObject circle3 = Instantiate(circlePrefab, spawnPosition, Quaternion.Euler(90, 0, 0));
            circle2.transform.localScale = new Vector3(2f, 2f, 2f);
            circle2.GetComponent<Circle>().hard = true;
            //circle2.GetComponent<Circle>().spd = 0.37f;
            circle2.GetComponent<Circle>().dodge = true;
            circle3.transform.localScale = new Vector3(2f, 2f, 2f);
            circle3.transform.rotation = Quaternion.Euler(90, 0, 180);
            circle3.GetComponent<Circle>().hard = true;
            //circle3.GetComponent<Circle>().spd = 0.37f;
            circle3.GetComponent<Circle>().dodge = true;

            yield return new WaitForSeconds(1f); // 0.5초마다 총알 발사
        }
    }
}
