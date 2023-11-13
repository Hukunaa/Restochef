using MPewsey.HexagonalUI;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class IngredientSelectionButton : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform _rectTransform;
        
        [SerializeField] 
        private Image _ingredientIcon;

        [SerializeField] 
        private Button _ingredientButton;

        [SerializeField]
        private CanvasGroup _ingredientCanvasGroup;
        
        private RawIngredient _ingredient;
        private StationManager _stationManager;
        
        public UnityAction<IngredientSelectionButton> _onIngredientSelected;
        public UnityAction<IngredientSelectionButton> _onIngredientDeselected;
        
        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }

        public void SetIngredient(RawIngredient _ingredient)
        {
            this._ingredient = _ingredient;
            _ingredientIcon.sprite = _ingredient.IngredientIcon;
        }
        
        public void IngredientButtonClicked()
        {
            _onIngredientSelected?.Invoke(this);
        }

        public void SetIngredientButtonInactive()
        {
            _ingredientButton.interactable = false;
            _ingredientCanvasGroup.alpha = 0.5f;
        }

        public void SetIngredientButtonActive()
        {
            _ingredientButton.interactable = true;
            _ingredientCanvasGroup.alpha = 1f;

        }

        public RectTransform RectTransform => _rectTransform;
        public RawIngredient Ingredient => _ingredient;
    }
}