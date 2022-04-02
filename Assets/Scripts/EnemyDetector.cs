using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    
    private bool playerView; //Is Player looking towards the same direction as the collider of this scripts GameObject
    public bool realDirection; // The Direction the player is actually facing in the world

    public GameObject panel;
    public GameObject blade;
    public GameObject enemy;
    private bool enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        playerView = false; //Player is not looking towards direction of this collider
        enabled = false; 

        realDirection = false; //Player alway's starts the game looking left
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Saves changes in Players view / direction changes
    public void setView(bool newView, bool newRealDirection)
    {
        playerView = newView;
        realDirection = newRealDirection;
    }

    public bool getView()
    {
        return realDirection;
    }

    //Erase Panel view - this is usually called when player direction is changed
    public void resetViewControl()
    {
       if(enemy != null && enemy.tag == "Enemy")
       {
           enemy.GetComponent<SpriteRenderer>().enabled = false;
           enabled = false;
       }
    }

    //If an enemy enters and stays in the detector area
     void OnTriggerStay2D(Collider2D col)
    {
         // Player is looking towards this detector and an enemy is within that detector
        if(col.tag == "Enemy" && playerView == true && enabled == false)
        {
            if(col.gameObject.GetComponent<Transform>().childCount > 0) //Check if entity has a blade
            {
                enemy = col.gameObject.GetComponent<Transform>().GetChild(0).gameObject; //Get the enemy blade and hold a ref to it
            }
            
            if(enemy.transform.GetComponent<EnemyBladeController>().getDirection() != realDirection) //Player and enemy face each other
            {
                Debug.Log("Enemy Detected");
                enemy.GetComponent<SpriteRenderer>().enabled = true; //Enable Enemies blade, allowing it to appear in combat panel
                enabled = true;
            }
            else
            {
                viewCleanUp(col); //Enemy blade view must be reset, enemy not looking in Players direction
            }
        }
        else if(col.tag == "Enemy" && playerView == false && enabled == true) //Player stops looking in direction of detector
        {
            viewCleanUp(col); //Enemy blade view must be reset
        }
    }

    void viewCleanUp(Collider2D col)
    {
        if(col.gameObject.GetComponent<Transform>().childCount > 0)
        {
            enemy = col.gameObject.GetComponent<Transform>().GetChild(0).gameObject;
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            enabled = false;
            enabled = false;
        }
    }

    //Enemy leaves detector range
    void OnTriggerExit2D(Collider2D col)
    {
        enemy = col.gameObject;
        viewCleanUp(col); //Enemy blade view must be reset
    }

}
