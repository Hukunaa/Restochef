using System;
using System.Collections.Generic;
using Runtime.DataContainers.Stats;
using Runtime.Managers;
using Runtime.Managers.GameplayManager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runtime.UI.UIUtility
{
    public class EndShiftChefXPManager : MonoBehaviour
    {
        [SerializeField] 
        private ShiftRewardManager _shiftRewardManager;
        
        [SerializeField] 
        private AssetReferenceT<GameObject> _chefXPItemAssetRef;

        private AsyncOperationHandle<GameObject> chefXPItemLoadHandle;

        private List<ChefXPItem> _chefXPItems = new List<ChefXPItem>();

        private GameObject _chefXPItemPrefab;

        public void Initialize()
        {
            chefXPItemLoadHandle = _chefXPItemAssetRef.LoadAssetAsync<GameObject>();
            chefXPItemLoadHandle.Completed += ChefXPItemLoadHandleOnCompleted;
        }

        private void OnDestroy()
        {
            if (!chefXPItemLoadHandle.IsValid()) return;
            Addressables.Release(chefXPItemLoadHandle);
        }

        private void ChefXPItemLoadHandleOnCompleted(AsyncOperationHandle<GameObject> _obj)
        {
            chefXPItemLoadHandle.Completed -= ChefXPItemLoadHandleOnCompleted;
            _chefXPItemPrefab = _obj.Result;
            GenerateChefXPItems();
        }

        private void GenerateChefXPItems()
        {
            ChefData[] _chefDatas =
                GameManager.Instance.PlayerDataContainer.SelectedKitchenData._brigadeChefs.ToArray();
            
            foreach (var brigadeChef in _chefDatas)
            {
                var chefXPItem = Instantiate(_chefXPItemPrefab, transform).GetComponent<ChefXPItem>();
                chefXPItem.Initialize(brigadeChef, _shiftRewardManager);
                _chefXPItems.Add(chefXPItem);
            }
            
            _shiftRewardManager.ApplyChefsXp();
            
            foreach (var chefXpItem in _chefXPItems)
            {
                chefXpItem.SetXpInfo();
            }
        }
    }
}