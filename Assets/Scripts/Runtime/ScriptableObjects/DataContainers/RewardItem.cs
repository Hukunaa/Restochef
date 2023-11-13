using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;

namespace ScriptableObjects.DataContainers
{
    public enum REWARD_TYPE
    {
        COINS,
        RANK,
        CHEF,
        RECIPE,
        CHEST
    }

    [Serializable]
    public struct RankReward
    {
        public int MaxStations;
        public int MaxStationsLVL;
        public int MaxChefs;
        public int MenuSize;
        public int BonusPoints;
    }

    [CreateAssetMenu(fileName = "RewardItem", menuName = "ScriptableObjects/Rewards/UIRewardItem", order = 0)]
    public class RewardItem : ScriptableObject
    {
        [SerializeField]
        private REWARD_TYPE _rewardType;
        [SerializeField]
        private string _title;
        [SerializeField]
        private string _description;
        [SerializeField]
        private Sprite _rewardSprite;
        //Needs to be updated to support any item later when the reward system is done
        //For now, we only give coins
        [SerializeField]
        private int _coinsReward;
        [SerializeField]
        private string _recipeReward;
        [SerializeField]
        private string _chefID;
        [SerializeField]
        private int _starsRequired;
        [SerializeField]
        private RankReward _rankReward;
        //[SerializeField]
        //private bool _isUsable;

        /*public void Complete()
        {
            _isUsable = false;
        }*/
        public string RecipeReward => _recipeReward;

        public string ChefID => _chefID;

        public int CoinsReward { get => _coinsReward;  }

        public string Title { get => _title; }
        public string Description { get => _description; }
        public Sprite RewardSprite { get => _rewardSprite; }
        public REWARD_TYPE RewardType { get => _rewardType; }
        public int StarsRequired { get => _starsRequired; }
        public RankReward KitchenRankReward { get => _rankReward; }
        //public bool IsUsable { get => _isUsable; }
    }
}
