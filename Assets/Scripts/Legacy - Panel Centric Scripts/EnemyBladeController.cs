using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBladeController : MonoBehaviour
{
    
    public GameObject permParent;
    private bool direction;
    private bool prevDirection;

    // Start is called before the first frame update
    void Start()
    {
        permParent = gameObject.transform.parent.gameObject;
        
        if(permParent.GetComponent<SpriteRenderer>().flipX == true)
        {
            setDirection(false);
        }
        else
        {
            setDirection(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setDirection(bool newDirection)
    {
        direction = newDirection;
        prevDirection = direction;
        activateDirection(direction);
    }

    public bool getDirection()
    {
        return direction;
    }

    public void activateDirection(bool direction)
    {
        if(direction == false)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = new Vector2(permParent.transform.position.x + 5f, permParent.transform.position.y - 5f);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.transform.position = new Vector2(permParent.transform.position.x - 5f, permParent.transform.position.y - 5f);
        }
    }


}
