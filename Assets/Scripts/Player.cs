using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection;
    public Transform currentPosition;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        Vector3 cubeCenter = currentPosition.position;
        float size = 4.75f;

        float minX = cubeCenter.x - size;
        float maxX = cubeCenter.x + size;
        float minZ = cubeCenter.z - size;
        float maxZ = cubeCenter.z + size;

        float xPos = Mathf.Clamp(transform.position.x, minX, maxX);
        float zPos = Mathf.Clamp(transform.position.z, minZ, maxZ);
        transform.position = new Vector3(xPos, transform.position.y, zPos);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
    }
}
