using Runtime.ScriptableObjects.DataContainers.Stations;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "NewIngredientMix", menuName = "Ingredients/IngredientMix", order = 0)]
    public class IngredientMix : ScriptableObject
    {
        [SerializeField]
        private RawIngredient _input;

        [SerializeField]
        private ProcessedIngredient _output;

        [SerializeField] 
        private float _processingDuration;
        
        [SerializeField] 
        private BaseStationAction _stationAction;
        
        public RawIngredient Input { get => _input; }
        public ProcessedIngredient Output { get => _output; }
        public float ProcessingDuration { get => _processingDuration; }
        public BaseStationAction StationAction => _stationAction;
        
        public bool HasInputIngredient(RawIngredient _ingredient)
        {
            return _input == _ingredient;
        }
    }
}
