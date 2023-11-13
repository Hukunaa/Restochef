using System;
using DG.Tweening;
using Runtime.Enums;
using Runtime.Pool;
using Runtime.ScriptableObjects.Gameplay.Ingredients;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Runtime.Managers.GameplayManager.Orders
{
    public class OrderIngredient : MonoBehaviour
    {
        [SerializeField] 
        private Image _rawIngredientImage;

        [SerializeField] 
        private Image _processIcon;

        [SerializeField] 
        private Color _assignedColor = Color.gray;

        [SerializeField] 
        private float _ingredientAssignedScale;

        [SerializeField] 
        private float _ingredientAssignedScaleEffectDuration;
        
        [SerializeField] 
        private float _ingredientAssignedFadeEffectDuration;

        [SerializeField]
        private AudioSource _ingredientAssignedAudio;

        [SerializeField] 
        private bool _logDebugMessage;
        
        private ProcessedIngredient _ingredient;
        private IngredientAssignedEffectPool _ingredientAssignedEffectPool;
        
        private IObjectPool<OrderIngredient> _pool;

        private Sequence _effectSequence;

        //Used by AI system to know if an ingredient has been assigned to a chef.
        private bool _assigned;
        private bool _completed;

        private void Awake()
        {
            _ingredientAssignedEffectPool = GameObject.FindGameObjectWithTag("IngredientAssignedEffectPool")
                .GetComponent<IngredientAssignedEffectPool>();
        }

        public void Initialize(ProcessedIngredient _ingredient)
        {
            this._ingredient = _ingredient;
            switch (_ingredient.IngredientType)
            {
                case EIngredientType.RawIngredient:
                    
                    break;
                case EIngredientType.ProcessedIngredient:
                    var processedIngredient = _ingredient;
                    ResetIngredient();
                    _rawIngredientImage.sprite = processedIngredient.IngredientMix.Input.IngredientIcon;
                    _processIcon.sprite = processedIngredient.IngredientMix.StationAction.StationIcon;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AssignIngredient()
        {
            DebugHelper.PrintDebugMessage($"{_ingredient.IngredientName} assigned to order", _logDebugMessage);
            _completed = true;
            PlayIngredientAssignedEffect();
        }

        private void PlayIngredientAssignedEffect()
        {
            _ingredientAssignedAudio.Play();
             
            var instance = _ingredientAssignedEffectPool.RequestIngredientAssignedEffect();
            instance.PlayParticleEffect(_rawIngredientImage.transform);

            _effectSequence = DOTween.Sequence();
            _effectSequence.Append(_rawIngredientImage.transform.DOScale(
                    new Vector3(_ingredientAssignedScale, _ingredientAssignedScale, 1),
                    _ingredientAssignedScaleEffectDuration))
                .Append(_rawIngredientImage.transform.DOScale(new Vector3(1, 1, 1),
                    _ingredientAssignedScaleEffectDuration))
                .Append(_rawIngredientImage.DOColor(_assignedColor, _ingredientAssignedFadeEffectDuration))
                .Join(_processIcon.DOColor(_assignedColor, _ingredientAssignedFadeEffectDuration));
        }
        
        public void SetPool(IObjectPool<OrderIngredient> _pool) => this._pool = _pool;

        public void ResetIngredient()
        {
            _completed = false;
            _assigned = false;
            _rawIngredientImage.transform.localScale = Vector3.one;
            _rawIngredientImage.color = Color.white;
            _processIcon.color = Color.white;
        }

        public void Release()
        {
            DebugHelper.PrintDebugMessage($"{Ingredient.IngredientName} Ingredient button Released", _logDebugMessage);
            StopAllCoroutines();
            KillDoTweens();
            ResetIngredient();
            _pool.Release(this);
        }

        private void KillDoTweens()
        {
            _effectSequence?.Kill();
        }

        public void Assign()
        {
            _assigned = true;
        }

        public ProcessedIngredient Ingredient => _ingredient;
        public bool Completed => _completed;
        public bool Assigned => _assigned;
    }
}