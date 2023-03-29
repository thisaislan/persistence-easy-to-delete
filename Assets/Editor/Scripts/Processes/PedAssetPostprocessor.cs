using System.Linq;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using UnityEditor;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Processes
{
    internal class PedAssetPostprocessor : AssetPostprocessor
    {

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            if (IsCheckInitializationNeeded(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths))
            {
                PedEditor.CheckInitialization();
            }
        }

        private static bool IsCheckInitializationNeeded(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        ) =>
                WasPedImported(importedAssets) ||
                WasPedSettingsDeleted(deletedAssets) ||
                WasPedSettingsMoved(movedAssets, movedFromAssetPaths);

        private static bool WasPedSettingsMoved(string[] movedAssets, string[] movedFromAssetPaths) =>
            ContainsValues(movedAssets, Metadata.SettingsFileName) ||
            ContainsValues(movedFromAssetPaths, Metadata.SettingsFileName);

        private static bool WasPedSettingsDeleted(string[] deletedAssets) =>
            ContainsValues(deletedAssets, Metadata.SettingsFileName);

        private static bool WasPedImported(string[] importedAssets) =>
            importedAssets.Length > 0 && importedAssets[0].Contains(Metadata.PackageFolderName);

        private static bool ContainsValues(string[] array, string value) =>
            !string.Equals(array.FirstOrDefault(x => x.Contains(value)), default);

    }
}