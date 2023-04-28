using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishGame : MonoBehaviour
{
    private static FinishGame instance;
    public static FinishGame Instance
    {
        get => instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public void ActiveGameObject()
    {
        gameObject.SetActive(true);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
