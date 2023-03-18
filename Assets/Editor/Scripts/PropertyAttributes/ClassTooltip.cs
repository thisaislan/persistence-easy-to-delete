using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Thisaislan.PersistenceEasyToDelete.Editor.PropertyAttributes
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ClassTooltip : PropertyAttribute
    {
        
        internal readonly string description;
 
        internal ClassTooltip(string description)
        {
            this.description = description;
        }
    }
 
    [CustomEditor(typeof(Object), editorForChildClasses: true)]
    internal class MyTooltipDrawer : UnityEditor.Editor
    {
        
        string tooltip;
 
        private void OnEnable()
        {
            var attributes = target.GetType().GetCustomAttributes(inherit: false);
            
            foreach(var attribute in attributes)
            {
                if(attribute is ClassTooltip tooltip) { this.tooltip = tooltip.description; }
            }
        }
        
        public override void OnInspectorGUI()
        {
            var textStyle = EditorStyles.label;
            textStyle.wordWrap = true;
            
            EditorGUILayout.LabelField(tooltip, textStyle);
            base.OnInspectorGUI();
        }
        
    }
}
