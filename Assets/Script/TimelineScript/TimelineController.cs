using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class TimelineController: MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera, camera10;
    public float smooth,t,smooth2, rotationInf;
    public GameObject lieSceneObject, eyeBlinking, finishGame, multiverse;
    private Quaternion newQuaternion;
    private CanvasGroup uiElement;
    private bool breakIle,fix;
    public GameObject canvas;
    public PlayableDirector director;
    private void Start()
    {
        Time.timeScale = 1;
        breakIle = true;
        fix = false;
        uiElement = canvas.GetComponent<CanvasGroup>();
        director.stopped += OnTimelineStopped;
        GetComponent<PlayableDirector>().Play();
    }

    private void Update()
    {
        if (lieSceneObject.activeInHierarchy)
        {
            t += Time.deltaTime;
            StartCoroutine(FirstScene(t));
        }

        if (eyeBlinking && breakIle)
        {
            StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 0,0));
            breakIle = false;
        }
    }

    IEnumerator FirstScene(float t)
    {
        yield return new WaitForSeconds(0);
        if ( cinemachineVirtualCamera.transform.rotation.z >= 100f)
        {
            smooth = 30f;
        }
        cinemachineVirtualCamera.m_Lens.OrthographicSize = 0.7f + t*smooth2;
        newQuaternion.eulerAngles += new Vector3(0, 0, smooth) * Time.deltaTime;
        cinemachineVirtualCamera.transform.Rotate(0,0,smooth*Time.deltaTime);
    }
    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, int change, float lerpTime = 1f)
    {
        breakIle = false;
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
    private void OnTimelineStopped(PlayableDirector directorTimeline)
    {
        if (directorTimeline == director)
        {
            finishGame.SetActive(true);
            camera10.gameObject.SetActive(true);
            multiverse.SetActive(false);
        }
    }
}
