using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Logic that allows walls to be broken by a pickaxe 
*/

public class Breakable : MonoBehaviour
{
    private GameObject Inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        Inventory = GameObject.FindWithTag("Inventory");
    }
    
    //If Object is hit by player who has a pickaxe item in thier Inventory
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange" && Inventory.GetComponent<Inventory>().CheckForItem(3))
        {
            Destroy(gameObject);
        }
    }
}
