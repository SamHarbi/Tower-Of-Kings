using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Makes a GameObject with a RigidBody fall when a player enters trigger
    Used to collapse an area of the floor dropping the player to the lower levels
*/

public class fallingPlatform : MonoBehaviour
{
    public GameObject fallCutScene;
 
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 4.5f;
        fallCutScene.GetComponent<FallCutScene>().fallExitActivate();
    }
 
}
