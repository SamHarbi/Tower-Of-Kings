using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCutScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Fall()
    {
        yield return new WaitForSeconds(0.3f);
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 4.5f;
    }
}
