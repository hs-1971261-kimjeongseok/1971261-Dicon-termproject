using UnityEngine;
using System.Collections;

public class RotateAroundPivot : MonoBehaviour
{
    public GameObject pivotObject; // 피봇 포인트로 사용할 게임오브젝트
    public Vector3 rotationAngle; // 회전 각도
    public float duration = 0.5f;  // 회전 시간

    private Quaternion startRotation;
    private Quaternion endRotation;
    private float timeElapsed;

    public bool rotateEnd = true;

    void Start()
    {
        // StartRotation();
    }

    public void StartRotation()
    {
        // 회전 각도에 따라 최종 회전값 설정
        startRotation = transform.rotation;
        endRotation = Quaternion.Euler(rotationAngle) * startRotation;
        timeElapsed = 0;
        rotateEnd = false;
        StartCoroutine(RotateCoroutine());
    }

    IEnumerator RotateCoroutine()
    {
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            transform.position = pivotObject.transform.position + (transform.rotation * (transform.position - pivotObject.transform.position));

            yield return null;
        }

        // 정확한 최종 상태로 설정
        transform.rotation = endRotation;
        rotateEnd = true;
    }
}
