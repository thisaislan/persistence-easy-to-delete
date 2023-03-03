namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants
{
    
    internal static class Metadata
    {
        internal const int AssetMenuDataOrder = 1;
        
        internal const int MenuItemDeleteDataPriority = 13;
        internal const int MenuItemNewDataPriority = 1;
        internal const int MenuItemOpenDataOptionPriority = 12;
        
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
        internal const string MenuItemOpenDataShortcut = " #F12";
        
        internal const string MenuItemDeleteDialogTitle = "DELETE";
        internal const string MenuItemDeleteDialogOkButton = "Yes";
        internal const string MenuItemDeleteDialogCancelButton = "No";
        internal const string MenuItemDeleteDialogMessage = "The current PedeData file will be deleted and this " +
                                                            "action cannot be undone. Are you sure about this?";
        
        internal const string SettingFolderPath = "Assets/Settings";
        internal const string SettingFullFileName = "PedeSettings.asset";
        internal const string SettingsFileName = "PedeSettings";
    }
    
}
