using System.Runtime.CompilerServices;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Metas;
using UnityEngine;

#if UNITY_EDITOR
[assembly: InternalsVisibleTo(Metadata.EditorAssemblyNameInternalsVisibleTo)]
#endif
[assembly: InternalsVisibleTo(Metadata.RuntimeAssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDelete.PedSerialize
{
    internal class DefaultPedSerializer : IPedSerializer
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