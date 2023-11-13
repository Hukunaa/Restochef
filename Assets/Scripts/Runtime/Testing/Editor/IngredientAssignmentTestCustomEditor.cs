using Cinemachine;
using Runtime.Testing;
using UnityEditor;
using UnityEngine;

namespace Runtime.Editor
{
    [CustomEditor(typeof(IngredientAssignmentTest))]
    public class IngredientAssignmentTestCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            IngredientAssignmentTest myTarget = (IngredientAssignmentTest)target;

            if (GUILayout.Button("AssignIngredient"))
            {
                myTarget.AssignIngredient();
            }
        }
    }
}