using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.DataContainers.Stats
{
    [Serializable]
    public class ChefLevelData
    {
        [SerializeField]
        private int _level;
        [SerializeField]
        private int _currentXP;
        [SerializeField]
        private int _currentLevelXPGoal;

        public event Action OnLevelUp;

        public ChefLevelData(int level, int currentXP, int currentLevelXPGoal)
        {
            _level = level;
            _currentLevelXPGoal = currentLevelXPGoal;
            _currentXP = currentXP;
        }

        public void SetNewGoal(int newLevelXPGoal)
        {
            _currentLevelXPGoal = newLevelXPGoal;
        }

        public void GainXP(int _amount, int _iteration = 0)
        {
            if (_amount <= 0)
                return;

            _currentXP += _amount;

            ChefLevelStatEntry _levelInfo = DataLoader.LoadChefLevelStat(_level + 1);
            Debug.Log("CurrentXP: " + _currentXP);
            Debug.Log("XPRequired: " + _levelInfo._xpRequired);

            if (_currentXP >= _levelInfo._xpRequired && _level < 10)
            {
                Debug.Log("Level Up!");
                _level++;
                int newXP = _currentXP - _levelInfo._xpRequired;
                _currentXP = 0;
                ChefLevelStatEntry _newlevelInfo = DataLoader.LoadChefLevelStat(_level + 1);
                SetNewGoal(_newlevelInfo._xpRequired);
                OnLevelUp?.Invoke();
                GainXP(newXP, _iteration + 1);
            }
        }

        public int Level { get => _level; }
        public int CurrentXP { get => _currentXP; }
        public int CurrentLevelXPGoal { get => _currentLevelXPGoal; }
        public float LevelCompletionPercentage
        {
            get
            {
                return (float)CurrentXP / CurrentLevelXPGoal;
            }
        }
    }

    [Serializable]
    public class ChefData
    {
        [SerializeField]
        private string _chefID;
        [SerializeField] 
        private string _chefName;
        [SerializeField]
        private string _chefInfoAddress;
        [SerializeField]
        private RARITY _rarity;
        [SerializeField]
        private List<Skill> _skills;
        [SerializeField]
        private ChefLevelData _levelData;
        [SerializeField] 
        private ChefSettings _chefSettings;
        
        private const string ChefsAddressPath = "ChefSettings/";
        
        AsyncOperationHandle<ChefSettings> loadHandle;
        
        public ChefData(string chefID, string chefInfoAddress, ChefLevelData levelData, RARITY rarity, Skill[] _skillSet)
        {
            _chefID = chefID;
            _chefInfoAddress = chefInfoAddress;
            _skills = _skillSet.ToList();
            _levelData = levelData;
            _rarity = rarity;

            _levelData.OnLevelUp += LevelUpSkills;
        }

        public async Task LoadAssetRef()
        {
            loadHandle = Addressables.LoadAssetAsync<ChefSettings>($"{ChefsAddressPath}{_chefInfoAddress}.asset");
            await loadHandle.Task;
            _chefSettings = loadHandle.Result;
        }
        
        ~ChefData()
        {
            Addressables.Release(loadHandle);
            _levelData.OnLevelUp -= LevelUpSkills;
        }

        public void DebugChefStats()
        {
            Debug.Log("Level: " + _levelData.Level);
            Debug.Log("XP: " + _levelData.CurrentXP);
            Debug.Log("CurrentLevelXPGoal: " + _levelData.CurrentLevelXPGoal);
            foreach(Skill s in _skills)
            {
                Debug.Log(s.Type);
                Debug.Log(s.Name);
                Debug.Log(s.Level);
            }
        }

        public void LevelUpSkills()
        {
            foreach(Skill _skill in _skills)
            {
                _skill.LevelUp();
            }
        }

        public void GainXP(int _amount)
        {
            DebugHelper.PrintDebugMessage($"{_chefSettings.ChefName} earned {_amount} XP!");
            _levelData.GainXP(_amount);
            DataLoader.SaveChefData(this);
        }
        
        public void ClearChefStats()
        {
            _skills.Clear();
        }

        public string ChefID => _chefID;
        public string ChefName
        {
            get
            {
                if (_chefSettings == null)
                {
                    Debug.LogWarning($"Trying to access the Chef Setting in Chef Data object, but this one is null.");
                    return "";
                }
                return _chefSettings.ChefName;
            }
        }

        public string ChefInfoAddress => _chefInfoAddress;
        public List<Skill> Skills => _skills;
        public ChefLevelData LevelData => _levelData;
        public ChefSettings ChefSettings => _chefSettings;

        public RARITY Rarity { get => _rarity; }
    }
}
