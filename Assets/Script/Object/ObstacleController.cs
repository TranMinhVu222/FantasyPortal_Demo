using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float speed,distanceGround, distance,boost;
    public bool movingRight, completeMonster;
    public Rigidbody2D rb;
    public Transform  exitPosition, enterPosition, groundDetection;
    public GameObject entryPortal, enterPortal;
    public Vector2 direction;
    public PlayerController playerController;
    public QuestManager questManager;
    public AudioManager audioManager;
    public float angle;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        movingRight = true;
    }

    void Update()
    {
        LayerMask layerMask = LayerMask.GetMask("Enemy","Platfoms","Item","VictoryZone");
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, 
            Vector2.down, distanceGround,1<<LayerMask.NameToLayer("Platfoms"));
        RaycastHit2D checkCollision = Physics2D.Raycast(groundDetection.position, 
            transform.right, distance,layerMask);
        if (PlayerPrefs.GetInt("Complete Menu FTUE") == 1)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (checkCollision.collider || groundInfo.collider == false)
            {
                if (movingRight)
                {
                    transform.eulerAngles = new Vector3(0, -180, 0);
                    movingRight = false;
                }
                else
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    movingRight = true;
                }
            }    
        }
        Debug.DrawRay(groundDetection.position, Vector2.down * distanceGround);
        Debug.DrawRay(groundDetection.position, transform.right * distance);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnterPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy)
        {
            transform.position = exitPosition.position;
            rb.AddForce(exitPosition.localPosition * boost);
        }
        if (other.gameObject.CompareTag("EntryPortal") && entryPortal.activeInHierarchy && enterPortal.activeInHierarchy)
        {
            transform.position = enterPosition.position;
            rb.AddForce(enterPosition.localPosition * boost);
        }
        direction = other.GetContact(0).normal;
        if (other.gameObject.CompareTag("Player"))
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle >= -95f && angle <= -80f)
            {
                completeMonster = true;
                audioManager.PlaySFX("DefeatMonster");
                questManager.countKillMonster++;
                gameObject.SetActive(false);
            }
            else
            {
                playerController.GameOver();
            }

            Debug.Log("----" + angle);
        }
        if (other.gameObject.CompareTag("Item"))
        {
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (angle >= -95f && angle <= -88f)
            {
                completeMonster = true;
                audioManager.PlaySFX("DefeatMonster");
                questManager.countKillMonster++;
                gameObject.SetActive(false);
            }
        }

        if (other.gameObject.CompareTag("DeathZone"))
        {
            completeMonster = true;
            audioManager.PlaySFX("DefeatMonster");
            questManager.countKillMonster++;
            gameObject.SetActive(false);
        }
    }
}
