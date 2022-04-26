using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if(activeObjects[i].active == true && activeObjects[i].GetComponent<AIController>().wrapperOverride == false)
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
