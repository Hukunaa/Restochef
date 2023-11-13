#if UNITY_EDITOR
using GD.MinMaxSlider;
#endif
using GeneralScriptableObjects;
using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "AISettings", menuName = "ScriptableObjects/Gameplay/AISetting", order = 0)]
    public class AISettings : ScriptableObject
    { 
#if UNITY_EDITOR
        [MinMaxSlider(0, 3f)]
#endif
        [SerializeField]
        private Vector2 _instructionReactionTime = new Vector2(.5f, 2);
        
#if UNITY_EDITOR
        [MinMaxSlider(.5f, 2)] 
#endif
        [SerializeField]
        private Vector2 _accidentReactionTime = new Vector2(.75f, 1.5f);
        
#if UNITY_EDITOR
        [MinMaxSlider(0.1f, .5f)] 
#endif
        [SerializeField]
        private Vector2 _repairSpeedRange = new Vector2(.2f, .4f);

        [SerializeField][Range(0.05f,1)]
        private float _updateRate = .2f;
        
        public Vector2 InstructionReactionTime => _instructionReactionTime;

        public Vector2 AccidentReactionTime => _accidentReactionTime;

        public Vector2 RepairSpeedRange => _repairSpeedRange;

        public float UpdateRate => _updateRate;
    }
}