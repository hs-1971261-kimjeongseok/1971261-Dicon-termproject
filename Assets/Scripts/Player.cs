using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection;
    public GameObject currentCube;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        Bounds cubeBounds = currentCube.GetComponent<Renderer>().bounds;
        float minX = cubeBounds.min.x;
        float maxX = cubeBounds.max.x;
        float minZ = cubeBounds.min.z;
        float maxZ = cubeBounds.max.z;

        float xPos = Mathf.Clamp(transform.position.x, minX, maxX);
        float zPos = Mathf.Clamp(transform.position.z, minZ, maxZ);
        transform.position = new Vector3(xPos, transform.position.y, zPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }
}
