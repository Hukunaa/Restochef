using UnityEngine;

namespace Runtime.ScriptableObjects.Gameplay.Ingredients
{
    [CreateAssetMenu(fileName = "NewProcessedIngredient", menuName = "Ingredients/RawIngredient", order = 0)]
    public class RawIngredient : Ingredient
    {
        [SerializeField] private StorageType _storageType;
        public StorageType StorageType => _storageType;
    }
}