using System.Collections;
using TMPro;
using UnityEngine;

public class ChangeText : MonoBehaviour
{
    public GameObject hpIncreaseTextPrefab; // "HP 증가" 텍스트 프리팹

    public void ShowHPIncreaseText(Vector3 startPosition)
    {
        StartCoroutine(ShowHPIncreaseTextCoroutine(startPosition));
    }

    private IEnumerator ShowHPIncreaseTextCoroutine(Vector3 startPosition)
    {
        // 텍스트 생성
        GameObject hpText = Instantiate(hpIncreaseTextPrefab, startPosition + new Vector3(0, 0, 1) * 2, Quaternion.Euler(90, 0, 0));
        TextMeshPro hpTextMesh = hpText.GetComponent<TextMeshPro>();

        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 텍스트 위치를 플레이어 위로 이동
            hpText.transform.position = startPosition + new Vector3(0, 0, 1) * 2 + new Vector3(0, 0, 1) * elapsedTime;

            // 텍스트 투명도 조절
            Color color = hpTextMesh.color;
            color.a = Mathf.Lerp(1.0f, 0.0f, elapsedTime / duration);
            hpTextMesh.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 텍스트 오브젝트 파괴
        Destroy(hpText);
    }

    public void ShowMaxHPText(Vector3 startPosition)
    {
        StartCoroutine(ShowMaxHPTextCoroutine(startPosition));
    }

    private IEnumerator ShowMaxHPTextCoroutine(Vector3 startPosition)
    {
        // 텍스트 생성
        GameObject maxHPText = Instantiate(hpIncreaseTextPrefab, startPosition + new Vector3(0, 0, 1) * 2, Quaternion.Euler(90, 0, 0));
        TextMeshPro maxHPTextMesh = maxHPText.GetComponent<TextMeshPro>();
        maxHPTextMesh.text = "Max HP";

        maxHPTextMesh.transform.localScale = new Vector3(3, 3, 3);

        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 텍스트 위치를 플레이어 위로 이동
            maxHPText.transform.position = startPosition + new Vector3(0, 0, 1) * 2 + new Vector3(0, 0, 1) * elapsedTime;

            // 텍스트 투명도 조절
            Color color = maxHPTextMesh.color;
            color.a = Mathf.Lerp(1.0f, 0.0f, elapsedTime / duration);
            maxHPTextMesh.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 텍스트 오브젝트 파괴
        Destroy(maxHPText);
    }

    public void ShowInvTimeText(Vector3 startPosition)
    {
        StartCoroutine(ShowInvTimeTextCoroutine(startPosition));
    }

    private IEnumerator ShowInvTimeTextCoroutine(Vector3 startPosition)
    {
        // 텍스트 생성
        GameObject invTimeText = Instantiate(hpIncreaseTextPrefab, startPosition + new Vector3(0, 0, 1) * 2, Quaternion.Euler(90, 0, 0));
        TextMeshPro invTimeTextMesh = invTimeText.GetComponent<TextMeshPro>();
        invTimeTextMesh.text = "Inv Time +1sec";
        invTimeTextMesh.color = Color.yellow;

        invTimeText.transform.localScale = new Vector3(3, 3, 3);

        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 텍스트 위치를 플레이어 위로 이동
            invTimeText.transform.position = startPosition + new Vector3(0, 0, 1) * 2 + new Vector3(0, 0, 1) * elapsedTime;

            // 텍스트 투명도 조절
            Color color = invTimeTextMesh.color;
            color.a = Mathf.Lerp(1.0f, 0.0f, elapsedTime / duration);
            invTimeTextMesh.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 텍스트 오브젝트 파괴
        Destroy(invTimeText);
    }
}
