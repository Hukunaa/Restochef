using System;
using UnityEngine;

namespace Voodoo.Sauce.Internal
{
    public class PrivacyScreenUIManager : MonoBehaviour
    {
        [SerializeField] private PrivacyScreenBehaviour privacyScreenPrefab;
        private const string advertisingConsentPref = "AdConsent";
        private const string analyticsConsentPref = "AnalyticsConsent";
        [SerializeField] private PrivacyPartnersScreenBehaviour privacyPartnersScreenPrefab;

        public string AdConsentPref => advertisingConsentPref;
        public string AnalyticsConsentPref => analyticsConsentPref;

        public static event Action<bool, bool> OnConsentGiven;
        public static bool ConsentReady;
        public static bool AdConsent;
        public static bool AnalyticsConsent;

        public static PrivacyScreenUIManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void OpenPrivacyScreen()
        {
            Instantiate(privacyScreenPrefab);
        }

        public void OpenPrivacyPartnersScreen()
        {
            Instantiate(privacyPartnersScreenPrefab);
        }
        
        public static void ConsentGiven(bool adsConsent, bool analyticsConsent)
        {
            AdConsent = adsConsent;
            AnalyticsConsent = analyticsConsent;
            ConsentReady = true;
            OnConsentGiven?.Invoke(adsConsent, analyticsConsent);
        }

        public void SaveNonGdprCountryConsents()
        {
            PlayerPrefs.SetInt(AdConsentPref,1);
            PlayerPrefs.SetInt(AnalyticsConsentPref, 1);
            PlayerPrefs.Save();
        }
    }
}
