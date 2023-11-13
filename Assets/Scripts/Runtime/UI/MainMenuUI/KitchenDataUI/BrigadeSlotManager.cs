using UnityEngine;

namespace Runtime.UI.MainMenuUI.KitchenDataUI
{
    public class BrigadeSlotManager : ItemSlotsManager
    {
        protected override void GenerateSlots()
        {
            for (int i = 0; i < _playerDataContainer.SelectedKitchenData.maxChefSlots; i++)
            {
                ItemSlot slot;
                if (i < _playerDataContainer.SelectedKitchenData.minChefSlots)
                {
                    slot = Instantiate(_mandatorySlotPrefab, transform).GetComponent<ItemSlot>();
                }
                else
                {
                    slot = Instantiate(_optionalSlotPrefab, transform).GetComponent<ItemSlot>();
                }

                _slots.Add(slot);
            }
            
            _slotsGeneratedEventChannel.RaiseEvent();
        }
    }
}