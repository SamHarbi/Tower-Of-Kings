using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool pauseState;
    public GameObject GameSave;
    
    //Inspired By 
    /*
        French, J. (2020). The right way to pause a game in Unity. 
        [online] Game Dev Beginner. Available at: https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/.

    */

    //No longer used as Pausing was integrated with Inventory, but this class remains useful to test pure pause functionality without any overhead 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
           if(pauseState == false)
           {
               Time.timeScale = 0;
               pauseState = true;
           }
           else
           {
               Time.timeScale = 1;
               pauseState = false;
               GameSave.GetComponent<GameSaveSystem>().UnPause();
           }
        }
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        pauseState = true;
    }

}
