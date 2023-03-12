using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Processes
{
    internal class PedeAssetPostprocessor : AssetPostprocessor
    {

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            PedeEditor.CheckInitialization();
        }

    }
}