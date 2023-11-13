using System.Collections.Generic;
using Runtime.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShiftRaycastManager : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current == null) return;
        
        if(Input.GetMouseButtonDown(0) && !IsMouseOverUIWithIgnores())
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(r, out RaycastHit hit, 500, LayerMask.GetMask("Clickable")))
            {
                switch (hit.collider.gameObject.tag)
                {
                    case "Station":
                        hit.collider.gameObject.GetComponentInParent<Station>().CheckFire();
                        break;
                    case "Chef":
                        hit.collider.gameObject.GetComponentInParent<Chef>().ChefClicked();
                        break;
                }
            }
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsMouseOverUIWithIgnores()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.CompareTag("ClickBlocker"))
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count > 0;
    }
}
