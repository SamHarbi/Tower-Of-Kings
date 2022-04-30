using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
    Controls menu cursor, which selects options and calls code for each options
    This class would fit very well into a Strategy Pattern. With if(menu) being replaced by another strategy. 
*/

public class CursorController : MonoBehaviour
{

    public GameObject[] allButtons; //relevant buttons in menu
    public int pointer; //points to button
    public string buttonTag; //tag that describes relevant buttons
    public Sprite clicked; //visual of clicked button
    public Sprite[] menuClicked; //visual of clicked button
    public Sprite prevSprite; //sprite before button was clicked
    public GameObject gameSave; //game saving system
    public bool menu; //Is this cursor part of main menu?

    private bool buttonClicked;
    private bool timeOut; //Is input timed out?
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        if(menu == false)
        {
            //get all buttons in relevant menu
            allButtons = GameObject.FindGameObjectsWithTag(buttonTag);

            //point cursor to first item
            pointer = 0;
            Move(0);
        }
        else
        {
            MenuSettings.loadGame = false; //reset value

            //get all buttons in relevant menu
            //allButtons = GameObject.FindGameObjectsWithTag(buttonTag);
            pointer = 0;
            Move(0);
        }

        buttonClicked = false;
        timeOut = false;
    }

    // Update is called once per frame
    void Update()
    {
         //ignore input if
         if(timeOut == true)
         {
             return;
         }
         else if(timeOut == false && buttonClicked == true)
         {
              //Change button to a clicked visual
              allButtons[pointer].GetComponent<SpriteRenderer>().sprite = prevSprite;
              buttonClicked = false;
              return;
         }
         
        if (Input.GetAxis("Dpad-Vertical") < 0 || Input.GetKey("a"))
        {
            pointer = pointer-1;
            if(pointer < 0) //handle negative pointers
            {
                pointer = allButtons.Length-1; //loop back
            }
            Move(pointer);
        }
        else if(Input.GetAxis("Dpad-Vertical") > 0 || Input.GetKey("d"))
         {
            pointer = Mathf.Abs((pointer+1) % allButtons.Length);
            Move(pointer);
         }
         else if(Input.GetButtonDown("Jump"))
         {
             Select(pointer);
         }
    }

    //Move the cursor to point at button mod
    void Move(int mod)
    {
        if(menu == false)
        {
            Vector3 change = new Vector3(allButtons[mod].transform.position.x +2, allButtons[mod].transform.position.y -1);
            gameObject.transform.position = change;
        }
        else
        {
            Vector3 change = new Vector3(allButtons[mod].transform.position.x-3, allButtons[mod].transform.position.y);
            gameObject.transform.position = change;
        }

        timeOut = true;
        startTime = Time.unscaledTime;
        StartCoroutine(delay()); //Delay before another action can be taken 
        
    }

    //Logic for what happens when button is selected, mod dictates two alternative strategies
    void Select(int mod)
    {
        if(menu == false)
        {
            prevSprite = allButtons[mod].GetComponent<SpriteRenderer>().sprite;
            allButtons[mod].GetComponent<SpriteRenderer>().sprite = clicked;
        }
        else
        {
            allButtons[mod].GetComponent<SpriteRenderer>().sprite = menuClicked[mod];
        }
        
        buttonClicked = true;
        timeOut = true;

        startTime = Time.unscaledTime;
        StartCoroutine(delay()); 

        if(mod == 0)
        {
            if(menu == false)
            {
                gameSave.GetComponent<GameSaveSystem>().SaveGame();
            }
            else
            {
                MenuSettings.loadGame = false;
                SceneManager.LoadScene("Main");
            }
        }
        else if(mod == 1)
        {
            if(menu == false)
            {
                gameSave.GetComponent<GameSaveSystem>().LoadGame();
            }
            else
            {
                MenuSettings.loadGame = true;
                SceneManager.LoadScene("Main");
            }
        }
        else if(mod == 2)
        {
            if(menu == false)
            {
                MenuSettings.loadGame = true;
                SceneManager.LoadScene("StartMenu");
            }
            else
            {
                Application.Quit(); 
            }
        }
        
    }

    IEnumerator delay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        timeOut = false;
    }
    
}
