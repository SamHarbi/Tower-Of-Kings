using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGate : MonoBehaviour
{
    
    public GameObject BossUIControl;
    private bool exit;

    // Start is called before the first frame update
    void Start()
    {
        exit = false; //Is the player leaving the boss room
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetExit(bool newExit)
    {
        exit = newExit;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player" && exit == false)
        {
            BossUIControl.GetComponent<BossUIControl>().startBossFight(); //On entry start fight

        }
        else if(col.tag == "Player" && exit == true)
        {
            BossUIControl.GetComponent<BossUIControl>().resetUI(); //On exit reset UI
        }
    }

}
