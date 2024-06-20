using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public bool hard = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void hitPlayer(Collision collision)
    {
        if (collision.gameObject.transform.tag == "Player")
        {

            Player tmp = collision.gameObject.GetComponent<Player>();

            if (!tmp.isInvincible)
            {
                // 충돌 지점 얻기
                ContactPoint contact = collision.contacts[0];
                Vector3 hitPosition = contact.point;
                // 파티클 이펙트를 충돌 지점에서 생성
                GameObject hit = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
                //hit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                ParticleSystem particleSystem = hit.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    var mainModule = particleSystem.main;
                    mainModule.simulationSpeed = 0.5f; // 2배 느리게 재생
                }
                tmp.reroll();
                if (!hard)
                {
                    Destroy(gameObject);
                }
                
            }
        }
        if(collision.gameObject.transform.tag == "Barrier") { 
            // 충돌 지점 얻기
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPosition = contact.point;
            // 파티클 이펙트를 충돌 지점에서 생성
            GameObject hit = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            //hit.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            ParticleSystem particleSystem = hit.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                var mainModule = particleSystem.main;
                mainModule.simulationSpeed = 0.5f; // 2배 느리게 재생
            }
            Destroy(gameObject);
        }
    }
}
