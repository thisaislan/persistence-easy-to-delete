using System;
using System.Linq;
using Thisaislan.PersistenceEasyToDeleteInEditor.Interfaces;
using Thisaislan.PersistenceEasyToDeleteInEditor.Metas;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition
{
    internal static class PedePlayerPrefs
    {
        
        internal static void DeletePlayerPrefs(string key, bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteKey(key);

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }
        
        internal static void DeleteAllPlayerPrefs( bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteAll();

            if (shouldSaveImmediately) { SavePlayerPrefs(); }
        }

        internal static void HasPlayerPrefsKey(string key, Action<bool> actionWithResult) => 
            actionWithResult.Invoke(PlayerPrefs.HasKey(key));

        internal static void SavePlayerPrefs() =>
            PlayerPrefs.Save();

        internal static void SetPlayerPrefs<T>(
            string key,
            T value,
            Pede.PlayerPrefsSetMode playerPrefsSetMode,
            ISerializer serializer
        )
        {
            SetPlayerPrefs(key, value, serializer);

            if (playerPrefsSetMode != Pede.PlayerPrefsSetMode.Normal)
            {
                SavePlayerPrefs();
            }
        }
        
        internal static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Pede.PlayerPrefsGetMode playerPrefsGetMode,
            ISerializer serializer
        )
        {
            HasPlayerPrefsKey(key, (result) =>
            {
                if (!result) { actionIfHasNotResult?.Invoke(); }
                else { GetPlayerPrefs(key, actionIfHasResult, serializer); }
            });

            if (playerPrefsGetMode != Pede.PlayerPrefsGetMode.Normal)
            {
                var shouldSaveImmediately = playerPrefsGetMode == Pede.PlayerPrefsGetMode.DestructiveAndPersistent;
                
                DeletePlayerPrefs(key, shouldSaveImmediately);
            }
        }
        
        private static void SetPlayerPrefs<T>(string key, T value, ISerializer serializer)
        {
            if (Metadata.BuildInTypes.Contains(typeof(T)))
            {
                SetPlayerPrefsStringValue(key, Convert.ToString(value));
            }
            else if (typeof(T) == typeof(nint))
            {
                SetPlayerPrefsStringValue(key, Convert.ToString(value));
            }
            else if (typeof(T) == typeof(nuint))
            {
                SetPlayerPrefsStringValue(key, Convert.ToString(value));
            }
            else
            {
                SetPlayerPrefsStringValue(key, serializer.Serialize(value));
            }
        }
        
        private static void GetPlayerPrefs<T>(string key, Action<T> actionWithResult, ISerializer serializer)
        {
            var value = PlayerPrefs.GetString(key, default);
            var decompressedValue = StringCompressor.DecompressString(value);
            
            if (Metadata.BuildInTypes.Contains(typeof(T)))
            {
                GetPlayerPrefsValue(decompressedValue, actionWithResult);
            }
            else if (typeof(T) == typeof(nint))
            {
                GetPlayerPrefsNint(decompressedValue, actionWithResult as Action<nint>);
            }
            else if (typeof(T) == typeof(nuint))
            {
                GetPlayerPrefsUnint(decompressedValue, actionWithResult as Action<nuint>);
            }
            else
            {
                GetPlayerPrefsObject(decompressedValue, actionWithResult, serializer);
            }
        }

        private static void GetPlayerPrefsObject<T>(
                    string decompressedValue,
                    Action<T> actionWithResult,
                    ISerializer serializer
                ) => 
            actionWithResult.Invoke(serializer.Deserialize<T>(decompressedValue));

        private static void GetPlayerPrefsValue<T>(string decompressedValue, Action<T> actionWithResult) =>
            actionWithResult.Invoke((T)Convert.ChangeType(decompressedValue, typeof(T)));
        
        private static void GetPlayerPrefsNint(string decompressedValue, Action<nint> actionWithResult) =>
            actionWithResult.Invoke(Convert.ToInt32(decompressedValue));
        
        private static void GetPlayerPrefsUnint(string decompressedValue, Action<nuint> actionWithResult) =>
            actionWithResult.Invoke(Convert.ToUInt32(decompressedValue));

        private static void SetPlayerPrefsStringValue(string key, string value) =>
            SetCompressedPlayerPrefs(key, StringCompressor.CompressString(value));
        
        private static void SetCompressedPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);
        
    }
}