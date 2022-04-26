using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    
    public int dir;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector2(gameObject.transform.position.x + dir * Time.deltaTime, gameObject.transform.position.y);
    }

     void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.tag != "Player")
        {
            dir = dir * -1;
        }
    }
}
