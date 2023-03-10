namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants
{
    internal static class Consts
    {
        
        internal const string DebugMessageSuffix = "Pede ->";
        
        internal const string DeleteLogMessage = "File PedeData was deleted";
        internal const string ValidationLogMessage = "File PedeData validation";
        internal const string NewItemLogMessage = "New PedeData file was created";
        
        internal const string MenuItemDeleteDialogTitle = "DELETE";
        internal const string MenuItemDeleteDialogOkButton = "Yes";
        internal const string MenuItemDeleteDialogCancelButton = "No";
        internal const string MenuItemDeleteDialogMessage = "The current PedeData file will be deleted and this " +
                                                            "action cannot be undone. Are you sure about this?";
        
        internal const string MenuItemValidationDialogTitle = "VALIDATION";
        internal const string MenuItemValidationDialogOkButton = "Ok";
        internal const string MenuItemValidationDialogSuccessMessage = "The crrent PedeData file in use is valid.";
        internal const string MenuItemValidationDialogErrorMessage = "The crrent PedeData file in use contains errors." +
                                                                     " You can see more information in the console.";
        
        internal const string PedeSettingsClassTipAttr = "That file must remain with that name to be used. In case " +
                                                         "of rename or deletion, a new file with the same name will " +
                                                         "be created automatically";
        internal const string PedeSettingsDataHeaderAttr = "Select the PedeData to be used";
        internal const string PedeSettingsDataTooltipAttr = "This field will help you to select the PedeData to be " +
                                                            "used.";
        internal const string PedeSettingsSerializerHeaderAttr = "Select the custom serializer class file if needed";
        internal const string PedeSettingsSerializerTooltipAttr = "If the project has a custom serializer class, " +
                                                                  "(setted using the method Pede.SetCustomSerializer) " +
                                                                  "drop the same class file here used in the " +
                                                                  "validation phase. By default Pede will use " +
                                                                  "JsonUtility. \nNote that the file must " +
                                                                  "contains just one class.";
        
        internal const string ValidationWarningMessage = "A custom serialization file has been configured, this " +
                                                         "file will be used to validate the data";
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
        internal const string ValidationSerializerClassErrorMessage = "There is some problem with the custom " +
                                                                      "serializer class";
        internal const string ValidationSerializerClassInterfaceMessage = "Interface ISerializer not found on custom " +
                                                                          "serializer class";
        internal const string ValidationSerializerMethodNotFoundMessage = "Serialize method not foaund in teh custom " +
                                                                          "serializer class: ";

    }
}