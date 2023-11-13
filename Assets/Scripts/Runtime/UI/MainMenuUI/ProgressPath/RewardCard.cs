using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableObjects.DataContainers;

namespace Runtime.UI.MainMenuUI.StarProgressPath
{
    public class RewardCard : MonoBehaviour
    {
        [SerializeField]
        private Image _rewardImage;
        [SerializeField]
        private TMP_Text _rewardTitle;
        [SerializeField]
        private TMP_Text _rewardDesc;
        [SerializeField]
        private TMP_Text _rewardStarsRequirement;
        [SerializeField]
        private Button _rewardCollectButton;
        [SerializeField]
        private TMP_Text _rewardCollectButtonText;
        [SerializeField]
        private GameObject _rewardComplete;
        [SerializeField]
        private GameObject _rewardLock;
        [SerializeField]
        private GameObject _rewardScalableElement;

        private int _rewardStars;
        RewardItem _linkedItem;

        public void SetRewardCard(RewardItem _reward, int _starsRequirement)
        {
            _linkedItem = _reward;
            _rewardStars = _starsRequirement;

            if (_linkedItem == null)
                return;

            if (_rewardTitle != null)
                _rewardTitle.text = _linkedItem.Title;
            if (_rewardDesc != null)
                _rewardDesc.text = _linkedItem.Description;
            if (_rewardImage != null)
                _rewardImage.sprite = _linkedItem.RewardSprite;
            if (_rewardStarsRequirement != null)
                _rewardStarsRequirement.text = _rewardStars.ToString();
        }

        public void SetRewardLockState(bool _value, bool _doLockButton = true)
        {
            _rewardLock.SetActive(_value);
            if(_doLockButton)
            {
                _rewardCollectButton.interactable = !_value;
                _rewardCollectButtonText.text = _rewardCollectButton.interactable ? "Claim" : "Locked";
            }
        }

        public void SetRewardState(bool _value)
        {
            if (_rewardComplete != null)
            {
                _rewardComplete.SetActive(!_value);
                _rewardCollectButton.gameObject.SetActive(_value);
            }
        }
        public Button InteractionButton { get => _rewardCollectButton; }
        public RewardItem LinkedItem { get => _linkedItem; }
        public GameObject RewardScalableElement { get => _rewardScalableElement; }
    }
}
