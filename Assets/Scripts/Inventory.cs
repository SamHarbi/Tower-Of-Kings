using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject settingsPanel;
    public GameObject helpPanel;
    private bool pauseStatus;
    private int openPanelID;
    public GameObject[] InventoryItems;

    // Start is called before the first frame update
    void Start()
    {
        openPanelID = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
           if(pauseStatus == false)
           {
               InventoryPanel.SetActive(true);
               pauseGame();
           }
           else
           {
               InventoryPanel.gameObject.SetActive(false);
               settingsPanel.gameObject.SetActive(false);
               helpPanel.gameObject.SetActive(false);
               pauseGame();
           }
        }

        if (Input.GetButtonDown("Change-Tab-Up") && pauseStatus == true)
        {
            openPanelID++;
            scrollTabs();
        }

        if (Input.GetButtonDown("Change-Tab-Down") && pauseStatus == true)
        {
            openPanelID--;
            scrollTabs();
        }
    }

    void scrollTabs()
    {
        InventoryPanel.gameObject.SetActive(false);
        settingsPanel.gameObject.SetActive(false);
        helpPanel.gameObject.SetActive(false);

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

    public void pauseGame()
    {
        if(pauseStatus == false)
           {
               Time.timeScale = 0;
               pauseStatus = true;
           }
        else
           {
               Time.timeScale = 1;
               pauseStatus = false;
           }
    }

    public void addItem(int id)
    {
        InventoryItems[id].SetActive(true);
    }

    public bool CheckForItem(int id)
    {
        if(InventoryItems[id] == null)
        {
            return false;
        }
        
        if(InventoryItems[id].activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

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
