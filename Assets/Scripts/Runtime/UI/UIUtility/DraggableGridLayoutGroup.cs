using Runtime.UI.UIUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableGridLayoutGroup : GridLayoutGroup, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        
        GetComponentInParent<SwipeMenuLayout>().OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponentInParent<SwipeMenuLayout>().OnPointerUp(eventData);
    }
}
