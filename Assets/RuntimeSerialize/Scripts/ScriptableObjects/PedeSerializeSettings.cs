using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Metas;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.ScriptableObjects.Bases;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.PropertyAttributes;
#endif

#if UNITY_EDITOR
[assembly: InternalsVisibleTo(Metadata.EditorAssemblyNameInternalsVisibleTo)]
#endif
[assembly: InternalsVisibleTo(Metadata.RuntimeAssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.ScriptableObjects
{
    internal class PedeSerializeSettings : SingletonScriptableObject<PedeSerializeSettings>
    {
        
        [Serializable]
        internal struct CustomSerializerData
        {
            
#if UNITY_EDITOR
            [ReadOnly]
#endif
            [SerializeField]
            internal string assemblyName;
            
#if UNITY_EDITOR
            [ReadOnly]
#endif
            [SerializeField]
            internal string className;

            internal CustomSerializerData(string className, string assemblyName)
            {
                this.className = className;
                this.assemblyName = assemblyName;
            }

            internal bool IsEmpty() =>
                string.IsNullOrEmpty(className) &&
                string.IsNullOrEmpty(assemblyName);
            
        }
        
        [SerializeField]
        internal CustomSerializerData customSerializerData;

        internal void SetAssemblyData(CustomSerializerData customSerializerData) =>
            this.customSerializerData = customSerializerData;
        
        internal void CleanAssemblyData() =>
            this.customSerializerData = default;

        internal IPedeSerializer GetSerializer()
        {
            if (customSerializerData.IsEmpty())
            {
                return new DefaultPedeSerializer();
            }
            else
            {
                var assembly = Assembly.Load(customSerializerData.assemblyName);
                var type = assembly.GetType(customSerializerData.className);
                
                return (IPedeSerializer)Activator.CreateInstance(type);
            }
        }
        
    }
}