using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using System.IO;
using System.Text;

public class GameSaveSystem : MonoBehaviour
{
    public GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameState CreateSaveGameObject(Vector2 PlayerPos)
    {
        GameState SaveState = new GameState();
        SaveState.playerPos = PlayerPos;

        return SaveState;
    }

    public void SaveGame()
    {
        // 1
        GameState save = CreateSaveGameObject(Player.transform.position);

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    { 
        // 1
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            GameState save = (GameState)bf.Deserialize(file);
            file.Close();

            Player.transform.position = save.playerPos;

            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }       



}

