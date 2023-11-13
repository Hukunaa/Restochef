using System;
using GameAnalyticsSDK.Setup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Runtime.ScriptableObjects.Gameplay
{
    [CreateAssetMenu(fileName = "NewChefSettings", menuName = "ScriptableObjects/Gameplay/ChefSetting", order = 0)]
    public class ChefSettings : ScriptableObject
    {
        [SerializeField] 
        private Sprite _chefHeadPortrait;
        public Sprite ChefHeadPortrait => _chefHeadPortrait;

        [SerializeField] private Sprite _chefBodyPortrait;
        public Sprite ChefBodyPortrait => _chefBodyPortrait;
        
        [SerializeField] private string _chefName;
        public string ChefName => _chefName;

        [SerializeField] 
        private CustomizationSettings _customizationSettings;
        public CustomizationSettings CustomizationSettings => _customizationSettings;
    }

    [Serializable]
    public class CustomizationSettings
    {
        [SerializeField] private AssetReferenceT<GameObject> _mesh;
        [FormerlySerializedAs("_material")] [SerializeField] private AssetReferenceT<Material> _bodyMaterial;
        [FormerlySerializedAs("_material")] [SerializeField] private AssetReferenceT<Material> _headMaterial;
        
        [SerializeField] private Color _earColor = new Color(1,1,1,1);
        
        [SerializeField] private Color _robePrimaryColor = new Color(0,0,0,1);
        
        [SerializeField] private Color _robeSecondaryColor = new Color(1,0,0,1);
        
        [SerializeField] private Color _eyeBallColor = new Color(1,1,1,1);
        
        [SerializeField] private Color _noseColor = new Color(0,0,0,1);
        
        [SerializeField] private Color _eyeBrowColor = new Color(1,0,0,1);
        [SerializeField] private Color _irisColor = new Color(1,0,0,1);
        
        public AssetReferenceT<GameObject> Mesh => _mesh;
        public AssetReferenceT<Material> BodyMaterial => _bodyMaterial;
        public AssetReferenceT<Material> HeadMaterial => _headMaterial;
        public Color EarColor => _earColor;
        public Color RobePrimaryColor => _robePrimaryColor;
        public Color RobeSecondaryColor => _robeSecondaryColor;
        public Color EyeBallColor => _eyeBallColor;
        public Color NoseColor => _noseColor;
        public Color EyeBrowColor => _eyeBrowColor;
        public Color IrisColor => _irisColor;
    }
}