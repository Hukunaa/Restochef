using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.UI.MainMenuUI.KitchenDataUI;
using Runtime.UI.MainMenuUI.StoreUI;

namespace Runtime.Utility
{
    class SortingHelper
    {
        public static int CompareByRarity(ChefItemUI s1, ChefItemUI s2)
        {
            int s1Rarity = ((int)s1.ChefData.Rarity);
            int s2Rarity = ((int)s2.ChefData.Rarity);

            return s2Rarity.CompareTo(s1Rarity);
        }

        public static int CompareByRarity(RecipeItemUI s1, RecipeItemUI s2)
        {
            int s1Rarity = ((int)s1.Recipe.Rarity);
            int s2Rarity = ((int)s2.Recipe.Rarity);

            return s2Rarity.CompareTo(s1Rarity);
        }

        public static int CompareByRarity(StoreChefItem s1, StoreChefItem s2)
        {
            int s1Rarity = (int)s1.ChefData.Rarity;
            int s2Rarity = (int)s2.ChefData.Rarity;

            return s2Rarity.CompareTo(s1Rarity);
        }
        public static int CompareByRarity(StoreRecipeItem s1, StoreRecipeItem s2)
        {
            int s1Rarity = (int)s1.Recipe.Rarity;
            int s2Rarity = (int)s2.Recipe.Rarity;

            return s2Rarity.CompareTo(s1Rarity);
        }
    }
}
