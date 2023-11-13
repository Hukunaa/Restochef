#if UNITY_EDITOR
using GD.MinMaxSlider;
using Runtime.Managers;
#endif

using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.ScriptableObjects.DataContainers
{
    [CreateAssetMenu(fileName = "NewOrderSettings", menuName = "ScriptableObjects/Settings/OrderManagerSettings", order = 0)]
    public class OrderManagerSettings : ScriptableObject
    {
        [SerializeField] 
        private float[] _orderSpeedMultiplierPerRank;

        [SerializeField]
        private float _appearanceSpeedPercentageForSmallOrders;

        [SerializeField]
        #if UNITY_EDITOR
        [MinMaxSlider(2, 30)]
        #endif
        [Tooltip("The time range delay between orders will be picked from")]
        private Vector2Int _orderApparitionSpeed = new Vector2Int(5, 20);
        public Vector2Int OrderApparitionSpeed => _orderApparitionSpeed;

        public float OrderSpeedMultiplierPerRank { get => _orderSpeedMultiplierPerRank[GameManager.Instance.PlayerDataContainer.GetKitchenRank()]; }
        public float AppearanceSpeedPercentageForSmallOrders { get => _appearanceSpeedPercentageForSmallOrders; }
    }
}