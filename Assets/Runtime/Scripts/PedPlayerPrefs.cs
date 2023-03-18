using System;
using System.Linq;
using Thisaislan.PersistenceEasyToDelete.Metas;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal static class PedPlayerPrefs
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
            Ped.PlayerPrefsSetMode playerPrefsSetMode,
            IPedSerializer serializer
        )
        {
            SetPlayerPrefs(key, value, serializer);

            if (playerPrefsSetMode != Ped.PlayerPrefsSetMode.Normal)
            {
                SavePlayerPrefs();
            }
        }
        
        internal static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Ped.PlayerPrefsGetMode playerPrefsGetMode,
            IPedSerializer serializer
        )
        {
            HasPlayerPrefsKey(key, (result) =>
            {
                if (!result) { actionIfHasNotResult?.Invoke(); }
                else { GetPlayerPrefs(key, actionIfHasResult, serializer); }
            });

            if (playerPrefsGetMode != Ped.PlayerPrefsGetMode.Normal)
            {
                var shouldSaveImmediately = playerPrefsGetMode == Ped.PlayerPrefsGetMode.DestructiveAndPersistent;
                
                DeletePlayerPrefs(key, shouldSaveImmediately);
            }
        }
        
        private static void SetPlayerPrefs<T>(string key, T value, IPedSerializer serializer)
        {
            if (Metadata.BuildInTypes.Contains(typeof(T)))
            {
                SetPlayerPrefsStringValue(key, Convert.ToString(value));
            }
            else
            {
                SetPlayerPrefsStringValue(key, serializer.Serialize(value));
            }
        }
        
        private static void GetPlayerPrefs<T>(string key, Action<T> actionWithResult, IPedSerializer serializer)
        {
            var value = PlayerPrefs.GetString(key, default);
            var decompressedValue = StringCompressor.DecompressString(value);
            
            if (Metadata.BuildInTypes.Contains(typeof(T)))
            {
                GetPlayerPrefsValue(decompressedValue, actionWithResult);
            }
            else
            {
                GetPlayerPrefsObject(decompressedValue, actionWithResult, serializer);
            }
        }

        private static void GetPlayerPrefsObject<T>(
                    string decompressedValue,
                    Action<T> actionWithResult,
                    IPedSerializer serializer
                ) => 
            actionWithResult.Invoke(serializer.Deserialize<T>(decompressedValue));

        private static void GetPlayerPrefsValue<T>(string decompressedValue, Action<T> actionWithResult) =>
            actionWithResult.Invoke((T)Convert.ChangeType(decompressedValue, typeof(T)));

        private static void SetPlayerPrefsStringValue(string key, string value) =>
            SetCompressedPlayerPrefs(key, StringCompressor.CompressString(value));
        
        private static void SetCompressedPlayerPrefs(string key, string value) =>
            PlayerPrefs.SetString(key, value);
        
    }
}