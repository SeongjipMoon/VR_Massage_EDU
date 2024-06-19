using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using OculusSampleFramework;

public class HandInteraction : MonoBehaviour
{
    public OVRInput.Controller controller;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                foreach (var result in results)
                {
                    if (result.gameObject.CompareTag("Interactable"))
                    {
                        ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                        Debug.Log("Clicked on " + result.gameObject.name);
                    }
                }
            }
        }
    }
}