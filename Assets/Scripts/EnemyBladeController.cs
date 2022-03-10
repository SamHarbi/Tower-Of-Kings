using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBladeController : MonoBehaviour
{
    public GameObject UIAlt;
    private Component SR;


    // Start is called before the first frame update
    void Start()
    {
        UIAlt.SetActive(false);

        SR = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "CombatEnter")
        {
            UIAlt.SetActive(true);
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if(col.tag == "CombatExit")
        {
            UIAlt.SetActive(false);
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        
    }

}
