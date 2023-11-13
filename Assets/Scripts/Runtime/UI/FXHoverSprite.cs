using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FXHoverSprite : MonoBehaviour
{
    [SerializeField]
    private float _amount;
    [SerializeField]
    private float _duration;

    private void Start()
    {
    }

    public void StartAnimation()
    {
        GetComponent<RectTransform>()?.DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + _amount, _duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
