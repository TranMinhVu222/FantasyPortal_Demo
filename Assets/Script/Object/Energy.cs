using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.Mathematics;
using UnityEngine;


public class Energy : MonoBehaviour
{
    
    
    public int countPortal;
    public int completeCastSpell;
    public Portal currentPortal = Portal.EnterPortal;
    public GameObject entryPortalAnim;
    public GameObject enterPortalAnim;
    public GameObject entryPortal,enterPortal;
    public Transform enterPosition, entryPosition;

    // public PlayerController playerController;
    public float angle;
    public Vector2 direction;
    
    public enum Portal
    {
        EnterPortal,
        EntryPortal
    }

    private void Start()
    {
        countPortal = 0;
        entryPosition = entryPortal.transform.Find("Exit Position");
        enterPosition = enterPortal.transform.Find("Enter Position");
    }
    
    private void Update()
    {
        switch (currentPortal)
        {
            case Portal.EnterPortal:
                enterPortalAnim.SetActive(true);
                entryPortalAnim.SetActive(false);
                if (countPortal % 2 != 0)
                {
                    ChangePortal(Portal.EntryPortal);
                }
                break;
            case Portal.EntryPortal:
                entryPortalAnim.SetActive(true);
                enterPortalAnim.SetActive(false);
                if (countPortal % 2 ==  0)
                {
                    ChangePortal(Portal.EnterPortal);
                }
                break;
            default:
                return;
        }
        
    }

    public void GetPortalAnim()
    {
        enterPortalAnim = PoolEnergy.Instance.enterPortal;
        entryPortalAnim = PoolEnergy.Instance.entryPortal;
    }
    private void ChangePortal(Portal portal)
    {
        if (portal == currentPortal) return;
        ExitCurrentPortal();
        currentPortal = portal;
        EnterNewPortal();
        // Debug.Log(currentPortal);
    }
    
    public void EnterNewPortal()
    {
        switch (currentPortal)
        {
            case Portal.EnterPortal:
                break;
            case Portal.EntryPortal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ExitCurrentPortal()
    {
        switch (currentPortal)
        {
            case Portal.EnterPortal:
                break;
            case Portal.EntryPortal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        // playerController.GetComponent<Trajectory>().check = false;
        if (currentPortal == Portal.EnterPortal && (other.gameObject.CompareTag("tilemap") || other.gameObject.CompareTag("Target")))
        {
            enterPortal.SetActive(true);
            enterPortal.transform.position = transform.position;
        }

        if (currentPortal == Portal.EntryPortal && (other.gameObject.CompareTag("tilemap") || other.gameObject.CompareTag("Target")))
        {
            entryPortal.SetActive(true);
            entryPortal.transform.position = transform.position;
        }
        direction = other.GetContact(0).normal;
        if (other.transform.CompareTag("tilemap"))
        {
            angle = Mathf.Abs(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            if (88f <= angle && angle <= 91f)
            {
                if (currentPortal == Portal.EnterPortal)
                {
                    enterPortal.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    enterPosition.localPosition = new Vector3(0f, direction.y * 20f, 0);
                    if (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg > 0)
                    {
                        PlayerController.Instance.xEnterPortal = true;
                    }
                    else
                    {
                        PlayerController.Instance.xEnterPortal = false;
                    }
                    Debug.Log("ngang " + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }

                if (currentPortal == Portal.EntryPortal)
                {
                    entryPortal.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    entryPosition.localPosition = new Vector3(0f, direction.y * 20f, 0);
                    if (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg > 0)
                    {
                        PlayerController.Instance.xEntryPortal = true;
                    }
                    else
                    {
                        PlayerController.Instance.xEntryPortal = false;
                    }
                    Debug.Log("ngang " + Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                }
            }
            else
            {
                if (currentPortal == Portal.EnterPortal)
                {
                    enterPortal.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    enterPosition.localPosition = new Vector3(0f, direction.x * -18f, 0f);
                    PlayerController.Instance.xEnterPortal = false;
                    // Debug.Log("doc " + angle);
                }

                if (currentPortal == Portal.EntryPortal)
                {
                    entryPortal.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    entryPosition.localPosition = new Vector3(0f, direction.x * -18f, 0f);
                    PlayerController.Instance.xEntryPortal = false;
                    // Debug.Log("doc " + angle);
                }
            }
        }
        countPortal += 1;
        gameObject.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (PlayerPrefs.GetInt("Completed FTUE") == 0)
        {
            if (other.gameObject.CompareTag("Target"))
            {
                GameObject.FindWithTag("Target").SetActive(false);
            }
    
            if (other.CompareTag("Target1"))
            {
                GameObject.FindWithTag("Target1").SetActive(false);
            }

            if (other.CompareTag("Target2"))
            {
                GameObject.FindWithTag("Target2").SetActive(false);
            }
            if (other.CompareTag("Target3"))
            {
                GameObject.FindWithTag("Target3").SetActive(false);
            }
        }
    }
}
