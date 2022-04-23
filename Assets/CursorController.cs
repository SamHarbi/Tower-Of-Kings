using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    
    private float deltaTime;
    private float lastDeltaTime;
    public GameObject[] allButtons;
    private int pointer;
    public string tag;
    public int timeOut;

    // Start is called before the first frame update
    void Start()
    {
        allButtons = GameObject.FindGameObjectsWithTag(tag);
        pointer = 0;
        Move(0);
        
        deltaTime = Time.unscaledTime;
        lastDeltaTime = deltaTime;
        pointer = 0;
    }

    // Update is called once per frame
    void Update()
    {
         if(timeOut > 0)
         {
             timeOut--;
             return;
         }
         
         if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
         {
            pointer = pointer - 1;
            Move(pointer);
         }
         else if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
         {
            pointer = pointer + 1;
            Move(pointer);
         }
         
         deltaTime = Time.unscaledTime - lastDeltaTime;
         lastDeltaTime = Time.unscaledTime;
    }

    void Move(int mod)
    {
        mod = Mathf.Abs(mod % allButtons.Length);
        Vector3 change = new Vector3(allButtons[mod].transform.position.x +2, allButtons[mod].transform.position.y -1);
        gameObject.transform.position = change;
        timeOut = 50;
    }
}
