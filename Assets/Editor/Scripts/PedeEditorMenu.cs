using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects;
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
        
        [MenuItem(Metadata.MenuItemOpenSettings + Metadata.MenuItemOpenSettingsShortcut, true)]
        private static bool ValidateOpenSettings() => 
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
        private static void ValidateData() =>
            RunDataValidation(true);
        
        [MenuItem(Metadata.MenuItemOpenSettings + Metadata.MenuItemOpenSettingsShortcut, 
            priority = Metadata.MenuItemOpenSettingsPriority)]
        private static void OpenSettings() => 
            PedeEditor.SelectSettingsFile();
        
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
        
        #region StartRegion
        
        [InitializeOnEnterPlayMode]
        private static void CheckValidateDataOnRunStart()
        {
            if (PedeEditor.ShouldVerifyDataOnRunStart() && PedeEditor.ShouldRunAValidation())
            {
                RunDataValidation(false);
            }
        }
        
        #endregion //StartRegion

        #region UtilsRegion

        private static void RunDataValidation(bool showDialog)
        {
            ShowValidationWarningMessageOnConsole($"{Consts.DebugMessageSuffix} {Consts.ValidationLogMessage}");
            
            if (PedeEditor.HasCustomSerializerFile())
            {
                ShowValidationWarningMessageOnConsole($"{Consts.DebugMessageSuffix} {Consts.ValidationWarningMessage}");
                
                if (!IsCustomSerializerFileValid())
                {
                    if (showDialog) { ShoValidationDialog(false); }
                    
                    return;
                }
            }
            
            var isDataValid = IsDataValid();

            if (showDialog) { ShoValidationDialog(isDataValid); }
            
            if (isDataValid)
            {
                ShowValidationWarningMessageOnConsole(
                    $"{Consts.DebugMessageSuffix} " +
                    $"{Consts.MenuItemValidationDialogSuccessMessage}"
                );
                
                PedeEditor.CleanDataChangFlag();
            }
        }

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
        
        private static bool IsCustomSerializerFileValid() =>
            PedeEditor.IsCustomSerializerFileValid(
                new PedeSettings.ValidationSerializerErrorHandler(
                    ShowValidationSerializerMethodNotFound,
                    ShowValidationSerializerClassError,
                    ShowValidationSerializerInterfaceError
                )
            );

        private static bool IsDataValid() =>
            PedeEditor.IsDataValid(
                new PedeData.ValidationDataErrorHandler(
                    ShowValidationErrorDataValueMessage,
                    ShowValidationErrorDataKeyMessage,
                    ShowValidationErrorDataTypeMessage
                )
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

        private static void ShowValidationSerializerClassError(bool isEncapsulationError)
        {
            var bodyMessage = isEncapsulationError ? 
                Consts.ValidationSerializerEncapsulationErrorClassErrorMessage : 
                Consts.ValidationSerializerClassErrorMessage;
            
            ShowValidationErrorMessageOnConsole($"{Consts.DebugMessageSuffix} {bodyMessage}");
        }

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