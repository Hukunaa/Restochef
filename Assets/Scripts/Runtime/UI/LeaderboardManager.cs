using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.DataContainers.Player;
using Runtime.Managers;
using Runtime.ScriptableObjects.DataContainers;
using Runtime.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private AssetReferenceT<GameObject> _leaderboardEntryAssetRef;
        [SerializeField] private GameObject _leaderboardLeaderContainer;
        [SerializeField] private GameObject _leaderboardEntriesContainer;

        [SerializeField] private UnityEvent _onOpenLeaderboard;
        [SerializeField] private UnityEvent _onCloseLeaderboard;
        
        private AsyncOperationHandle<GameObject> _operationHandle;

        private GameObject _leaderboardPrefab;

        private PlayerDataContainer _playerDataContainer;

        private List<LeaderboardEntryUI> _leaderboardEntries = new List<LeaderboardEntryUI>();
        
        [SerializeField]
        private ScrollRect scrollRect;
        
        [SerializeField]
        private RectTransform contentPanel;

        private void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();

            contentPanel.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
                - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }
        
        private void Awake()
        {
            _operationHandle = _leaderboardEntryAssetRef.LoadAssetAsync<GameObject>();
            _operationHandle.Completed += OnLeaderboardPrefabLoaded;
        }

        private void OnLeaderboardPrefabLoaded(AsyncOperationHandle<GameObject> _obj)
        {
            _leaderboardPrefab = _obj.Result;
            StartCoroutine(InitializeLeaderboardCoroutine());
        }

        private IEnumerator InitializeLeaderboardCoroutine()
        {
            while (GameManager.Instance == null || !GameManager.Instance.DataLoaded)
            {
                yield return null;
            }

            _playerDataContainer = GameManager.Instance.PlayerDataContainer;
            _playerDataContainer.LeaderboardData.OnLeaderboardChanged += UpdateLeaderboard;

            GenerateLeaderboard();
            CloseLeaderboard();
        }

        private void GenerateLeaderboard()
        {
            for (int i = 0; i < _playerDataContainer.LeaderboardData.LeaderboardEntries.Count; i++)
            {
                var entryUI =
                    Instantiate(_leaderboardPrefab,
                            i == 0 ? _leaderboardLeaderContainer.transform : _leaderboardEntriesContainer.transform)
                        .GetComponent<LeaderboardEntryUI>();
                var entry = _playerDataContainer.LeaderboardData.LeaderboardEntries[i];
                entryUI.UpdateContent(i + 1, entry, IsPlayerEntry(entry));
                _leaderboardEntries.Add(entryUI);
            }
        }

        private void UpdateLeaderboard()
        {
            ClearLeaderboard();
            GenerateLeaderboard();
        }

        private void ClearLeaderboard()
        {
            for (int i = _leaderboardEntries.Count - 1; i >= 0; i--)
            {
                Destroy(_leaderboardEntries[i]);
            }
            _leaderboardEntries.Clear();
        }

        private bool IsPlayerEntry(LeaderboardEntry _entry)
        {
            return _entry.playerName == _playerDataContainer.PlayerName;
        }

        public void OpenLeaderboard()
        {
            gameObject.SetActive(true);
            _onOpenLeaderboard?.Invoke();
            
            var playerEntry = _leaderboardEntries.FirstOrDefault(x => x.Entry.playerName == _playerDataContainer.PlayerName);
            if (playerEntry == null)
            {
                DebugHelper.PrintDebugMessage($"There is no entry with the player name {_playerDataContainer.PlayerName}");
                return;
            } 
            SnapTo((RectTransform)playerEntry.transform);
        }

        public void CloseLeaderboard()
        {
            gameObject.SetActive(false);
            _onCloseLeaderboard?.Invoke();
        }

        private void OnDestroy()
        {
            _playerDataContainer.LeaderboardData.OnLeaderboardChanged -= UpdateLeaderboard;
            Addressables.Release(_operationHandle);
        }
    }
}