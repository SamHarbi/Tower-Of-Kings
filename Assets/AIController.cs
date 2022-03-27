using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class AIController : MonoBehaviour
{
    
    public GameObject path; //GameObject Path with Tilemap

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


    // Start is called before the first frame update
    async void Start()
    {
        tm = path.GetComponent<Tilemap>();
        area = tm.cellBounds;
        TileBase[] allTiles = tm.GetTilesBlock(area);

        tm.SetTileFlags(position, TileFlags.None);

        actionState = 0; // Idle

        direction = gameObject.GetComponent<SpriteRenderer>().flipX;

        anim = gameObject.GetComponent<Animator>();

        State_Idle();

        getGoalNode(new Vector2(3, -1));
    }

    // Update is called once per frame
    void Update()
    {
        setGoalNodeToPlayer();
        search();

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

    void search()
    {
        float currPOS = getNodeOnGrid(gameObject.transform.position).x;

        if(getNodeOnGrid(gameObject.transform.position).x + 2 == goalNode.x)
        {
            print("At Goal");
            State_Idle();
            return;
        }
        else if(getNodeOnGrid(gameObject.transform.position).x - 2 == goalNode.x)
        {
            print("At Goal");
            State_Idle();
            return;
        }
        
        if(goalNode.x > currPOS)
        {
            State_MoveRight();
            if(Mathf.Floor(goalNode.x) <= Mathf.Floor(currPOS))
            {
                State_Idle();
            }
        }
        else if(goalNode.x < currPOS)
        {
            State_MoveLeft();
            if(Mathf.Floor(goalNode.x) >= Mathf.Floor(currPOS))
            {
                State_Idle();
            }
        }
        
    }

    void State_MoveRight()
    {
        actionState = 2;
        anim.SetBool("Running", true);
    }

    void State_MoveLeft()
    {
        actionState = 1;
        anim.SetBool("Running", true);
    }

    void State_Idle()
    {
         actionState = 0;
         anim.SetBool("Running", false);
    }
}
