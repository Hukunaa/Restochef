using Runtime.DataContainers;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using System.Threading.Tasks;
using Runtime.DataContainers.Stats;
using Runtime.DataContainers.Inventory;
using Runtime.DataContainers.Player;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.UI.MainMenuUI.StarProgressPath;
using System.Linq;

namespace Runtime.Utility
{
    public class DataVersionParser
    {
        public string version;

        public DataVersionParser(string _version)
        {
            version = _version;
        }
    }

    public class TutorialDataParser
    {
        public bool shiftTutorialComplete;
        public bool editKitchenTutorialComplete;
        public bool upgradeTutorialComplete;
        public bool brigadeTutorialComplete;
        public bool menuMenuTutorialComplete;
        public bool shopTutorialComplete;
        public bool leaderboardTutorialComplete;
        public bool rankTutorialComplete;

        public TutorialDataParser(TutorialData _data)
        {
            shiftTutorialComplete = _data.ShiftTutorialComplete;
            editKitchenTutorialComplete = _data.EditKitchenTutorialComplete;
            brigadeTutorialComplete = _data.BrigadeTutorialComplete;
            upgradeTutorialComplete = _data.UpgradeTutorialComplete;
            menuMenuTutorialComplete = _data.MenuTutorialComplete;
            shopTutorialComplete = _data.ShopTutorialComplete;
            leaderboardTutorialComplete = _data.LeaderboardTutorialComplete;
            rankTutorialComplete = _data.RankTutorialComplete;
        }
    }

    public class PlayerNameParser
    {
        public string playerName;

        public PlayerNameParser(string _playerName)
        {
            playerName = _playerName;
        }
    }
    
    [Serializable]
    public class CurrenciesDataParser
    {
        public int softCurrency;
        public int hardCurrency;

        public CurrenciesDataParser(int _softCurrency, int _hardCurrency)
        {
            softCurrency = _softCurrency;
            hardCurrency = _hardCurrency;
        }
    }

    [Serializable]
    public class ScoreDataParser
    {
        public int playerScore;

        public ScoreDataParser(int _score)
        {
            playerScore = _score;
        }
    }

    [Serializable]
    public class RecipesDataParser
    {
        public List<string> recipes;

        public RecipesDataParser(List<string> _recipes)
        {
            recipes = _recipes;
        }
    }

    [Serializable]
    public class ProgressPathItem
    {
        public string _addressablePath;
        public bool _isUsed;
    }

    [Serializable]
    public class ProgressPathParser
    {
        public List<ProgressPathItem> Rewards;
    }

    [Serializable]
    public class KitchenDataParser
    {
        public string kitchenLayoutName;
        public int kitchenSizeX;
        public int kitchenSizeY;
        public int minRecipeSlots;
        public int maxRecipeSlots;
        public int minChefSlots;
        public int maxChefSlots;
        public int stationSlots;
        public int maxStationLevel;
        public int kitchenStars;
        public int kitchenRank;
        public int bonusPoints;

        public List<string> brigade;
        public List<string> menu;
        
        public KitchenDataParser(KitchenData _kitchenData)
        {
            kitchenLayoutName = _kitchenData.kitchenLayoutName;
            kitchenSizeX = _kitchenData.kitchenSizeX;
            kitchenSizeY = _kitchenData.kitchenSizeY;
            minRecipeSlots = _kitchenData.minRecipeSlots;
            maxRecipeSlots = _kitchenData.maxRecipeSlots;
            minChefSlots = _kitchenData.minChefSlots;
            maxChefSlots = _kitchenData.maxChefSlots;
            brigade = _kitchenData.brigade;
            menu = _kitchenData.menu;
            stationSlots = _kitchenData.maxStationSlots;
            maxStationLevel = _kitchenData.maxStationLevel;
            kitchenStars = _kitchenData.kitchenStars;
            kitchenRank = _kitchenData.kitchenRank;
            bonusPoints = _kitchenData.bonusPoints;
        }
    }

    [Serializable]
    public class GridTileSetParser
    {
        public List<KitchenTile> Tiles;
    }
    
    [Serializable]
    public class ChefDataParser
    {
        public string ChefID;
        public string ChefInfoAddress;
        public RARITY ChefRarity;
        public ChefLevelData LevelData;
        public List<Skill> Skills;

        public ChefDataParser(ChefData _data)
        {
            ChefID = _data.ChefID;
            ChefInfoAddress = _data.ChefInfoAddress;
            ChefRarity = _data.Rarity;
            LevelData = _data.LevelData;
            Skills = _data.Skills;
        }
    }

    [Serializable]
    public class ChefRarityStatEntry
    {
        public RARITY _rarity;
        public int _maxLevel;
        public int _startingPoints;
    }

    [Serializable]
    public class ChefRarityStatTableParser
    {
        public List<ChefRarityStatEntry> ChefRarityTable;
    }

    [Serializable]
    public class ChefLevelStatEntry
    {
        public int _level;
        public int _xpRequired;
    }

    [Serializable]
    public class ChefLevelStatTableParser
    {
        public List<ChefLevelStatEntry> ChefLevelTable;
    }

