using System;
using System.Collections;
using System.Collections.Generic;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay
{
    public class Storage : MonoBehaviour
    {
        [SerializeField] 
        private StorageType _storageType;
        
        [SerializeField] 
        private string _storageName;
        
        [SerializeField] 
        private float _timeToTakeIngredient;
        
        [SerializeField] 
        private FloatEventChannel _onShiftStartRushEventChannel;
       
        [SerializeField] 
        private bool _logDebugMessage = false;

        public event Action<Ingredient> OnIngredientTaken;
        
        private readonly LinkedList<Instruction> _instructionQueue = new LinkedList<Instruction>();
        public Instruction CurrentInstruction => _instructionQueue.First.Value;

        private readonly HashSet<Ingredient> _storedIngredients = new HashSet<Ingredient>();

        private float _speedMultiplier = 1;

        private Coroutine _takeIngredientCoroutine;
        
        private void Awake()
        {
            _onShiftStartRushEventChannel.onEventRaised += OnShiftRushStart;
        }

        private void OnDestroy()
        {
            _onShiftStartRushEventChannel.onEventRaised -= OnShiftRushStart;
        }
        
        private void OnShiftRushStart(float _speedIncrease)
        {
            _speedMultiplier = _speedIncrease;
        }
        
        public void QueueInstruction(Instruction _instruction)
        {
            _instructionQueue.AddLast(_instruction);
        }

        public void InstructionComplete()
        {
            _instructionQueue.RemoveFirst();
        }

        public void InstructionCancelled(Instruction _cancelledInstruction)
        {
            _instructionQueue.Remove(_cancelledInstruction);
        }

        public void StoreIngredient(Ingredient _ingredient)
        {
            if (_storedIngredients.Contains(_ingredient)) return;
            
            _storedIngredients.Add(_ingredient);
            DebugHelper.PrintDebugMessage($"Create storage entry for {_ingredient.IngredientName} in {_storageName}", _logDebugMessage);
        }

        public void TakeStorageIngredient(Ingredient _ingredient)
        {
            _takeIngredientCoroutine = StartCoroutine(TakeIngredientCoroutine(_ingredient));
        }
        
        public IEnumerator TakeIngredientCoroutine(Ingredient _ingredient)
        {
            if (!_storedIngredients.Contains(_ingredient))
            {
                DebugHelper.PrintDebugMessage($"The ingredient {_ingredient.IngredientName} is not available in {_storageName}.", _logDebugMessage);
                yield break;
            }
            
            var timeToTakeIngredient = _timeToTakeIngredient / _speedMultiplier;
            float currentTime = 0;
            while (currentTime < timeToTakeIngredient)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
            
            OnIngredientTaken?.Invoke(_ingredient);
            DebugHelper.PrintDebugMessage($"{_ingredient.IngredientName} was taken from {_storageName}.", _logDebugMessage);
        }
        
        public bool HasIngredientInStorage(Ingredient _ingredient)
        {
            return _storedIngredients.Contains(_ingredient);
        }
        
        public string StorageName => _storageName;
        public StorageType StorageType => _storageType;
        public HashSet<Ingredient> StoredIngredients => _storedIngredients;
        public Vector3 Position => transform.position;

        public Coroutine CurrentTakeIngredientCoroutine => _takeIngredientCoroutine;
    }
}