using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCutScene : MonoBehaviour
{
    public GameObject[] fallExit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fallExitActivate()
    {
        for(int i=0; i<fallExit.Length; i++)
        {
            fallExit[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    
}
