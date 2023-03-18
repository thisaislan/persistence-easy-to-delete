using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Constants
{
    internal static class Consts
    {
        
        internal const string DebugMessageSuffix = "Ped ->";
        
        internal const string DeleteLogMessage = "File PedData was deleted";
        internal const string ValidationLogMessage = "File PedData validation";
        internal const string NewItemLogMessage = "New PedData file was created";
        
        internal const string MenuItemDeleteDialogTitle = "DELETE";
        internal const string MenuItemDeleteDialogOkButton = "Yes";
        internal const string MenuItemDeleteDialogCancelButton = "No";
        internal const string MenuItemDeleteDialogMessage = "The current PedData file will be deleted and this " +
                                                            "action cannot be undone. Are you sure about this?";
        
        internal const string MenuItemValidationDialogTitle = "VALIDATION";
        internal const string MenuItemValidationDialogOkButton = "Ok";
        internal const string MenuItemValidationDialogSuccessMessage = "The crrent PedData file in use is valid.";
        internal const string MenuItemValidationDialogErrorMessage = "The current PedData file or custom serializer " +
                                                                     "class in use contains error. You can see more " +
                                                                     "information in the console.";
        
        internal const string PedSettingsClassTipAttr = "That file must remain with that name to be used. In case " +
                                                         "of rename or deletion, a new file with the same name will " +
                                                         "be created automatically";
        internal const string PedSettingsDataHeaderAttr = "Select the PedData to be used";
        internal const string PedSettingsDataTooltipAttr = "This field will help you to select the PedData to be " +
                                                            "used.";
        internal const string PedDataPlayerPrefsTooltipAttr = "This field will save all data that will be stored as" +
                                                               " PlayerPrefs when the game is in runtime.";
        internal const string PedDataFileToolTipAttr = "This field will save all data that will be stored as" +
                                                               " File when the game is in runtime.";
        internal const string PedDataFileInfoAttr = "Here you can see more info about that PedFile";

        internal const string PedDataAvoidChangesTooltipAttr = "Set this flag to true if you want Ped to try to " +
                                                                "avoid any changes to this PedData when running the " +
                                                                "editor. Ped will create a backup of the data when " +
                                                                "the editor enters play mode and will set the data " +
                                                                "back when play mode stops.";

        internal const string PedSettingsSerializerHeaderAttr = "Select the custom serializer class file if needed";

        internal const string PedSettingsSerializerTooltipAttr = "If your project needs a custom serializer class, " +
                                                                  "drop the file with the class here to allow Ped " +
                                                                  "to use the custom serialization." +
                                                                  "\nNote that file must contain only one a " +
                                                                  "public class, the class must have a unique name " +
                                                                  "and implement " + nameof(IPedSerializer) + 
                                                                  " interface.";
        internal const string PedSettingsSerializerPathTooltipAttr = "Custom serializer file path";
        internal const string PedSettingsSettingsHeaderAttr = "Select the desired behavior";
        internal const string PedSettingsVerifyOnStartTooltipAttr = "Set this flag to true if you want Ped to " +
                                                                     "perform a data validation on every start of " +
                                                                     "run in the editor.\nNote that validation will " +
                                                                     "only be performed if the data has new changes " +
                                                                     "or the editor was just launched.";
        internal const string PedDataClassTipAttr = "Here you can see and manipulate the data used by the game.\n" +
                                                     "You can rename and move this file if you like, but if the " +
                                                     "PedSettings file is recreated it will look for name PedData " +
                                                     "in the Ped folder.";
        internal const string PedDataHeaderAttr = "Data manipulete by Ped";
        internal const string PedDataSettingsHeaderAttr = "Data Settingse";
        
        internal const string ValidationWarningMessage = "A custom serialization file has been configured, this " +
                                                         "file will be used to validate the data";
        internal const string ValidationErrorMessagePlayerPrefsType = "(Player Prefs Data, index: ";
        internal const string ValidationErrorMessageFileType = "(File Data, index: ";
        internal const string ValidationValueErrorMessage = ") there is an error in the value or type of the element " +
                                                            "that has the key:";
        internal const string ValidationEmptyKeyErrorMessage = ") there is an error in the key of the element that " +
                                                               "has the value (key cannot be empty): ";
        internal const string ValidationDuplicatedKeyErrorMessage = ") there is an error in the key of the element " +
                                                                    "that has the value (duplicate key - keys in Ped " +
                                                                    "uses pair key and type):";
        internal const string ValidationTypeErrorMessage = ") there is an error in the type of the element that has " +
                                                           "the key (type cannot be empty): ";
        internal const string ValidationSerializerClassErrorMessage = "There is some problem with the custom " +
                                                                      "serializer class";
        internal const string ValidationSerializerEncapsulationErrorClassErrorMessage = "Custom serializer class must" +
                                                                                        " be a public class";
        internal const string ValidationSerializerClassInterfaceMessage = "Interface " + nameof(IPedSerializer) + 
                                                                          " not found on custom serializer class";
        internal const string ValidationSerializerMethodNotFoundMessage = "Serialize method not foaund in teh custom " +
                                                                          "serializer class: ";
        internal const string SessionStartFlag = "PedFirstInitDone";
        internal const string SessionOnScriptReloadFlag = "OnScriptReloadFlag";
        
        internal const string InfoDateFormation = "F";

    }
}