using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCircle : MonoBehaviour
{
    public GameObject Boss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Boss.transform.position, Vector3.back, 20 * Time.deltaTime);
    }
}
