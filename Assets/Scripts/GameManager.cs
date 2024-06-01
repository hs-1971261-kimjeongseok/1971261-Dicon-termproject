using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapArray
{
    public GameObject[] map;
    public void setArray(GameObject[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            array[i] = map[i];
        }
    }
}
public class GameManager : MonoBehaviour
{
    public MapArray[] allCubes; // 0 시작 1 서 2 동 3 북 4 남 5 아래
    public GameObject[] currentCubes = new GameObject[16];
    public AudioClip[] musics; //0은 항상 플레이됨, 1~4 중 하나와 5~8 중 하나가 플레이됨
    public Transform[] planePositions = new Transform[16];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
