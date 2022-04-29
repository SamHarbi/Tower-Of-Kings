using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.IO;
using System.Text;
using UnityEngine;

/*
    This Script was made by following a tutorial and thus very closely replicates a lot of
    code shown there 

    raywenderlich.com. (2017). How to Save and Load a Game in Unity. [online] Available at: 
    <https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity> [Accessed 24 April 2022].

    But due to recent updates on security issues with BinaryFormatter, this has been reworked to use the recommended alternative 
    
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
        
        //Open a BinaryWriter to a file 
        using (var stream = File.Open("GameSave.DontTouch", FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                //Write the player's health
                writer.Write(Player.GetComponent<Player>().getHealth());

                if (Debug.isDebugBuild)
                {
                    Debug.Log("Saved health of value: " + Player.GetComponent<Player>().getHealth());
                }
                
                //Write Players position
                writer.Write((int)PlayerPos.x);
                writer.Write((int)PlayerPos.y+1);
                writer.Write((int)PlayerPos.z);

                //Write if every item is in Inventory or not at time of saving game
                int itemNum = Inventory.GetComponent<Inventory>().InventoryItems.Length;
                for(int i=0; i<itemNum; i++)
                {
                    if(Inventory.GetComponent<Inventory>().CheckForItem(i))
                    {
                        writer.Write(1);
                    }
                    else
                    {
                        writer.Write(0);
                    }
                }
                
                //Save Enemy info
                SaveObjectData();

                //Write how many Enemies have been saved
                writer.Write((int)ObjectsLocation.Length);

                //Write Vector2 cords as int for every enemy
                for(int i=0; i<ObjectsLocation.Length; i++)
                {
                    writer.Write((int)ObjectsLocation[i].x);
                    writer.Write((int)ObjectsLocation[i].y);
                }
            }
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
        int LoadedPlayer_X;
        int LoadedPlayer_Y;
        int LoadedPlayer_Z;
        
        //If file exists open a reading stream to it
        if (File.Exists("GameSave.DontTouch"))
        {
            using (var stream = File.Open("GameSave.DontTouch", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    //load health 
                    loadedHealth = reader.ReadInt32();
                    Player.GetComponent<Player>().setHealth(loadedHealth, false);

                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Loaded health of value: " + loadedHealth);
                    }
                    
                    //Load Player's position
                    LoadedPlayer_X = reader.ReadInt32();
                    LoadedPlayer_Y = reader.ReadInt32();
                    LoadedPlayer_Z = reader.ReadInt32();

                    //Set Player's Position
                    Player.transform.position = new Vector3(LoadedPlayer_X, LoadedPlayer_Y, LoadedPlayer_Z);

                    //load checks that state if each item in inventory was picked up or not
                    int itemNum = Inventory.GetComponent<Inventory>().InventoryItems.Length;
                    for(int i=0; i<itemNum; i++)
                    {
                        int itemState = reader.ReadInt32();

                        if(itemState == 1)
                        {
                            if(Inventory.GetComponent<Inventory>().checkIfItemExists(i))
                            {
                                Inventory.GetComponent<Inventory>().addItem(i);
                            }
                        }
                    }

                    //Number of enemies loaded
                    int loadedObjectsLocationLength = reader.ReadInt32();

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
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();

                        Instantiate(enemyPrefab, new Vector3(x, y+1, 0), Quaternion.identity);
                    }

                    //AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();
                    reLoadAnim = true; //Animation System needs to be reloaded
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Game Loaded");
                    }
                }
            }
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
            if(activeObjects[i].active == true && activeObjects[i].GetComponent<AIController>().wrapperOverride == false)
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

