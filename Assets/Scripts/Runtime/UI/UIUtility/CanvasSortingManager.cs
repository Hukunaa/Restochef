using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.UIUtility
{
    public class CanvasSortingManager : MonoBehaviour
    {
        private Canvas _canvas;
        [SerializeField]
        private bool _defaultOverrideSorting = false;
        
        [SerializeField]
        private int _defaultSortingOrder = 0;
        
        public void OverrideSorting(int _sortingOrder)
        {
            if (_canvas == null)
            {
                CreateCanvas();
            }
            
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = _sortingOrder;
        }

        public void StopOverride()
        {
            if (_canvas == null)
            {
                CreateCanvas();
            }
            _canvas.overrideSorting = _defaultOverrideSorting;
            _canvas.sortingOrder = _defaultSortingOrder;
        }

        private void CreateCanvas()
        {
            _canvas = transform.AddComponent<Canvas>();
            if (transform.GetComponent<GraphicRaycaster>() != null) return;
            transform.AddComponent<GraphicRaycaster>();
        }
    }
}