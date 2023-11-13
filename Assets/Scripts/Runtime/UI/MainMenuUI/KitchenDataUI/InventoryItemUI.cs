using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public abstract class InventoryItemUI : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField]
        private Canvas _itemCanvas;

        [SerializeField] 
        private GameObject _selectedUI;
        
        [SerializeField] 
        private Button _changeUseStateButton;
        
        [SerializeField] 
        private TMP_Text _changeUseStateText;

        [SerializeField][Tooltip("Color of the ChangeStateButton when the item is used.")]
        private Sprite _itemDefaultButtonImage;
        
        [SerializeField][Tooltip("Color of the ChangeStateButton when the item is not used.")]
        private Sprite _itemUsedButtonImage;

        [SerializeField][Tooltip("The text displayed on the ChangeUseStateButton when the item is used.")]
        private string _usedText;
        
        [SerializeField][Tooltip("The text displayed on the ChangeUseStateButton when the item is not used.")]
        private string _defaultText;
        
        private bool _isSelected;
        private bool _used;

        private InventoryItemUIManager _inventoryItemManager;
        
        private void Start()
        {
            ChangeUseStateButtonImage(_itemDefaultButtonImage);
            ChangeUseStateButtonText(_defaultText);
            DeselectItem();
        }

        public void OnItemButtonClicked()
        {
            if (_isSelected)
            {
                _inventoryItemManager.UpdateSelectedItem(null);
                DeselectItem();
            }
            else
            {
                _inventoryItemManager.UpdateSelectedItem(this);
                SelectItem();
            }
        }

        public void SetInventoryItemManager(InventoryItemUIManager _manager)
        {
            _inventoryItemManager = _manager;
        }

        private void SelectItem()
        {
            _isSelected = true;
            _selectedUI.SetActive(true);
            _itemCanvas.overrideSorting = true;
            _itemCanvas.sortingOrder = 1;
        }

        public void DeselectItem()
        {
            _isSelected = false;
            _selectedUI.SetActive(false);
            _itemCanvas.overrideSorting = false;
        }

        public void OnChangeUseStateButtonClicked()
        {
            if (_used)
            {
                RemoveItem();
                DeselectItem();
            }

            else
            {
                UseItem();
                DeselectItem();
            }
        }
        
        public void UseItem()
        {
            if (!_inventoryItemManager.UseItem(this)) return;
            
            _used = true;
            ChangeUseStateButtonImage(_itemUsedButtonImage);
            ChangeUseStateButtonText(_usedText);
        }

        public void RemoveItem()
        {
            if(!_inventoryItemManager.RemoveItem(this)) return;
            
            _used = false;
            ChangeUseStateButtonImage(_itemDefaultButtonImage);
            ChangeUseStateButtonText(_defaultText);
        }
        
        private void ChangeUseStateButtonImage(Sprite _image)
        {
            _changeUseStateButton.image.sprite = _image;
        }

        private void ChangeUseStateButtonText(string _newText)
        {
            _changeUseStateText.text = _newText;
        }

        public abstract void ShowItemInfo();
    }
}