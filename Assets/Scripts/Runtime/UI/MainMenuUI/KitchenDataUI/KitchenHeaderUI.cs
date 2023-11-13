using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.Utility;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class KitchenHeaderUI : MonoBehaviour
{
    [SerializeField]
    private Image _light;
    [SerializeField]
    private TextMeshProUGUI _starsText;
    [SerializeField]
    private TextMeshProUGUI _nextStarsText;
    [SerializeField]
    private TextMeshProUGUI _currentStarsText;
    [SerializeField]
    private TextMeshProUGUI _rankText;
    [SerializeField]
    private TextMeshProUGUI _kitchenName;
    [SerializeField]
    private GameObject _notification;
    [SerializeField]
    private PlayerDataContainer _playerDataContainer;

    private void Awake()
    {        
        _playerDataContainer.OnStarRewardsLoaded += GetUpdateInfo;
    }

    void Start()
    {
        StartCoroutine("WaitForDataToLoad");
    }

    private IEnumerator WaitForDataToLoad()
    {
        while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
        {
            yield return null;
        }
        _playerDataContainer.SelectedKitchenData.OnXPValueChanged += GetUpdateInfo;
        GetUpdateInfo();
    }

    public void GetUpdateInfo()
    {
        Debug.Log("Updating Kitchen header");
        if (_playerDataContainer.PlayerRewards.Any(p => _playerDataContainer.SelectedKitchenData.kitchenStars >= p.StarsRequired
        && !_playerDataContainer.PlayerRewardsTextList[_playerDataContainer.PlayerRewards.IndexOf(p)]._isUsed && p.RewardType != ScriptableObjects.DataContainers.REWARD_TYPE.RANK))
            _notification.SetActive(true);
        else
            _notification.SetActive(false);

        int _level = _playerDataContainer.GetKitchenRank();
        int _stars = _playerDataContainer.KitchenRanks[_level].StarsRequired;
        int _current = _playerDataContainer.SelectedKitchenData.kitchenStars;
        int _nextRankStars = (_level + 1) >= _playerDataContainer.KitchenRanks.Count ? _playerDataContainer.KitchenRanks[_level].StarsRequired : _playerDataContainer.KitchenRanks[_level + 1].StarsRequired;

        if (_light)
        {
            _light.fillAmount = (float)_stars / (float)_nextRankStars;
        }

        if (_rankText)
            _rankText.text = (_level + 1).ToString();

        if (_starsText && _nextStarsText)
            _starsText.text = _stars.ToString() + " / " + _nextRankStars.ToString();

       // if (_nextStarsText)
         //   _nextStarsText.text = _nextRankStars.ToString();
    }

    void Update()
    {
    }

    private void OnDestroy()
    {
        _playerDataContainer.SelectedKitchenData.OnXPValueChanged -= GetUpdateInfo;
        _playerDataContainer.OnStarRewardsLoaded -= GetUpdateInfo;
    }
}
