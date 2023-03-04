using System;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants
{
    internal static class Metadata
    {
        
        internal const int AssetMenuDataOrder = 1;
        
        internal const int MenuItemNewDataPriority = 1;
        internal const int MenuItemOpenDataOptionPriority = 12;
        internal const int MenuItemValidateDataOptionPriority = 13;
        internal const int MenuItemDeleteDataPriority = 14;
        
        internal const int TextAreaDataMaxLines = 10;
        internal const int TextAreaDataMinLines = 4;
        
        internal const string AssemblyName = "Thisaislan.PersistenceEasyToDeleteInEditor.Pede";
        
        internal const string AssetMenuDataName = "Pede/Data";
        
        internal const string DataFileName = "PedeData";
        internal const string DataFilExtension = "asset";
        internal const string DataFolderPath = "Assets/Pede";
        internal const string DataFullFileName = "PedeData.asset";
        internal const string DataOldFilePrefix = "Old";
        
        internal const string MenuItemDeleteData = "Tools/Pede/Delete PedeData";
        internal const string MenuItemNewData = "Tools/Pede/New PedeData";
        internal const string MenuItemOpenData = "Tools/Pede/Open PedeData";
        internal const string MenuItemValidateData = "Tools/Pede/Validate PedeData";
        internal const string MenuItemValidateDataShortcut = " #F11";
        internal const string MenuItemOpenDataShortcut = " #F12";
        
        internal const string MenuItemDeleteDialogTitle = "DELETE";
        internal const string MenuItemDeleteDialogOkButton = "Yes";
        internal const string MenuItemDeleteDialogCancelButton = "No";
        internal const string MenuItemDeleteDialogMessage = "The current PedeData file will be deleted and this " +
                                                            "action cannot be undone. Are you sure about this?";
        
        internal const string MenuItemValidationDialogTitle = "VALIDATION";
        internal const string MenuItemValidationDialogOkButton = "Ok";
        internal const string MenuItemValidationDialogSuccessMessage = "The crrent PedeData file in use is valid.";
        internal const string MenuItemValidationDialogErrorMessage = "The crrent PedeData file in use contains errors. " +
                                                                     "You can see more information in the console.";
        
        internal const string PedeSettingsHeaderAttr = "Select the PedeData to be used";
        internal const string PedeSettingsTooltipAttr = "This fild will help you to select the PedeData to be used.\n" +
                                                        "The file PedeSettings must remain with that name to be used, " +
                                                        "in case of rename or deletion, a new file with the same name" +
                                                        "will be created automatically";

        internal const string SettingFolderPath = "Assets/Settings";
        internal const string SettingFullFileName = "PedeSettings.asset";
        internal const string SettingsFileName = "PedeSettings";

        internal const string ValidationErrorMessageSuffix = "Pede ->";
        internal const string ValidationErrorMessagePlayerPrefsType = "(Player Prefs Data, index: ";
        internal const string ValidationErrorMessageFileType = "(File Data, index: ";
        internal const string ValidationValueErrorMessage = ") there is an error in the value or type of the element " +
                                                            "that has the key:";
        internal const string ValidationEmptyKeyErrorMessage = ") there is an error in the key of the element that " +
                                                               "has the value (key cannot be empty): ";
        internal const string ValidationDuplicatedKeyErrorMessage = ") there is an error in the key of the element " +
                                                                    "that has the value (duplicate key):";
        internal const string ValidationTypeErrorMessage = ") there is an error in the type of the element that has " +
                                                           "the key (type cannot be empty): ";

        internal static Type[] BuildInTypes =
        {
            typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float),
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(string)
        };
        
    }
}
