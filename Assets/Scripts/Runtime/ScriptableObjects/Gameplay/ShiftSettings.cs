using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "ShiftSettings", menuName = "ScriptableObjects/Gameplay/ShiftSettings", order = 0)]
    public class ShiftSettings : ScriptableObject
    {
        [SerializeField][Tooltip("Total duration of the shift in seconds")]
        private int _shiftDuration = 150;
        
        [SerializeField][Tooltip("Start of the rush in seconds from the end of shift")]
        private int _shiftRushStart = 30;

        [SerializeField] private float _rushSpeedIncrease = 1.5f;

        public int ShiftDuration => _shiftDuration;
        public int ShiftRushStart => _shiftRushStart;
        public float RushSpeedIncrease => _rushSpeedIncrease;
    }
}