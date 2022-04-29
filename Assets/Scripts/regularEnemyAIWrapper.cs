using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class regularEnemyAIWrapper : MonoBehaviour
{
    public AIController Controller; //AI Controller that is being wrapped
    
    void Awake()
    {
        Controller = gameObject.GetComponent<AIController>();
        Controller.WrappedAwake();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Controller.WrappedStart();
    }

    // Update is called once per frame
    void Update()
    {
        Controller.WrappedUpdate();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Controller.WrappedOnTriggerEnter2D(col);
    }

}
