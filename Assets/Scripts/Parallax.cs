using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{

    public GameObject Player;
    private Vector2 playersLastPos;
    private Vector2 playersCurrPos;

    public float moveMod;

    // Start is called before the first frame update
    void Start()
    {
        playersCurrPos = Player.GetComponent<Transform>().position;
        playersLastPos = playersCurrPos;
    }

    // Update is called once per frame
    void Update()
    {
        playersCurrPos = Player.GetComponent<Transform>().position;
        if(playersCurrPos != playersLastPos)
        {
            if(playersCurrPos.x > playersLastPos.x)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x - moveMod, gameObject.transform.position.y);
            }
            else if(playersCurrPos.x < playersLastPos.x)
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x + moveMod, gameObject.transform.position.y);
            }
        }
        playersLastPos = playersCurrPos;
    }
}
