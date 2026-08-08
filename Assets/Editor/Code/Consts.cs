namespace Thisaislan.PersistenceEasyToDelete.Editor.Constants
{
    internal static class Consts
    {
        internal const string DebugMessageSuffix = "Ped ->";
        internal const string NewItemLogMessage = "New PedData file was created";
        internal const string MultipleActivePedDataWarningMessage =
            "More than one PedData is marked in use. Only the oldest one will be kept, " +
            "the others were unmarked and saved.";
        internal const string PedDataDuplicateActiveMessage =
            "Another PedData is already marked in use. If you want to use this one instead, " +
            "click the button below and the other will be unmarked.";
        internal const string IsActivePropertyName = "isActive";
        internal const string AvoidChangesPropertyName = "avoidChanges";
        internal const string PlayerPrefsPropertyName = "playerPrefData";
        internal const string FileDataPropertyName = "fileData";
        internal const string KeyPropertyName = "key";
        internal const string TypePropertyName = "type";
        internal const string ValuePropertyName = "value";
        internal const string TreeEditorTrashIcon = "d_TreeEditor.Trash";
        internal const string RowIndexLabelSuffix = " - ";
        internal const string NewLineChar = "\n";

        internal const string PedDataActiveDataTooltipAttr = "Set this flag to true if you want this PedData to be " +
                                                              "the one used by Ped. Setting this flag on a PedData " +
                                                              "will automatically uncheck it on the other PedData.";
        internal const string PedDataAvoidChangesTooltipAttr = "Set this flag to true if you want Ped to try to " +
                                                              "avoid any changes to this PedData when running the " +
                                                              "editor. Ped will create a backup of the data when " +
                                                              "the editor enters play mode and will set the data " +
                                                              "back when play mode stops.";

        internal const string PedDataActiveButtonLabel = "Use this PedData";
        internal const string PedDataActiveDisabledButtonLabel = "This PedData is in use";
        internal const string PedDataStatusCardLabel = "Status";
        internal const string PedDataAvoidChangesButtonLabel = "Block changes while the scene is running";
        internal const string PedDataAvoidAllowButtonLabel = "Allow changes while the scene is running";
        internal const string PedDataAvoidChangesToggledTooltip = "Click to switch between blocking and allowing runtime changes to this PedData.";
        internal const string PedDataPlayerPrefsSectionLabel = "Player Prefs Data";
        internal const string PedDataFileSectionLabel = "File Data";
        internal const string PedValidationSectionLabel = "Validation";
        internal const string PedValidationButtonLabel = "Validate Data";
        internal const string PedValidationRunningMessage = "Validation is only available when the scene is not running.";
        internal const string PedValidationSuccessMessage = "The PedData is valid.";
        internal const string PedValidationCleanButtonLabel = "Clean results";
        internal const string PedDataKeyColumnLabel = "Key";
        internal const string PedDataTypeColumnLabel = "Type";
        internal const string PedDataDeleteEntryButtonTooltip = "Delete this entry";
        internal const string PedDataDeleteAllEntriesButtonTooltip = "Delete all entries from this list";
        internal const string PedDataAddEntryButtonTooltip = "Add a new entry to this list";
        internal const string PedDataAddEntryButtonLabel = "Add entry";
        internal const string PedDataEmptyListMessage = "No entries yet.";
        internal const string PedDataClearAllButtonLabel = "Clear all data";
        internal const string PedDataClearAllDialogTitle = "CLEAR ALL DATA";
        internal const string PedDataClearAllDialogMessage = "All entries of both lists in this PedData will be erased. " +
                                                              "This action cannot be undone. Are you sure?";
        internal const string PedDataDeleteAllDialogTitle = "DELETE ALL ENTRIES";
        internal const string PedDataDeleteAllDialogMessage = "All entries from this list will be erased. " +
                                                              "This action cannot be undone. Are you sure?";
        internal const string DialogOkButton = "Yes";
        internal const string DialogCancelButton = "No";
        internal const string PedDataPlayerPrefsTooltipAttr = "This field will save all data that will be stored as" +
                                                              " PlayerPrefs when the game is in runtime.";
        internal const string PedDataFileToolTipAttr = "This field will save all data that will be stored as" +
                                                       " File when the game is in runtime.";
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
        internal const string UnableToValidateSerializedValueLogMessage =
            "{0}: unable to validate the serialization of the value with key '{1}' and type '{2}' " +
            "on the validation attempt, because the serialized value is not recognized as JSON.";
    }
}
