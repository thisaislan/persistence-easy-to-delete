using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor
{
    internal static class PedeEditorMenu
    {
        
        #region ValidateRegion
        
        [MenuItem(Metadata.MenuItemOpenData + Metadata.MenuItemOpenDataShortcut, true)]
        private static bool OpenDataValidate() =>
            PedeEditor.IsDataFileAccessible();
        
        [MenuItem(Metadata.MenuItemValidateData + Metadata.MenuItemValidateDataShortcut, true)]
        private static bool ValidateDataValidate() => 
            PedeEditor.IsDataFileAccessible();
        
        [MenuItem(Metadata.MenuItemDeleteData, true)]
        private static bool DeleteDataValidate() => 
            PedeEditor.IsDataFileAccessible();

        #endregion // ValidateRegion
        
        #region DaraHandleRegion
        
        [MenuItem(Metadata.MenuItemNewData, priority = Metadata.MenuItemNewDataPriority)]
        private static void NewData() =>
            PedeEditor.CreateAnotherDataFile();

        [MenuItem(
            Metadata.MenuItemOpenData + Metadata.MenuItemOpenDataShortcut,
            priority = Metadata.MenuItemOpenDataOptionPriority)]
        private static void OpenData() =>
            PedeEditor.SelectDataFile();
        
        [MenuItem(
            Metadata.MenuItemValidateData + Metadata.MenuItemValidateDataShortcut,
            priority = Metadata.MenuItemValidateDataOptionPriority)]
        private static void ValidateData() =>
            ShoValidationDialog(
                PedeEditor.IsDataValid(
                    new PedeData.ValidationErrorHandler(
                        ShowValidationErrorValueMessage,
                        ShowValidationErrorKeyMessage,
                        ShowValidationErrorTypeMessage
                        )
                    )
                );

        [MenuItem(Metadata.MenuItemDeleteData, priority = Metadata.MenuItemDeleteDataPriority)]
        private static void DeleteData()
        {
            if (ShouldDelete()) { PedeEditor.DeleteDataFile(); }
        }
        
        #endregion //DaraHandleRegion

        #region UtilsRegion
        
        private static bool ShouldDelete() =>
            EditorUtility.DisplayDialog(
                    Metadata.MenuItemDeleteDialogTitle, 
                    Metadata.MenuItemDeleteDialogMessage, 
                    Metadata.MenuItemDeleteDialogOkButton,
                    Metadata.MenuItemDeleteDialogCancelButton
                );
        
        private static void ShoValidationDialog(bool isDataValid) =>
            EditorUtility.DisplayDialog(
                Metadata.MenuItemValidationDialogTitle, 
                isDataValid ? Metadata.MenuItemValidationDialogSuccessMessage :
                    Metadata.MenuItemValidationDialogErrorMessage, 
                Metadata.MenuItemValidationDialogOkButton
            );
        
        private static void ShowValidationErrorValueMessage(string key, int index, bool isFileData) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorMessage(index, isFileData) +
                $"{Metadata.ValidationValueErrorMessage} " +
                $"{key}"
            );
        
        private static void ShowValidationErrorKeyMessage(string value, int index, bool isFileData, bool isDuplicity) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorMessage(index, isFileData) +
                $"{(isDuplicity? Metadata.ValidationDuplicatedKeyErrorMessage : Metadata.ValidationEmptyKeyErrorMessage)} " +
                $"{value}"
            );
        
        private static void ShowValidationErrorTypeMessage(string key, int index, bool isFileData) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorMessage(index, isFileData) +
                $"{Metadata.ValidationTypeErrorMessage} " +
                $"{key}"
            );

        private static string GetFirstPartOfErrorMessage(int index, bool isFileData) =>
            $"{Metadata.ValidationErrorMessageSuffix} " +
            $"{(isFileData ? Metadata.ValidationErrorMessageFileType : Metadata.ValidationErrorMessagePlayerPrefsType)} " +
            $"{index} ";

        private static void ShowValidationErrorMessageOnConsole(string message)
        {
            var stackTraceLogType = Application.GetStackTraceLogType(LogType.Error);
            
            SetErrorStackTraceLogType(StackTraceLogType.None);
            
            Debug.LogError(message);

            SetErrorStackTraceLogType(stackTraceLogType);
        }

        private static void SetErrorStackTraceLogType(StackTraceLogType stackTraceLogType) =>
            Application.SetStackTraceLogType(LogType.Error, stackTraceLogType);

        #endregion //UtilsRegion

    }
}