using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDoorCheck : MonoBehaviour
{
    public GameObject Inventory;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Player")
        {
            Inventory inv = Inventory.GetComponent<Inventory>();
            if(inv.CheckForItem(0) && inv.CheckForItem(1) && inv.CheckForItem(2))
            {
                Destroy(gameObject);
                Debug.Log("Game Won");
            }
        }
    }
}
