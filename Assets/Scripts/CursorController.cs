using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{

    public GameObject[] allButtons; //relevant buttons in menu
    public int pointer; //points to button
    public string tag; //tag that describes relevant buttons
    public Sprite clicked; //visual of clicked button
    public Sprite prevSprite; //sprite before button was clicked
    public GameObject gameSave; //game saving system

    private bool buttonClicked;
    private bool timeOut; //Is input timed out?
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        //get all buttons in relevant menu
        allButtons = GameObject.FindGameObjectsWithTag(tag);

        //point cursor to first item
        pointer = 0;
        Move(0);
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
              allButtons[pointer].GetComponent<SpriteRenderer>().sprite = prevSprite;
              buttonClicked = false;
              return;
         }
         
        if (Input.GetAxis("Dpad-Vertical") < 0 || Input.GetKey("a"))
        {
            pointer = Mathf.Abs((pointer-1)) % allButtons.Length;
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

    void Move(int mod)
    {
        Vector3 change = new Vector3(allButtons[mod].transform.position.x +2, allButtons[mod].transform.position.y -1);
        gameObject.transform.position = change;
        timeOut = true;

        startTime = Time.unscaledTime;
        StartCoroutine(delay()); 
    }

    void Select(int mod)
    {
        prevSprite = allButtons[mod].GetComponent<SpriteRenderer>().sprite;
        allButtons[mod].GetComponent<SpriteRenderer>().sprite = clicked;
        buttonClicked = true;
        timeOut = true;

        startTime = Time.unscaledTime;
        StartCoroutine(delay()); 

        if(mod == 0)
        {
            gameSave.GetComponent<GameSaveSystem>().SaveGame();
        }
        else if(mod == 1)
        {
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
        }
        else if(mod == 2)
        {
            MenuSettings.loadGame = true;
            SceneManager.LoadScene("StartMenu");
        }
        
    }

    IEnumerator delay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        timeOut = false;
    }
    
}
