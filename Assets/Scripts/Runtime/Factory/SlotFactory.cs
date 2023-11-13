using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Runtime.Factory
{
    public static class SlotFactory
    {
        public static void InitializeSlots(ref List<VisualElement> _slots, VisualElement _slotsContainer, int _slotsQuantity, StyleSheet _slotStyleSheet, string _slotClassName)
        {
            _slotsContainer.Clear();
            
            for (int i = 0; i < _slotsQuantity; i++)
            {
                var slot = CreateSlot(_slotStyleSheet, _slotClassName);
                _slotsContainer.Add(slot);
                _slots.Add(slot);
            }
        }

        private static VisualElement CreateSlot(StyleSheet _slotStyleSheet, string _slotClassName)
        {
            var slot = new VisualElement();
            slot.styleSheets.Add(_slotStyleSheet);
            slot.AddToClassList(_slotClassName);
            slot.AddToClassList("slot");
            return slot;
        }
    }
}