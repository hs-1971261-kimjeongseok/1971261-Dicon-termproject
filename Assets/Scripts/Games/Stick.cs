using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : Enemy { 
    public float speed = 5.0f;
    public Vector3 direction;

    public bool stickdodgegame = true;

    void Start()
    {
        speed = 6.0f;
        Destroy(gameObject, 5f); // 5초 후 자동 파괴
        

    }

    void Update()
    {
        if (stickdodgegame) { transform.position += direction * speed * Time.deltaTime; }

    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPlayer(collision);
    }
}
