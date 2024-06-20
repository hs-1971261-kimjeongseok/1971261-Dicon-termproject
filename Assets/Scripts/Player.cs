using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int maxHP = 4;
    public GameManager manager;

    public bool canMove = true;
    public bool isInvincible = false; // 무적 상태를 나타내는 변수
    private float invincibleTime = 0f; // 무적 시간 카운트
    public float maxinvincible = 2f;

    public GameObject invObject;

    public bool barrier = false;
    public GameObject barrier1;
    public GameObject barrier2;
    public GameObject barrierSet;
    public GameObject wallSet;

    public void increaseHP()
    {
        if (hp < maxHP) { hp++; }
    }
    public void decideRotation(int curHP)
    {

        if (curHP < 0) { return; }
        if (curHP > maxHP) { curHP = maxHP; }
        currentRotation = Quaternion.Euler(hpRotation[hp-1].x, hpRotation[hp-1].y, hpRotation[hp-1].z);

    }

    void Update()
    {
        barrierSet.transform.position = manager.planePositions[manager.currentPlayerIndex].position;
        wallSet.transform.position = manager.planePositions[manager.currentPlayerIndex].position;

        transform.rotation = currentRotation;
        if (canMove)
        {
            if (barrier)
            {
               
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    this.transform.position = manager.planePositions[manager.currentPlayerIndex].position + new Vector3(-0.01f, 0, 0);
                    barrierSet.transform.rotation = Quaternion.Euler(0, 90f, 0);
                    //currentRotation = Quaternion.Euler(hpRotation[hp - 1].x, hpRotation[hp - 1].y - 90f, hpRotation[hp - 1].z);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    this.transform.position = manager.planePositions[manager.currentPlayerIndex].position + new Vector3(0.01f, 0, 0);
                    barrierSet.transform.rotation = Quaternion.Euler(0, -90f, 0);
                    //currentRotation = Quaternion.Euler(hpRotation[hp - 1].x, hpRotation[hp - 1].y + 90f, hpRotation[hp - 1].z);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    this.transform.position = manager.planePositions[manager.currentPlayerIndex].position + new Vector3(0, 0, 0.01f);
                    barrierSet.transform.rotation = Quaternion.Euler(0, 180f, 0);
                    //currentRotation = Quaternion.Euler(hpRotation[hp - 1].x, hpRotation[hp - 1].y, hpRotation[hp - 1].z);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    this.transform.position = manager.planePositions[manager.currentPlayerIndex].position + new Vector3(0, 0, -0.01f);
                    barrierSet.transform.rotation = Quaternion.Euler(0, 0f, 0);
                    //currentRotation = Quaternion.Euler(hpRotation[hp - 1].x, hpRotation[hp - 1].y + 180f, hpRotation[hp - 1].z);
                }
                else
                {
                    this.transform.position = manager.planePositions[manager.currentPlayerIndex].position;
                }

            }
            else
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
        }

        if (isInvincible)
        {
            invObject.SetActive(true);
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0)
            {
                isInvincible = false;
            }
        }
        else
        {
            invObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            reroll();
        }
    }

    public void reroll()
    {
        if (!isInvincible)
        {
            StartCoroutine(RerollAnimation());
        }
    }

    private void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
        setInvincible(1f);
    }

    public void setInvincible(float time)
    {
        invincibleTime = time;
        isInvincible = true;
    }

    private IEnumerator RerollAnimation()
    {
        
        
        isInvincible = true; // 무적 상태로 설정
        invincibleTime = maxinvincible; // 무적 시간 설정
        hp--;
        if (hp < 1) { manager.showGameOverUI(false); }

        barrier1.SetActive(false);

        bool t = false;
        if (barrier2.activeSelf)
        {
            t = true;
            barrier2.SetActive(false);
        }
        

       

        audioSource.Stop();
        audioSource.Play();
        audioSource.volume = 0.05f;
        // 저장된 원래 위치
        originalPosition = transform.position;

        // 점프 및 회전 애니메이션
        float duration = 0.35f;
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
        duration = 0.05f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            decideRotation(hp);
            yield return null;
        }

        transform.position = originalPosition;

        if (barrier) { barrier1.SetActive(true); }
        
        if (t) { barrier2.SetActive(true); }
        
    }
}
