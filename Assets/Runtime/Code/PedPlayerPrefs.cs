using System;
using System.Globalization;
using System.Linq;
using Thisaislan.PersistenceEasyToDelete.Metas;
using Thisaislan.PersistenceEasyToDelete.Interfaces;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal class PedPlayerPrefs : IPedPlayerPrefs
    {
        public void Set<T>(
            string key,
            T value,
            Ped.PlayerPrefsSetMode playerPrefsSetMode,
            IPedSerializer serializer
        )
        {
            string formattedKey = GetFormattedKey(key, typeof(T));

            SetPlayerPrefs(formattedKey, value, serializer);

            if (playerPrefsSetMode != Ped.PlayerPrefsSetMode.Normal)
            {
                Save();
            }
        }

        public void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Ped.PlayerPrefsGetMode playerPrefsGetMode,
            IPedSerializer serializer
        )
        {
            string formattedKey = GetFormattedKey(key, typeof(T));

            HasPlayerPrefsKey(formattedKey, (result) =>
            {
                if (!result)
                {
                    actionIfHasNotResult?.Invoke();
                }
                else
                {
                    GetPlayerPrefs(formattedKey, actionIfHasResult, serializer);
                }
            });

            if (playerPrefsGetMode != Ped.PlayerPrefsGetMode.Normal)
            {
                bool shouldSaveImmediately = playerPrefsGetMode == Ped.PlayerPrefsGetMode.DestructiveAndPersistent;

                DeletePlayerPrefs(formattedKey, shouldSaveImmediately);
            }
        }

        public void Delete<T>(string key, bool shouldSaveImmediately) =>
            DeletePlayerPrefs(GetFormattedKey(key, typeof(T)), shouldSaveImmediately);

        public void DeleteAll(bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteAll();

            if (shouldSaveImmediately)
            {
                Save();
            }
        }

        public void HasKey<T>(string key, Action<bool> actionWithResult) =>
            HasPlayerPrefsKey(GetFormattedKey(key, typeof(T)), actionWithResult);

        public void Save() =>
            PlayerPrefs.Save();

        private void DeletePlayerPrefs(string formattedKey, bool shouldSaveImmediately)
        {
            PlayerPrefs.DeleteKey(formattedKey);

            if (shouldSaveImmediately)
            {
                Save();
            }
        }

        private void HasPlayerPrefsKey(string formattedKey, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(PlayerPrefs.HasKey(formattedKey));

        private void SetPlayerPrefs<T>(string formattedKey, T value, IPedSerializer serializer)
        {
            string stringValue = Metadata.BuiltInTypes.Contains(typeof(T))
                ? Convert.ToString(value, CultureInfo.InvariantCulture)
                : serializer.Serialize(value);

            PlayerPrefs.SetString(formattedKey, StringCompressor.CompressString(stringValue));
        }

        private void GetPlayerPrefs<T>(string formattedKey, Action<T> actionWithResult, IPedSerializer serializer)
        {
            string value = PlayerPrefs.GetString(formattedKey, default);
            string decompressedValue = StringCompressor.DecompressString(value);

            if (Metadata.BuiltInTypes.Contains(typeof(T)))
            {
                GetPlayerPrefsValue(decompressedValue, actionWithResult);
            }
            else
            {
                GetPlayerPrefsObject(decompressedValue, actionWithResult, serializer);
            }
        }

        private void GetPlayerPrefsObject<T>(
            string decompressedValue,
            Action<T> actionWithResult,
            IPedSerializer serializer
        ) =>
            actionWithResult.Invoke(serializer.Deserialize<T>(decompressedValue));

        private void GetPlayerPrefsValue<T>(string decompressedValue, Action<T> actionWithResult) =>
            actionWithResult.Invoke((T)Convert.ChangeType(decompressedValue, typeof(T), CultureInfo.InvariantCulture));

        private static string GetFormattedKey(string key, Type type) =>
            KeyFormatter.Format(key, type);

    }
}