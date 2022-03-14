using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBladeController : MonoBehaviour
{
    
    private bool locked;
    public GameObject panel;
    public GameObject permParent;
    public bool direction;

    // Start is called before the first frame update
    void Start()
    {
        permParent = gameObject.transform.parent.gameObject;
        switchDirection();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "CombatEnter")
        {
            gameObject.transform.parent = panel.transform;
            locked = true;
        }
        
    }

    public void switchDirection()
    {
        if(direction == true)
        {
            direction = false;
            permParent.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.position = new Vector2(permParent.transform.position.x + 5f, permParent.transform.position.y - 5f);
        }
        else
        {
            direction = true;
            permParent.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.transform.position = new Vector2(permParent.transform.position.x - 5f, permParent.transform.position.y - 5f);
        }
    }

    public void CombatExit()
    {
        gameObject.transform.parent = permParent.transform;
        locked = false;

        if(direction == false)
        {
            gameObject.transform.position = new Vector2(permParent.transform.position.x + 5f, permParent.transform.position.y - 5f);
        }
        else
        {
            gameObject.transform.position = new Vector2(permParent.transform.position.x - 5f, permParent.transform.position.y - 5f);
        }
        
    }
   

}
