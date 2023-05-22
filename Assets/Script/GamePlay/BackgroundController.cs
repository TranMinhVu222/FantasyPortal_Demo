using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    public GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, gameObject.transform.position.z); 
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, gameObject.transform.position.z);
    }
}
