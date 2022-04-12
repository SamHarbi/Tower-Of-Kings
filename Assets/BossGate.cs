using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGate : MonoBehaviour
{
    
    public GameObject mainCam;
    public GameObject[] bossCams;
    public GameObject Canvas;
    public GameObject Gate;
    public GameObject bossName;
    public int bossID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            mainCam.SetActive(false);
            bossCams[bossID].SetActive(true);
            Canvas.GetComponent<Canvas>().worldCamera = bossCams[bossID].GetComponent<Camera>();
            Gate.SetActive(true);
            bossName.SetActive(true);
            StartCoroutine(bossTextFade());
        }
    }

    IEnumerator bossTextFade()
    {
        while(!Mathf.Approximately(bossName.GetComponent<Text>().color.a, 0f))
        {
            yield return new WaitForSeconds(0.2f);
            bossName.GetComponent<Text>().color = new Color(0.6705883f,0.2588235f,0.08235294f,  bossName.GetComponent<Text>().color.a - 0.05f);
        }
        
        bossName.SetActive(false);
        yield return null;
        
    }
}
