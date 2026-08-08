using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.Constants
{
    internal static class Consts
    {
        internal const string PedKeyFormat = "{0}~{1}";
        internal const string PedFilePathFormat = "{0}/{1}";
        internal const string PedFileRootFolderNamePath = "/Ped";
        internal const uint FnvHashOffsetBasis = 2166136261;
        internal const uint FnvHashPrime = 16777619;

        internal static readonly string PedFileRootFolderName =
            $"{Application.persistentDataPath}{PedFileRootFolderNamePath}";
    }
}
