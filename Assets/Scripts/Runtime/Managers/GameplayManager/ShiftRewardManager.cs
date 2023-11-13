using System;
using System.Linq;
using Runtime.Enums;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;

namespace Runtime.Managers.GameplayManager
{
    public class ShiftRewardManager : MonoBehaviour
    {
        private int _softCurrencyEarned;
        private int _hardCurrencyEarned;

        private int _chefXpEarned;
        private int _kitchenXpEarned;

        private int _scoreWithBonus;

        [SerializeField] 
        private VoidEventChannel _retrieveShiftGains;
        
        [SerializeField] private ShiftScoreManager _shiftScoreManager;
        [SerializeField] private float xpMultiplierChefs = 1;
        [SerializeField] private float xpMultiplierKitchen = 1;

        private void Awake()
        {
            _retrieveShiftGains.onEventRaised += RetrieveGains;
        }

        private void OnDestroy()
        {
            _retrieveShiftGains.onEventRaised -= RetrieveGains;
        }

        public void CalculateShiftRewards()
        {
            _scoreWithBonus = _shiftScoreManager.CurrentScore + (_shiftScoreManager.CurrentScore * (GameManager.Instance.PlayerDataContainer.SelectedKitchenData.bonusPoints / 100));
            CalculateCurrencyGain();
            CalculateChefXpReward();
            CalculateKitchenReward();
        }

        private void CalculateCurrencyGain()
        {
            _softCurrencyEarned = _scoreWithBonus;
        }

        private void CalculateKitchenReward()
        {
            _kitchenXpEarned = Mathf.RoundToInt(_scoreWithBonus * xpMultiplierKitchen);
        }
        private void CalculateChefXpReward()
        {
            _chefXpEarned = Mathf.RoundToInt(_scoreWithBonus * xpMultiplierChefs);
        }

        private void RetrieveGains()
        {
            ApplyCurrenciesGain();
            ApplyKitchenXP();
        }

        private void ApplyCurrenciesGain()
        {
            GameManager.Instance.PlayerDataContainer.Currencies.AddBalance(ECurrencyType.SoftCurrency, _softCurrencyEarned);
        }

        public void ApplyChefsXp()
        {
            foreach (var chefData in GameManager.Instance.PlayerDataContainer.SelectedKitchenData._brigadeChefs)
            {
                chefData.GainXP(_chefXpEarned);
            }
        }

        private void ApplyKitchenXP()
        {
            GameManager.Instance.PlayerDataContainer.SelectedKitchenData.AddKitchenXP(_kitchenXpEarned);
        }

        public int SoftCurrencyEarned => _softCurrencyEarned;
        public int HardCurrencyEarned => _hardCurrencyEarned;

        public int ChefXpEarned => _chefXpEarned;
        public int KitchenXpEarned => _kitchenXpEarned;
    }
}