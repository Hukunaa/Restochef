using Runtime.DataContainers.Inventory;
using Runtime.DataContainers.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerCurrencies _playerCurrencies;
    private PlayerProgression _playerProgression;
    private PlayerInventory _playerInventory;

    private void Start()
    {
        //Need to be replaced by server info
        _playerProgression = new PlayerProgression(4, 1565, 2400, 1.2f);
        _playerInventory = GetComponent<PlayerInventory>();
    }

    public int GetBalance { get => _playerCurrencies.SoftCurrencyBalance; }
    public int GetCurrentLevel { get => _playerProgression.Level; }
    public List<InventoryItem> GetPlayerItems { get => _playerInventory.Items; }
}
