using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Runtime.UI
{
    public class ClickBlocker : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private UnityEvent _onBlockerClicked;
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _onBlockerClicked?.Invoke();
        }
    }
}