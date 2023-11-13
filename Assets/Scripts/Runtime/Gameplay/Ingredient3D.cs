using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Runtime.Gameplay
{
    [Serializable]
    public class Ingredient3D : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        private IObjectPool<Ingredient3D> _pool;

        public void ChangeMesh(Mesh _mesh)
        {
            _meshFilter.mesh = _mesh;
        }

        public void SetPool(IObjectPool<Ingredient3D> _pool) => this._pool = _pool;

        public void Release()
        {
            if (_pool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _pool.Release(this);
            }
        }
    }
}