using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickFixedJump : Games
{
    public GameObject stickPrefab;

    private Coroutine gameCoroutine;

    public Transform[] spawnpoints;
    public Vector3[] dir;
    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(Spawnsticks());
        manager.player.transform.position = manager.planePositions[manager.currentPlayerIndex].position;
        manager.player.GetComponent<Player>().canjump = true;
        manager.player.GetComponent<Player>().onlyjump = true;
    }

    public override void GameStop()
    {
        if (gameCoroutine != null)
        {
            StopCoroutine(gameCoroutine);
        }
        // 총알 제거
        foreach (var stick in FindObjectsOfType<Stick>())
        {
            Destroy(stick.gameObject);
        }
        manager.player.GetComponent<Player>().canjump = false;
        manager.player.GetComponent<Player>().onlyjump = false;
    }
    IEnumerator Spawnsticks()
    {
        while (true)
        {
            manager.player.transform.position = new Vector3(manager.planePositions[manager.currentPlayerIndex].position.x, 
                manager.player.transform.position.y, manager.planePositions[manager.currentPlayerIndex].position.z);               


            Quaternion rot;
            int spawnloc = Random.Range(0, spawnpoints.Length);
            while (spawnloc == manager.closestDir) { spawnloc = Random.Range(0, spawnpoints.Length); }
            Vector3 spawnpoint = Vector3.zero;
            if (spawnloc == 0 || spawnloc == 3)
            {
                rot = Quaternion.Euler(90, 0, 90);
                int tmp = Random.Range(0, 2);
                if (tmp == 0) { spawnpoint = new Vector3(spawnpoints[spawnloc].position.x, spawnpoints[spawnloc].position.y, spawnpoints[spawnloc].position.z); }
                else { spawnpoint = new Vector3(spawnpoints[spawnloc].position.x, spawnpoints[spawnloc].position.y, spawnpoints[spawnloc].position.z); }
            }
            else
            {
                rot = Quaternion.Euler(90, 0, 0);
                int tmp = Random.Range(0, 2);
                if (tmp == 0) { spawnpoint = new Vector3(spawnpoints[spawnloc].position.x, spawnpoints[spawnloc].position.y, spawnpoints[spawnloc].position.z); }
                else { spawnpoint = new Vector3(spawnpoints[spawnloc].position.x, spawnpoints[spawnloc].position.y, spawnpoints[spawnloc].position.z); }
            }


            GameObject stick = Instantiate(stickPrefab, spawnpoint, rot);
            stick.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            stick.GetComponent<Stick>().direction = dir[spawnloc];

            int spawnloc2 = Random.Range(0, spawnpoints.Length);
            while (spawnloc2 == manager.closestDir || spawnloc == spawnloc2) { spawnloc2 = Random.Range(0, spawnpoints.Length); }
            spawnpoint = Vector3.zero;
            if (spawnloc2 == 0 || spawnloc2 == 3)
            {
                rot = Quaternion.Euler(90, 0, 90);
                int tmp = Random.Range(0, 2);
                if (tmp == 0) { spawnpoint = new Vector3(spawnpoints[spawnloc2].position.x, spawnpoints[spawnloc2].position.y, spawnpoints[spawnloc2].position.z); }
                else { spawnpoint = new Vector3(spawnpoints[spawnloc2].position.x, spawnpoints[spawnloc2].position.y, spawnpoints[spawnloc2].position.z); }
            }
            else
            {
                rot = Quaternion.Euler(90, 0, 0);
                int tmp = Random.Range(0, 2);
                if (tmp == 0) { spawnpoint = new Vector3(spawnpoints[spawnloc2].position.x, spawnpoints[spawnloc2].position.y, spawnpoints[spawnloc2].position.z); }
                else { spawnpoint = new Vector3(spawnpoints[spawnloc2].position.x, spawnpoints[spawnloc2].position.y, spawnpoints[spawnloc2].position.z); }
            }


            GameObject stick2 = Instantiate(stickPrefab, spawnpoint, rot);
            stick2.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            stick2.GetComponent<Stick>().direction = dir[spawnloc2];

            yield return new WaitForSeconds(1.666f); // 0.5초마다 총알 발사
        }
    }
}
