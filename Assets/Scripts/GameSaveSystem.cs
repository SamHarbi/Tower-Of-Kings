using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

/*
    This Script was made by following a tutorial and thus very closely replicates a lot of
    code shown there 

    raywenderlich.com. (2017). How to Save and Load a Game in Unity. [online] Available at: 
    <https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity> [Accessed 24 April 2022].

    But due to recent updates on security issues with BinaryFormatter, this has been heavily reworked to use the recommended alternative 
    
    GrabYourPitchforks (03/11/2022). Deserialization risks in use of BinaryFormatter and related types. [online] docs.microsoft.com. Available at: 
    https://docs.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-security-guide.

    Using some code from the examples here

    dotnet-bot (n.d.). BinaryReader Class (System.IO). [online] docs.microsoft.com. Available at: 
    https://docs.microsoft.com/en-us/dotnet/api/system.io.binaryreader?view=net-6.0 [Accessed 24 Apr. 2022].

    This script handles saving and loading 

*/

public class GameSaveSystem : MonoBehaviour
{
    public GameObject Player; 
    public GameObject Inventory;
    public GameObject[] activeObjects; //Active enemies that have not been killed
    public Vector3[] ObjectsLocation; //Enemy positions
    public GameObject enemyPrefab; 
    public GameObject AnimSystem;

    private bool reLoadAnim;
    private int loadedHealth;

    // Start is called before the first frame update
    void Start()
    {
        reLoadAnim = false;
        loadedHealth = -1;
    }

    public void SaveGame()
    {
        Vector3 PlayerPos = Player.transform.position;
        

        //Write the player's health
        PlayerPrefs.SetInt("player_health", Player.GetComponent<Player>().getHealth());

        if (Debug.isDebugBuild)
        {
            Debug.Log("Saved health of value: " + Player.GetComponent<Player>().getHealth());
        }
                
        //Write Players position
        PlayerPrefs.SetFloat("player_x", PlayerPos.x);
        PlayerPrefs.SetFloat("player_y", PlayerPos.y+1);
        PlayerPrefs.SetFloat("player.z", PlayerPos.z);

        //Write if every item is in Inventory or not at time of saving game
        int itemNum = Inventory.GetComponent<Inventory>().InventoryItems.Length;
        for(int i=0; i<itemNum; i++)
        {
            if(Inventory.GetComponent<Inventory>().CheckForItem(i))
            {
                PlayerPrefs.SetInt("inv_"+i, 1);
            }
            else
            {
                PlayerPrefs.SetInt("inv_"+i, 0);
            }
        }
                
        //Save Enemy info
        SaveObjectData();

        //Write how many Enemies have been saved
        PlayerPrefs.SetInt("active_enemies", ObjectsLocation.Length);

        //Write Vector2 cords as int for every enemy
        for(int i=0; i<ObjectsLocation.Length; i++)
        {
            PlayerPrefs.SetFloat("enemy_x_"+i, ObjectsLocation[i].x);
            PlayerPrefs.SetFloat("enemy_y_"+i, ObjectsLocation[i].y);
        }


        if (Debug.isDebugBuild)
        {
            Debug.Log("Game Saved");
        }
        //LoadGame();
    }

    public void LoadGame()
    {
        //Ref's to player's position
        float LoadedPlayer_X;
        float LoadedPlayer_Y;
        float LoadedPlayer_Z;
        
        //load health 
        loadedHealth = PlayerPrefs.GetInt("player_health");
        Player.GetComponent<Player>().setHealth(loadedHealth, false);

        if (Debug.isDebugBuild)
        {
            Debug.Log("Loaded health of value: " + loadedHealth);
        }
                    
        //Load Player's position
        LoadedPlayer_X = PlayerPrefs.GetFloat("player_x");
        LoadedPlayer_Y = PlayerPrefs.GetFloat("player_y");
        LoadedPlayer_Z = PlayerPrefs.GetFloat("player_z");

        //Set Player's Position
        Player.transform.position = new Vector3(LoadedPlayer_X, LoadedPlayer_Y, LoadedPlayer_Z);

        //load checks that state if each item in inventory was picked up or not
        int itemNum = Inventory.GetComponent<Inventory>().InventoryItems.Length;
        for(int i=0; i<itemNum; i++)
        {
            int itemState = PlayerPrefs.GetInt("inv_"+i);

            if(itemState == 1)
            {
                if(Inventory.GetComponent<Inventory>().checkIfItemExists(i))
                {
                    Inventory.GetComponent<Inventory>().addItem(i);
                }
            }
        }

        //Number of enemies loaded
        int loadedObjectsLocationLength = PlayerPrefs.GetInt("active_enemies");

        //Wipe Animation System's AnimationData array's
        AnimSystem.GetComponent<LogicalAnimationSystem>().deleteObjectAll();

        //Destroy each enemy in scene
        activeObjects = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i=0; i<activeObjects.Length; i++)
        {
            Destroy(activeObjects[i]);
        }
                    
        //Read each X,Y int pair and assign it as a position to newly created enemy
        for(int i=0; i<loadedObjectsLocationLength; i++)
        {
            float x = PlayerPrefs.GetFloat("enemy_x_"+i);
            float y = PlayerPrefs.GetFloat("enemy_y_"+i);

            Instantiate(enemyPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }

        //AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();
        reLoadAnim = true; //Animation System needs to be reloaded
        if (Debug.isDebugBuild)
        {
            Debug.Log("Game Loaded");
        }

    }

    void SaveObjectData()
    {
        //Find every enemy in scene and create a vector3 array for each
        activeObjects = GameObject.FindGameObjectsWithTag("Enemy");
        int arrayPoint = 0;
        ObjectsLocation = new Vector3[activeObjects.Length];

        //Put only active enemies cordinates into vector3 array
        for(int i=0; i<activeObjects.Length; i++)
        {
            if(activeObjects[i].activeSelf == true && activeObjects[i].GetComponent<AIController>().wrapperOverride == false)
            {
                ObjectsLocation[arrayPoint] = activeObjects[i].transform.position;
                arrayPoint++;
                //Destroy(activeObjects[i]);
            }
        }
    }

    public void UnPause()
    {
        if(reLoadAnim == true)
        {
            //Update Animation list to remove null references
            AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList(); //reload Animation system / remake array of all AnimationData in scene
            reLoadAnim = false;
        }
    }

}

