using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    // Start is called before the first frame update
    private CanvasGroup uiElement;
    void Start()
    {
        uiElement = gameObject.GetComponent<CanvasGroup>();
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0));
    }
    
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float lerpTime = 1f)
    {
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
        gameObject.SetActive(false);
    }
}


