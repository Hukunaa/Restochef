using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.DataContainers;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class RecipeItemUI : InventoryItemUI
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
        private  RecipeEventChannel _onShowRecipeInfoEventChannel;

        public void Initialize(Recipe _recipe)
        {
            this.Recipe = _recipe;
            _recipeImage.sprite = _recipe.RecipeIcon;

            if (_recipeButton != null)
                _recipeButton.GetComponent<Image>().color = _rarityColors.Values[(int)_recipe.Rarity];
        }
        
        public override void ShowItemInfo()
        {
            DeselectItem();
            _onShowRecipeInfoEventChannel.RaiseEvent(Recipe);
        }

        public Recipe Recipe { get; private set; }
    }
}