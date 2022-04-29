using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Start's a bounce animation when player makes contact with pillow

public class PillowLogic : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.transform.tag == "Player")
        {
            gameObject.GetComponent<AnimationData>().Running = true;
        }
    }

}
