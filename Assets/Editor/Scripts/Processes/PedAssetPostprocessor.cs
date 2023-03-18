using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

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
            PedEditor.CheckInitialization();
        }

    }
}