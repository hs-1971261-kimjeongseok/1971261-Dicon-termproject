using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickJump : Games
{
    public GameObject stickPrefab;

    private Coroutine gameCoroutine;

    public Transform[] spawnpoints;
    public Vector3[] dir;
    public override void GameStart()
    {
        gameCoroutine = StartCoroutine(Spawnsticks());
        manager.player.GetComponent<Player>().canjump = true;
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
    }
    IEnumerator Spawnsticks()
    {
        while (true)
        {
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
            yield return new WaitForSeconds(0.666f); // 0.5초마다 총알 발사
        }
    }
}
