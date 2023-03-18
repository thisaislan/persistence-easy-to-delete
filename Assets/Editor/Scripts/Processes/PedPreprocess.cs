using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Processes
{
    internal class PedPreprocess : IPreprocessBuildWithReport
    {
        
        public int callbackOrder => Metadata.PreprocessCallbackOrder;

        public void OnPreprocessBuild(BuildReport report)
        {
            PedEditor.CheckCustomSerializer();
        }
        
    }
}