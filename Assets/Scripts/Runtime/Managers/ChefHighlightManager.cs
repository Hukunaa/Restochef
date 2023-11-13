using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefHighlightManager : MonoBehaviour
{
    [SerializeField] private Transform _meshesParent;
    
    public void HighlightChef()
    {
        var childCount = _meshesParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _meshesParent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("OutlineYellow");
        }
    }

    public void RemoveHighlight()
    {
        var childCount = _meshesParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            _meshesParent.GetChild(i).gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
