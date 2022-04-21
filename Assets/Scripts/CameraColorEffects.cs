using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColorEffects : MonoBehaviour
{
    public Color midLevelColor;
    public Color lowerLevelColor;
    public Color upperLevelColor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.transform.position.y > 81f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = midLevelColor;
        }
        else
        {
            gameObject.GetComponent<Camera>().backgroundColor = lowerLevelColor;
        }

        if(gameObject.transform.position.y > 143f)
        {
            gameObject.GetComponent<Camera>().backgroundColor = upperLevelColor;
        }
    }
}
