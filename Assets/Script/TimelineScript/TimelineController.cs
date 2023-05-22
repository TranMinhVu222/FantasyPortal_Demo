using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TimelineController: MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public float smooth,t,smooth2;

    private void Start()
    {
       
    }

    private void Update()
    {
        t += Time.deltaTime;
       StartCoroutine(FirstScene(t));
    }

    IEnumerator FirstScene(float t)
    {
        yield return new WaitForSeconds(0.2f);
        Quaternion target = Quaternion.Euler(0, 0, 270f);
        cinemachineVirtualCamera.m_Lens.OrthographicSize = 0.7f + t*smooth2;
        cinemachineVirtualCamera.transform.rotation = Quaternion.Slerp(cinemachineVirtualCamera.transform.rotation, target,  t*smooth);
    }
}
