using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class AIController : MonoBehaviour
{
    
    public GameObject path; //GameObject Path with Tilemap
    public GameObject attackRange;

    public float speed;
    private Tilemap tm; //Tilemap Component
    public BoundsInt area;
    public Vector3Int position;
    public Tile goalTile;
    public Tile regularTile;
    public GameObject Player;
    private Vector2 goalNode;

    private int actionState;

    private bool direction;
    private Animator anim;

    private float distanceToPlayer;
    public GameObject[] AnimationSet;
    public GameObject LAS;
    private int currAnim;
    private bool deathAnim;
    public bool Boss;
    private float BossHealth;
    public GameObject hitBoss;
    private int startDamageFrame;
    private int endDamageFrame;
    public GameObject bossWave;
    private bool bossWaveStarted;
 


    // Start is called before the first frame update
    void Start()
    {
        tm = path.GetComponent<Tilemap>();
        area = tm.cellBounds;
        TileBase[] allTiles = tm.GetTilesBlock(area);

        tm.SetTileFlags(position, TileFlags.None);

        actionState = 0; // Idle

        direction = gameObject.GetComponent<SpriteRenderer>().flipX;

        //anim = gameObject.GetComponent<Animator>();

        State_Idle();

        setGoalModifiers();

        getGoalNode(new Vector2(3, -1));

        deathAnim = false;

        if(Boss == false)
        {
            startDamageFrame = 3;
            endDamageFrame = 9;
        }
        else
        {
            startDamageFrame = 8;
            endDamageFrame = 9;
        }

        bossWaveStarted = false;


    }

    void Awake()
    {
        //Get Array of Animations from Central Animation System LAS
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        //Start Idle Animation
        currAnim = 1;
        enableAnimation(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(deathAnim == true)
        {
            return; //Ensure no other code runs during the death Animation
        }
        
        //Reset Attacking
        //anim.SetBool("Attack", false); 
        attackRange.SetActive(false);

        //Find Player as long as they are within a 15 units distance
        setGoalNodeToPlayer();
        search(15f);

        //Based on actionState, move left or right
        if(actionState == 1)
        {
            gameObject.transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0); // move right
            if(Boss == false)
            {
                setDirection(true);
            }
        }
        if(actionState == 2)
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0); // move left
            if(Boss == false)
            {
                setDirection(false);
            }
        }
        
        actionState = 0; //Reset actionState

        //If an Attack Animation is playing, ensure that damage can be done by this enemy to Player
        if(getAnimationProgress(2) == true)
        {
            if(Boss == false)
            {
                attackRange.SetActive(true);
            }
            else
            {
                if(bossWaveStarted == false)
                {
                    Instantiate(bossWave, gameObject.transform.position, Quaternion.identity);
                    bossWaveStarted = true;
                }
            }
        }
        else
        {
            if(Boss == false)
            {
                attackRange.SetActive(false);
            }
            else
            {
                bossWaveStarted = false;
            }
        }

    }

    void setGoalModifiers()
    {
        distanceToPlayer = 3f;
    }

    void setDirection(bool newDirection)
    {
        direction = newDirection;
        gameObject.GetComponent<SpriteRenderer>().flipX = direction;
        //gameObject.transform.GetChild(0).gameObject.GetComponent<EnemyBladeController>().setDirection(!direction);
    }

    //Find the Tile closest corresponding to a goal cordinate
    void getGoalNode(Vector2 goalCords)
    {
        Vector3Int goalCords3D = tm.WorldToCell(new Vector3(goalCords.x, goalCords.y, 0));
        //TileBase goal = tm.GetTile(goalCords3D);
        //tm.SetTile(goalCords3D, goalTile);

        goalNode = goalCords;
    }

    void resetNode(Vector2 node)
    {
        Vector3Int goalCords3D = tm.WorldToCell(new Vector3(node.x, node.y, 0));
        TileBase goal = tm.GetTile(goalCords3D);
        tm.SetTile(goalCords3D, regularTile);
    }

    void setGoalNodeToPlayer()
    {
        resetNode(goalNode);
        Vector2 playerPos = new Vector2(Player.transform.position.x, Player.transform.position.y);
        getGoalNode(playerPos);
    }

    Vector3 getNodeOnGrid(Vector2 node)
    {
        Vector3Int convertedNode = tm.WorldToCell(new Vector3(node.x, node.y, 0));
        return convertedNode;
    }

    //Enter one of multiple States based on Input (Player Distance from this Enemy)
    void search(float maxDistance)
    {
        Vector3 currPOS = gameObject.transform.position; //Current position of Enemy

        //If Player is beyond max distance or is aprox at this Enemies Position
        if(Mathf.Abs(goalNode.x - currPOS.x) <= 1 || Mathf.Abs(goalNode.x - currPOS.x) > maxDistance)
        {
            State_Idle();
        }
        else if(goalNode.y > gameObject.transform.position.y + maxDistance/2 || goalNode.y < gameObject.transform.position.y - maxDistance/2)
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
            State_Attack();
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

    void State_Idle()
    {
         actionState = 0;
         enableAnimation(0);
    }

    void State_Attack()
    {
        attackRange.SetActive(true);
        enableAnimation(2);
    }

    void State_SetStance()
    {
        //Unused - Legacy from Panel Centric Design
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
            if(Boss == false)
            {
                enableAnimation(3);
                deathAnim = true;
                StartCoroutine(deathTimer());
            }
            else
            {
                //enableAnimation(3);
                //deathAnim = true;
                //StartCoroutine(BossHitTimer());

                BossHealth = BossHealth - 1;
                Vector3 hitPos = new Vector3(col.transform.position.x + 2, col.transform.position.y + 2, gameObject.transform.position.z);
                Instantiate(hitBoss, hitPos, Quaternion.identity);
            }
        }
    }

    IEnumerator deathTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    IEnumerator BossHitTimer()
    {
        yield return new WaitForSeconds(0.5f);
        deathAnim = false;
    }

    void enableAnimation(int num)
    {
        //Run set animation and deactivate current running Animation
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim)
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
        }
    }

    private bool getAnimationProgress(int id)
    {
        //Check if certain frames are running / Attacking frames that should cause damage
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame > 3 && AnimationSet[id].GetComponent<AnimationData>().activeFrame < 9)
        {
            return true;
        }
        return false;
    }
}
