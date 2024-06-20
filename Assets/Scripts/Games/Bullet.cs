using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Enemy
{
    public float speed = 10.0f;
    private Transform target; Vector3 direction;


    public float acceleration = 2.0f; // 속도 증가율

    public bool bulletdodgegame = true;

    void Start()
    {
        //transform.Rotate(new Vector3(90,0,0));
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, 5f); // 5초 후 자동 파괴
        if (bulletdodgegame) { direction = (new Vector3(target.position.x, 0.183f, target.position.z) - transform.position).normalized; }
        else { direction = new Vector3(0,0,-1); }
    }

    void Update()
    {
        if (!bulletdodgegame) { speed += acceleration * Time.deltaTime * 2; }
        if (target != null)
        {

            transform.position += direction * speed * Time.deltaTime;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPlayer(collision);
       /* if (bulletdodgegame) { hitPlayer(collision); }
        else { Destroy(this); }*/
    }
}
