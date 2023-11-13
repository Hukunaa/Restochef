using System;
using ScriptableObjects.DataContainers;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RewardItem))]
public class RewardItemCustomEditor : Editor
{
    private SerializedProperty _title;
    private SerializedProperty _description;
    private SerializedProperty _starsRequired;
    private SerializedProperty _rewardSprite;
    private SerializedProperty _rewardType;
    private SerializedProperty _coinsReward;
    private SerializedProperty _rankReward;
    private SerializedProperty _chefID;
    private SerializedProperty _recipeReward;

    private void OnEnable()
    {
        GetSerializedProperties();
    }
    
    private void GetSerializedProperties()
    {
        _title = serializedObject.FindProperty("_title");
        _description = serializedObject.FindProperty("_description");
        _starsRequired = serializedObject.FindProperty("_starsRequired");
        _rewardSprite = serializedObject.FindProperty("_rewardSprite");
        _rewardType = serializedObject.FindProperty("_rewardType");
        _rankReward = serializedObject.FindProperty("_rankReward");
        _coinsReward = serializedObject.FindProperty("_coinsReward");
        _recipeReward = serializedObject.FindProperty("_recipeReward");
        _chefID = serializedObject.FindProperty("_chefID");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_title, new GUIContent("Title"));
        
        if (_rewardType.intValue != 1)
        {
            EditorGUILayout.PropertyField(_description, new GUIContent("Description"));
        }

        EditorGUILayout.PropertyField(_starsRequired, new GUIContent("Stars Required"));
        EditorGUILayout.PropertyField(_rewardSprite, new GUIContent("Reward Sprite"));
        EditorGUILayout.PropertyField(_rewardType, new GUIContent("Reward Type"));
        EditorGUI.indentLevel++;
        switch (_rewardType.intValue)
        {
            case 0:
                EditorGUILayout.PropertyField(_coinsReward, new GUIContent("CoinsAmount"));
                break;
            case 1:
                EditorGUILayout.PropertyField(_rankReward, new GUIContent("Rank Reward"));
                break;
            case 2:
                EditorGUILayout.PropertyField(_chefID, new GUIContent("Chef ID"));
                break;
            case 3:
                EditorGUILayout.PropertyField(_recipeReward, new GUIContent("Recipe Name"));
                break;
            case 4:
                EditorGUILayout.LabelField("Coming Soon!");
                break;
        }
        EditorGUI.indentLevel--;
        
        serializedObject.ApplyModifiedProperties();
    }
}
