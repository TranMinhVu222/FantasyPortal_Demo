using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameEvent : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
        Debug.Log("da thoat game");
    }
}