using System;
using System.Collections;
using System.Collections.Generic;
using SpriteGlow;
using TMPro;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    private Rigidbody2D rb;
    public Transform pickUpPoint, player, exitPosition, enterPosition;
    public GameObject entryPortal, enterPortal, grabButton;
    public PlayerController playerController;
    public float force, forceMulti; 
    public bool itemIsPicked, readToThrow, point, completeGrab;
    public AudioManager audioManager;
    
    void Start()
    {
        player = GameObject.Find("Player").transform;
        pickUpPoint = GameObject.Find("PickUpPoint").transform;
        rb = GetComponent<Rigidbody2D>();
        force = 50f;
        completeGrab = false;
    }
    
    void Update()
    {
        Physics2D.IgnoreLayerCollision(9,10);
        Physics2D.IgnoreLayerCollision(14,15);
        if (point && itemIsPicked && readToThrow)
        {
            forceMulti += 300 * Time.deltaTime;
            AudioManager.AudioManger.PlaySFX("controlorthrowobject");
        }
        if (playerController.checkGrab)
        {
            if (point && itemIsPicked == false && pickUpPoint.childCount < 1)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<SpriteGlowEffect>().OutlineWidth = 10;
                transform.position = pickUpPoint.position;
                transform.parent = GameObject.Find("PickUpPoint").transform;
                itemIsPicked = true;
                forceMulti = 0;
                AudioManager.AudioManger.PlaySFX("controlorthrowobject");
                completeGrab = true;
            }
        }

        if (point == false && itemIsPicked)
        {
            readToThrow = true;
            if (forceMulti > 10)
            {
                this.transform.parent = null;
                GetComponent<Rigidbody2D>().isKinematic = false;
                GetComponent<SpriteGlowEffect>().OutlineWidth = 0;
                itemIsPicked = false;
                forceMulti = 0;
                readToThrow = false;
            }
            forceMulti = 0;
        }
        
        if (itemIsPicked)
        {
            grabButton.SetActive(true);
        }
        else
        {
            grabButton.SetActive(false);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnterPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy && itemIsPicked == false)
        {
           transform.position = exitPosition.position;
           rb.AddForce(exitPosition.localPosition * force);
        }
        if (other.gameObject.CompareTag("EntryPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy && itemIsPicked == false)
        {
            transform.position = enterPosition.position;
            rb.AddForce(enterPosition.localPosition * force);
        }
    }

    public void EnterGrabButton()
    {
        point = true;
    }

    public void ExitGrabButton()
    {
        point = false;
    }
}
