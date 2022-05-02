using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Shakes Camera (or any other gameObject for that matter) at a set intensity within a set timeframe
*/

public class CameraShake : MonoBehaviour
{
    /*
        Code inspired by (and partially copied) from 
        Brackeys (2018). CAMERA SHAKE in Unity. [online] www.youtube.com.
         Available at: https://www.youtube.com/watch?v=9A9yj8KnM8c [Accessed 1 May 2022]. 
    */
    
    Vector3 originalPos;

    public void startShake(float magnitude, float duration)
    {
        StartCoroutine(CamShake(magnitude, duration * Time.deltaTime));
    }

    public IEnumerator CamShake(float magnitude, float duration)
    {
        //Get original camera position
        originalPos = gameObject.transform.position;
        float elapsedTime = 0.0f;

        //While shaking
        while(elapsedTime < duration)
        {
            float x = Random.Range(-1.0f, 1.0f) * magnitude;
            float y = Random.Range(-1.0f, 1.0f) * magnitude;

            yield return new WaitForSeconds(0.05f);

            //set camera to a random offset positon calculated from magintude
            transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsedTime += Time.deltaTime;

        }

        //return back to original positions relative to player
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        transform.position = new Vector3(playerPos.x, playerPos.y + 1, -10);
        

        yield return null;
    }
}
