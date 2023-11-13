using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Runtime.UI.UIUtility
{
    public class AnimateElementToScreen : MonoBehaviour
    {
        enum ANIMATION_DIRECTION
        {
            RIGHT, LEFT, DOWN, UP
        }

        [Tooltip("Toogle on if you want the element to be out of screen at the beginning of the animation")]
        [SerializeField]
        private bool _isOffsetAtStart;
        [SerializeField]
        private ANIMATION_DIRECTION _animationDir;
        [SerializeField]
        private Ease _animationEaseType;
        [SerializeField]
        private float _animationDuration;
        [SerializeField]
        private float _animationStartDelay;

        private RectTransform _rect;
        private Vector2 _startPos;

        private void Start()
        {
            _rect = GetComponent<RectTransform>();
            _startPos = _rect.anchoredPosition;
            if(_isOffsetAtStart)
            {
                switch(_animationDir)
                {
                    case ANIMATION_DIRECTION.RIGHT:
                        _rect.anchoredPosition -= new Vector2(_rect.rect.width, 0);
                        break;
                    case ANIMATION_DIRECTION.LEFT:
                        _rect.anchoredPosition += new Vector2(_rect.rect.width, 0);
                        break;
                    case ANIMATION_DIRECTION.DOWN:
                        _rect.anchoredPosition += new Vector2(0, _rect.rect.height);
                        break;
                    case ANIMATION_DIRECTION.UP:
                        _rect.anchoredPosition -= new Vector2(0, _rect.rect.height);
                        break;
                }
            }

            AnimateElement(_animationDuration, _animationStartDelay, _animationEaseType);
        }

        void AnimateElement(float _duration, float delay, Ease _ease)
        {
            if(_isOffsetAtStart)
            {
                _rect.DOKill();
                _rect.DOAnchorPos(_startPos, _duration).SetDelay(delay).SetEase(_ease);
            }
        }
    }
}
