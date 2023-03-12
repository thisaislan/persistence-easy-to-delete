#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.PropertyAttributes
{
    public class ReadOnly : PropertyAttribute { }
    
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIState = GUI.enabled;
            GUI.enabled = false;
            
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGUIState;
        }
    }
    
}

#endif