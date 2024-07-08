using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Enemy
{
    public float speed = 10.0f;
    private Transform target; public Vector3 direction;
    public float destroytime = 2f;

    public float acceleration = 2.0f; // 속도 증가율

    public bool bulletdodgegame = true;
    public bool centerdodgegame = false;

    public bool addf = false;

    void Start()
    {
        Destroy(gameObject, destroytime); // 5초 후 자동 파괴
        direction = new Vector3(0,0,0).normalized;


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
