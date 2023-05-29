using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject eyeBlinking;
    private Quaternion newQuaternion;
    private CanvasGroup uiElement;
    public GameObject canvas;
    private bool breakIle;
    void Start()
    {
        breakIle = true;
        uiElement = canvas.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eyeBlinking && breakIle)
        {
            StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0,0));
            breakIle = false;
        }
    }
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, int change, float lerpTime = 1f)
    {
        breakIle = false;
        Debug.Log(change);
        float timeStartedLerp = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerp;
        float percentageComplete = timeSinceStarted / lerpTime;
        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerp;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            canvasGroup.alpha = currentValue;

            if (percentageComplete >= 1)
            {
                break;
            } 
            
            yield return new WaitForEndOfFrame();
        }

        if (change == 0)
        {
            if (canvasGroup.alpha <= 0.2f)
            {
                StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1,1));
            }    
        }

        if (change == 1)
        {
            if (canvasGroup.alpha >= 0.8f)
            {
                StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0,0));
            } 
        }
    }
}
