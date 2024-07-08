using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Games : MonoBehaviour
{
    public GameManager manager;
    public abstract void GameStart();
    public abstract void GameStop();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
