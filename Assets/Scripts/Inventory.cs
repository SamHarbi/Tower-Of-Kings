using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Singleton Pattern Class- Only one instance of Inventory Exists and it manages all items picked up by player and displays it in a UI implementation
    
    Awake Method copied from 
    French, J. (2021). Singletons in Unity (done right). [online] Game Dev Beginner. 
    Available at: https://gamedevbeginner.com/singletons-in-unity-the-right-way/ [Accessed 1 May 2022].
*/

public class Inventory : MonoBehaviour
{
    public static Inventory inventory;
    
    public GameObject InventoryPanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    public GameObject[] InventoryItems;
    public GameObject GameSave; //Game Save System

    private bool pauseStatus; //Is game paused?
    private int openPanelID; //Which panel is open
    private bool timeOut;

    private void Awake() 
    { 
        //If there is another LAS Instance, delete this one
        if (inventory != null && inventory != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            inventory = this; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        openPanelID = 1; //Inventory Panel is open first
        timeOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
           if(pauseStatus == false) //if game not already paused
           {
               InventoryPanel.SetActive(true); //Show first panel in pause menu
               pauseGame();
           }
           else //else unpause game 
           {
               InventoryPanel.gameObject.SetActive(false);
               settingsPanel.gameObject.SetActive(false);
               helpPanel.gameObject.SetActive(false);
               pauseGame();
           }
        }

        if(timeOut == true)
        {
            return; //ignore all input below
        }

        //Open next panel  
        if (Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("w") || Input.GetButtonDown("subup"))
        {
            if(pauseStatus == true)
            {
                openPanelID++;
                scrollTabs();
                StartCoroutine(delay()); 
            }
        }

        //Open prev panel
        if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("s") || Input.GetButtonDown("subdown"))
        {
            if(pauseStatus == true)
            {
                openPanelID--;
                scrollTabs();
                StartCoroutine(delay()); 
            }
        }
    }

    IEnumerator delay()
    {
        timeOut = true;
        yield return new WaitForSecondsRealtime(0.2f);
        timeOut = false;
    }

    //Show a panel based on openPanelID
    void scrollTabs()
    {
        //turn off all panels
        InventoryPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        helpPanel.gameObject.SetActive(false);

        //Pick a panel to show
        switch(openPanelID % 4)
        {
            case 1:
                InventoryPanel.gameObject.SetActive(true);
                openPanelID = 1;
                break;
            
            case 2:
                settingsPanel.gameObject.SetActive(true);
                openPanelID = 2;
                break;
            case 3:
                helpPanel.gameObject.SetActive(true);
                openPanelID = 3;
                break;
            case 0:
                InventoryPanel.gameObject.SetActive(true);
                openPanelID = 1;
                break;  
        }
    }

    //Pauses the game by setting the time to 0
    public void pauseGame()
    {
        if(pauseStatus == false)
           {
               //pause game
               Time.timeScale = 0;
               pauseStatus = true;
           }
        else
           {
               //unpause game
               Time.timeScale = 1;
               pauseStatus = false;
               GameSave.GetComponent<GameSaveSystem>().UnPause();
           }
    }

    //Add an item to Inventory
    public void addItem(int id)
    {
        InventoryItems[id].SetActive(true);
    }

    //Check if an item in in Inventory
    public bool CheckForItem(int id)
    {
        if(InventoryItems[id] == null) //if no item tied to slot
        {
            return false;
        }
        
        //else check if item is in slot
        if(InventoryItems[id].activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Check if an Inventory slot belongs to an item (this is needed because there are more slots than items)
    public bool checkIfItemExists(int id)
    {
        if(InventoryItems[id] == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
