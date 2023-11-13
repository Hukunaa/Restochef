using System.Collections;
using DG.Tweening;
using Runtime.Managers.GameplayManager;
using Runtime.UI.UIUtility;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.UI.GameplayUI
{
    public class ShiftEndUI : MonoBehaviour
    {
        [SerializeField] 
        private ShiftScoreManager _shiftScoreManager;
        
        [SerializeField] 
        private ShiftRewardManager _shiftRewardManager;

        [SerializeField] 
        private OrderCountUI _orderCountUI;

        [SerializeField] 
        private EndShiftChefXPManager _chefXPManager;
        
        [SerializeField] 
        private TMP_Text _successfulOrderCountText;
        
        [SerializeField]
        private TMP_Text _failedOrderCountText;
        
        [SerializeField] 
        private TMP_Text _highScoreText;
        
        [SerializeField] 
        private TMP_Text _scoreText;
        
        [SerializeField] 
        private TMP_Text _currencyText;
        
        [SerializeField]
        private TMP_Text _kitchenXPText;

        [SerializeField] 
        private CanvasGroup _canvasGroup;
        
        [SerializeField] 
        private UnityEvent _onDisplayUI;
        
        private void Start()
        {
            HideUI(false);
        }
        
        public void DisplayUI(bool _fadeIn)
        {
            _onDisplayUI?.Invoke();
            
            SetOrderCount();

            _highScoreText.gameObject.SetActive(_shiftScoreManager.IsHighScore);
            SetScoreEarner();
            
            _shiftRewardManager.CalculateShiftRewards();
            SetCurrencyEarned();
            SetKitchenXPEarned();
            _chefXPManager.Initialize();
            
            
            if (_fadeIn)
            {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(true);
                StartCoroutine(FadeIn());
            }

            else
            {
                _canvasGroup.alpha = 1;
                _canvasGroup.interactable = true;
                gameObject.SetActive(true);
            }
        }

        private IEnumerator FadeIn()
        {
            yield return _canvasGroup.DOFade(1, 1);
            _canvasGroup.interactable = true;
        }

        private void HideUI(bool _fade)
        {
            if (_fade)
            {
                StartCoroutine(FadeOut());
            }

            else
            {
                _canvasGroup.alpha = 0;
                _canvasGroup.interactable = false;
                gameObject.SetActive(false);
            }
        }

        private IEnumerator FadeOut()
        {
            yield return _canvasGroup.DOFade(0, 1);
            _canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }

        private void SetOrderCount()
        {
            _successfulOrderCountText.text = _orderCountUI.OrderSuccessCount.ToString();
            _failedOrderCountText.text = _orderCountUI.OrderFailureCount.ToString();
        }
        
        private void SetScoreEarner()
        {
            _scoreText.text = _shiftScoreManager.CurrentScore.ToString();
        }

        private void SetCurrencyEarned()
        {
            _currencyText.text = _shiftRewardManager.SoftCurrencyEarned.ToString();
        }

        private void SetKitchenXPEarned()
        {
            _kitchenXPText.text = _shiftRewardManager.KitchenXpEarned.ToString();
        }
    }
}