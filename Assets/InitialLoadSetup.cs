using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialLoadSetup : MonoBehaviour
{
    
    public GameObject gameSave;
    public GameObject AnimSystem;
    public GameObject loadingScreen;
    
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
        loadingScreen.SetActive(true);
        //AnimSystem.GetComponent<LogicalAnimationSystem>().deleteObjectAll();
        yield return new WaitForSeconds(1.0f);
        if(MenuSettings.loadGame == true)
        {
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
            yield return new WaitForSeconds(1.0f);
        }
        AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();

        if(MenuSettings.loadGame == false)
        {
            gameSave.GetComponent<GameSaveSystem>().SaveGame();
            yield return new WaitForSeconds(1.0f);
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
            yield return new WaitForSeconds(1.0f);
            AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();
        }
        loadingScreen.SetActive(false);
        yield return null;
    }
}
