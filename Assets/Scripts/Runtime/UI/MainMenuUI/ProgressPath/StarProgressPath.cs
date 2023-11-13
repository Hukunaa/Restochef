using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.MainMenuUI.StarProgressPath;
using Runtime.Utility;
using ScriptableObjects.DataContainers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Runtime.UI.MainMenuUI.ProgressPath
{
    public class StarProgressPath : MonoBehaviour
    {
        [SerializeField]
        private AssetReference _rankRewardPrefab;
        [SerializeField]
        private AssetReference _cardRewardPrefab;
        [SerializeField]
        private AssetReference _cardRewardInvertedPrefab;
        [SerializeField]
        private GameObject _starProgressContent;
        [SerializeField]
        private ScrollRect _starProgressScrollRect;
        [SerializeField]
        private RectTransform _progressBar;
        [SerializeField]
        private GameObject _chefLoadingLogo;

        [SerializeField] 
        private RewardPopUpManager _rewardPopUpManager;
        
        private List<RewardCard> _rewardCards;
        private int _side = 0;
        private PlayerDataContainer _playerContainer;

        private bool _isOpen;
        private RewardCard _maxStarsSelectedCard;
        private RewardCard _maxDifferenceStarsSelectedCard;
        private AsyncOperationHandle<IList<Recipe>> _recipesLoadHandle;

        public async void ShowProgressPath()
        {
            if (_rewardCards == null)
            {
                _chefLoadingLogo.SetActive(true);
                await LoadAssets();
            }

            foreach (RewardCard _card in _rewardCards)
            {
                if (_card == null)
                    break;

                bool _lockButton = _card.LinkedItem.RewardType != REWARD_TYPE.RANK ? true : false;
                _card.SetRewardLockState(!(_playerContainer.SelectedKitchenData.kitchenStars >= _card.LinkedItem.StarsRequired), _lockButton);
                AnimateCard(_card);
            }
            _isOpen = true;
            _chefLoadingLogo.SetActive(false);
            ShowProgressBar();
            ScrollViewFocusFunctions.FocusOnItem(_starProgressScrollRect, _maxDifferenceStarsSelectedCard.GetComponent<RectTransform>());
        }

        public void ShowProgressBar()
        {
            List<RewardCard> _sortedCards = _rewardCards.OrderBy(p => p.LinkedItem.StarsRequired).ToList();
            for (int i = 0; i < _sortedCards.Count; ++i)
            {
                if ((_playerContainer.SelectedKitchenData.kitchenStars >= _sortedCards[i].LinkedItem.StarsRequired))
                {
                    _maxStarsSelectedCard = _sortedCards[i];
                    _maxDifferenceStarsSelectedCard = (i == 0) || (i == _sortedCards.Count - 1) ? _sortedCards[i] : _sortedCards[i + 1];
                }
            }
        }

        private void Update()
        {
            if(_isOpen)
            {
                if(_maxStarsSelectedCard != null)
                {
                    if (_maxDifferenceStarsSelectedCard.transform.position.y == _maxStarsSelectedCard.transform.position.y)
                    {
                        _progressBar.position = new Vector3(_progressBar.position.x, _maxDifferenceStarsSelectedCard.transform.position.y, _progressBar.position.z);
                    }
                    else
                    {
                        float offsetY = MathCalculation.Remap(_playerContainer.SelectedKitchenData.kitchenStars,
                            _maxStarsSelectedCard.LinkedItem.StarsRequired, _maxDifferenceStarsSelectedCard.LinkedItem.StarsRequired,
                            _maxStarsSelectedCard.transform.position.y, _maxDifferenceStarsSelectedCard.transform.position.y);
                        _progressBar.position = new Vector3(_progressBar.position.x, offsetY, _progressBar.position.z);
                    }
                }
                
            }
        }
        public void HideProgressPath()
        {
            foreach (RewardCard _card in _rewardCards)
            {
                _card.DOKill();
                _card.RewardScalableElement.GetComponent<RectTransform>().localScale = Vector3.zero;
            }
            _isOpen = false;
            gameObject.SetActive(false);

        }
        async Task LoadAssets()
        {
            _rewardCards = new List<RewardCard>();

            if (_playerContainer == null)
                _playerContainer = GameManager.Instance.PlayerDataContainer;

            for (int i = 0; i < _playerContainer.PlayerRewards.Count; ++i)
            {
                RewardItem item = _playerContainer.PlayerRewards[i];
                if (item.RewardType == REWARD_TYPE.RANK)
                {
                    GameObject _inst = await _rankRewardPrefab.InstantiateAsync(_starProgressContent.GetComponent<RectTransform>()).Task;
                    RewardCard card = _inst.GetComponent<RewardCard>();
                    _rewardCards.Add(card);
                    card.SetRewardCard(item, item.StarsRequired);
                    
                    card.InteractionButton.onClick.AddListener(delegate 
                    {
                        _rewardPopUpManager.ShowRewardItemPopup(item);
                    });

                    card.RewardScalableElement.GetComponent<RectTransform>().localScale = Vector3.zero;
                }
                else
                {
                    AssetReference prefab = _side % 2 == 0 ? _cardRewardPrefab : _cardRewardInvertedPrefab;
                    GameObject _inst = await prefab.InstantiateAsync(_starProgressContent.GetComponent<RectTransform>()).Task;
                    RewardCard card = _inst.GetComponent<RewardCard>();
                    _rewardCards.Add(card);
                    card.SetRewardCard(item, item.StarsRequired);

                    if (!_playerContainer.PlayerRewardsTextList[i]._isUsed)
                    {
                        card.InteractionButton.onClick.AddListener(delegate { RewardPlayer(item, card); });
                        card.SetRewardState(true);
                    }
                    else
                        card.SetRewardState(false);

                    card.RewardScalableElement.GetComponent<RectTransform>().localScale = Vector3.zero;
                    _side++;
                }
            }
        }

        void CompleteReward(RewardCard _card, RewardItem _item)
        {
            int index = _playerContainer.PlayerRewards.IndexOf(_item);
            _playerContainer.PlayerRewardsTextList[index]._isUsed = true;
            DataLoader.SaveListOfProgressRewards(_playerContainer.PlayerRewardsTextList);
            _card.SetRewardState(false);
        }

        void RewardPlayer(RewardItem _item, RewardCard _card)
        {
            switch (_item.RewardType)
            {
                case REWARD_TYPE.COINS:
                    _playerContainer.Currencies.AddBalance(Enums.ECurrencyType.SoftCurrency, _item.CoinsReward);
                    _rewardPopUpManager.ShowRewardItemPopup(_item);
                    break;
                
                case REWARD_TYPE.RANK:
                    break;
                
                case REWARD_TYPE.CHEF:
                    var chefReward = _playerContainer.ChefRewardData.FirstOrDefault(x => x.ChefID == _item.ChefID);
                    if (chefReward == null)
                    {
                        Debug.LogWarning($"Can't find any ChefData with the Chef ID {_item.ChefID}. Can't give reward.");
                        return;
                    }
                    _playerContainer.AddChefToPlayerInventory(chefReward);
                    _rewardPopUpManager.ShowRewardItemPopup(_item);
                    break;
                
                case REWARD_TYPE.RECIPE:
                    var recipeReward = _playerContainer.RecipesRewards.FirstOrDefault(x => x.name == _item.RecipeReward);
                    if (recipeReward == null)
                    {
                        Debug.LogWarning($"Can't find any Recipe Reward with the name {_item.RecipeReward}. Can't give reward.");
                        return;
                    }
                    _playerContainer.AddRecipeToPlayerInventory(recipeReward);
                    _rewardPopUpManager.ShowRewardItemPopup(_item);
                    break;
                
                case REWARD_TYPE.CHEST:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
                
            }
            CompleteReward(_card, _item);
        }

        void AnimateCard(RewardCard _card)
        {
            _card.DOKill();
            _card.RewardScalableElement.GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }
}
