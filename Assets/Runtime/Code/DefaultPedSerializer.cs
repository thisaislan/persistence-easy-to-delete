using Thisaislan.PersistenceEasyToDelete.Interfaces;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete
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