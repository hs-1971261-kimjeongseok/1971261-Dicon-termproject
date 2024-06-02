using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 30.0f;

    private bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            Vector3 targetPos = targetPosition.position + new Vector3(0, 16.48f, 0);
            Quaternion targetRot = Quaternion.Euler(90, 0, 0);
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.01f && Quaternion.Angle(transform.rotation, targetRot) < 0.01f)
            {
                isMoving = false;
            }
        }
    }

    private void Start()
    {
        StartMoving(false);
    }

    public void StartMoving(bool a)
    {
        if ((a))
        {
            Vector3 targetPos = targetPosition.position + new Vector3(0, 16.48f, 0);
            transform.position = targetPos;
        }
        else
        {
            isMoving = true;
        }
        
    }
}
