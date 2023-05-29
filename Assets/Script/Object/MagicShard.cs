using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShard : MonoBehaviour
{ 
    public bool floatUp, completeMagicShard;
    private Rigidbody2D rb;
    public float s0, v, t, g, floatPosition, fallPosition, smax;
    public QuestManager questManager;
    public AudioManager audioManager;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        floatUp = false;
        s0 = transform.position.y;
        completeMagicShard = false;
    }

    
    private void Update()
    {
        if (floatUp)
        {
            floatPosition = s0 + v * t + 0.5f * g * t * t;
            transform.position = new Vector3(gameObject.transform.position.x, floatPosition, 0);
            if (floatPosition - s0 >= smax)
            {
                t = 0;
                floatUp = false;
                s0 = transform.position.y;
            }
        }
        if (!floatUp)
        {
            fallPosition = s0 - 0.5f * g * t * t;
            transform.position = new Vector3(gameObject.transform.position.x, fallPosition, 0);
        }
        t += Time.deltaTime;
    }
    
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            completeMagicShard = true;
            questManager.countCollectMagicShard += 1;
            AudioManager.AudioManger.PlaySFX("magicshardsound");
            gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("tilemap"))
        {
            t = 0;
            s0 = transform.position.y;
            floatUp = true;
        }
    }
}
