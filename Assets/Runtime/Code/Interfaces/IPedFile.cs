using System;
using Thisaislan.PersistenceEasyToDelete.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal interface IPedFile
    {
        void Set<T>(string key, T value, IPedSerializer serializer);

        void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedSerializer serializer,
            bool destroyAfter
        );

        void Delete<T>(string key);

        void DeleteAll();

        void HasKey<T>(string key, Action<bool> actionWithResult);

    }
}