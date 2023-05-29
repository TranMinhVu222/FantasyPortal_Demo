using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsTouchUI : MonoBehaviour
{
    public bool checkTouch;
    public int countTouchUI;
    
    private static IsTouchUI instance;

    public static IsTouchUI Instance { get => instance; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Error !!!");
        }
        instance = this;
    }

    private void Start()
    {
        checkTouch = false;
    }

    public void checkTouchUI()
    {
        checkTouch = true;
    }

    public void checkTouchOverUI()
    {
        checkTouch = false;
        countTouchUI = 1;
    }
}
