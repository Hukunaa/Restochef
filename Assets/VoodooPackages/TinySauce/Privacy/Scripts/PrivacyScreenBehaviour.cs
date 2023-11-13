using System;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Voodoo.Sauce.Internal
{
    public class PrivacyScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private Toggle advertisingToggle;
        [SerializeField] private Button advertisingLearnMore;

        [SerializeField] private Toggle analyticsToggle;
        [SerializeField] private Button analyticsLearnMore;
        
        [SerializeField] private Toggle ageToggle;
        [SerializeField] private Button ageLearnMore;

        [SerializeField] private Button playButton;
        
        [SerializeField] private Button privacyPolicyButton;
        private TinySauceSettings _sauceSettings;
        
        [SerializeField] private Color activePlayButtonColor;
        [SerializeField] private Color disabledPlayButtonColor;

        [SerializeField] private GameObject mainUIObject;

        [SerializeField] private VoidEventChannel _onPlayButtonClicked;

        private EventSystem _eventSystemPrefab;
        private EventSystem _eventSystem;

        private void Start()
        {
            ageToggle.onValueChanged.AddListener(OnToggleAge);

            advertisingLearnMore.onClick.AddListener(OnPressLearnMore);
            analyticsLearnMore.onClick.AddListener(OnPressLearnMore);
            ageLearnMore.onClick.AddListener(OnPressLearnMore);

            playButton.onClick.AddListener(OnPressPlay);
            privacyPolicyButton.onClick.AddListener(OnPressPrivacyPolicy);
            
            _sauceSettings = TinySauceSettings.Load();
            if (_sauceSettings == null)
            {
                throw new Exception("Can't find the Settings ScriptableObject " +
                                    "in the Resources/TinySauce folder.");
            }
            
            InitEventSystem();
        }

        private void OnToggleAge(bool consent)
        {
            playButton.GetComponent<Image>().color = consent ? activePlayButtonColor : disabledPlayButtonColor;
            playButton.interactable = consent;
        }

        private void OnPressLearnMore()
        {
            PrivacyScreenUIManager.Instance.OpenPrivacyPartnersScreen();
        }

        private void OnPressPlay()
        {
            mainUIObject.SetActive(false);
            SaveConsents();
            RemoveListeners();
            _onPlayButtonClicked.RaiseEvent();
            
            var adConsent = PlayerPrefs.GetInt(PrivacyScreenUIManager.Instance.AdConsentPref) != 0;
            var analyticsConsent = PlayerPrefs.GetInt(PrivacyScreenUIManager.Instance.AnalyticsConsentPref) != 0;
            
            TinySauceBehaviour.Instance.InitAllTracking(adConsent, analyticsConsent);
            
            if (_eventSystem != null)
                Destroy(_eventSystem.gameObject);
            
            Destroy(gameObject);
        }

        private void SaveConsents()
        {
            PlayerPrefs.SetInt(PrivacyScreenUIManager.Instance.AdConsentPref, advertisingToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt(PrivacyScreenUIManager.Instance.AnalyticsConsentPref, analyticsToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void OnPressPrivacyPolicy()
        {
            if (_sauceSettings.privacyPolicyURL != "")
            {
                Application.OpenURL(_sauceSettings.privacyPolicyURL);
            }
        }

        private void RemoveListeners()
        {
            ageToggle.onValueChanged.RemoveAllListeners();
            playButton.onClick.RemoveAllListeners();
            privacyPolicyButton.onClick.RemoveAllListeners();
        }
        
        private void InitEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null) return;
            
            if (_eventSystemPrefab == null)
            {
                EventSystem[] eventSystemList = Resources.LoadAll<EventSystem>("Prefabs");
                _eventSystemPrefab = eventSystemList[0];
            }
                    
            if (_eventSystemPrefab == null)
                Debug.LogError("There is no TSEventSystem prefab in the 'Assets/VoodooPackages/TinySauce/Resources/Prefabs' folder");

            _eventSystem = Instantiate(_eventSystemPrefab);
        }
    }
}
