using Runtime.Gameplay;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChefCustomizationManager))]
public class ChefCustomizationManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChefCustomizationManager customizationManager = (ChefCustomizationManager)target;
        if (GUILayout.Button("Customize Chef"))
        {
            customizationManager.CustomizeChef(customizationManager.ChefSettings.CustomizationSettings);
        }
    }
}
