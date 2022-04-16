using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    private GameObject Inventory;
    // Start is called before the first frame update
    void Start()
    {
        Inventory = GameObject.FindWithTag("Inventory");
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange" && Inventory.GetComponent<Inventory>().CheckForItem(3))
        {
            Destroy(gameObject);
        }
    }
}
