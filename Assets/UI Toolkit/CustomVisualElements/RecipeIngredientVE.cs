using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace UI_Toolkit.CustomVisualElements
{
    public class RecipeIngredientVE : VisualElement
    {
        public Image ingredientSprite;

        public RecipeIngredientVE()
        {
            ingredientSprite = new Image();

            Add(ingredientSprite);
            ingredientSprite.AddToClassList("ingredient-sprite");
        }
        
        public void SetIngredientSprite(Texture _image)
        {
            ingredientSprite.image = _image;
        }

        public async Task SetIngredientAssigned()
        {
            Scale scale = new Scale(new Vector3(2, 2, 1));
            ingredientSprite.style.scale = scale;
            await Task.Delay(250);
            scale.value = new Vector3(1, 1, 1);
            ingredientSprite.style.scale = scale;
            await Task.Delay(250);
            ingredientSprite.tintColor = Color.gray;
        }

        #region UXML
        [Preserve]
        public new class UxmlFactory : UxmlFactory<RecipeIngredientVE, UxmlTraits> { }
        [Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}