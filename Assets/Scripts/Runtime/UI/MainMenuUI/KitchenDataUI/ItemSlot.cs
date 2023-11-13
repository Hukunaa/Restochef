using UnityEngine;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class ItemSlot : MonoBehaviour
    {
        private bool _filled;

        private InventoryItemUI _itemInSlot;

        [SerializeField]
        private bool _isMandatory;
        
        public ItemSlot() {}

        public void AssignItem(InventoryItemUI _item)
        {
            _filled = true;
            _itemInSlot = _item;
            _itemInSlot.transform.SetParent(transform, false);
            var rectTransform = (RectTransform)_item.transform;
            rectTransform.anchorMin = new Vector2(.5f, .5f);
            rectTransform.anchorMax = new Vector2(.5f, .5f);
            rectTransform.pivot = new Vector2(.5f, .5f);
            rectTransform.anchoredPosition = new Vector2(0, 0);
        }

        public void ClearSlot()
        {
            _filled = false;
            _itemInSlot = null;
        }

        public bool Filled => _filled;
        public InventoryItemUI ItemInSlot => _itemInSlot;
    }
}