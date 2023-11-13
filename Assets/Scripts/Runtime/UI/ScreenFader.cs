using System.Collections;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.UI
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private FadeChannel _fadeChannel;
		
        [SerializeField] private CanvasGroup _faderCanvasGroup;

        [SerializeField] private GameObject _loadingEffect;
        
        private void OnEnable()
        {
            _fadeChannel.onEventRaised += InitiateFade;
        }

        private void OnDisable()
        {
            _fadeChannel.onEventRaised -= InitiateFade;
        }

        private IEnumerator Fade(bool _fadeIn, float _fadeDuration)
        {
            var finalAlpha = _fadeIn ? 0 : 1;
            
            if (_fadeIn)
            {
                _loadingEffect.SetActive(false);
            }
            else
            {
                _faderCanvasGroup.blocksRaycasts = true;
            }

            _faderCanvasGroup.blocksRaycasts = true;
            float fadeSpeed = Mathf.Abs(_faderCanvasGroup.alpha - finalAlpha) / _fadeDuration;
            while (!MathCalculation.ApproximatelyEqualFloat(_faderCanvasGroup.alpha, finalAlpha, .05f))
            {
                _faderCanvasGroup.alpha = Mathf.MoveTowards(_faderCanvasGroup.alpha, finalAlpha,
                    fadeSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
            _faderCanvasGroup.alpha = finalAlpha;

            if (!_fadeIn)
            {
                _loadingEffect.SetActive(true);
            }
            else
            {
                _faderCanvasGroup.blocksRaycasts = false;
            }
        }

        public void SetAlpha(float _alpha)
        {
            _faderCanvasGroup.alpha = _alpha;
        }
		
        private void InitiateFade(bool _fadeIn, float _duration)
        {
            StartCoroutine(Fade(_fadeIn, _duration));
        }
    }
}