using Runtime.DataContainers;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KitchenEditButtonNotification : MonoBehaviour
{
    [SerializeField]
    private GameObject _notification;
    private PlayerDataContainer _playerContainer;
    private void Start()
    {
        StartCoroutine("WaitForDataToLoad");
    }

    private IEnumerator WaitForDataToLoad()
    {
        while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
        {
            yield return null;
        }
        _playerContainer = GameManager.Instance.PlayerDataContainer;
        _playerContainer.Currencies.BalanceChanged += UpdateNotification;
        UpdateNotification();
    }

    void UpdateNotification()
    {
        Debug.Log("Updating Notification");
        List<KitchenTile> _tiles = _playerContainer.SelectedKitchenData.tiles.Cast<KitchenTile>()
                                    .Where(p => p.UpgradableData != null && p.TileData != "locked_slot").ToList();

        if (_tiles.Count() != 0)
        {
            if (_tiles.Any(t => _playerContainer.Currencies.SoftCurrencyBalance >= t.UpgradableData.Data.CostToUpgrade && !t.UpgradableData.IsMaxLevel))
                _notification.SetActive(true);
            else
                _notification.SetActive(false);

        }
    }

    private void OnDestroy()
    {
        _playerContainer.Currencies.BalanceChanged -= UpdateNotification;
    }
}
