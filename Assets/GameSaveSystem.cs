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

*/

public class GameSaveSystem : MonoBehaviour
{
    public GameObject Player;
    public GameObject Inventory;
    public GameObject[] activeObjects;
    public Vector3[] ObjectsLocation;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGame()
    {
        Vector3 PlayerPos = Player.transform.position;
        
        using (var stream = File.Open("GameSave.DontTouch", FileMode.Create))
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
            {
                writer.Write(Player.GetComponent<Player>().getHealth());
                Debug.Log(Player.GetComponent<Player>().getHealth());
                
                writer.Write((int)PlayerPos.x);
                writer.Write((int)PlayerPos.y+1);
                writer.Write((int)PlayerPos.z);

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
                
                SaveObjectData();

                writer.Write((int)ObjectsLocation.Length);

                for(int i=0; i<ObjectsLocation.Length; i++)
                {
                    writer.Write((int)ObjectsLocation[i].x);
                    writer.Write((int)ObjectsLocation[i].y);
                }
            }
        }

        Debug.Log("Game Saved");
        //LoadGame();
    }

    public void LoadGame()
    {
        int LoadedPlayer_X;
        int LoadedPlayer_Y;
        int LoadedPlayer_Z;
        
        if (File.Exists("GameSave.DontTouch"))
        {
            using (var stream = File.Open("GameSave.DontTouch", FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    loadedHealth = reader.ReadInt32();
                    Debug.Log(loadedHealth);//test
                    
                    LoadedPlayer_X = reader.ReadInt32();
                    LoadedPlayer_Y = reader.ReadInt32();
                    LoadedPlayer_Z = reader.ReadInt32();

                    Player.transform.position = new Vector3(LoadedPlayer_X, LoadedPlayer_Y, LoadedPlayer_Z);

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

                    int loadedObjectsLocationLength = reader.ReadInt32();

                    activeObjects = GameObject.FindGameObjectsWithTag("Enemy");

                    AnimSystem.GetComponent<LogicalAnimationSystem>().deleteObjectAll();

                    for(int i=0; i<activeObjects.Length; i++)
                    {
                        Destroy(activeObjects[i]);
                    }
                        
                    for(int i=0; i<loadedObjectsLocationLength; i++)
                    {
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();

                        Instantiate(enemyPrefab, new Vector3(x, y+1, 0), Quaternion.identity);
                    }

                    //AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();
                    reLoadAnim = true;
                    Debug.Log("Game Loaded");
                }
            }
        }
    }

    void SaveObjectData()
    {
        activeObjects = GameObject.FindGameObjectsWithTag("Enemy");
        int arrayPoint = 0;
        ObjectsLocation = new Vector3[activeObjects.Length];

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
            AnimSystem.GetComponent<LogicalAnimationSystem>().updateAnimationList();
            reLoadAnim = false;
            Player.GetComponent<Player>().setHealth(loadedHealth, false);
        }
    }

}

