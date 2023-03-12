using System;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas
{
    internal static class Metadata
    {
        
        internal const int AssetMenuDataOrder = 1;
        
        internal const int PreprocessCallbackOrder = 0;
        
        internal const int MenuItemNewDataPriority = 1;
        internal const int MenuItemOpenDataOptionPriority = 12;
        internal const int MenuItemValidateDataOptionPriority = 13;
        internal const int MenuItemOpenSettingsPriority = 14;
        internal const int MenuItemDeleteDataPriority = 15;
        
        internal const int TextAreaDataMaxLines = 10;
        internal const int TextAreaDataMinLines = 4;
        
        internal const string AssemblyNameInternalsVisibleTo = "Thisaislan.PersistenceEasyToDeleteInEditor.Pede";
        
        internal const string AssetMenuDataName = "Pede/PedeData";
        
        internal const string DataFileName = "PedeData";
        internal const string DataFilExtension = "asset";
        internal const string DataFolderPath = "Assets/Pede";
        internal const string DataFullFileName = "PedeData.asset";
        internal const string DataOldFilePrefix = "Old";
        
        internal const string MenuItemDeleteData = "Tools/Pede/Delete PedeData";
        internal const string MenuItemNewData = "Tools/Pede/New PedeData";
        internal const string MenuItemOpenData = "Tools/Pede/Open PedeData";
        internal const string MenuItemValidateData = "Tools/Pede/Validate PedeData";
        internal const string MenuItemOpenSettings = "Tools/Pede/Open PedeSettings";
        internal const string MenuItemOpenSettingsShortcut = " #F10";
        internal const string MenuItemValidateDataShortcut = " #F11";
        internal const string MenuItemOpenDataShortcut = " #F12";
        
        internal const int DefaultFieldDataTopSpace = 14;
        internal const int SettingsFirstFieldTopSpace = 15;
        internal const int SettingsFieldTopSpace = 30;

        internal const string SettingFolderPath = "Assets/Settings";
        internal const string SettingFullFileName = "PedeSettings.asset";
        internal const string SettingsFileName = "PedeSettings";
        
        internal const string SerializerSerializeMethodName = "Serialize";
        internal const string SerializerDeserializeMethodName = "Deserialize";
        internal const string SerializerInterfaceName = nameof(IPedeSerializer);

        internal static Type[] BuildInTypes =
        {
            typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float),
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(string)
        };
        
    }
}
