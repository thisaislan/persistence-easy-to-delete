using System.Runtime.CompilerServices;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Metas;
using UnityEngine;

#if UNITY_EDITOR
[assembly: InternalsVisibleTo(Metadata.EditorAssemblyNameInternalsVisibleTo)]
#endif
[assembly: InternalsVisibleTo(Metadata.RuntimeAssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize
{
    internal class DefaultPedeSerializer : IPedeSerializer
    {
        
        public string Serialize(object obj)
        {
#if UNITY_EDITOR
            return JsonUtility.ToJson(obj, true);
#else
            return JsonUtility.ToJson(obj);
#endif
        }
        
        public T Deserialize<T>(string json) =>
            JsonUtility.FromJson<T>(json);
        
    }
}