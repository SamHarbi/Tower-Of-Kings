using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Initial Setup that runs when game is started- figures out if game needs to be loaded or not
*/

public class InitialLoadSetup : MonoBehaviour
{
    
    public GameObject gameSave; //game Save System
    public GameObject AnimSystem; //Logical Animation System
    public GameObject loadingScreen;
    public GameObject Player;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(loadDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator loadDelay()
    {
        //Show loading Screen and stop all Player Input
        loadingScreen.SetActive(true);
        Player.GetComponent<Player>().loading = true;
        
        //AnimSystem.GetComponent<LogicalAnimationSystem>().deleteObjectAll();

        //Ensure that Time is at 1
        Time.timeScale = 1;

        yield return new WaitForSeconds(1.0f);
        if(MenuSettings.loadGame == true) //Game needs to be loaded
        {
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
            yield return new WaitForSeconds(1.0f);
        }
        AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList(); //Rebuild AnimationData Array

        if(MenuSettings.loadGame == false) //This is a new game- needs to be saved
        {
            //Save Game
            gameSave.GetComponent<GameSaveSystem>().SaveGame();
            yield return new WaitForSeconds(1.0f);

            //Load Game
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
            yield return new WaitForSeconds(1.0f);

            AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList(); //Rebuild AnimationData Array
        }

        //Loading is done, return control to player
        loadingScreen.SetActive(false);
        Player.GetComponent<Player>().loading = false;
        yield return null;
    }
}
