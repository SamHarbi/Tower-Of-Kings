using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    
    public GameObject Player;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "AttackRange")
        {
            Player.GetComponent<Rigidbody2D>().AddForce(transform.right * 5);
            Debug.Log("ATTACK");
        }
    }
}
