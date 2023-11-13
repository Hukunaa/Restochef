using System.Linq;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.ProgressPath
{
    public class RecipeRewardPopup : RewardPopup
    {
        [SerializeField] 
        private TMP_Text _recipeNameText;
        
        [SerializeField] 
        private Image _recipeIconImage;
        
        [SerializeField] 
        private TMP_Text _recipePointsText;

        [SerializeField] 
        private RarityColors _rarityColors;

        public Color RarityColor { get; private set; }
        
        [SerializeField] 
        private RecipeIngredientInfoManager _recipeIngredientInfoManager;
        
        public override void InitializePopup(RewardItem _rewardItem)
        {
            var recipe = GameManager.Instance.PlayerDataContainer.RecipesRewards.FirstOrDefault(x =>
                x.name == _rewardItem.RecipeReward);

            if (recipe == null)
            {
                Debug.LogWarning($"No recipes with the name {_rewardItem.RecipeReward} could be found in the Recipe Rewards.");
                return;
            }

            _recipeNameText.text = recipe.RecipeName;
            _recipeIconImage.sprite = recipe.RecipeIcon;
            _recipePointsText.text = recipe.RecipePoints.ToString();
            _recipeIngredientInfoManager.InitializeIngredients(recipe);
            RarityColor = _rarityColors.Values[(int)recipe.Rarity];
        }
    }
}