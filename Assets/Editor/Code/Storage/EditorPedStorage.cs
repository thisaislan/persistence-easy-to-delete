using System;
using Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects;
using Thisaislan.PersistenceEasyToDelete.Interfaces;
using Thisaislan.PersistenceEasyToDelete.PedComposition;

namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    internal class EditorPlayerPrefsStorage : IPedPlayerPrefs
    {
        public void Set<T>(
            string key,
            T value,
            Ped.PlayerPrefsSetMode playerPrefsSetMode,
            IPedSerializer serializer
        ) =>
            PedEditor.GetDataForStorage().SetPlayerPrefs(key, value, serializer);

        public void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Ped.PlayerPrefsGetMode playerPrefsGetMode,
            IPedSerializer serializer
        ) =>
            PedEditor.GetDataForStorage().GetPlayerPrefs(
                    key,
                    actionIfHasResult,
                    serializer,
                    actionIfHasNotResult,
                    playerPrefsGetMode != Ped.PlayerPrefsGetMode.Normal
                );

        public void Delete<T>(string key, bool shouldSaveImmediately) =>
            PedEditor.GetDataForStorage().DeletePlayerPrefs<T>(key);

        public void DeleteAll(bool shouldSaveImmediately) =>
            PedEditor.GetDataForStorage().DeleteAllPlayerPrefs();

        public void HasKey<T>(string key, Action<bool> actionWithResult) =>
            PedEditor.GetDataForStorage().HasPlayerPrefsKey<T>(key, actionWithResult);

        public void Save() { }

    }

    internal class EditorFileStorage : IPedFile
    {
        public void Set<T>(string key, T value, IPedSerializer serializer) =>
            PedEditor.GetDataForStorage().SetFile(key, value, serializer);

        public void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedSerializer serializer,
            bool destroyAfter
        ) =>
            PedEditor.GetDataForStorage().GetFile(
                    key,
                    actionIfHasResult,
                    serializer,
                    actionIfHasNotResult,
                    destroyAfter
                );

        public void Delete<T>(string key) =>
            PedEditor.GetDataForStorage().DeleteFile<T>(key);

        public void DeleteAll() =>
            PedEditor.GetDataForStorage().DeleteAllFiles();

        public void HasKey<T>(string key, Action<bool> actionWithResult) =>
            PedEditor.GetDataForStorage().HasFileKey<T>(key, actionWithResult);

    }
}