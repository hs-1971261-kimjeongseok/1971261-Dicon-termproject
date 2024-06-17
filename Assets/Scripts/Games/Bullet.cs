using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    private Transform target; Vector3 direction;

    void Start()
    {
        //transform.Rotate(new Vector3(90,0,0));
        target = GameObject.FindGameObjectWithTag("Player").transform;
        Destroy(gameObject, 5f); // 5초 후 자동 파괴
        direction = (new Vector3(target.position.x, 0.183f, target.position.z) - transform.position).normalized;
    }

    void Update()
    {
        if (target != null)
        {
            
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {
            Player tmp = collision.gameObject.GetComponent<Player>();
            tmp.reroll();
            Destroy(gameObject);
        }
    }
}
