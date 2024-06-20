using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : Enemy
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
        Destroy(gameObject, 5f); // 5초 후 자동 파괴
        if (bulletdodgegame) { direction = (new Vector3(target.position.x, 0.183f, target.position.z) - transform.position).normalized; }
        else if (centerdodgegame) { direction = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1f, 1f)); }
        else { direction = new Vector3(0,0,-1); }

        if(addf) this.GetComponent<Rigidbody>().AddForce(direction.normalized * speed* 40f);
    }

    void Update()
    {
        if (!bulletdodgegame && !centerdodgegame) { speed += acceleration * Time.deltaTime * 2; }
        if (!addf)
        {
            if (target != null)
            {

                transform.position += direction * speed * Time.deltaTime;
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPlayer(collision);
       /* if (bulletdodgegame) { hitPlayer(collision); }
        else { Destroy(this); }*/
       if(collision.transform.tag == "Wall" && addf)
        {
            direction = new Vector3(-1f* direction.x, -1f * direction.y, -1f * direction.z);
            this.GetComponent<Rigidbody>().AddForce(direction * speed * 80f * 2f);
        }
    }
}
