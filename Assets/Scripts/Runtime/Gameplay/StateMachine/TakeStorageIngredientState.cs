using System;
using System.Collections;
using Runtime.Managers.GameplayManager;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.Gameplay.StateMachine
{
    [Serializable]
    public class TakeStorageIngredientState : ChefState
    {
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int OpenFridge = Animator.StringToHash("TakeFridgeIngredient");
        private static readonly int TakeVegetable = Animator.StringToHash("TakeVegetable");
        private static readonly int OpenCupboard = Animator.StringToHash("TakeCupboardIngredient");
        
        private StorageManager _storageManager;

        private Instruction _chefInstruction;
        private Storage _storage;
        
        private void Awake()
        {
            _storageManager = FindObjectOfType<StorageManager>();
        }
        
        public override void Enter()
        {
            _chefInstruction = _chef.CurrentInstruction;
            _chef.onInstructionCancelled += OnInstructionCancelled;
            _storage = _storageManager.GetClosestStorage(((RawIngredient)_chef.CurrentInstruction.Ingredient).StorageType,
                _chef.CurrentInstruction.Chef.Position);
        }
        
        private void OnInstructionCancelled(Instruction _cancelledInstruction)
        {
            if (_chefInstruction != _cancelledInstruction) return;
            
            _storage.InstructionCancelled(_cancelledInstruction);
            StopAllCoroutines();
            _chef.Nav.StopCurrentMovement();
            _chef.Nav.OnChefMovementPaused -= OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed -= OnChefMovementResumed;
            _storage.OnIngredientTaken -= OnIngredientTaken;
            _chef.StateMachine.TransitionTo(_chef.StateMachine.returnToWaitingPositionState);
        }
        
        public override IEnumerator ExecuteStateBehavior()
        {
            _storage.QueueInstruction(_chef.CurrentInstruction);
            yield return StartCoroutine(MoveToStorage());
            TakeIngredient();
        }
        
        private IEnumerator MoveToStorage()
        {
            _chef.Nav.MoveToStorage(_storage);
            _chef.Nav.OnChefMovementPaused += OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed += OnChefMovementResumed;
            
            PlayAnimation(Walk);
            while (_chef.Nav.IsMoving)
            {
                yield return null;
            }
            
            _chef.Nav.OnChefMovementPaused -= OnChefMovementPaused;
            _chef.Nav.OnChefMovementResumed -= OnChefMovementResumed;
        }

        private void OnChefMovementResumed()
        {
            PlayAnimation(Walk);
        }

        private void OnChefMovementPaused()
        {
            PlayAnimation(Idle);
        }

        private void TakeIngredient()
        {
            PlayStorageInteractionAnimation();
            _storage.OnIngredientTaken += OnIngredientTaken;
            _storage.TakeStorageIngredient(_chefInstruction.Ingredient);
        }

        private void OnIngredientTaken(Ingredient _ingredient)
        {
            if (_chefInstruction.Ingredient.Ingredient3DSettings.IngredientMesh != null)
            {
                _chef.PickupInstructionIngredient();
            }

            _storage.OnIngredientTaken -= OnIngredientTaken;
            
            _storage.InstructionComplete();
            _chef.StateMachine.TransitionTo(_chef.StateMachine.processIngredientState);
        }

        private void PlayStorageInteractionAnimation()
        {
            switch (_storage.StorageType.name)
            {
                case "storageCold":
                    PlayAnimation(OpenFridge);
                    break;
                case "storageFresh":
                    PlayAnimation(TakeVegetable);
                    break;
                case "storageStd":
                    PlayAnimation(OpenCupboard);
                    break;
            }
        }
        
        public override void PlayAnimation(int _animationHash)
        {
            AnimatorHelper.ResetAllAnimatorTriggers(_chef.ChefAnimator);
            _chef.ChefAnimator.SetTrigger(_animationHash);
        }

        public override void Exit()
        {
            _chef.onInstructionCancelled -= OnInstructionCancelled;
        }
    }
}