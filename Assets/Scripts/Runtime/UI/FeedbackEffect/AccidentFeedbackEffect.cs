using System;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.Utility;
using UnityEngine;

namespace Runtime.UI.FeedbackEffect
{
    public class AccidentFeedbackEffect : MonoBehaviour
    {
        [SerializeField] 
        private VoidEventChannel _onAccidentStart;
        
        [SerializeField] 
        private VoidEventChannel _onAccidentEnd;

        [SerializeField]
        private int _accidentCount;

        [SerializeField] 
        private bool _logDebugMessage;

        [SerializeField]
        private AudioSource _accidentEndAudioSource;

        [SerializeField]
        private AudioClip _accidentEndAudioClip;
        
        private void Awake()
        {
            _onAccidentStart.onEventRaised += AccidentStart;
            _onAccidentEnd.onEventRaised += AccidentEnd;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _onAccidentStart.onEventRaised -= AccidentStart;
            _onAccidentEnd.onEventRaised -= AccidentEnd;
        }

        private void AccidentStart()
        {
            DebugHelper.PrintDebugMessage("Station accident Start", _logDebugMessage);
            _accidentCount++;
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
        }

        private void AccidentEnd()
        {
            DebugHelper.PrintDebugMessage("Station accident End", _logDebugMessage);
            _accidentCount--;

            _accidentEndAudioSource.PlayOneShot(_accidentEndAudioClip);

            if (_accidentCount == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}