using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    
    private bool playerView; //Which direction player is facing. 1 if direction of this script 
    public bool realDirection; // The Direction the player is actually facing in the world

    public GameObject panel;
    public GameObject blade;
    private GameObject enemy;
    private bool enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        playerView = false;
        enabled = false;
        realDirection = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setView(bool newView, bool newRealDirection)
    {
        playerView = newView;
        realDirection = newRealDirection;
        if(enemy != null)
        {
            viewCleanUp(enemy.GetComponent<Collider2D>());//clean up
        }
    }

    public void resetViewControl()
    {
       if(enemy != null)
       {
           enemy.GetComponent<SpriteRenderer>().enabled = false;
           enabled = false;
       }
    }

     void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Enemy" && playerView == true && enabled == false)
        {
            if(col.gameObject.GetComponent<Transform>().childCount > 0)
            {
                enemy = col.gameObject.GetComponent<Transform>().GetChild(0).gameObject;
            }
            else
            {
                enemy = panel.gameObject.GetComponent<Transform>().GetChild(6).gameObject;
            }
            
            if(enemy.transform.GetComponent<EnemyBladeController>().direction != realDirection)
            {
                Debug.Log("Enemy Detected");
                enemy.GetComponent<SpriteRenderer>().enabled = true;
                enabled = true;
            }
            else
            {
                
                viewCleanUp(col);
            }
        }
        else if(col.tag == "Enemy" && playerView == false && enabled == true)
        {
            
            viewCleanUp(col);
        }
    }

    void viewCleanUp(Collider2D col)
    {
        if(col.gameObject.GetComponent<Transform>().childCount > 0)
        {
            enemy = col.gameObject.GetComponent<Transform>().GetChild(0).gameObject;
            enemy.GetComponent<EnemyBladeController>().CombatExit();
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            enabled = false;
            enabled = false;
        }
        else if(panel.GetComponent<Transform>().childCount > 6)
        {
            enemy = panel.GetComponent<Transform>().GetChild(6).gameObject;
            enemy.GetComponent<EnemyBladeController>().CombatExit();
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            enabled = false;
            enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        enemy = col.gameObject;
        viewCleanUp(col);
    }

}
