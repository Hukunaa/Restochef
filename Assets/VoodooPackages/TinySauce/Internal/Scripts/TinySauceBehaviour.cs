using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using com.adjust.sdk;
using UnityEngine;
using Voodoo.Sauce.Internal.Analytics;
using Voodoo.Sauce.Internal.IdfaAuthorization;
using Facebook.Unity;
using Runtime.ScriptableObjects.EventChannels;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Voodoo.Sauce.Internal
{
    internal class TinySauceBehaviour : MonoBehaviour
    {
        private const string TAG = "TinySauce";
        public static TinySauceBehaviour Instance { get; private set; }
        private TinySauceSettings _sauceSettings;
        private bool _advertiserTrackingEnabled;
        private static IABTestManager aBTestManager;

        public static IABTestManager ABTestManager => aBTestManager;
        
        private const string FirstStartPref = "FirstStart";
        private const string ConsentUrl = "https://api-gdpr.voodoo-tech.io/need_consent?bundle_id=&popup_version=&os_type=&locale=&app_version=&uuid=0";

        [SerializeField] private VoidEventChannel _onGDRPReady;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
                return;
            }
            
            VoodooLog.Initialize(VoodooLogLevel.WARNING);
            Action<bool, bool> grdpAction = delegate (bool a, bool b) { _onGDRPReady?.RaiseEvent(); };
            TinySauce.SubscribeToConsentGiven(grdpAction);
            InitABTest();

            LoadSauceSettings();

            CheckForGdpr();

            if (transform != transform.root)
                throw new Exception("TinySauce prefab HAS to be at the ROOT level!");
        }
        

        private void InitCallback()
        {
            if (FB.IsInitialized)
            {
                #if UNITY_IOS
                    FB.Mobile.SetAdvertiserTrackingEnabled(_advertiserTrackingEnabled); // iOS only call, do not need to be done on Android
                    FB.Mobile.SetAdvertiserIDCollectionEnabled(_advertiserTrackingEnabled); 
                #elif UNITY_ANDROID
                    FB.Mobile.SetAdvertiserIDCollectionEnabled(true);
                #endif
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
                
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void InitABTest() //All initializations should be done like this. Would be useful for module/sdk addition/removal
        {
            if(GetAbTestingManager().Count == 0) return;
            aBTestManager = (IABTestManager) Activator.CreateInstance(GetAbTestingManager()[0]);
            aBTestManager.Init();
        }

        private static List<Type> GetAbTestingManager()
        {
            Type interfaceType = typeof(IABTestManager);
            List<Type> AbTest = GetTypes(interfaceType);

            return AbTest;
        }
        
        private async void CheckForGdpr()
        {
            var privacyScreenUIManager = GetComponent<PrivacyScreenUIManager>();
            
            if (!PlayerPrefs.HasKey(FirstStartPref))
            {
                var isGdpr = await ConsentRequest();
                
                if (isGdpr)
                {
                    privacyScreenUIManager.OpenPrivacyScreen();
                }
                else
                {
                    privacyScreenUIManager.SaveNonGdprCountryConsents();
                    InitAllTracking(true, true);
                }
            }
            else
            {
                InitAllTracking(
                    (PlayerPrefs.GetInt(privacyScreenUIManager.AdConsentPref) != 0),
                    (PlayerPrefs.GetInt(privacyScreenUIManager.AnalyticsConsentPref) != 0));
            }
            
        }
        
        public void InitAllTracking(bool consentAds, bool consentAnalytics)
        {
            #if UNITY_IOS
            
            NativeWrapper.RequestAuthorization((status) =>
            {
                _advertiserTrackingEnabled = status == IdfaAuthorizationStatus.Authorized;
                
                if (consentAds)
                {
                    InitFacebook();
                    InitAdjust();
                }
            
                if (consentAnalytics)
                {
                    InitAnalytics();
                }
            });
            
            #elif UNITY_ANDROID
            
            if (consentAds)
            {
                InitFacebook();
                InitAdjust();
            }
            
            if (consentAnalytics)
            {
                InitAnalytics();
            }
            
            #endif

            PrivacyScreenUIManager.ConsentGiven(consentAds, consentAnalytics);

            if (!PlayerPrefs.HasKey(FirstStartPref))
            {
                PlayerPrefs.SetInt(FirstStartPref, 0);
                PlayerPrefs.Save();
            }
        }

        private void InitAnalytics()
        {
            VoodooLog.Log(TAG, "Initializing Analytics");
            
            AnalyticsManager.Initialize(_sauceSettings, true);
            AnalyticsManager.TrackApplicationLaunch(); 
        }
        
        private void InitFacebook()
        {
            if (!FB.IsInitialized) FB.Init(InitCallback, OnHideUnity);
            
            else InitCallback();         
        }
        
        private void InitAdjust()
        {
            //GetComponent<Adjust>().InitAdjust();
            #if UNITY_IOS
            AdjustConfig adjustConfig = new AdjustConfig(_sauceSettings.adjustIOSToken, AdjustEnvironment.Production);
            #elif UNITY_ANDROID
            AdjustConfig adjustConfig = new AdjustConfig(_sauceSettings.adjustAndroidToken, AdjustEnvironment.Production);
            #else
            Debug.LogWarning("Please make sur to select iOS or Android platform on your build settings");
            AdjustConfig adjustConfig = new AdjustConfig("", AdjustEnvironment.Sandbox);
            #endif
            
            Adjust.start(adjustConfig);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // Brought forward after soft closing 
            // Brought forward after soft closing 
            if (!pauseStatus) {
                AnalyticsManager.OnApplicationResume();
            }
        }
        
        internal static void InvokeCoroutine(IEnumerator coroutine)
        {
            if (Instance == null) return;
            Instance.StartCoroutine(coroutine);
        }

        private static List<Type> GetTypes(Type toGetType)
        {
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => toGetType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            types.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

            return types;
        }

        private void LoadSauceSettings()
        {
            _sauceSettings = TinySauceSettings.Load();
            if (_sauceSettings == null)
                throw new Exception("Can't find TinySauce sauceSettings file.");
        }
        
        private async Task<bool> ConsentRequest()
        {
            var request = UnityWebRequest.Get(ConsentUrl);
            request.SendWebRequest();

            while (!request.isDone)
            {
                await Task.Yield();
            }
            
#if UNITY_2020_1_OR_NEWER
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.Log(request.error);
                return false;
            }
            
            var consentInfo = JsonUtility.FromJson<ConsentInfo>(request.downloadHandler.text);
            return consentInfo.is_gdpr;
        }
    }
}