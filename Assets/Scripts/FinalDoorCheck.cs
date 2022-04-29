using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Big door in mainhall, checks if player has all parts of the key before letting them in
*/

public class FinalDoorCheck : MonoBehaviour
{
    public GameObject Inventory;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            Inventory inv = Inventory.GetComponent<Inventory>();
            if(inv.CheckForItem(0) && inv.CheckForItem(1) && inv.CheckForItem(2))
            {
                Destroy(gameObject);
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Game Won");
                }
            }
        }
    }
}
