using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*

This class implements a State Machine structure, based on what state currently is active as recorded in actionstate variable. Only one state can be active at a time and every 
state loops back to State_Idle on update, before the search method decides on what state should be set before the next Update based on input information on the state of the 
world (Distance to Player). 

This structure has been particulary useful when working on the search algorithm, regardless of how search is implemented- it doesn't affect the states. All that search
has to do is decide when to call which state. 

========================================================================================================================================================================================

This class also further is part of a Decorator pattern and is wrapped by other wrapper classes to extend it's functionality while appearing exactly the same to the client (which is 
in this case Unity). The wrapper provides the same interface as this class (Start, Update, OnTriggerEnter2d) but does something to extend functionality before passing it on to this class. 
In the case of OnTriggerEnter2d, BossWrappers do not pass on requests so it can be considered as a Chain of Responsibility pattern only in that case. 

*/


public class AIController : MonoBehaviour
{
    //***Legacy Search AI Variables - See Explanation below or in the Report***
    //public GameObject path; //GameObject Path with Tilemap
    //private Tilemap tm; //Tilemap Component
    //public BoundsInt area;
    //public Vector3Int position;
    //public Tile goalTile;
    //public Tile regularTile;

    //***Legacy Unity Animation***
    //private Animator anim;

    public GameObject attackRange; //GameObject with Collider that causes damage to Player on contact
    public float speed; //Walking Speed
    public float searchRange; //Maximum distance from object to search for Player 
    public bool wrapperOverride; //If this is true, then there is a wrapper class that overtakes some functionality 

    private GameObject Player;
    private Vector2 goalNode; //The positional goal that enemies move towards
    private int actionState; //ID of the state that is currently active
    private bool direction; //Which direction the enemy is looking towards
    private float distanceToPlayer; // How close the player should be for a different state to activate 
    private GameObject[] AnimationSet; //All AnimationData Objects with animations that affect this Object
    private GameObject LAS; //Logical Animation System
    private int currAnim; //ID of currently running Animation
    private bool deathAnim; //Is the death Animation running?
    private bool left; //What bool value is left- makes code easier to read by setting a name to false
    private bool right; //What bool value is right- makes code easier to read by setting a name to true
    private int startDamageFrame; //At which frame of an animation damage is dealt 
    private int endDamageFrame; //Last frame of an animation damage is dealt 
 
    public void WrappedAwake()
    {
        //Get Array of Animations from Central Animation System LAS
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        //Start Idle Animation
        currAnim = 1;
        enableAnimation(0);
    }
    
    // Start is called before the first frame update by Wrapper
    public void WrappedStart()
    {
        //Legacy Search AI Variables - See Explanation below or in the Report
        //path = GameObject.FindWithTag("Path"); 
        //tm = path.GetComponent<Tilemap>();
        //area = tm.cellBounds;
        //TileBase[] allTiles = tm.GetTilesBlock(area);
        //tm.SetTileFlags(position, TileFlags.None);

        //Legacy Unity Animation
        //anim = gameObject.GetComponent<Animator>();


        Player = GameObject.FindWithTag("Player");
        
        //Set to Idle
        actionState = 0;
        State_Idle();

        //Setup Goal to move towards
        setGoalModifiers(3f);
        goalNode = new Vector2(3, -1);
        searchRange = 15f;

        //Setup other variables
        deathAnim = false;
        left = false;
        right = true;
        startDamageFrame = 3;
        endDamageFrame = 9;
        
    }

    public void setDamageFrames(int newStart, int newEnd)
    {
        startDamageFrame = newStart;
        endDamageFrame = newEnd;
    }

    public void setDirections(bool newLeft, bool newRight)
    {
        left = newLeft;
        right = newRight;
    }

    public void setSearchRange(float newRange)
    {
        searchRange = newRange;
    }

    public void setDeathAnim(bool newAnimValue)
    {
        deathAnim = newAnimValue;
    }

    public bool getDeathAnim()
    {
        return deathAnim;
    }

    // Update is called once per frame by Wrapper
    public void WrappedUpdate()
    {
        //Reset Attacking
        //anim.SetBool("Attack", false); Legacy Animation using Unity's built in Animator
        attackRange.SetActive(false);
        
        if(deathAnim == true)
        {
            return; //Ensure no other code runs during the death Animation
        }

        //Find Player as long as they are within a 15 units distance
        setGoalNodeToPlayer();
        if(wrapperOverride == false)
        {
            search(); //Start Searching for the Player if not overridden by a Boss Wrapper
        }
        
        //Based on actionState, move left or right
        if(actionState == 1)
        {
            gameObject.transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0); // move right
            setDirection(right);
        }
        if(actionState == 2)
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0); // move left
            setDirection(left);
        }
        
        actionState = 0; //Reset actionState

        //If an Attack Animation is playing, ensure that damage can be done by this enemy to Player
        if(getAnimationProgress(2) == true)
        {
            if(wrapperOverride == false)
            {
                attackRange.SetActive(true);
            }
        }
        else
        {
            if(wrapperOverride == false)
            {
                attackRange.SetActive(false);
            }
        }

    }

    void setGoalModifiers(float newDistPlayer)
    {
        distanceToPlayer = newDistPlayer;
    }

    void setDirection(bool newDirection)
    {
        direction = newDirection;
        gameObject.GetComponent<SpriteRenderer>().flipX = direction;
        //gameObject.transform.GetChild(0).gameObject.GetComponent<EnemyBladeController>().setDirection(!direction);
    }

