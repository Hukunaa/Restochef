using System.Collections;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Managers.GameplayManager.Orders.CustomClass
{
    public class IngredientAssignedEffect : MonoBehaviour
    {
        [SerializeField] 
        private ParticleSystem _particleSystem;

        private IObjectPool<IngredientAssignedEffect> _pool;
        
        public void PlayParticleEffect(Transform _ingredientTransform)
        {
            gameObject.transform.position = _ingredientTransform.position;
            StartCoroutine(PlayParticleEffectCoroutine());
        }

        private IEnumerator PlayParticleEffectCoroutine()
        {
            _particleSystem.Play(true);
            yield return new WaitForSeconds(_particleSystem.main.duration);
            
            Release();
        }
        
        public void SetPool(IObjectPool<IngredientAssignedEffect> _pool) => this._pool = _pool;

        private void Release()
        {
            _pool.Release(this);
        }
    }
}