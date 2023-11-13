using System;
using System.Collections.Generic;
using Runtime.DataContainers;
using Runtime.DataContainers.Stats;
using Runtime.Enums;
using Runtime.Gameplay.StateMachine;
using Runtime.Pool;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.ScriptableObjects.Gameplay;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Gameplay
{
    public class Chef : MonoBehaviour
    {
        [Header("Chef Components")]
        [SerializeField]
        private ChefNavigation _nav;
        
        [SerializeField]
        private ChefStateMachine _stateMachine;
        
        [SerializeField]
        private Animator _chefAnimator;
        
        [SerializeField]
        private ChefCustomizationManager _chefCustomizationManager;
        
        private Transform _chefMesh;

        private ChefSocketsManager _chefSocketsManager;

        private ChefHighlightManager _chefHighlightManager;
        
        [Header("Event Channels")]
        [Header("Listening to")]
        [SerializeField] 
        private FloatEventChannel _onShiftRushStart;
        
        [SerializeField] 
        private VoidEventChannel _onShiftEnd;
        
        [Header("Broadcasting on")]
        [SerializeField] 
        private ChefSelectionEventChannel _onSelectChef;
        
        [SerializeField]
        private List<Instruction> _instructions = new List<Instruction>();
        
        [Header("Debug")]
        [SerializeField]
        private bool _logDebugMessage;

        public UnityAction onChefClicked;
        public UnityAction onInstructionsChanged;
        public UnityAction<Instruction> onInstructionCancelled;

        private KitchenTile _waitingTile;
        
        private Ingredient3DSpawner _ingredient3DSpawner;
        private Ingredient3D _ingredient3D;
        
        private ChefSettings _chefSettings;
        private ChefData _chefData;
        
        private static readonly int AnimationSpeedMultiplier = Animator.StringToHash("AnimationSpeedMultiplier");
        
        private void Awake()
        {
            _nav = GetComponent<ChefNavigation>();
            _chefCustomizationManager = GetComponent<ChefCustomizationManager>();
            
            _ingredient3DSpawner = FindObjectOfType<Ingredient3DSpawner>();

            _onShiftRushStart.onEventRaised += OnShiftRushStart;
            _onShiftEnd.onEventRaised += StopShift;
        }

        private void OnShiftRushStart(float _speedMultiplier)
        {
            _chefAnimator.SetFloat(AnimationSpeedMultiplier, _speedMultiplier);
            _nav.IncreaseChefSpeed(_speedMultiplier);
        }

        private void OnDestroy()
        {
            _onShiftRushStart.onEventRaised -= OnShiftRushStart;
            _onShiftEnd.onEventRaised -= StopShift;
        }
        
        private void StopShift()
        {
            for (int i = _instructions.Count - 1; i >= 0; i--)
            {
                CancelInstruction(_instructions[i]);
            }
            
            ReleaseIngredient();
            _instructions.Clear();
        }

        public void InitializeChef(ChefData _chefData, KitchenTile _waitingTile)
        {
            this._chefData = _chefData;
            _chefSettings = _chefData.ChefSettings;
            _chefCustomizationManager.onChefCustomized += OnMeshInitialized;
            _chefCustomizationManager.CustomizeChef(_chefSettings.CustomizationSettings);
            
            gameObject.name = _chefSettings.ChefName;
            this._waitingTile = _waitingTile;
        }

        private void OnMeshInitialized()
        {
            _chefSocketsManager = GetComponentInChildren<ChefSocketsManager>();
            _chefSocketsManager.InitializeIngredientSockets();
            _chefHighlightManager = GetComponentInChildren<ChefHighlightManager>();
            _chefCustomizationManager.onChefCustomized -= OnMeshInitialized;
            _chefMesh = transform.GetChild(0);
            _chefAnimator = _chefMesh.GetComponent<Animator>();
            _stateMachine.Initialize(_stateMachine.waitForInstructionState);
        }

        public void AddInstruction(Instruction _instruction)
        {
            _instructions.Add(_instruction);
            onInstructionsChanged?.Invoke();
            DebugHelper.PrintDebugMessage($"{_chefSettings.ChefName}: Instruction {_instruction.FormatInstruction()} queued. Currently {_instructions.Count} instruction in queue.", _logDebugMessage);
        }
        
        public void OnInstructionProcessed(Instruction _instruction)
        {
            if (CurrentInstruction != _instruction) return;
            
            _instructions.Remove(_instruction);
            onInstructionsChanged?.Invoke();
            DebugHelper.PrintDebugMessage($"{_chefSettings.ChefName}: Instruction {_instruction.FormatInstruction()} processed. Currently {_instructions.Count} instructions in queue.", _logDebugMessage);
        }

        public void CancelInstruction(Instruction _instruction)
        {
            DebugHelper.PrintDebugMessage($"{ChefSettings.ChefName} instruction cancelled");
            if (!_instructions.Contains(_instruction)) return;
            
            _instructions.Remove(_instruction);
            onInstructionCancelled?.Invoke(_instruction);
            onInstructionsChanged?.Invoke();
            DebugHelper.PrintDebugMessage($"{_chefSettings.ChefName}: Instruction {_instruction.FormatInstruction()} cancelled. Currently {_instructions.Count} instructions in queue.", _logDebugMessage);
        }

        public void PickupInstructionIngredient()
        {
            ReleaseIngredient();
            
            var ingredientSetting = CurrentInstruction.Ingredient.Ingredient3DSettings;
            if (ingredientSetting.IngredientMesh == null) return;
            
            _ingredient3D = _ingredient3DSpawner.RequestIngredient3D();
            _ingredient3D.ChangeMesh(ingredientSetting.IngredientMesh);
            _ingredient3D.transform.SetParent(ChefSocketsManager.GetIngredientSocket(ingredientSetting.IngredientSocket), false);
        }

        public void TransformIngredient(Station _station)
        {
            if (CurrentInstruction == null)
            {
                ReleaseIngredient();
                return;
            }
            
            var ingredientSetting = CurrentInstruction.IngredientOutput.Ingredient3DSettings;
            if (ingredientSetting.IngredientMesh == null)
            {
                ReleaseIngredient();
                return;
            }
            
            _ingredient3D.ChangeMesh(ingredientSetting.IngredientMesh);
            _ingredient3D.transform.SetParent(_station.IngredientSocket, false);
        }

        public void ChefClicked()
        {
            onChefClicked?.Invoke();
        }
        
        public void ReleaseIngredient()
        {
            if (_ingredient3D == null) return;
            
            _ingredient3D.Release();
            _ingredient3D = null;
        }
        
        public ChefNavigation Nav { get => _nav; }
        public ChefSettings ChefSettings { get => _chefSettings; set => _chefSettings = value; }
        public Vector3 Position { get => transform.position; }
        public ChefData Data { get => _chefData; }
        public Animator ChefAnimator { get => _chefAnimator; }
        public int InstructionCount { get => _instructions.Count; }
        public ChefStateMachine StateMachine { get => _stateMachine; }
        public ChefSocketsManager ChefSocketsManager => _chefSocketsManager;
        public ChefHighlightManager ChefHighlightManager => _chefHighlightManager;
        public KitchenTile WaitingTile { get => _waitingTile; }
        
        public Instruction QueuedInstruction
        {
            get
            {
                if (_instructions.Count > 1 && _instructions[1] != null)
                {
                    return _instructions[1];
                }

                else
                {
                    return null;
                }
            }
        }

        public Instruction CurrentInstruction
        {
            get
            {
                if (_instructions.Count > 0 && _instructions[0] != null)
                {
                    return _instructions[0];
                }
                
                return null;
            }
        }

    }
}