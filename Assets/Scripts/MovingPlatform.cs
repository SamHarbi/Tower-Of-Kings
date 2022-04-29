using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Moving platform logic- Constantly moves in a single direction and only changes direction if a collison with anything other than a player happens
*/

public class MovingPlatform : MonoBehaviour
{
    
    public int dir;

    // Update is called once per frame
    void Update()
    {
        //Move towards direction
        gameObject.transform.position = new Vector2(gameObject.transform.position.x + dir * Time.deltaTime, gameObject.transform.position.y);
    }

     void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag != "Player")
        {
            dir = dir * -1; //Change to opposite direction
        }
    }
}