/*
    ===========================================================================================================================================
    This is legacy code for setting up and testing a AI search algorithm based on Unity's tiles and thier respective transforms as possible 
    states that could be moved to by the AI. This is what I orginally wanted to do for my AI but due to time restrictions I had to do replace
    the AI search with a simpler distance comparison. 

    I keep this code here because I hope to expand the AI in the future with it. 

*/
/*
    //Find the Tile closest corresponding to a goal cordinate
    void getGoalNode(Vector2 goalCords)
    {
        Vector3Int goalCords3D = tm.WorldToCell(new Vector3(goalCords.x, goalCords.y, 0));
        TileBase goal = tm.GetTile(goalCords3D);
        tm.SetTile(goalCords3D, goalTile);
        goalNode = goalCords;
    }

    //Changes goal colored tile to a regular tile- useful for debugging to visually identify the goal node on screen
    void resetNode(Vector2 node)
    {
        Vector3Int goalCords3D = tm.WorldToCell(new Vector3(node.x, node.y, 0));
        TileBase goal = tm.GetTile(goalCords3D);
        tm.SetTile(goalCords3D, regularTile);
    }

    Vector3 getNodeOnGrid(Vector2 node)
    {
        Vector3Int convertedNode = tm.WorldToCell(new Vector3(node.x, node.y, 0));
        return convertedNode;
    }
    ===========================================================================================================================================
*/
    void setGoalNodeToPlayer()
    {
        goalNode = new Vector2(Player.transform.position.x, Player.transform.position.y);
    }

    //Enter one of multiple States based on Input (Player Distance from this Enemy)
    public void search()
    {
        Vector3 currPOS = gameObject.transform.position; //Current position of Enemy

        //If Player is beyond max distance or is aprox at this Enemies Position
        if(Mathf.Abs(goalNode.x - currPOS.x) <= 1 || Mathf.Abs(goalNode.x - currPOS.x) > searchRange)
        {
            State_Idle();
        }
        else if(goalNode.y > gameObject.transform.position.y + searchRange/2 || goalNode.y < gameObject.transform.position.y - searchRange/2)
        {
            State_Idle(); //If Player is too far above or below this enemy
        }
        else if(goalNode.x > currPOS.x) //Player is to the right
        {
            State_MoveRight();
        }
        else if(goalNode.x < currPOS.x) //Player is to the left
        {
            State_MoveLeft();
        }
        
        if(Mathf.Abs(goalNode.x - currPOS.x) <= distanceToPlayer) //If Player close enough to attack based on goal mod 
        {
            if(Mathf.Abs(goalNode.y - currPOS.y) <= distanceToPlayer && wrapperOverride == false)
            {
                State_Attack();
            }

            if(wrapperOverride == true)
            {
                State_Attack();
            }
        }
    }

    void State_MoveRight()
    {
        actionState = 2;
        enableAnimation(1);
    }

    void State_MoveLeft()
    {
        actionState = 1;
        enableAnimation(1);
    }

    public void State_Idle()
    {
         actionState = 0;
         enableAnimation(0);
    }

    void State_Attack()
    {
        attackRange.SetActive(true);
        enableAnimation(2);
    }

/*
    void State_SetStance()
    {
        //Unused - Legacy from Panel Centric Design
    }
*/
    public void WrappedOnTriggerEnter2D(Collider2D col)
    {
        
        //Squeezes the enemy when player jumps on thier heads, lot's of issues with this so it's deactivated
        if(wrapperOverride == false && col.tag == "Boots")
        {
            Vector3 colliderSize = GetComponent<BoxCollider2D>().size;
            GetComponent<BoxCollider2D>().size = new Vector3(colliderSize.x, colliderSize.y - 1, colliderSize.z); //Make Collider size smaller
            transform.localScale = new Vector3(1, 0.5f, 1); //Make the visual enemy smaller
        }
        
        //If hit by Player
        if(col.tag == "PlayerAttackRange")
        {
            enableAnimation(3);
            deathAnim = true;
            GetComponent<SoundFXManager>().Hit();
            StartCoroutine(deathTimer());
        }
    }

    //Wait a set time before enemy is deactivated 
    IEnumerator deathTimer()
    {
        //Destroy Collider here maybe TODO
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    //Select an AnimationData Object to play
    public void enableAnimation(int num)
    {
        //Run set animation and deactivate current running Animation
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim) //If another animation was previously running
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false; //turn off previous running animation
            currAnim = num;
        }
    }

    //Check if the current running animation is currently a within a certain range of frames
    public bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() > startDamageFrame && AnimationSet[id].GetComponent<AnimationData>().getActiveFrame() < endDamageFrame)
        {
            if(wrapperOverride == false)
            {
                GetComponent<SoundFXManager>().Slash();
            }
            return true;
        }
        return false;
    }
}