using Runtime.ScriptableObjects.DataContainers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDataContainer))]
public class PlayerDataContainerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        PlayerDataContainer playerDataContainer = (PlayerDataContainer)target;
        
        if (GUILayout.Button("Load Data"))
        {
            playerDataContainer.LoadPlayerData();
        }
    }
}
