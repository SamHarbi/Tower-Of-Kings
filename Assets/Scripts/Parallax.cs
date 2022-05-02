using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Moves an Object based on far the Player moves from it- Used by farBackground tilemap to create a Parallax effect
*/

public class Parallax : MonoBehaviour
{
    public GameObject Player;
    public float maxDist; 
    public float moveMod; //How far to move per update

    //Used to calulate how far the player moved
    private Vector2 playersLastPos;
    private Vector2 playersCurrPos;

    private Vector2 origPos; //Original Position of this object
    private float origMoveMod; //How far should the background move

    // Start is called before the first frame update
    void Start()
    {
        //Initialse Values
        playersCurrPos = Player.GetComponent<Transform>().position;
        playersLastPos = playersCurrPos;
        origPos = gameObject.transform.position;
        origMoveMod = moveMod;
    }

    // Update is called once per frame
    void Update()
    {
        playersCurrPos = Player.GetComponent<Transform>().position; //Get Player's Current position
        if(playersCurrPos != playersLastPos) //If player has moved
        {
            if(playersCurrPos.x > playersLastPos.x ) //Right
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x - moveMod, gameObject.transform.position.y);
            }
            else if(playersCurrPos.x < playersLastPos.x ) //Left
            {
                gameObject.transform.position = new Vector2(gameObject.transform.position.x + moveMod, gameObject.transform.position.y);
            }
        }
        playersLastPos = playersCurrPos;

        /*
            Using an Asymptote function to limit the movment of the parralex effect as the
            Player gets further away from orgin, the function choosen for this is 

            M / 20*X^2 + 1 Where M is the max parallax displacment in a single step and X is displacment so far

        */

        moveMod = origMoveMod / (Mathf.Pow(Mathf.Abs(origPos.x - gameObject.transform.position.x), 2) + 1);
        
        
    }
}
