using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    private Vector3 moveDirection;
    public Transform currentPosition;
    private Vector3 originalPosition;
    AudioSource audioSource;
    public int hp = 3;
    public Vector3[] hpRotation;
    private Quaternion currentRotation = Quaternion.Euler(0, 0, 0);
    private int maxHP = 4;

    public bool canMove = true;

    public void decideRotation(int curHP)
    {
        if(curHP<0) { return;}
        if(curHP>maxHP) { curHP = maxHP; }
        currentRotation = Quaternion.Euler(hpRotation[hp].x, hpRotation[hp].y, hpRotation[hp].z);
    }
    void Update()
    {
        transform.rotation = currentRotation;
        if (canMove)
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(RerollAnimation());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.transform.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            
        }
    }
    public void reroll()
    {
        StartCoroutine(RerollAnimation());
    }
    private void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();  
    }
    private IEnumerator RerollAnimation()
    {
        hp--;
        if (hp < 1) { hp = 1; }

        audioSource.Stop();
        audioSource.Play();
        audioSource.volume = 0.05f;
        // 저장된 원래 위치
        originalPosition = transform.position;

        // 점프 및 회전 애니메이션
        float duration = 0.2f;
        float elapsedTime = 0f;
        Vector3 targetPosition = originalPosition + new Vector3(0, 1, 0);
        Vector3 rotationPerFrame = new Vector3(1080f / (duration * 60), 1080f / (duration * 60), 0); // 60 FPS 기준

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / duration));
            transform.Rotate(rotationPerFrame * Time.deltaTime * 60); // 프레임 단위로 회전 적용
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // 원래 위치로 돌아오는 애니메이션
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            decideRotation(hp);
            yield return null;
        }

        transform.position = originalPosition;
        
    }
}
