using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    
    private float deltaTime; //How much time has passed
    private float lastDeltaTime;
    public GameObject[] allButtons;
    public int pointer;
    public string tag;
    public int timeOut; //Time for how long should input be ignored
    public Sprite clicked;
    public Sprite prevSprite;
    private bool buttonClicked;
    public GameObject gameSave;

    // Start is called before the first frame update
    void Start()
    {
        //get all buttons in relevant menu
        allButtons = GameObject.FindGameObjectsWithTag(tag);

        //point cursor to first item
        pointer = 0;
        Move(0);
        buttonClicked = false;
        
        //Calculate own deltaTime based on how much time has passed since game started
        deltaTime = Time.unscaledTime;
        lastDeltaTime = deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
         //ignore input if
         if(timeOut > 1)
         {
             timeOut--; //countdown
             return;
         }
         else if(timeOut == 1 && buttonClicked == true)
         {
              allButtons[pointer].GetComponent<SpriteRenderer>().sprite = prevSprite;
              timeOut--;
              buttonClicked = false;
              return;
         }
         
         /*if (Input.GetAxis("Dpad-Horizontal") < 0 || Input.GetKey("a"))
         {
            pointer = Mathf.Abs((pointer-1) % allButtons.Length);
            Move(pointer);
         }*/
        if(Input.GetAxis("Dpad-Vertical") > 0 || Input.GetKey("d"))
         {
            pointer = Mathf.Abs((pointer+1) % allButtons.Length);
            Move(pointer);
         }
         else if(Input.GetButtonDown("Jump"))
         {
             Select(pointer);
         }
         
         deltaTime = Time.unscaledTime - lastDeltaTime;
         lastDeltaTime = Time.unscaledTime;
    }

    void Move(int mod)
    {
        Vector3 change = new Vector3(allButtons[mod].transform.position.x +2, allButtons[mod].transform.position.y -1);
        gameObject.transform.position = change;
        timeOut = 50;
    }

    void Select(int mod)
    {
        prevSprite = allButtons[mod].GetComponent<SpriteRenderer>().sprite;
        allButtons[mod].GetComponent<SpriteRenderer>().sprite = clicked;
        buttonClicked = true;
        timeOut = 50;

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
}
