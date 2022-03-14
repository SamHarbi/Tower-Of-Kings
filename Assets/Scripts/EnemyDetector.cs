using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    
    private bool playerView; //Which direction player is facing. left is 0, right is 1
    private GameObject enemy;
    private bool enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        playerView = false;
        enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setView(bool newView)
    {
        playerView = newView;
    }

    public void resetViewControl()
    {
       if(enemy != null)
       {
           enemy.GetComponent<SpriteRenderer>().enabled = false;
       }
    }

     void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "EnemyBlade" && playerView == true && enabled == false)
        {
            Debug.Log("Enemy Detected");
            enemy = col.gameObject;
            col.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            enabled = true;
        }
        else if(col.tag == "EnemyBlade" && playerView == false && enabled == true)
        {
            resetViewControl();
            enabled = false;

            col.GetComponent<EnemyBladeController>().CombatExit();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        //resetViewControl();
        //enabled = false;

    }

}
