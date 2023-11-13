using DG.Tweening;
using UnityEngine;

namespace Runtime.UI.Tutorial
{
    public class OrderTutorialManager : MonoBehaviour
    {
        [SerializeField] 
        private Material _highlightMaterial;

        [SerializeField] 
        private float _scaleAmount = 1.2f;

        [SerializeField] 
        private float _scaleDuration = 0.5f;

        public void HighlightTimerBar()
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var timerBar = order.Find("GaugeBackgroundMask");
            
            timerBar.DOScale(_scaleAmount, _scaleDuration).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        
        public void RemoveTimeBarHighlight()
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var timerBar = order.Find("GaugeBackgroundMask");

            timerBar.DOKill();
            timerBar.DOScale(1, _scaleDuration).SetUpdate(true);
        }
        
        
        public void HighlightOrderRawIngredient(int _ingredientIndex)
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var orderIngredient = order.Find("IngredientContainer").GetChild(_ingredientIndex);

            var rawIngredientImage = orderIngredient.GetChild(0);

            rawIngredientImage.DOScale(_scaleAmount, _scaleDuration).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        
        public void RemoveRawIngredientHighlight(int _ingredientIndex)
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var orderIngredient = order.Find("IngredientContainer").GetChild(_ingredientIndex);

            var rawIngredientImage = orderIngredient.GetChild(0);
            
            rawIngredientImage.DOKill();
            rawIngredientImage.DOScale(1, _scaleDuration).SetUpdate(true);
        }
        
        public void HighlightOrderIngredientStation(int _ingredientIndex)
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var orderIngredient = order.Find("IngredientContainer").GetChild(_ingredientIndex);

            var ingredientStationImage = orderIngredient.GetChild(1);
            
            ingredientStationImage.DOScale(_scaleAmount, _scaleDuration).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        
        public void RemoveIngredientStationHighlight(int _ingredientIndex)
        {
            var order = transform.GetChild(0);

            if (order == null)
            {
                Debug.LogWarning($"No order was found.");
            }

            var orderIngredient = order.Find("IngredientContainer").GetChild(_ingredientIndex);

            var ingredientStationImage = orderIngredient.GetChild(1);
            ingredientStationImage.DOKill();
            ingredientStationImage.DOScale(1, _scaleDuration).SetUpdate(true);
        }
    }
}