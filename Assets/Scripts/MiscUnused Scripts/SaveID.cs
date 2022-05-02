using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Not used, was created to test how best to save enemy data
    Structure Inspired by 
    
    raywenderlich.com. 2017. How to Save and Load a Game in Unity. 
    [online] Available at: <https://www.raywenderlich.com/418-how-to-save-and-load-a-game-in-unity> [Accessed 24 April 2022].
*/


public class SaveID : MonoBehaviour
{
    
    public GameObject[] activeObjects;
    public Vector3[] ObjectsLocation;
    
    // Start is called before the first frame update
    void SaveObjectData()
    {
        activeObjects = GameObject.FindGameObjectsWithTag("Enemy");
        int arrayPoint = 0;
        ObjectsLocation = new Vector3[activeObjects.Length];

        for(int i=0; i<activeObjects.Length; i++)
        {
            if(activeObjects[i].activeSelf == true && activeObjects[i].GetComponent<AIController>().wrapperOverride == false)
            {
                ObjectsLocation[arrayPoint] = activeObjects[i].transform.position;
                arrayPoint++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