    [Serializable]
    public class StationSlotStatEntry
    {
        public StationSlotStatEntry()
        {
            accident_bonus = 99;
            processing_time_bonus = 99;
            cost = 9999999;
        }

        public StationSlotStatEntry(int _accidentBonus = 99, int _processingTime = 99, int _cost = 9999999)
        {
            accident_bonus = _accidentBonus;
            processing_time_bonus = _processingTime;
            cost = _cost;
        }

        public int accident_bonus;
        public int processing_time_bonus;
        public int cost;
    }

    [Serializable]
    public class SlotStatTableParser
    {
        public List<StationSlotStatEntry> SlotsStatStable;
    }

    [Serializable]
    public class InventoryDataParser
    {
        public List<InventoryItem> Inventory;
    }
    
    [Serializable]
    public class LeaderboardParser
    {
        public List<LeaderboardEntry> LeaderboardEntries;

        public LeaderboardParser(List<LeaderboardEntry> _entries)
        {
            LeaderboardEntries = _entries;
        }
    }

    [Serializable]
    public class PriceTableParser
    {
        public List<PriceTableEntry> prices;
    }

    [Serializable]
    public class PriceTableEntry
    {
        public int rarity;
        public int price;
    }
    
    public static class DataLoader
    {
        private static string PersistentBasePath => Application.persistentDataPath + "/data";
        private static string PlayerDataPath => "PlayerData";
        private static string PlayerChefsDataPath => PlayerDataPath + "/PlayerChefs";
        private static string KitchenLayoutsDataPath => PlayerDataPath + "/KitchenLayouts";
        private static string LeaderboardDataPath => "LeaderboardData";
        private static string StoreDataPath => "StoreData";
        private static string StoreChefsDataPath => StoreDataPath + "/StoreChefs";
        private static string TablesDataPath => "Tables";
        private static string ProgressionDataPath => "ProgressionData";
        private static string ChefRewardDataPath => ProgressionDataPath + "/ChefReward";
        private static string PersistentPlayerDataPath => Path.Combine(PersistentBasePath, PlayerDataPath);
        private static string PersistentPlayerChefsDataPath => Path.Combine(PersistentBasePath, PlayerChefsDataPath);
        private static string PersistentPlayerKitchenLayoutsDataPath => Path.Combine(PersistentBasePath, KitchenLayoutsDataPath);
        private static string PersistentLeaderboardDataPath => Path.Combine(PersistentBasePath, LeaderboardDataPath);
        private static string PersistentStoreDataPath => Path.Combine(PersistentBasePath, StoreDataPath);
        private static string PersistentStoreChefsDataPath => Path.Combine(PersistentBasePath, StoreChefsDataPath);
        private static string PersistentTablesDataPath => Path.Combine(PersistentBasePath, TablesDataPath);
        private static string PersistentProgressionDataPath => Path.Combine(PersistentBasePath, ProgressionDataPath);
        private static string PersistentChefRewardDataPath => Path.Combine(PersistentBasePath, ChefRewardDataPath);
        private static string DataVersionFileName => "DataVersion.json";
        private static string TutorialDataFileName => "TutorialData.json";
        private static string PlayerNameFileName => "PlayerName.json";
        private static string PlayerInventoryFileName => "PlayerInventory.json";
        private static string PlayerCurrenciesFileName => "PlayerCurrencies.json";
        private static string PlayerScoreFileName => "PlayerScore.json";
        private static string PlayerKitchenDataFileName => "PlayerKitchenData.json";
        private static string PlayerRecipesDataFileName => "PlayerRecipes.json";
        private static string KitchenLayoutDataFileName => "KitchenLayout_1.json"; 
        private static string StoreRecipesDataFileName => "StoreRecipes.json";
        private static string StationStatsTableDataFileName => "StationStatsTable.json";
        private static string ChefPricesDataFileName => "ChefPricesTable.json";
        private static string ChefRarityStatTableFileName => "ChefsRarityStatTable.json";
        private static string ChefsLevelStatTableFileName => "ChefsLevelTable.json";
        private static string RecipePricesDataFileName => "RecipePricesTable.json";
        private static string LeaderboardDataFileName => "LeaderboardData.json";
        private static string StarProgressPathFileName => "StarProgressPath.json";
        private static string RewardRecipesPathFileName => "RecipeReward.json";
        
