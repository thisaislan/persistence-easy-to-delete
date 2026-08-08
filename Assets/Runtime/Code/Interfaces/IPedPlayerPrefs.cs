using System;
using Thisaislan.PersistenceEasyToDelete.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal interface IPedPlayerPrefs
    {
        void Set<T>(string key, T value, Ped.PlayerPrefsSetMode playerPrefsSetMode, IPedSerializer serializer);

        void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            Ped.PlayerPrefsGetMode playerPrefsGetMode,
            IPedSerializer serializer
        );

        void Delete<T>(string key, bool shouldSaveImmediately);

        void DeleteAll(bool shouldSaveImmediately);

        void HasKey<T>(string key, Action<bool> actionWithResult);

        void Save();

    }
}