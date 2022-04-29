using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This class is a wrapper that wraps AIController, see that class or the report for more info on the pattern 
*/

public class regularEnemyAIWrapper : MonoBehaviour
{
    public AIController Controller; //AI Controller that is being wrapped
    
    void Awake()
    {
        Controller = gameObject.GetComponent<AIController>();
        Controller.WrappedAwake(); //Pass on to wrapped
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Controller.WrappedStart(); //Pass on to wrapped
    }

    // Update is called once per frame
    void Update()
    {
        Controller.WrappedUpdate(); //Pass on to wrapped
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Controller.WrappedOnTriggerEnter2D(col); //Pass on to wrapped
    }

}
