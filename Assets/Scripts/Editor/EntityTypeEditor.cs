using UnityEngine;
using UnityEditor;
using Runtime.DataContainers;
using System.Linq;

[CustomEditor(typeof(EntityType))]
public class EntityTypeEditor : Editor
{
    SerializedProperty _entityType;
    SerializedProperty _entityName;
    SerializedProperty _entityDescription;

    string[] _entityTypes = new string[]
    {
        "table_station",
        "cutting_station",
        "stove_station",
        "boiling_station",
        "walkable_area",
        "entry_point",
        "cold_storage",
        "starches_storage",
        "vegetables_storage",
        "chef_spawnpoint",
        "locked_slot"
    };
    private int indexEntityType;

    private void OnEnable()
    {
        _entityType = serializedObject.FindProperty("_type");
        _entityName = serializedObject.FindProperty("_name");
        _entityDescription = serializedObject.FindProperty("_description");
        indexEntityType = _entityTypes.ToList().IndexOf(_entityType.stringValue);
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        GUILayout.Label("Entity Type", style);
        indexEntityType = EditorGUILayout.Popup(indexEntityType, _entityTypes);
        _entityType.stringValue = _entityTypes[indexEntityType];
        GUILayout.Label("Entity Name", style);
        EditorGUILayout.PropertyField(_entityName);
        GUILayout.Label("Entity Description", style);
        EditorGUILayout.PropertyField(_entityDescription);
        serializedObject.ApplyModifiedProperties();

    }
}
