using System;
using System.Collections;
using Runtime.ScriptableObjects.Gameplay;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.Gameplay
{
    public class ChefCustomizationManager : MonoBehaviour
    {
        [SerializeField] 
        private ChefSettings _chefSettings;

        private CustomizationSettings _customizationSettings;

        private GameObject _meshInstance;
        private GameObject _meshPrefab;
        private Material _bodyMaterial;
        private Material _headMaterial;
        private ChefMaterialManager _chefMaterialManager;
        
        private AsyncOperationHandle<GameObject> _meshLoadingHandle;
        private AsyncOperationHandle<Material> _bodyMaterialLoadingHandle;
        private AsyncOperationHandle<Material> _headMaterialLoadingHandle;

        public Action onChefCustomized;
        
        private static readonly int IrisColor = Shader.PropertyToID("_IrisColor");
        private static readonly int EyeBrowColor = Shader.PropertyToID("_EyeBrowColor");
        private static readonly int NoseColor = Shader.PropertyToID("_NoseColor");
        private static readonly int EyeBallColor = Shader.PropertyToID("_EyeBallColor");
        private static readonly int RobeSecondaryColor = Shader.PropertyToID("_RobeSecondaryColor");
        private static readonly int RobePrimaryColor = Shader.PropertyToID("_RobePrimaryColor");
        private static readonly int EarColor = Shader.PropertyToID("_EarColor");

        [ContextMenu("Customization Test")]
        private void CustomizationTest()
        {
            CustomizeChef(_chefSettings.CustomizationSettings);
        }
        
        public void CustomizeChef(CustomizationSettings _customizationSetting)
        {
            _customizationSettings = _customizationSetting;
            
            if (_meshInstance != null)
            {
                Destroy(_meshInstance);
                _meshInstance = null;
            }

            if (_meshLoadingHandle.IsValid())
            {
                Addressables.Release(_meshLoadingHandle);
                print("MeshLoadingHandle is valid.");
            }
            
            if (_bodyMaterialLoadingHandle.IsValid())
            {
                Addressables.Release(_bodyMaterialLoadingHandle);
                print("MaterialLoadingHandle is valid.");
            }

            StartCoroutine(CustomizeChefCoroutine());
        }


        private IEnumerator CustomizeChefCoroutine()
        {
            if (!_customizationSettings.Mesh.IsValid())
            {
                yield return StartCoroutine(LoadMeshCoroutine());
            }
            
            _meshPrefab = (GameObject)_customizationSettings.Mesh.Asset;
            
            InstantiateMeshPrefab();
            
            if (!_customizationSettings.BodyMaterial.IsValid())
            {
                yield return StartCoroutine(LoadBodyMaterialCoroutine());
            }
            
            _bodyMaterial = (Material)_customizationSettings.BodyMaterial.Asset;
            
            if (!_customizationSettings.HeadMaterial.IsValid())
            {
                yield return StartCoroutine(LoadHeadMaterialCoroutine());
            }

            _headMaterial = (Material)_customizationSettings.HeadMaterial.Asset;
            
            CustomizeMaterial();
            
            
            onChefCustomized?.Invoke();
        }
        
        private IEnumerator LoadMeshCoroutine()
        {
            _meshLoadingHandle = _customizationSettings.Mesh.LoadAssetAsync<GameObject>();
            while (!_meshLoadingHandle.IsDone)
            {
                print("Loading Mesh");
                yield return null;
            }
        }

        private IEnumerator LoadBodyMaterialCoroutine()
        {
            _bodyMaterialLoadingHandle = _customizationSettings.BodyMaterial.LoadAssetAsync<Material>();
            while (!_bodyMaterialLoadingHandle.IsDone)
            {
                print("Loading Body Material");
                yield return null;
            }
        }
        
        private IEnumerator LoadHeadMaterialCoroutine()
        {
            _headMaterialLoadingHandle = _customizationSettings.HeadMaterial.LoadAssetAsync<Material>();
            while (!_headMaterialLoadingHandle.IsDone)
            {
                print("Loading Head Material");
                yield return null;
            }
        }
        
        private void InstantiateMeshPrefab()
        {
            _meshInstance = Instantiate(_meshPrefab, transform);
            _chefMaterialManager = _meshInstance.GetComponentInChildren<ChefMaterialManager>();
        }
        
        private void CustomizeMaterial()
        {
            Material bodyMat = new Material(_bodyMaterial);
            bodyMat.SetColor(EarColor, _customizationSettings.EarColor);
            bodyMat.SetColor(RobePrimaryColor, _customizationSettings.RobePrimaryColor);
            bodyMat.SetColor(RobeSecondaryColor, _customizationSettings.RobeSecondaryColor);

            Material headMat = new Material(_headMaterial);
            headMat.SetColor(EyeBallColor, _customizationSettings.EyeBallColor);
            headMat.SetColor(NoseColor, _customizationSettings.NoseColor);
            headMat.SetColor(EyeBrowColor, _customizationSettings.EyeBrowColor);
            headMat.SetColor(IrisColor, _customizationSettings.IrisColor);
 
            _chefMaterialManager.AssignBodyMaterial(bodyMat);
            _chefMaterialManager.AssignHeadMaterial(headMat);
        }

        public ChefSettings ChefSettings => _chefSettings;
    }
}