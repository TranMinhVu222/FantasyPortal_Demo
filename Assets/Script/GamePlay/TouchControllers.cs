using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchControllers : MonoBehaviour
{
    void Update()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> raycastResult = new List<RaycastResult>();
        foreach (Touch touch in Input.touches)
        {
            pointer.position = touch.position;
            EventSystem.current.RaycastAll(pointer, raycastResult);

            foreach (RaycastResult result in raycastResult)
            {
                if (result.gameObject.tag == "CancelButton")
                {
                    result.gameObject.GetComponent<CancelSkill>().EnterCancelButton();
                }
                else
                {
                    result.gameObject.GetComponent<CancelSkill>().ExitCancelButton();
                }
            }
            raycastResult.Clear();
        }
    }
}
