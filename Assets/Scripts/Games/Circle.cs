using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : Enemy
{
    public float spd = 1f;

    

    // Start is called before the first frame update
    void Start()
    {
        //this.transform.position = GameObject.Find("Player").transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 스케일을 가져오기
        Vector3 currentScale = transform.localScale;

        // 스케일 감소
        currentScale -= Vector3.one * spd * Time.deltaTime;

        // 새로운 스케일을 적용
        transform.localScale = currentScale;

        // 스케일이 모든 축에서 0 이하가 되면 오브젝트를 파괴
        if (currentScale.x <= 0 || currentScale.y <= 0 || currentScale.z <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitPlayer(collision);
    }
}
