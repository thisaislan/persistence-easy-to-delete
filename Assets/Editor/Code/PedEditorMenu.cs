using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    internal static class PedEditorMenu
    {
        [MenuItem(Metadata.MenuItemNewData, priority = Metadata.MenuItemNewDataPriority)]
        private static void NewData()
        {
            PedEditor.CreateAnotherDataFile();

            ShowValidationWarningMessageOnConsole(
                $"{Consts.DebugMessageSuffix} {Consts.NewItemLogMessage}"
            );
        }

        [MenuItem(
            Metadata.MenuItemOpenData,
            priority = Metadata.MenuItemOpenDataOptionPriority)]
        private static void OpenData() =>
            PedEditor.SelectDataFile();

        [MenuItem(Metadata.MenuItemOpenData, true)]
        private static bool OpenDataValidate() =>
            PedEditor.IsDataFileAccessible();

        private static void ShowValidationWarningMessageOnConsole(string message)
        {
            StackTraceLogType previousStackTraceLogType = Application.GetStackTraceLogType(LogType.Warning);

            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

            Debug.LogWarning(message);

            Application.SetStackTraceLogType(LogType.Warning, previousStackTraceLogType);
        }

    }
}