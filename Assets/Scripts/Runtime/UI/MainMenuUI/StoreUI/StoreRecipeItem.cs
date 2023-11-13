using System;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.StoreUI
{
    public class StoreRecipeItem : StoreItem
    {
        [SerializeField]
        private Button _recipeButton;
        
        [SerializeField] 
        private Image _recipeImage;
        
        [Header("Asset References")]
        [SerializeField]
        private RarityColors _rarityColors;

        [Header("Broadcasting on")]
        [SerializeField] 
        private  StoreRecipeItemEventChannel _onShowRecipeInfoEventChannel;
        
        private Recipe _recipe;
        
        public event Action<StoreRecipeItem> OnPurchase; 

        public void Initialize(Recipe _recipe, int _price)
        {
            this._recipe = _recipe;
            this._price = _price;
            
            _recipeImage.sprite = _recipe.RecipeIcon;
            if (_recipeButton != null)
                _recipeButton.GetComponent<Image>().color = _rarityColors.Values[(int)_recipe.Rarity];
        }

        public override void PurchaseItem()
        {
            OnPurchase?.Invoke(this);
        }

        public override void ShowItemInfo()
        {
            DeselectItem();
            _onShowRecipeInfoEventChannel.RaiseEvent(this);
        }

        public Recipe Recipe => _recipe;
    }
}