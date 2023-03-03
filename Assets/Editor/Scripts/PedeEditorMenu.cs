using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using UnityEditor;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor
{
    public class PedeEditorMenu
    {
        [MenuItem(Metadata.MenuItemOpenData, true)]
        private static bool OpenDataValidate() =>
            PedeEditor.IsDataFileAccessible();
        
        [MenuItem(Metadata.MenuItemDeleteData, true)]
        private static bool DeleteDataValidate() => 
            PedeEditor.IsDataFileAccessible();
        
        [MenuItem(Metadata.MenuItemNewData, priority = Metadata.MenuItemNewDataPriority)]
        private static void NewData() =>
            PedeEditor.CreateAnotherDataFile();

        [MenuItem(
            Metadata.MenuItemOpenData + Metadata.MenuItemOpenDataShortcut,
            priority = Metadata.MenuItemOpenDataOptionPriority)]
        private static void OpenData() =>
            PedeEditor.SelectDataFile();

        [MenuItem(Metadata.MenuItemDeleteData, priority = Metadata.MenuItemDeleteDataPriority)]
        private static void DeleteData()
        {
            if (ShouldDelete()) { PedeEditor.DeleteDataFile(); }
        }

        private static bool ShouldDelete() =>
            EditorUtility.DisplayDialog(
                    Metadata.MenuItemDeleteDialogTitle, 
                    Metadata.MenuItemDeleteDialogMessage, 
                    Metadata.MenuItemDeleteDialogOkButton,
                    Metadata.MenuItemDeleteDialogCancelButton
                );

    }
}