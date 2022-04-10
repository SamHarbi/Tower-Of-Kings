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


    // Start is called before the first frame update
    void Start()
    {
        tm = path.GetComponent<Tilemap>();
        area = tm.cellBounds;
        TileBase[] allTiles = tm.GetTilesBlock(area);

        tm.SetTileFlags(position, TileFlags.None);

        actionState = 0; // Idle

        direction = gameObject.GetComponent<SpriteRenderer>().flipX;

        anim = gameObject.GetComponent<Animator>();

        State_Idle();

        setGoalModifiers();

        getGoalNode(new Vector2(3, -1));

        deathAnim = false;

    }

    void Awake()
    {
        LAS = GameObject.FindWithTag("LAS");
        AnimationSet = LAS.GetComponent<LogicalAnimationSystem>().getAnimationDataArray(gameObject);

        currAnim = 1;
        enableAnimation(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(deathAnim == true)
        {
            return;
        }
        
        
        anim.SetBool("Attack", false);
        attackRange.SetActive(false);

        setGoalNodeToPlayer();
        search(15f);

        if(actionState == 1)
        {
            gameObject.transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0); // move right
            setDirection(true);
        }
        if(actionState == 2)
        {
            gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0); // move left
            setDirection(false);
        }
        actionState = 0;

        if(getAnimationProgress(2) == true)
        {
            attackRange.SetActive(true);
        }
        else
        {
            attackRange.SetActive(false);
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
        gameObject.transform.GetChild(0).gameObject.GetComponent<EnemyBladeController>().setDirection(!direction);
    }

    //Find the Tile closest corresponding to a goal cordinate
    void getGoalNode(Vector2 goalCords)
    {
        Vector3Int goalCords3D = tm.WorldToCell(new Vector3(goalCords.x, goalCords.y, 0));
        TileBase goal = tm.GetTile(goalCords3D);
        tm.SetTile(goalCords3D, goalTile);

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

    void search(float maxDistance)
    {
        float currPOS = gameObject.transform.position.x;

        if(Mathf.Approximately(goalNode.x, currPOS) || Mathf.Abs(goalNode.x - currPOS) > maxDistance)
        {
            State_Idle();
            return;
        }

        
        if(goalNode.y > gameObject.transform.position.y + 8 || goalNode.y < gameObject.transform.position.y - 8)
        {
            State_Idle();
        }
        else if(goalNode.x >= currPOS)
        {
            State_MoveRight();
            
        }
        else if(goalNode.x <= currPOS)
        {
            State_MoveLeft();
            
        }
        
        if(Mathf.Abs(goalNode.x - currPOS) <= distanceToPlayer)
        {
            State_Attack();
            //State_Idle();
        }
    }

    void State_MoveRight()
    {
        actionState = 2;
        //anim.SetBool("Running", true);
        enableAnimation(1);
    }

    void State_MoveLeft()
    {
        actionState = 1;
        //anim.SetBool("Running", true);
        enableAnimation(1);
    }

    void State_Idle()
    {
         Vector2 newGoal = new Vector2(goalNode.x + Random.Range(-8.0f, 8.0f), goalNode.y);
         getGoalNode(newGoal);

         actionState = 0;
         //anim.SetBool("Running", false);
         enableAnimation(0);

    }

    void State_Attack()
    {
        //anim.SetBool("Attack", true);
        attackRange.SetActive(true);
        enableAnimation(2);
    }

    void State_SetStance()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "PlayerAttackRange")
        {
            enableAnimation(3);
            deathAnim = true;
            StartCoroutine(deathTimer());
        }
    }

    IEnumerator deathTimer()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    void enableAnimation(int num)
    {
        AnimationSet[num].GetComponent<AnimationData>().Running = true;
        if(num != currAnim)
        {
            AnimationSet[currAnim].GetComponent<AnimationData>().Running = false;
            currAnim = num;
        }
    }

    private bool getAnimationProgress(int id)
    {
        if(AnimationSet[id].GetComponent<AnimationData>().activeFrame > 3 && AnimationSet[id].GetComponent<AnimationData>().activeFrame < 9)
        {
            return true;
        }
        return false;
    }
}
