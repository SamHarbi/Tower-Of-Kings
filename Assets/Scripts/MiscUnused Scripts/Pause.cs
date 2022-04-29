using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool pauseState;
    public GameObject GameSave;
    
    //Inspired By https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/
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
