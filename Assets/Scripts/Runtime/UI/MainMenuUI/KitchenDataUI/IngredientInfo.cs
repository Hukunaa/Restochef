using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class IngredientInfo : MonoBehaviour
    {
        [SerializeField] private Image _rawIngredientImage;
        [SerializeField] private Image _processStationImage;

        public void SetIngredient(Sprite _rawIngredient, Sprite _processType)
        {
            _rawIngredientImage.sprite = _rawIngredient;
            _processStationImage.sprite = _processType;
        }
    }
}