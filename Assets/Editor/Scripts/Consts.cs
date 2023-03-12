using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;

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
        internal const string MenuItemValidationDialogErrorMessage = "The current PedeData file or custom serializer " +
                                                                     "class in use contains error. You can see more " +
                                                                     "information in the console.";
        
        internal const string PedeSettingsClassTipAttr = "That file must remain with that name to be used. In case " +
                                                         "of rename or deletion, a new file with the same name will " +
                                                         "be created automatically";
        internal const string PedeSettingsDataHeaderAttr = "Select the PedeData to be used";
        internal const string PedeSettingsDataTooltipAttr = "This field will help you to select the PedeData to be " +
                                                            "used.";
        internal const string PedeDataPlayerPrefsTooltipAttr = "This field will save all data that will be stored as" +
                                                               " PlayerPrefs when the game is in runtime.";
        internal const string PedeDataFileToolTipAttr = "This field will save all data that will be stored as" +
                                                               " File when the game is in runtime.";
        internal const string PedeDataFileInfoAttr = "Here you can see more info about that PedeFile";

        internal const string PedeDataAvoidChangesTooltipAttr = "Set this flag to true if you want Pede to try to " +
                                                                "avoid any changes to this PedeData when running the " +
                                                                "editor. Pede will create a backup of the data when " +
                                                                "the editor enters play mode and will set the data " +
                                                                "back when play mode stops.";

        internal const string PedeSettingsSerializerHeaderAttr = "Select the custom serializer class file if needed";

        internal const string PedeSettingsSerializerTooltipAttr = "If your project needs a custom serializer class, " +
                                                                  "drop the file with the class here to allow Pede " +
                                                                  "to use the custom serialization." +
                                                                  "\nNote that file must contain only one a " +
                                                                  "public class, the class must have a unique name " +
                                                                  "and implement " + nameof(IPedeSerializer) + 
                                                                  " interface.";
        internal const string PedeSettingsSerializerPathTooltipAttr = "Custom serializer file path";
        internal const string PedeSettingsSettingsHeaderAttr = "Select the desired behavior";
        internal const string PedeSettingsVerifyOnStartTooltipAttr = "Set this flag to true if you want Pede to " +
                                                                     "perform a data validation on every start of " +
                                                                     "run in the editor.\nNote that validation will " +
                                                                     "only be performed if the data has new changes " +
                                                                     "or the editor was just launched.";
        internal const string PedeDataClassTipAttr = "Here you can see and manipulate the data used by the game.\n" +
                                                     "You can rename and move this file if you like, but if the " +
                                                     "PedeSettings file is recreated it will look for name PedeData " +
                                                     "in the Pede folder.";
        internal const string PedeDataHeaderAttr = "Data manipulete by Pede";
        internal const string PedeDataSettingsHeaderAttr = "Data Settingse";
        
        internal const string ValidationWarningMessage = "A custom serialization file has been configured, this " +
                                                         "file will be used to validate the data";
        internal const string ValidationErrorMessagePlayerPrefsType = "(Player Prefs Data, index: ";
        internal const string ValidationErrorMessageFileType = "(File Data, index: ";
        internal const string ValidationValueErrorMessage = ") there is an error in the value or type of the element " +
                                                            "that has the key:";
        internal const string ValidationEmptyKeyErrorMessage = ") there is an error in the key of the element that " +
                                                               "has the value (key cannot be empty): ";
        internal const string ValidationDuplicatedKeyErrorMessage = ") there is an error in the key of the element " +
                                                                    "that has the value (duplicate key - keys in Pede " +
                                                                    "uses pair key and type):";
        internal const string ValidationTypeErrorMessage = ") there is an error in the type of the element that has " +
                                                           "the key (type cannot be empty): ";
        internal const string ValidationSerializerClassErrorMessage = "There is some problem with the custom " +
                                                                      "serializer class";
        internal const string ValidationSerializerEncapsulationErrorClassErrorMessage = "Custom serializer class must" +
                                                                                        " be a public class";
        internal const string ValidationSerializerClassInterfaceMessage = "Interface " + nameof(IPedeSerializer) + 
                                                                          " not found on custom serializer class";
        internal const string ValidationSerializerMethodNotFoundMessage = "Serialize method not foaund in teh custom " +
                                                                          "serializer class: ";
        internal const string SessionStartFlag = "PedeFirstInitDone";
        internal const string SessionOnScriptReloadFlag = "OnScriptReloadFlag";
        
        internal const string InfoDateFormation = "F";

    }
}