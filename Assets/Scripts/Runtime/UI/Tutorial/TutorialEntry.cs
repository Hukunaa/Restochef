using System;
using Runtime.ScriptableObjects.EventChannels;
using Runtime.UI.Tutorial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Runtime.UI.ShiftTutorial
{
    [Serializable]
    public class TutorialEntry
    {
        [TextArea]
        [SerializeField] 
        private string _text;
        
        [SerializeField] 
        private Vector2 _tutorialPosition;

        [SerializeField] 
        private bool _pauseTime;

        [SerializeField] 
        private bool _useFadeBackground;

        [SerializeField] 
        private UnityEvent _onTutorialEntryStart;
        
        [SerializeField] 
        private UnityEvent _onTutorialEntryEnd;
        
        [SerializeField] 
        private ETutorialAppearCondition _appearCondition;

        [SerializeField] 
        private float _delayDuration;
        
        [SerializeField] 
        private VoidEventChannel _showTutorialEventChannel;
        
        [SerializeField] 
        private EHideTutorialCondition _hideCondition;
       
        [SerializeField] 
        private Button _hideTutorialButton;
        
        [SerializeField] 
        private VoidEventChannel _hideTutorialEventChannel;

        public TutorialEntry()
        {
            _text = "";
        }
        
        public enum ETutorialAppearCondition
        {
            Sequence,
            Delay,
            Event
        }

        public enum EHideTutorialCondition
        {
            NextButton,
            ClickAnyWhere,
            ClickOnButton,
            EventChannel
        }

        public string Text => _text;

        public Vector2 TutorialWindowPosition => _tutorialPosition;

        public bool PauseTime => _pauseTime;
        public bool UseFadeBackground => _useFadeBackground;

        public UnityEvent OnTutorialEntryStart => _onTutorialEntryStart;
        public UnityEvent OnTutorialEntryEnd => _onTutorialEntryEnd;

        public ETutorialAppearCondition AppearCondition => _appearCondition;
        public float DelayDuration => _delayDuration;
        public VoidEventChannel ShowTutorialEventChannel => _showTutorialEventChannel;
        public EHideTutorialCondition HideCondition => _hideCondition;

        public Button TrackedHideTutorialButton => _hideTutorialButton;
        public VoidEventChannel HideTutorialEventChannel => _hideTutorialEventChannel;
    }
}