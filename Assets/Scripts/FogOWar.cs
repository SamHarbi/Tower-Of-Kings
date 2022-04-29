using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Visually Hides areas of the map until Player enters trigger and this object is destroyed
*/

public class FogOWar : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
