using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Enemy
{
    public float speed = 10.0f;
    private Transform target; public Vector3 direction;


    public float acceleration = 2.0f; // 속도 증가율

    public bool bulletdodgegame = true;
    public bool centerdodgegame = false;

    public bool addf = false;

    void Start()
    {
        //transform.Rotate(new Vector3(90,0,0));
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, 7f); // 5초 후 자동 파괴
        direction = (new Vector3(target.position.x, 0.183f, target.position.z) - transform.position).normalized;


        this.GetComponent<Rigidbody>().AddForce(direction.normalized * speed * 4f);
    }

    void Update()
    {


    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPlayer(collision);
    }
}
