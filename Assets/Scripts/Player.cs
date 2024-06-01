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


        float xPos = Mathf.Clamp(currentCube.transform.position.x, -4.75f, 4.75f);
        float zPos = Mathf.Clamp(currentCube.transform.position.z, -4.75f, 4.75f);
        transform.position = new Vector3(xPos, transform.position.y, zPos);
    }
}
