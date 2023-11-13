using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Runtime.DataContainers;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.ScriptableObjects.DataContainers.Stations
{
    public class BaseStationAction : ScriptableObject
    {
        [SerializeField]
        private Sprite _stationIcon;
        [SerializeField] 
        private ActionType _stationActionType;
        [SerializeField]
        private IngredientMix[] mixes;

        public bool CanProcessIngredient(RawIngredient _ingredient)
        {
            return mixes.Any(_ingredientMix => _ingredientMix.HasInputIngredient(_ingredient));
        }

        public void AssignMixes(IngredientMix[] _mixes)
        {
            this.mixes = _mixes;
        }

        public ProcessedIngredient GetMixOutput(RawIngredient _ingredientInput)
        {
            foreach (var ingredientMix in mixes)
            {
                if (ingredientMix.HasInputIngredient(_ingredientInput))
                {
                    return ingredientMix.Output;
                }
            }

            return null;
        }

        public virtual IngredientResult ProcessIngredient(RawIngredient _ingredient) { return null; }

        public Sprite StationIcon { get => _stationIcon; }
        public IngredientMix[] Mixes { get => mixes;}
        public ActionType StationActionType { get => _stationActionType; }
    }
}
