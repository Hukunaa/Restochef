using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.Managers.GameplayManager.Orders.CustomClass
{
    public class OrderFeedback : MonoBehaviour
    {
        [SerializeField] 
        private CanvasGroup _orderCanvasGroup;
        
        [SerializeField] 
        private Image _orderCompleteFeedbackImage;

        [SerializeField]
        private AudioSource _orderCompleteAudio;

        [SerializeField] 
        private Color _orderSuccessFeedbackColor = Color.green;

        [SerializeField]
        private AudioClip _orderSucessFeedbackAudio;
        
        [SerializeField] 
        private Color _orderFailureFeedbackColor = Color.red;

        [SerializeField]
        private AudioClip _orderFailureFeedbackAudio;

        [SerializeField] 
        private float _feedbackFadeInDuration = .5f;
        
        [SerializeField] 
        private float _orderCompleteFadeOutDuration = .5f;

        [SerializeField] 
        private float _feedbackDuration = 1;

        [SerializeField] 
        private float _orderCompleteScale = 1.2f;

        [SerializeField] private UnityEvent _onOrderFeedbackEffectComplete;
        
        public void PlayFeedback(bool _orderSuccessful)
        {
            StartCoroutine(PlayOrderCompleteFeedbackCoroutine(_orderSuccessful));
        }
        
        private IEnumerator PlayOrderCompleteFeedbackCoroutine(bool _success)
        {
            _orderCompleteAudio.PlayOneShot(_success ? _orderSucessFeedbackAudio : _orderFailureFeedbackAudio);

            transform.DOScale(new Vector3(_orderCompleteScale, _orderCompleteScale, 1), _feedbackFadeInDuration);
            _orderCompleteFeedbackImage.DOColor(_success ? _orderSuccessFeedbackColor : _orderFailureFeedbackColor,
                _feedbackFadeInDuration);
            yield return new WaitForSeconds(_feedbackFadeInDuration);
            
            transform.DOScale(new Vector3(1, 1, 1), _feedbackFadeInDuration);


            yield return new WaitForSeconds(_feedbackDuration);

            _orderCanvasGroup.DOFade(0, _orderCompleteFadeOutDuration);
            transform.DOScale(new Vector3(0.1f, 0.1f, 1), _orderCompleteFadeOutDuration);
            yield return new WaitForSeconds(_orderCompleteFadeOutDuration);
            _onOrderFeedbackEffectComplete?.Invoke();
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
            _orderCanvasGroup.alpha = 1;
            _orderCompleteFeedbackImage.color = new Color(1, 1, 1, 0);
        }
    }
}