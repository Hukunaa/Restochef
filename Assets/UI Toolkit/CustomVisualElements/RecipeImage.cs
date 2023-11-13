using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace UI_Toolkit.VisualElements
{
    public class RecipeImage : VisualElement
    {
        public Image Icon;
        public string ItemGuid = "";

        public RecipeImage()
        {
            Icon = new Image();
            Add(Icon);
            
            Icon.AddToClassList("recipe-image");
        }

        public void SetImage(Texture _image)
        {
            Icon.image = _image;
        }

        #region UXML
        [Preserve]
        public new class UxmlFactory : UxmlFactory<RecipeImage, UxmlTraits> { }
        [Preserve]
        public new class UxmlTraits : VisualElement.UxmlTraits { }
        #endregion
    }
}