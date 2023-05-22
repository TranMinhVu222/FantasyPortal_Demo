using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTimeline : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if (other.CompareTag("tilemap"))
        {
            
        }
    }
}
