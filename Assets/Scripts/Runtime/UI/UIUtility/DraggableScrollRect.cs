using Runtime.UI.UIUtility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableScrollRect : ScrollRect
{
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        GetComponentInParent<SwipeMenuLayout>().OnDrag(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        GetComponentInParent<SwipeMenuLayout>().OnPointerDown(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        GetComponentInParent<SwipeMenuLayout>().OnPointerUp(eventData);
    }
}
