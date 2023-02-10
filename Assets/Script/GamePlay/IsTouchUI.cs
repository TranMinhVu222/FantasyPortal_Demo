using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IsTouchUI : MonoBehaviour
{
    public bool checkTouch;
    public int countTouchUI;
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
