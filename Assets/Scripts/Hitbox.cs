using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    
    public GameObject Player;

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
        if(col.tag == "AttackRange")
        {
            Player.GetComponent<Rigidbody2D>().AddForce(transform.right * 5);
            Debug.Log("ATTACK");
        }
    }
}
