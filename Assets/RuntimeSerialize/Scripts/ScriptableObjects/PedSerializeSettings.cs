using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Metas;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.ScriptableObjects.Bases;
using UnityEngine;

#if UNITY_EDITOR
using Thisaislan.PersistenceEasyToDelete.PedSerialize.PropertyAttributes;
#endif

#if UNITY_EDITOR
[assembly: InternalsVisibleTo(Metadata.EditorAssemblyNameInternalsVisibleTo)]
#endif
[assembly: InternalsVisibleTo(Metadata.RuntimeAssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDelete.PedSerialize.ScriptableObjects
{
    internal class PedSerializeSettings : SingletonScriptableObject<PedSerializeSettings>
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

        internal IPedSerializer GetSerializer()
        {
            if (customSerializerData.IsEmpty())
            {
                return new DefaultPedSerializer();
            }
            else
            {
                var assembly = Assembly.Load(customSerializerData.assemblyName);
                var type = assembly.GetType(customSerializerData.className);
                
                return (IPedSerializer)Activator.CreateInstance(type);
            }
        }
        
    }
}