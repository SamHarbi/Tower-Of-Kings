using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == "Player")
        {
            gameObject.GetComponent<AnimationData>().Running = true;
        }
    }

}
