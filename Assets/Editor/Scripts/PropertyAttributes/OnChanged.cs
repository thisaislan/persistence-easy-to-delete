using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.PropertyAttributes
{
    internal class OnChanged : PropertyAttribute
    {
        
        internal string methodName;
        internal OnChanged(string methodNameNoArguments)
        {
            methodName = methodNameNoArguments;
        }
        
    }

    [CustomPropertyDrawer(typeof(OnChanged))]
    internal class OnChangedCallAttributePropertyDrawer : PropertyDrawer
    {
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);

            if (EditorGUI.EndChangeCheck())
            {
                var onChanged = attribute as OnChanged;
                
                var method = property.serializedObject.targetObject.GetType().GetMethod(onChanged.methodName);

                if (method == null)
                {
                    method = property.serializedObject.targetObject.GetType().GetMethod(
                            onChanged.methodName, BindingFlags.NonPublic | BindingFlags.Instance
                        );
                }

                if (method != null && !method.GetParameters().Any())
                {
                    method.Invoke(property.serializedObject.targetObject, null);
                }
            }
        }
        
    }
}