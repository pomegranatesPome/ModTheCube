using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    public MeshRenderer Renderer;
    public float scaleDuration = 1f; // how long does the scaling last
    public Vector3 originalScale = Vector3.one;
    public Vector3 targetScale = Vector3.one * 1.5f;
    public Color originalColor = new Color(0.51f, 0.71f, 0.27f, 1f);
    public Color targetColor = new Color(0.96f, 0.69f, 0.19f, .4f);
    private Camera myCam;
    private Material material;
    private bool isClicking = false;
    private bool isScaling = false;
    private float depthWorld = 1f; // this is fixed for now

    private float depthScreen = 0f;
    void Start()
    {   
        myCam = Camera.main;
        transform.position = new Vector3(3, 4, depthWorld);
        transform.localScale = originalScale;
        
        material = Renderer.material;
        
        material.color = originalColor;

        // since we won't change the cube's z value for now,
        // get the screen space z value once for later use
        depthScreen = myCam.WorldToScreenPoint(transform.position).z;
        
    }
    
    void Update()
    {   
        

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isClicking) // only change the properties when the button is freshly pressed
                isClicking = false;
            else
            {
                isClicking = true;
                // move the cube to mouse position
                // In this case, mousePosScreen.z is ALWAYS 0. 
                Vector3 mousePosScreen = Input.mousePosition;
                // so we set this to previously calculated depthScreen. 
                mousePosScreen.z = depthScreen;
                // put the cube here
                Vector3 mousePosWorld = myCam.ScreenToWorldPoint(mousePosScreen);
                transform.position = mousePosWorld;
            
                // scale the cube 
                if (!isScaling)
                    StartCoroutine(ScaleCube(originalScale, targetScale));
            }
        }

        // rotate the cube 
        if (isScaling)
        {
            Quaternion randomRotation = Random.rotation;
            Quaternion smallerRotation = Quaternion.Slerp(randomRotation, Quaternion.identity, 0.9f);
            transform.rotation = randomRotation;
        }
            
        else
            transform.Rotate(10.0f * Time.deltaTime, 0.0f, 0.0f);

    }

    // the coroutine for scaling the cube
    // Also I want to recolor the cube over time
    IEnumerator ScaleCube(Vector3 originalScale, Vector3 targetScale)
    {
        isScaling = true;
        
        float startTime = Time.time;
        //Debug.Log("Coroutine start time = " + startTime +". scale = "+ transform.localScale);
        while (Time.time - startTime < scaleDuration)
        {   
            // t = how much time has passed relative to the total scaling window, i.e. scaleDuration
            float t = (Time.time - startTime) / scaleDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            material.color = Color.Lerp(originalColor, targetColor, t);
    
            yield return null; // pause until the next frame
        }

        transform.localScale = targetScale;

        // Then scale it back down to original

        startTime = Time.time;
        //Debug.Log("Mid point time = " + startTime +". scale = "+ transform.localScale);
        while (Time.time - startTime < scaleDuration)
        {   
            // t = how much time has passed relative to the total scaling window, i.e. scaleDuration
            float t = (Time.time - startTime) / scaleDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            material.color = Color.Lerp(targetColor, originalColor, t);

            yield return null; // pause until the next frame
        }

        transform.localScale = originalScale;
        //Debug.Log("End time = " + Time.time +". scale = "+ transform.localScale);

        isScaling = false;
    }
}
