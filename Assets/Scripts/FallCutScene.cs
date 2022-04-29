using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Makes a GameObject with a RigidBody fall/collapse when fallExitActivate is called
    Used to open an exit to other levels from lowerlevel
*/

public class FallCutScene : MonoBehaviour
{
    public GameObject[] fallExit;

    public void fallExitActivate()
    {
        for(int i=0; i<fallExit.Length; i++)
        {
            fallExit[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }
    
}
