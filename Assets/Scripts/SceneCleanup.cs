using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Destroys anything that enters trigger, used to clean up boss projectiles once they pass out of the map to save resources
*/

public class SceneCleanup : MonoBehaviour
{
     void OnTriggerEnter2D(Collider2D col)
    {
        Destroy(col.gameObject);
    }
}
