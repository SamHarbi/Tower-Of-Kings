using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    
    private float deltaTime;
    private float lastDeltaTime;
    public GameObject[] allButtons;
    public int pointer;
    public string tag;
    public int timeOut;
    public Sprite clicked;
    public Sprite prevSprite;
    private bool buttonClicked;
    public GameObject gameSave;

    // Start is called before the first frame update
    void Start()
    {
        allButtons = GameObject.FindGameObjectsWithTag(tag);
        pointer = 0;
        Move(0);
        
        deltaTime = Time.unscaledTime;
        lastDeltaTime = deltaTime;
        pointer = 0;
        buttonClicked = false;
    }

    // Update is called once per frame
    void Update()
    {
         if(timeOut > 1)
         {
             timeOut--;
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
        if(Input.GetAxis("Dpad-Horizontal") > 0 || Input.GetKey("d"))
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
        else  if(mod == 1)
        {
            gameSave.GetComponent<GameSaveSystem>().LoadGame();
        }
        
    }
}
