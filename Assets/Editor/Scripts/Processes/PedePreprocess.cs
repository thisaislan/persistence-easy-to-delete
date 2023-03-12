using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Processes
{
    internal class PedePreprocess : IPreprocessBuildWithReport
    {
        
        public int callbackOrder => Metadata.PreprocessCallbackOrder;

        public void OnPreprocessBuild(BuildReport report)
        {
            PedeEditor.CheckCustomSerializer();
        }
        
    }
}