        //TEMPORARY UNTIL WE GO SERVER SIDE
        public static void CopyFromResourcesToPersistent()
        {
            bool replaceData = ShouldReplaceData();

            if (replaceData)
            {
                ResetDirectories();
                SaveDataVersion(LoadDataVersion(true));
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, TutorialDataFileName)))
            {
                ResetTutorialData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerNameFileName)))
            {
                ResetPlayerNameData();
            }

            if (replaceData)
            {
                ResetPlayerChefsData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerKitchenLayoutsDataPath, KitchenLayoutDataFileName)))
            {
                ResetKitchenLayoutData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerCurrenciesFileName)))
            {
                ResetPlayerCurrenciesData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerInventoryFileName)))
            {
                ResetPlayerInventoryData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerRecipesDataFileName)))
            {
                ResetPlayerRecipesData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerScoreFileName)))
            {
                ResetPlayerScoreData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentPlayerDataPath, PlayerKitchenDataFileName)))
            {
                ResetPlayerKitchenData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentLeaderboardDataPath, LeaderboardDataFileName)))
            {
                ResetLeaderboardData();
            }
            
            if (replaceData)
            {
                ResetStoreChefsData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentStoreDataPath, StoreRecipesDataFileName)))
            {
                ResetStoreRecipesData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentTablesDataPath, StationStatsTableDataFileName)))
            {
                ResetStationStatsTableData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentTablesDataPath, ChefPricesDataFileName)))
            {
                ResetChefPricesTableData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentTablesDataPath, ChefsLevelStatTableFileName)))
            {
                ResetChefsLevelTableData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentTablesDataPath, ChefRarityStatTableFileName)))
            {
                ResetChefRarityStatTableData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentTablesDataPath, RecipePricesDataFileName)))
            {
                ResetRecipePricesTableData();
            }
            
            if (replaceData || !File.Exists(Path.Combine(PersistentProgressionDataPath, StarProgressPathFileName)))
            {
                ResetStarProgressData();
            }
            
            if (replaceData)
            {
                ResetChefRewardData();
            }

            if (replaceData || !File.Exists(Path.Combine(PersistentProgressionDataPath, RewardRecipesPathFileName)))
            {
                ResetRecipeRewardData();
            }
        }
        
        public static void ResetAllData()
        {
            ResetDirectories();
            SaveDataVersion(LoadDataVersion(true));

            ResetTutorialData();
            ResetPlayerNameData();
            ResetLeaderboardData();
            ResetPlayerKitchenData();
            ResetPlayerChefsData();
            ResetPlayerScoreData();
            ResetPlayerRecipesData();
            ResetPlayerInventoryData();
            ResetPlayerCurrenciesData();
            ResetKitchenLayoutData();
            ResetStoreChefsData();
            ResetStoreRecipesData();
            ResetRecipePricesTableData();
            ResetChefRarityStatTableData();
            ResetChefsLevelTableData();
            ResetChefPricesTableData();
            ResetStationStatsTableData();
            ResetStarProgressData();
            ResetChefRewardData();
            ResetRecipeRewardData();
        }

        private static void ResetDirectories()
        {
            if (Directory.Exists(PersistentBasePath))
            {
                Directory.Delete(PersistentBasePath, true);
            }

            Directory.CreateDirectory(PersistentBasePath);
            Directory.CreateDirectory(PersistentPlayerDataPath);
            Directory.CreateDirectory(PersistentPlayerChefsDataPath);
            Directory.CreateDirectory(PersistentPlayerKitchenLayoutsDataPath);
            Directory.CreateDirectory(PersistentLeaderboardDataPath);
            Directory.CreateDirectory(PersistentStoreDataPath);
            Directory.CreateDirectory(PersistentStoreChefsDataPath);
            Directory.CreateDirectory(PersistentTablesDataPath);
            Directory.CreateDirectory(PersistentProgressionDataPath);
            Directory.CreateDirectory(PersistentChefRewardDataPath);
        }

        public static void ResetStarProgressData()
        {
            string resourcePath = $"{ProgressionDataPath}/{Path.GetFileNameWithoutExtension(StarProgressPathFileName)}";
            TextAsset starProgressData = Resources.Load<TextAsset>(resourcePath);
            if (StarProgressPathFileName != null)
                File.WriteAllText(Path.Combine(PersistentProgressionDataPath, StarProgressPathFileName), starProgressData.text);
        }
        
        public static void ResetRecipePricesTableData()
        {
            string resourcePath = $"{TablesDataPath}/{Path.GetFileNameWithoutExtension(RecipePricesDataFileName)}";
            TextAsset recipePrices = Resources.Load<TextAsset>(resourcePath);
            File.WriteAllText(Path.Combine(PersistentTablesDataPath, RecipePricesDataFileName), recipePrices.text);
            Debug.Log("Recipe prices table data restored to default.");
        }

        public static void ResetChefRarityStatTableData()
        {
            string resourcePath = $"{TablesDataPath}/{Path.GetFileNameWithoutExtension(ChefRarityStatTableFileName)}";
            TextAsset rarityStatTable = Resources.Load<TextAsset>(resourcePath);
            File.WriteAllText(Path.Combine(PersistentTablesDataPath, ChefRarityStatTableFileName), rarityStatTable.text);
            Debug.Log("Chef rarity stat table data restored to default.");
        }

        public static void ResetChefsLevelTableData()
        {
            string resourcePath = $"{TablesDataPath}/{Path.GetFileNameWithoutExtension(ChefsLevelStatTableFileName)}";
            TextAsset levelStatTable = Resources.Load<TextAsset>(resourcePath);
            File.WriteAllText(Path.Combine(PersistentTablesDataPath, ChefsLevelStatTableFileName), levelStatTable.text);
            Debug.Log("Chefs level table data restored to default.");
        }

        public static void ResetChefPricesTableData()
        {
            string resourcePath = $"{TablesDataPath}/{Path.GetFileNameWithoutExtension(ChefPricesDataFileName)}";
            TextAsset chefPrices = Resources.Load<TextAsset>(resourcePath);
            File.WriteAllText(Path.Combine(PersistentTablesDataPath, ChefPricesDataFileName), chefPrices.text);
            Debug.Log("Chef prices table data restored to default.");
        }

        public static void ResetStationStatsTableData()
        {
            string resourcePath = $"{TablesDataPath}/{Path.GetFileNameWithoutExtension(StationStatsTableDataFileName)}";
            TextAsset stationTable = Resources.Load<TextAsset>(resourcePath);
            File.WriteAllText(Path.Combine(PersistentTablesDataPath, StationStatsTableDataFileName), stationTable.text);
            Debug.Log("Station stats table data restored to default.");
        }

        public static void ResetPlayerChefsData()
        {
            if (Directory.Exists(PersistentPlayerChefsDataPath))
            {
                Directory.Delete(PersistentPlayerChefsDataPath, true);
            }

            Directory.CreateDirectory(PersistentPlayerChefsDataPath);
            
            TextAsset[] playerStartingChefs = Resources.LoadAll<TextAsset>(PlayerChefsDataPath);
            foreach (var chef in playerStartingChefs)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerChefsDataPath, $"{chef.name}.json"), chef.text);
                Debug.Log("Player Chefs data restored to default.");
            }
        }

        public static void ResetStoreChefsData()
        {
            if (Directory.Exists(PersistentStoreChefsDataPath))
            {
                Directory.Delete(PersistentStoreChefsDataPath, true);
            }

            Directory.CreateDirectory(PersistentStoreChefsDataPath);
            
            TextAsset[] storeChefs = Resources.LoadAll<TextAsset>(StoreChefsDataPath);
            foreach (var chef in storeChefs)
            {
                File.WriteAllText(Path.Combine(PersistentStoreChefsDataPath, $"{chef.name}.json"), chef.text);
                Debug.Log("Store Chefs data restored to default.");
            }
        }

      

        public static void ResetStoreRecipesData()
        {
            string resourcePath = $"{StoreDataPath}/{Path.GetFileNameWithoutExtension(StoreRecipesDataFileName)}";
            TextAsset storeRecipes = Resources.Load<TextAsset>(resourcePath);
            if (StoreRecipesDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentStoreDataPath, StoreRecipesDataFileName), storeRecipes.text);
                Debug.Log("Store recipes data restored to default.");
            }
        }

        public static void ResetLeaderboardData()
        {
            string resourcePath = $"{LeaderboardDataPath}/{Path.GetFileNameWithoutExtension(LeaderboardDataFileName)}";
            TextAsset leaderboardData = Resources.Load<TextAsset>(resourcePath);
            if (LeaderboardDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentLeaderboardDataPath, LeaderboardDataFileName), leaderboardData.text);
                Debug.Log("Leaderboard data restored to default.");
            }
        }

        public static void ResetPlayerKitchenData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerKitchenDataFileName)}";
            TextAsset kitchenData = Resources.Load<TextAsset>(resourcePath);
            if (PlayerKitchenDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerKitchenDataFileName), kitchenData.text);
                Debug.Log("Player Kitchen data restored to default.");
            }
        }

        public static void ResetPlayerScoreData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerScoreFileName)}";
            TextAsset score = Resources.Load<TextAsset>(resourcePath);
            if (PlayerScoreFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerScoreFileName), score.text);
                Debug.Log("Player score data restored to default.");
            }
        }

        public static void ResetPlayerRecipesData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerRecipesDataFileName)}";
            TextAsset playerRecipes = Resources.Load<TextAsset>(resourcePath);
            if (PlayerRecipesDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerRecipesDataFileName), playerRecipes.text);
                Debug.Log("Player Recipes data restored to default.");
            }
        }

        public static void ResetPlayerInventoryData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerInventoryFileName)}";
            TextAsset inventory = Resources.Load<TextAsset>(resourcePath);
            if (PlayerInventoryFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerInventoryFileName), inventory.text);
                Debug.Log("Player Inventory data restored to default.");
            }
        }

        public static void ResetPlayerCurrenciesData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerCurrenciesFileName)}";
            TextAsset currencies = Resources.Load<TextAsset>(resourcePath);
            if (PlayerCurrenciesFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerCurrenciesFileName), currencies.text);
                Debug.Log("Player Currencies data restored to default.");
            }
        }

        public static void ResetKitchenLayoutData()
        {
            string resourcePath = $"{KitchenLayoutsDataPath}/{Path.GetFileNameWithoutExtension(KitchenLayoutDataFileName)}";
            TextAsset defaultKitchenLayout = Resources.Load<TextAsset>(resourcePath);
            if (KitchenLayoutDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerKitchenLayoutsDataPath, KitchenLayoutDataFileName),
                    defaultKitchenLayout.text);
                Debug.Log("Kitchen Layout data restored to default.");
            }
        }

        public static void ResetPlayerNameData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(PlayerNameFileName)}";
            TextAsset playerName = Resources.Load<TextAsset>(resourcePath);
            if (PlayerNameFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, PlayerNameFileName), playerName.text);
                Debug.Log("Player name data restored to default.");
            }
        }

        public static void ResetTutorialData()
        {
            string resourcePath = $"{PlayerDataPath}/{Path.GetFileNameWithoutExtension(TutorialDataFileName)}";
            TextAsset tutorialData = Resources.Load<TextAsset>(resourcePath);
            if (TutorialDataFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentPlayerDataPath, TutorialDataFileName), tutorialData.text);
                Debug.Log("Tutorial data restored to default.");
            }
        }

        public static void SetAllTutorialDataComplete()
        {
            var tutorialData = new TutorialData(true, true, true, true, true, true, true, true);
            SaveTutorialData(tutorialData);
        }

        public static void ResetChefRewardData()
        {
            if (Directory.Exists(PersistentChefRewardDataPath))
            {
                Directory.Delete(PersistentChefRewardDataPath, true);
            }

            Directory.CreateDirectory(PersistentChefRewardDataPath);
            
            TextAsset[] chefReward = Resources.LoadAll<TextAsset>(ChefRewardDataPath);
            foreach (var chef in chefReward)
            {
                File.WriteAllText(Path.Combine(PersistentChefRewardDataPath, $"{chef.name}.json"), chef.text);
                Debug.Log("Chefs reward data restored to default.");
            }
        }

        public static void ResetRecipeRewardData()
        {
            string resourcePath = $"{ProgressionDataPath}/{Path.GetFileNameWithoutExtension(RewardRecipesPathFileName)}";
            TextAsset recipeRewards = Resources.Load<TextAsset>(resourcePath);
            if (RewardRecipesPathFileName != null)
            {
                File.WriteAllText(Path.Combine(PersistentProgressionDataPath, RewardRecipesPathFileName), recipeRewards.text);
                Debug.Log("Recipe Reward data restored to default.");
            }
        }

        private static bool ShouldReplaceData()
        {
            var replaceData = false;
            if (!Directory.Exists(PersistentBasePath) || !File.Exists(Path.Combine(PersistentBasePath, DataVersionFileName)))
            {
                replaceData = true;
            }

            else
            {
                var currentVersion = LoadDataVersion();
                string lastVersion = LoadDataVersion(true);
                if (currentVersion != lastVersion)
                {
                    replaceData = true;
                }
            }

            return replaceData;
        }

        public static string LoadDataVersion(bool _loadFromResources = false)
        {
            if (_loadFromResources)
            {
                TextAsset dataVersion = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(DataVersionFileName));
                return JsonUtility.FromJson<DataVersionParser>(dataVersion.text).version;
            }
            
            string filePath = Path.Combine(PersistentBasePath, DataVersionFileName);
            if (!File.Exists(filePath))
                return null;

            string data = File.ReadAllText(filePath);
            DataVersionParser versionData = JsonUtility.FromJson<DataVersionParser>(data);

            return versionData.version;
        }

        private static void SaveDataVersion(string _version)
        {
            string filePath = Path.Combine(PersistentBasePath, DataVersionFileName);
            
            if (!Directory.Exists(PersistentBasePath))
                Directory.CreateDirectory(PersistentBasePath);
            
            DataVersionParser data = new DataVersionParser(_version);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static TutorialData LoadTutorialData()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, TutorialDataFileName);

            string jsonData = File.ReadAllText(filePath);
            TutorialDataParser parseData = JsonUtility.FromJson<TutorialDataParser>(jsonData);
            TutorialData tutorialData = new TutorialData(parseData);
            return tutorialData;
        }

        public static void SaveTutorialData(TutorialData _data)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, TutorialDataFileName);

            TutorialDataParser data = new TutorialDataParser(_data);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static string LoadPlayerName()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerNameFileName);

            string jsonData = File.ReadAllText(filePath);
            PlayerNameParser parseData = JsonUtility.FromJson<PlayerNameParser>(jsonData);
            return parseData.playerName;
        }
        
        public static void SavePlayerName(string _playerName)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerNameFileName);

            PlayerNameParser data = new PlayerNameParser(_playerName);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }
        
        public static List<InventoryItem> LoadInventory(List<InventoryItem> _itemsInit = null)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerInventoryFileName);

            string jsonData = File.ReadAllText(filePath);
            InventoryDataParser data = JsonUtility.FromJson<InventoryDataParser>(jsonData);

            return data.Inventory == null ? null : data.Inventory;
        }

        public static void SaveInventory(List<InventoryItem> _items)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerInventoryFileName);
            
            InventoryDataParser data = new InventoryDataParser();
            data.Inventory = _items;

            string dataJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, dataJson);
        }

        public static List<ProgressPathItem> GetListOfProgressRewards()
        {
            string filePath = Path.Combine(PersistentProgressionDataPath, StarProgressPathFileName);

            string jsonData = File.ReadAllText(filePath);
            ProgressPathParser data = JsonUtility.FromJson<ProgressPathParser>(jsonData);

            return data.Rewards;
        }
        
        public static void SaveListOfProgressRewards(List<ProgressPathItem> _list)
        {
            string filePath = Path.Combine(PersistentProgressionDataPath, StarProgressPathFileName);

            ProgressPathParser data = new ProgressPathParser();
            data.Rewards = _list;

            string dataJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, dataJson);
        }

        public static  async Task<ChefData[]> LoadAllStoreChefs()
        {
            if (!Directory.Exists(PersistentStoreChefsDataPath))
            {
                Debug.LogWarning("Can't load Store chefs because the Store Chef data path doesn't exist");
                return null;
            }

            string[] filePaths = Directory.GetFiles(PersistentStoreChefsDataPath);
            List<ChefData> chefsData = new List<ChefData>();

            List<Task<ChefData>> tasks = new List<Task<ChefData>>();
            
            foreach (string f in filePaths)
            {
                if (Path.GetExtension(f) != ".json")
                    continue;
                
                tasks.Add(LoadChefData(PersistentStoreChefsDataPath, Path.GetFileNameWithoutExtension(f)));
            }

            while (tasks.Count > 0)
            {
                Task<ChefData> loadTaskComplete = await Task.WhenAny(tasks);
                tasks.Remove(loadTaskComplete);
                ChefData data = loadTaskComplete.Result;
                if (data != null)
                {
                    chefsData.Add(data);
                }
            }

            return chefsData.ToArray();
        }
        
        public static void RemoveStoreChefItem(ChefData _chefData)
        {
            File.Delete(Path.Combine(PersistentStoreChefsDataPath, _chefData.ChefInfoAddress + ".json"));
        }

        public static async Task<ChefData[]> LoadAllPlayerChefs()
        {
            if (!Directory.Exists(PersistentPlayerChefsDataPath))
            {
                Debug.LogWarning("Can't load Player chefs because the Player Chef data path doesn't exist");
                return null;
            }

            string[] filePaths = Directory.GetFiles(PersistentPlayerChefsDataPath);
            List<ChefData> chefsData = new List<ChefData>();
            
            List<Task<ChefData>> tasks = new List<Task<ChefData>>();
            
            foreach (string f in filePaths)
            {
                if (Path.GetExtension(f) != ".json")
                    continue;
                
                tasks.Add(LoadChefData(PersistentPlayerChefsDataPath, Path.GetFileNameWithoutExtension(f)));
            }

            while (tasks.Count > 0)
            {
                Task<ChefData> loadTaskComplete = await Task.WhenAny(tasks);
                tasks.Remove(loadTaskComplete);
                ChefData data = loadTaskComplete.Result;
                if (data != null)
                {
                    chefsData.Add(data);
                }
            }

            return chefsData.ToArray();
        }

        public static async Task<ChefData[]>  LoadAllRewardChefs()
        {
            if (!Directory.Exists(PersistentChefRewardDataPath))
            {
                Debug.LogWarning("Can't load Reward chefs because the Reward Chef data path doesn't exist");
                return null;
            }

            string[] filePaths = Directory.GetFiles(PersistentChefRewardDataPath);
            List<ChefData> chefsData = new List<ChefData>();

            List<Task<ChefData>> tasks = new List<Task<ChefData>>();
            
            foreach (string f in filePaths)
            {
                if (Path.GetExtension(f) != ".json")
                    continue;
                
                tasks.Add(LoadChefData(PersistentChefRewardDataPath, Path.GetFileNameWithoutExtension(f)));
            }

            while (tasks.Count > 0)
            {
                Task<ChefData> loadTaskComplete = await Task.WhenAny(tasks);
                tasks.Remove(loadTaskComplete);
                ChefData data = loadTaskComplete.Result;
                if (data != null)
                {
                    chefsData.Add(data);
                }
            }

            return chefsData.ToArray();
        }

        private static async Task<ChefData> LoadChefData(string _path, string _fileName)
        {
            string filePath = Path.Combine(_path, _fileName + ".json");

            if (!Directory.Exists(_path) && !File.Exists(filePath))
            {
                Debug.Log("File or Directory doesn't exist, creating asset...");
            }

            string jsonData = File.ReadAllText(filePath);
            ChefDataParser data = JsonUtility.FromJson<ChefDataParser>(jsonData);

            if (data.ChefInfoAddress == "null")
            {
                Debug.LogError($"Chef data parser does not have a ChefInfoAddress assigned. Cannot load chef settings.");
                return null;
            }
            
            var chefData = new ChefData(data.ChefID, data.ChefInfoAddress, data.LevelData, data.ChefRarity, data.Skills.ToArray());
            await chefData.LoadAssetRef();
            return chefData;
        }
        
        public static void SaveChefData(ChefData _data)
        {
            string filePath = Path.Combine(PersistentPlayerChefsDataPath, _data.ChefInfoAddress + ".json");

            if (!Directory.Exists(PersistentPlayerChefsDataPath))
                Directory.CreateDirectory(PersistentPlayerChefsDataPath);

            ChefDataParser data = new ChefDataParser(_data);

            string dataJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, dataJson);
        }

        private static void SaveStationStatTable(StationSlotStatEntry[] _table)
        {
            if (!Directory.Exists(PersistentTablesDataPath))
                Directory.CreateDirectory(PersistentTablesDataPath);

            string filePath = Path.Combine(PersistentTablesDataPath, StationStatsTableDataFileName);
            SlotStatTableParser data = new SlotStatTableParser();
            data.SlotsStatStable = _table.ToList();
            string text = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, text);
        }

        public static StationSlotStatEntry LoadStationStat(int _level)
        {
            var filePath = Path.Combine(PersistentTablesDataPath, StationStatsTableDataFileName);

            if (!File.Exists(filePath))
                SaveStationStatTable(new StationSlotStatEntry[] { new StationSlotStatEntry() });

            var jsonData = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<SlotStatTableParser>(jsonData);
            var levelArray = Mathf.Clamp(_level, 1, data.SlotsStatStable.Count);
            return data.SlotsStatStable[levelArray - 1];
        }

        public static ChefRarityStatEntry LoadChefRarityStat(RARITY _rarity)
        {
            var filePath = Path.Combine(PersistentTablesDataPath, ChefRarityStatTableFileName);

            var jsonData = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<ChefRarityStatTableParser>(jsonData);
            var levelArray = Mathf.Clamp((int)_rarity, 1, data.ChefRarityTable.Count);
            return data.ChefRarityTable[levelArray - 1];
        }
        public static ChefLevelStatEntry LoadChefLevelStat(int _level)
        {
            var filePath = Path.Combine(PersistentTablesDataPath, ChefsLevelStatTableFileName);

            var jsonData = File.ReadAllText(filePath);
            var data = JsonUtility.FromJson<ChefLevelStatTableParser>(jsonData);
            var levelArray = Mathf.Clamp(_level, 1, data.ChefLevelTable.Count);
            return data.ChefLevelTable[levelArray - 1];
        }

        private static KitchenTile[,] LoadGrid(string _fileName, int _sizeX, int _sizeY)
        {
            string filePath = Path.Combine(PersistentPlayerKitchenLayoutsDataPath, _fileName + ".json");
            string data = File.ReadAllText(filePath);

            GridTileSetParser tiles1D = JsonUtility.FromJson<GridTileSetParser>(data);

            if (tiles1D == null || tiles1D.Tiles == null)
                return null;

            KitchenTile[,] tilesWrapped = new KitchenTile[_sizeX, _sizeY];
            for (int x = 0; x < _sizeX; ++x)
            {
                for (int y = 0; y < _sizeY; ++y)
                {
                    tilesWrapped[x, y] = tiles1D.Tiles[x * _sizeY + y];
                }
            }
            
            return tilesWrapped;
        }

        public static void SaveGrid(string _fileName, KitchenTile[,] _tiles, int _sizeX, int _sizeY)
        {
            if (!Directory.Exists(PersistentPlayerKitchenLayoutsDataPath))
                Directory.CreateDirectory(PersistentPlayerKitchenLayoutsDataPath);

            string path = Path.Combine(PersistentPlayerKitchenLayoutsDataPath, _fileName + ".json");

            KitchenTile[] jsonData = new KitchenTile[_sizeX * _sizeY];
            GridTileSetParser tileList = new GridTileSetParser();

            for (int x = 0; x < _sizeX; ++x)
            {
                for (int y = 0; y < _sizeY; ++y)
                {
                    jsonData[x * _sizeY + y] = _tiles[x, y];
                }
            }

            tileList.Tiles = jsonData.ToList();
            string data = JsonUtility.ToJson(tileList, true);
            File.WriteAllText(path, data);
            Debug.Log("Saved Tiles");
        }

        public static int[] LoadCurrencies()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerCurrenciesFileName);
            if (!File.Exists(filePath))
                SaveCurrencies(0, 0);

            string data = File.ReadAllText(filePath);
            CurrenciesDataParser currenciesData = JsonUtility.FromJson<CurrenciesDataParser>(data);
            int[] currencies = new int[2] { 0, 0 };

            currencies[0] = currenciesData.softCurrency;
            currencies[1] = currenciesData.hardCurrency;

            return currencies;
        }

        public static void SaveCurrencies(int _softCurrency, int _hardCurrency)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerCurrenciesFileName);

            if (!Directory.Exists(PersistentPlayerDataPath))
                Directory.CreateDirectory(PersistentPlayerDataPath);

            CurrenciesDataParser data = new CurrenciesDataParser(_softCurrency, _hardCurrency);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static int LoadScore()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerScoreFileName);
            if (!File.Exists(filePath))
                SaveScore(0);

            string data = File.ReadAllText(filePath);
            ScoreDataParser scoreData = JsonUtility.FromJson<ScoreDataParser>(data);

            return scoreData.playerScore;
        }

        public static void SaveScore(int _score)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerScoreFileName);

            if (!Directory.Exists(PersistentPlayerDataPath))
                Directory.CreateDirectory(PersistentPlayerDataPath);

            ScoreDataParser data = new ScoreDataParser(_score);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static List<string> LoadPlayerRecipes()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerRecipesDataFileName);

            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();

            string data = File.ReadAllText(filePath);
            RecipesDataParser recipesData = JsonUtility.FromJson<RecipesDataParser>(data);
            return recipesData.recipes;
        }
        
        public static List<string> LoadRewardRecipes()
        {
            string filePath = Path.Combine(PersistentProgressionDataPath, RewardRecipesPathFileName);

            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();

            string data = File.ReadAllText(filePath);
            RecipesDataParser recipesData = JsonUtility.FromJson<RecipesDataParser>(data);
            return recipesData.recipes;
        }

        public static List<string> LoadStoreRecipe()
        {
            string filePath = Path.Combine(PersistentStoreDataPath, StoreRecipesDataFileName);

            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();

            string data = File.ReadAllText(filePath);
            RecipesDataParser recipesData = JsonUtility.FromJson<RecipesDataParser>(data);
            return recipesData.recipes;
        }

        public static void SavePlayerRecipes(List<string> _recipes)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerRecipesDataFileName);
            
            RecipesDataParser data = new RecipesDataParser(_recipes);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static void SaveStoreRecipes(List<string> _recipes)
        {
            string filePath = Path.Combine(PersistentStoreDataPath, StoreRecipesDataFileName);
            
            RecipesDataParser data = new RecipesDataParser(_recipes);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }
        
        public static KitchenData LoadKitchenData()
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerKitchenDataFileName);

            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();

            string data = File.ReadAllText(filePath);
            KitchenDataParser parsedData = JsonUtility.FromJson<KitchenDataParser>(data);
            KitchenData kitchenData = new KitchenData
            {
                kitchenLayoutName = parsedData.kitchenLayoutName,
                kitchenSizeX = parsedData.kitchenSizeX,
                kitchenSizeY = parsedData.kitchenSizeY,
                minChefSlots = parsedData.minChefSlots,
                maxChefSlots = parsedData.maxChefSlots,
                minRecipeSlots = parsedData.minRecipeSlots,
                maxRecipeSlots = parsedData.maxRecipeSlots,
                maxStationSlots = parsedData.stationSlots,
                maxStationLevel = parsedData.maxStationLevel,
                kitchenStars = parsedData.kitchenStars,
                kitchenRank = parsedData.kitchenRank,
                bonusPoints = parsedData.bonusPoints,
                brigade = parsedData.brigade,
                menu = parsedData.menu
            };

            kitchenData.tiles = LoadGrid(kitchenData.kitchenLayoutName, kitchenData.kitchenSizeX,
                kitchenData.kitchenSizeY);

            for (int x = 0; x < kitchenData.kitchenSizeX; ++x)
                for (int y = 0; y < kitchenData.kitchenSizeY; ++y)
                    if (kitchenData.tiles[x, y].UpgradableData != null)
                        kitchenData.tiles[x, y].UpgradableData.InitUpgradable();

            return kitchenData;
        }

        public static void SaveKitchenData(KitchenData _kitchenData)
        {
            string filePath = Path.Combine(PersistentPlayerDataPath, PlayerKitchenDataFileName);
            
            if (!Directory.Exists(PersistentPlayerDataPath))
                Directory.CreateDirectory(PersistentPlayerDataPath);
            
            KitchenDataParser data = new KitchenDataParser(_kitchenData);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }
        
        public static async Task<LeaderboardData> LoadLeaderboardData()
        {
            string filePath = Path.Combine(PersistentLeaderboardDataPath, LeaderboardDataFileName);

            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();

            string data = await File.ReadAllTextAsync(filePath);
            LeaderboardParser parsedData = JsonUtility.FromJson<LeaderboardParser>(data);

            LeaderboardData leaderboardData = new LeaderboardData
            {
                LeaderboardEntries = parsedData.LeaderboardEntries
            };
            leaderboardData.SortLeaderboard();
            return leaderboardData;
        }

        public static void SaveLeaderboard(List<LeaderboardEntry> _entries)
        {
            string filePath = Path.Combine(PersistentLeaderboardDataPath, LeaderboardDataFileName);
            
            if (!Directory.Exists(PersistentLeaderboardDataPath))
                Directory.CreateDirectory(PersistentLeaderboardDataPath);
            
            LeaderboardParser data = new LeaderboardParser(_entries);
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(filePath, jsonData);
        }

        public static async Task<Dictionary<RARITY, int>> LoadChefPricesTable()
        {
            string filePath = Path.Combine(PersistentTablesDataPath, ChefPricesDataFileName);
            
            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();
            
            string data = await File.ReadAllTextAsync(filePath);
            PriceTableParser parsedData = JsonUtility.FromJson<PriceTableParser>(data);

            Dictionary<RARITY, int> priceTable = new Dictionary<RARITY, int>();
            foreach (var priceEntry in parsedData.prices)
            {
                priceTable[(RARITY)priceEntry.rarity] = priceEntry.price;
            }

            return priceTable;
        }
        
        public static async Task<Dictionary<RARITY, int>> LoadRecipesPricesTable()
        {
            string filePath = Path.Combine(PersistentTablesDataPath, RecipePricesDataFileName);
            
            if (!File.Exists(filePath))
                CopyFromResourcesToPersistent();
            
            string data = await File.ReadAllTextAsync(filePath);
            PriceTableParser parsedData = JsonUtility.FromJson<PriceTableParser>(data);
            
            Dictionary<RARITY, int> priceTable = new Dictionary<RARITY, int>();
            foreach (var priceEntry in parsedData.prices)
            {
                priceTable[(RARITY)priceEntry.rarity] = priceEntry.price;
            }

            return priceTable;
        }
    }
}
