using UnityEditor;

namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    [InitializeOnLoad]
    internal static class PedStorageRegistration
    {
        static PedStorageRegistration()
        {
            Ped.SetPlayerPrefsStorage(new EditorPlayerPrefsStorage());
            Ped.SetFileStorage(new EditorFileStorage());
        }

    }
}