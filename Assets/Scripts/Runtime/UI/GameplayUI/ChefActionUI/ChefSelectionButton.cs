using System;
using DG.Tweening;
using Runtime.Gameplay;
using Runtime.ScriptableObjects.DataContainers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI.GameplayUI.ChefActionUI
{
    public class ChefSelectionButton : MonoBehaviour
    {
        [SerializeField] private TMP_Text _chefNameText;
        
        [SerializeField] 
        private Image _chefHeadPortraitImage;
        
        [SerializeField] 
        private InstructionCancelButton _currentInstructionButton;
        
        [SerializeField] 
        private InstructionCancelButton _queuedInstructionButton;

        [SerializeField] 
        private Image _headerImage;
        
        [SerializeField] 
        private Transform _chefCard;

        [SerializeField]
        private Image _cardBackground;

        [Header("Chef Selected animation settings")]
        [SerializeField] 
        private Color _defaultHeaderTint;
        
        [SerializeField] 
        private Color _chefSelectedHeaderTint;
        
        [SerializeField] 
        private float _animationDuration = .2f;
        
        [SerializeField] 
        private float _translationAmount = 5;
        
        [SerializeField] 
        private float _scaleAmount = 1.1f;

        [SerializeField]
        private RarityColors _rarityColors;

        [SerializeField]
        private Chef _chef;

        public UnityAction<ChefSelectionButton> onChefButtonSelected;
        public UnityAction<ChefSelectionButton> onChefButtonDeselected;

        private bool _isSelected;

        public void Initialize(Chef _chef)
        {
            this._chef = _chef;
            _chefNameText.text = _chef.Data.ChefName;
            _chefHeadPortraitImage.sprite = _chef.Data.ChefSettings.ChefHeadPortrait;
            _cardBackground.color = _rarityColors.Values[(int)_chef.Data.Rarity];
            _chef.onInstructionsChanged += UpdateInstructionButtons;
            UpdateInstructionButtons();
            _chef.onChefClicked += OnChefButtonClicked;
        }

        private void UpdateInstructionButtons()
        {
            if (_chef.CurrentInstruction == null)
            {
                _currentInstructionButton.RemoveInstruction();
            }
            else
            {
                _currentInstructionButton.SetInstruction(_chef.CurrentInstruction);
            }

            if (_chef.QueuedInstruction == null)
            {
                _queuedInstructionButton.RemoveInstruction();
            }

            else
            {
                _queuedInstructionButton.SetInstruction(_chef.QueuedInstruction);
            }
        }

        public void OnChefButtonClicked()
        {
            if (!_isSelected)
            {
                SelectChef();
            }

            else
            {
                DeselectChef();
            }
        }

        private void SelectChef()
        {
            _isSelected = true;
            onChefButtonSelected?.Invoke(this);
            _headerImage.DOColor(_chefSelectedHeaderTint, _animationDuration);
            _chefCard.DOMove( _chefCard.position + new Vector3(0, _translationAmount), _animationDuration);
            _chefCard.DOScale(new Vector3(_scaleAmount, _scaleAmount, 1), _animationDuration);
            _chef.ChefHighlightManager.HighlightChef();
        }

        public void DeselectChef()
        {
            _isSelected = false;
            onChefButtonDeselected?.Invoke(this);
            _headerImage.DOColor(_defaultHeaderTint, _animationDuration);
            _chefCard.DOMove(_chefCard.position - new Vector3(0, _translationAmount), _animationDuration);
            _chefCard.DOScale(new Vector3(1, 1, 1), _animationDuration);
            _chef.ChefHighlightManager.RemoveHighlight();
        }
        
        public Chef Chef => _chef;
    }
}