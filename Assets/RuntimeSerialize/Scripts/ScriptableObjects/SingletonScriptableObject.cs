using System.Linq;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.PedSerialize.ScriptableObjects.Bases
{
    internal class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
    {
        
        private static T privateInstance;

        internal static T instance
        {
            get
            {
                if (privateInstance == null)
                {
                    privateInstance = Resources.LoadAll<T>(string.Empty).First();
                }
                
                return privateInstance;
            }
        }
        
    }
}