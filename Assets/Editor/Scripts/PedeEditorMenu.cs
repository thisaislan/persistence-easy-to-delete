using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Settings;
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
        private static void NewData()
        {
            PedeEditor.CreateAnotherDataFile();
            
            ShowValidationWarningMessageOnConsole(
                $"{Consts.DebugMessageSuffix} " +
                $"{Consts.NewItemLogMessage}"
            );
        }

        [MenuItem(
            Metadata.MenuItemOpenData + Metadata.MenuItemOpenDataShortcut,
            priority = Metadata.MenuItemOpenDataOptionPriority)]
        private static void OpenData() =>
            PedeEditor.SelectDataFile();

        [MenuItem(
            Metadata.MenuItemValidateData + Metadata.MenuItemValidateDataShortcut,
            priority = Metadata.MenuItemValidateDataOptionPriority)]
        private static void ValidateData()
        {
            ShowValidationWarningMessageOnConsole(
                $"{Consts.DebugMessageSuffix} " +
                $"{Consts.ValidationLogMessage}"
            );
            
            if (PedeEditor.HasCustomSerializerFile())
            {
                ShowValidationWarningMessageOnConsole(
                        $"{Consts.DebugMessageSuffix} " +
                        $"{Consts.ValidationWarningMessage}"
                    );
                
                var isCustomSerializerFileValid = 
                        PedeEditor.IsCustomSerializerFileValid(
                            new PedeSettings.ValidationSerializerErrorHandler(
                                    ShowValidationSerializerMethodNotFound,
                                    ShowValidationSerializerClassError,
                                    ShowValidationSerializerInterfaceError
                                )
                            );

                if (!isCustomSerializerFileValid) { return; }
            }

            ShoValidationDialog(
                PedeEditor.IsDataValid(
                    new PedeData.ValidationDataErrorHandler(
                        ShowValidationErrorDataValueMessage,
                        ShowValidationErrorDataKeyMessage,
                        ShowValidationErrorDataTypeMessage
                    )
                )
            );
        }

        [MenuItem(Metadata.MenuItemDeleteData, priority = Metadata.MenuItemDeleteDataPriority)]
        private static void DeleteData()
        {
            if (ShouldDelete())
            {
                PedeEditor.DeleteDataFile();
                
                ShowValidationWarningMessageOnConsole(
                    $"{Consts.DebugMessageSuffix} " +
                    $"{Consts.DeleteLogMessage}"
                );
            }
        }
        
        #endregion //DaraHandleRegion

        #region UtilsRegion
        
        private static bool ShouldDelete() =>
            EditorUtility.DisplayDialog(
                    Consts.MenuItemDeleteDialogTitle, 
                    Consts.MenuItemDeleteDialogMessage, 
                    Consts.MenuItemDeleteDialogOkButton,
                    Consts.MenuItemDeleteDialogCancelButton
                );
        
        private static void ShoValidationDialog(bool isDataValid) =>
            EditorUtility.DisplayDialog(
                Consts.MenuItemValidationDialogTitle, 
                isDataValid ? Consts.MenuItemValidationDialogSuccessMessage :
                    Consts.MenuItemValidationDialogErrorMessage, 
                Consts.MenuItemValidationDialogOkButton
            );
        
        private static void ShowValidationErrorDataValueMessage(string key, int index, bool isFileData) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorDataMessage(index, isFileData) +
                $"{Consts.ValidationValueErrorMessage} " +
                $"{key}"
            );
        
        private static void ShowValidationErrorDataKeyMessage(string value, int index, bool isFileData, bool isDuplicity) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorDataMessage(index, isFileData) +
                $"{(isDuplicity? Consts.ValidationDuplicatedKeyErrorMessage : Consts.ValidationEmptyKeyErrorMessage)} " +
                $"{value}"
            );
        
        private static void ShowValidationErrorDataTypeMessage(string key, int index, bool isFileData) =>
        ShowValidationErrorMessageOnConsole(
                GetFirstPartOfErrorDataMessage(index, isFileData) +
                $"{Consts.ValidationTypeErrorMessage} " +
                $"{key}"
            );

        private static string GetFirstPartOfErrorDataMessage(int index, bool isFileData) =>
            $"{Consts.DebugMessageSuffix} " +
            $"{(isFileData ? Consts.ValidationErrorMessageFileType : Consts.ValidationErrorMessagePlayerPrefsType)} " +
            $"{index} ";
        
        private static void ShowValidationSerializerMethodNotFound(bool isSerializerMethod) =>
            ShowValidationErrorMessageOnConsole(
                $"{Consts.DebugMessageSuffix} " +
                $"{Consts.ValidationSerializerMethodNotFoundMessage} " +
                $"{(isSerializerMethod? Metadata.SerializerSerializeMethodName : Metadata.SerializerDeserializeMethodName)}"
            );
        
        private static void ShowValidationSerializerClassError() =>
            ShowValidationErrorMessageOnConsole(
                    $"{Consts.DebugMessageSuffix} {Consts.ValidationSerializerClassErrorMessage}"
                );
        
        private static void ShowValidationSerializerInterfaceError() =>
            ShowValidationErrorMessageOnConsole(
                    $"{Consts.DebugMessageSuffix} {Consts.ValidationSerializerClassInterfaceMessage}"
                );

        private static void ShowValidationErrorMessageOnConsole(string message) =>
            ShowValidationMessageOnConsole(LogType.Error, message);
        
        private static void ShowValidationWarningMessageOnConsole(string message) =>
            ShowValidationMessageOnConsole(LogType.Warning, message);
        
        private static void ShowValidationMessageOnConsole(LogType logType ,string message)
        {
            var stackTraceLogType = Application.GetStackTraceLogType(logType);
            
            Application.SetStackTraceLogType(logType, StackTraceLogType.None);

            if (logType == LogType.Error) { Debug.LogError(message); }
            else { Debug.LogWarning(message); }

            Application.SetStackTraceLogType(logType, stackTraceLogType);
        }
        
        #endregion //UtilsRegion

    }
}