using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCursorController : MonoBehaviour
{
    public GameObject[] allButtons;
    public int pointer;
    public string tag;
    public Sprite[] clicked;
    public int timeOut;
    
    // Start is called before the first frame update
    void Start()
    {
        MenuSettings.loadGame = false;
        allButtons = GameObject.FindGameObjectsWithTag(tag);
        pointer = 0;
        Move(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(timeOut > 0)
         {
             timeOut--;
             return;
         }
        
        if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
         {
            pointer = Mathf.Abs((pointer+1) % allButtons.Length);
            Move(pointer);
         }
         else if(Input.GetButtonDown("Jump"))
         {
            Select(pointer);
         }
    }

    void Move(int mod)
    {
        Vector3 change = new Vector3(allButtons[mod].transform.position.x-3, allButtons[mod].transform.position.y);
        gameObject.transform.position = change;
        timeOut = 50;
    }

    void Select(int mod)
    {
        allButtons[mod].GetComponent<SpriteRenderer>().sprite = clicked[mod];

        if(mod == 0)
        {
            MenuSettings.loadGame = false;
            SceneManager.LoadScene("Main");
        }
        else if(mod == 1)
        {
            MenuSettings.loadGame = true;
            SceneManager.LoadScene("Main");
        }
        
    }
}
