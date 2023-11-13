using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.DataContainers.Player;
using Runtime.DataContainers.Stats;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.Utility;
using ScriptableObjects.DataContainers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.ScriptableObjects.DataContainers
{
    [CreateAssetMenu(fileName = "PlayerDataManager", menuName = "ScriptableObjects/DataContainer/PlayerData/PlayerDataManager", order = 0)]
    public class PlayerDataContainer : ScriptableObject
    {
        [SerializeField]
        private LeaderboardData _leaderboardData;

        [SerializeField] 
        private string _playerName;
        
        [SerializeField]
        private PlayerScore _playerScore;
        
        [SerializeField]
        private PlayerCurrencies _playerCurrencies;

        [SerializeField]
        private List<Recipe> _playerRecipes;
        
        [SerializeField] 
        private List<ChefData> _playerChefs;


        [SerializeField] 
        private KitchenData _selectedKitchenData;

        [SerializeField] 
        private TutorialData _tutorialData;

        [SerializeField] 
        private PlayerInventory _playerInventory = new PlayerInventory();
        
        [SerializeField] 
        private bool _logDebugMessage;
        
        private List<ProgressPathItem> _playerRewardsTextList;
        private List<RewardItem> _playerRewards;
        private Recipe[] _recipesRewards;
        private ChefData[] _chefRewardData;

        private AsyncOperationHandle<IList<Recipe>> _recipesLoadHandle;
        private AsyncOperationHandle<IList<ChefSettings>> _chefsLoadHandle;
        public UnityAction<ChefData> onChefAddedToCollection;
        public UnityAction<Recipe> onRecipeAddedToCollection;
        public Action OnStarRewardsLoaded;

        private const string RecipesAddressPath = "Assets/ScriptableObjects/DataContainers/Recipes/";
        private const string RewardRecipesAddressPath = "Assets/ScriptableObjects/DataContainers/Recipes/";

        public async Task LoadPlayerData()
        {
            DataLoader.CopyFromResourcesToPersistent();

            _tutorialData = DataLoader.LoadTutorialData();
            _playerName = DataLoader.LoadPlayerName();
            _playerCurrencies = new PlayerCurrencies();
            _playerCurrencies.LoadBalance();
            _playerScore = new PlayerScore();
            _playerScore.LoadScore();
            _playerInventory = new PlayerInventory();
            _playerInventory.LoadInventory();
            _playerRewards = new List<RewardItem>();
            _playerRewardsTextList = new List<ProgressPathItem>();
            
            var loadRewardRecipes = LoadRewards(_playerRewards);
            var getLeaderboardEntries = GetLeaderboardEntries();
            var loadRecipeDataTask = GetPlayerRecipesData();
            var loadChefDataTask = GetChefsData();
            var asyncLoadTasks = new List<Task>{getLeaderboardEntries, loadRecipeDataTask, loadChefDataTask, loadRewardRecipes};
            while (asyncLoadTasks.Count > 0)
            {
                Task loadTaskComplete = await Task.WhenAny(asyncLoadTasks);
                if (loadTaskComplete == getLeaderboardEntries)
                {
                    DebugHelper.PrintDebugMessage("Leaderboard data Loaded", _logDebugMessage);
                }
                if (loadTaskComplete == loadRecipeDataTask)
                {
                    DebugHelper.PrintDebugMessage("Recipes data Loaded", _logDebugMessage);
                }
                else if (loadTaskComplete == loadChefDataTask)
                {
                    DebugHelper.PrintDebugMessage("Chefs Data loaded", _logDebugMessage);
                }
                else if(loadTaskComplete == loadRewardRecipes)
                {
                    DebugHelper.PrintDebugMessage("Reward Data loaded", _logDebugMessage);
                }
                asyncLoadTasks.Remove(loadTaskComplete);
            }
            
            GetPlayerKitchenData();
        }

        private async Task GetLeaderboardEntries()
        {
            _leaderboardData = await DataLoader.LoadLeaderboardData();
            if(_playerName != "") _leaderboardData.AddPlayerEntry(_playerName);
        }
        
        private async Task GetPlayerRecipesData()
        {
            _playerRecipes.Clear();
            var recipesName= DataLoader.LoadPlayerRecipes();

            var paths= recipesName.Select(x => $"{RecipesAddressPath}{x}.asset").ToArray();
            var recipes = await LoadRecipesByKey(paths);
            _playerRecipes = recipes.ToList();
        }

        private async Task LoadRewards(List<RewardItem> _list)
        {
            _playerRewardsTextList = DataLoader.GetListOfProgressRewards();
            List<AsyncOperationHandle<RewardItem>> _handles = new List<AsyncOperationHandle<RewardItem>>();
            foreach(ProgressPathItem path in _playerRewardsTextList)
            {
                _handles.Add(Addressables.LoadAssetAsync<RewardItem>(path._addressablePath));
            }

            while (!_handles.All(p => p.IsDone))
            {
                await Task.Delay(50);
            }

            foreach(AsyncOperationHandle<RewardItem> item in _handles)
            {
                _list.Add(item.Result);
            }

            _chefRewardData = await DataLoader.LoadAllRewardChefs();
            var recipes = DataLoader.LoadRewardRecipes();
            var paths = recipes.Select(x => $"{RewardRecipesAddressPath}{x}.asset").ToArray();
            _recipesRewards = await LoadRecipesByKey(paths);

            OnStarRewardsLoaded?.Invoke();
        }

        private async Task<Recipe[]> LoadRecipesByKey(string[] _keys)
        {
            List<Recipe> _recipes = new List<Recipe>();
            _recipesLoadHandle =
                Addressables.LoadAssetsAsync<Recipe>(_keys,
                    _obj =>
                    {
                        _recipes.Add(_obj);
                        DebugHelper.PrintDebugMessage($"Loaded {_obj.name}", _logDebugMessage);
                    }, Addressables.MergeMode.Union);

            await _recipesLoadHandle.Task;
            return _recipes.ToArray();
        }
        
        private void GetPlayerKitchenData()
        {
            _selectedKitchenData = DataLoader.LoadKitchenData();
            _selectedKitchenData.SetPlayerDataContainer(this);
            _selectedKitchenData.UpdateKitchenData();
            _selectedKitchenData.RetrieveRecipes();
            _selectedKitchenData.RetrieveBrigadeChefs();
            _selectedKitchenData.GetIngredientTree();
        }

        private async Task GetChefsData()
        {
            _playerChefs.Clear();
            
            var result = await DataLoader.LoadAllPlayerChefs();
            _playerChefs = result.ToList();
        }
        
        public int GetKitchenRank()
        {
            int maxStars = 0;
            int _rank = 0;
            List<RewardItem> _ranks = _playerRewards.Where(r => r.RewardType == REWARD_TYPE.RANK).OrderBy(p => p.StarsRequired).ToList();
            for (int i = 0; i < _ranks.Count; ++i)
            {
                if ((SelectedKitchenData.kitchenStars >= _ranks[i].StarsRequired) && maxStars < _ranks[i].StarsRequired)
                {
                    maxStars = _ranks[i].StarsRequired;
                    _rank++;
                }
            }
            return _rank;
        }
        public void ComputePlayerScore(int _newHighScore)
        {
            if (_newHighScore <= PlayerScore.Score)
            {
                DebugHelper.PrintDebugMessage("Shift score is lower or equal to current score.", _logDebugMessage);
                return;
            }
            PlayerScore.UpdateScore(_newHighScore);
            LeaderboardData.UpdatePlayerEntryLeaderboard(_playerName, _newHighScore);
        }

        public void SetPlayerName(string _playerName)
        {
            this._playerName = _playerName;
            DataLoader.SavePlayerName(_playerName);
            LeaderboardData.AddPlayerEntry(_playerName);
            OnPlayerNameChanged?.Invoke();
        }

        public void AddRecipeToPlayerInventory(Recipe _recipe)
        {
            _playerRecipes.Add(_recipe);
            DataLoader.SavePlayerRecipes(_playerRecipes.Select(_x => _x.name).ToList());
            onRecipeAddedToCollection?.Invoke(_recipe);
        }

        public void TutorialComplete(string _tutorialName)
        {
            switch (_tutorialName)
            {
                case "Shift":
                    _tutorialData.ShiftTutorialComplete = true;
                    break;
                case "Upgrade":
                    _tutorialData.UpgradeTutorialComplete = true;
                    break;
                case "EditKitchen":
                    _tutorialData.EditKitchenTutorialComplete = true;
                    break;
                case "Brigade":
                    _tutorialData.BrigadeTutorialComplete = true;
                    break;
                case "Menu":
                    _tutorialData.MenuTutorialComplete = true;
                    break;
                case "Store":
                    _tutorialData.ShopTutorialComplete = true;
                    break;
                case "Leaderboard":
                    _tutorialData.LeaderboardTutorialComplete = true;
                    break;
                case "Rank":
                    _tutorialData.RankTutorialComplete = true;
                    break;
            }
            _tutorialData.SaveData();
        }

        public bool IsTutorialComplete(string _tutorialName)
        {
            return _tutorialName switch
            {
                "Shift" => _tutorialData.ShiftTutorialComplete,
                "Upgrade" => _tutorialData.UpgradeTutorialComplete,
                "EditKitchen" => _tutorialData.EditKitchenTutorialComplete,
                "Brigade" => _tutorialData.BrigadeTutorialComplete,
                "Menu" => _tutorialData.MenuTutorialComplete,
                "Store" => _tutorialData.ShopTutorialComplete,
                "Leaderboard" => _tutorialData.LeaderboardTutorialComplete,
                "Rank" => _tutorialData.RankTutorialComplete,
                _ => false
            };
        }

        public void AddChefToPlayerInventory(ChefData _chefData)
        {
            if (_playerChefs.Any(x => x.ChefID == _chefData.ChefID))
            {
                Debug.LogWarning($"Can't add chef with ID {_chefData.ChefID}. You already own a chef with that Chef ID.");
                return;
            }
            _playerChefs.Add(_chefData);
            DataLoader.SaveChefData(_chefData);
            onChefAddedToCollection?.Invoke(_chefData);
        }

        public TutorialData TutorialData => _tutorialData;
        public string PlayerName => _playerName;
        public LeaderboardData LeaderboardData => _leaderboardData;
        public List<Recipe> PlayerRecipes => _playerRecipes;
        public List<ChefData> PlayerChefs => _playerChefs;
        public KitchenData SelectedKitchenData => _selectedKitchenData;
        public PlayerScore PlayerScore => _playerScore;
        public PlayerCurrencies Currencies => _playerCurrencies;
        public PlayerInventory PlayerInventory => _playerInventory;
        public List<RewardItem> PlayerRewards { get => _playerRewards; }
        public List<ProgressPathItem> PlayerRewardsTextList { get => _playerRewardsTextList; }
        public List<RewardItem> KitchenRanks { get => _playerRewards.Where(r => r.RewardType == REWARD_TYPE.RANK).OrderBy(p => p.StarsRequired).ToList(); }
        public Recipe[] RecipesRewards { get => _recipesRewards; }
        public ChefData[] ChefRewardData { get => _chefRewardData; }

        public UnityAction OnPlayerNameChanged;
    }
}