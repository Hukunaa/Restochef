using Runtime.Utility;

namespace Runtime.ScriptableObjects.DataContainers
{
    public class TutorialData
    {
        private bool _shiftTutorialComplete;
        private bool _editKitchenTutorialComplete;
        private bool _upgradeTutorialComplete;
        private bool _brigadeTutorialComplete;
        private bool _menuTutorialComplete;
        private bool _shopTutorialComplete;
        private bool _leaderboardTutorialComplete;
        private bool _rankTutorialComplete;

        public TutorialData(TutorialDataParser _parsedData)
        {
            _shiftTutorialComplete = _parsedData.shiftTutorialComplete;
            _editKitchenTutorialComplete = _parsedData.editKitchenTutorialComplete;
            _upgradeTutorialComplete = _parsedData.upgradeTutorialComplete;
            _brigadeTutorialComplete = _parsedData.brigadeTutorialComplete;
            _menuTutorialComplete = _parsedData.menuMenuTutorialComplete;
            _shopTutorialComplete = _parsedData.shopTutorialComplete;
            _leaderboardTutorialComplete = _parsedData.leaderboardTutorialComplete;
            _rankTutorialComplete = _parsedData.rankTutorialComplete;
        }

        public TutorialData(
            bool shiftTutorialComplete, 
            bool editKitchenTutorialComplete, 
            bool upgradeTutorialComplete,
            bool brigadeTutorialComplete, 
            bool menuTutorialComplete, 
            bool shopTutorialComplete,
            bool leaderboardTutorialComplete,
            bool rankTutorialComplete
            )
        {
            _shiftTutorialComplete = shiftTutorialComplete;
            _editKitchenTutorialComplete = editKitchenTutorialComplete;
            _upgradeTutorialComplete = upgradeTutorialComplete;
            _brigadeTutorialComplete = brigadeTutorialComplete;
            _menuTutorialComplete = menuTutorialComplete;
            _shopTutorialComplete = shopTutorialComplete;
            _leaderboardTutorialComplete = leaderboardTutorialComplete;
            _rankTutorialComplete = rankTutorialComplete;
        }
        
        

        public bool ShiftTutorialComplete
        {
            get => _shiftTutorialComplete;
            set
            {
                _shiftTutorialComplete = value;
            } 
        }
        
        public bool UpgradeTutorialComplete
        {
            get => _upgradeTutorialComplete;
            set
            {
                _upgradeTutorialComplete = value;
            }
        }

        public bool BrigadeTutorialComplete
        {
            get => _brigadeTutorialComplete;
            set
            {
                _brigadeTutorialComplete = value;
            }
        }

        public bool MenuTutorialComplete
        {
            get => _menuTutorialComplete;
            set
            {
                _menuTutorialComplete = value;
            }
        }

        public bool ShopTutorialComplete
        {
            get => _shopTutorialComplete;
            set
            {
                _shopTutorialComplete = value;
            }
        }

        public bool EditKitchenTutorialComplete
        {
            get => _editKitchenTutorialComplete;
            set
            {
                _editKitchenTutorialComplete = value;
            }
        }
        
        public bool LeaderboardTutorialComplete
        {
            get => _leaderboardTutorialComplete;
            set
            {
                _leaderboardTutorialComplete = value;
            }
        }

        public bool RankTutorialComplete
        {
            get => _rankTutorialComplete;
            set
            {
                _rankTutorialComplete = value;
            }
        }

        public void SaveData()
        {
            DataLoader.SaveTutorialData(this);
        }
    }
}