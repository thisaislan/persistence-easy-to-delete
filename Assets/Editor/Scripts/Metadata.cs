using System;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.Editor.Metas
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
        
        internal const string AssemblyNameInternalsVisibleTo = "Thisaislan.PersistenceEasyToDelete.Ped";
        
        internal const string AssetMenuDataName = "Ped/PedData";
        
        internal const string DataFileName = "PedData";
        internal const string DataFilExtension = "asset";
        internal const string DataFolderPath = "Assets/Ped";
        internal const string DataFullFileName = "PedData.asset";
        internal const string DataOldFilePrefix = "Old";
        
        internal const string MenuItemDeleteData = "Tools/Ped/Delete PedData";
        internal const string MenuItemNewData = "Tools/Ped/New PedData";
        internal const string MenuItemOpenData = "Tools/Ped/Open PedData";
        internal const string MenuItemValidateData = "Tools/Ped/Validate PedData";
        internal const string MenuItemOpenSettings = "Tools/Ped/Open PedSettings";
        internal const string MenuItemOpenSettingsShortcut = " #F10";
        internal const string MenuItemValidateDataShortcut = " #F11";
        internal const string MenuItemOpenDataShortcut = " #F12";
        
        internal const int DefaultFieldDataTopSpace = 14;
        internal const int SettingsFirstFieldTopSpace = 15;
        internal const int SettingsFieldTopSpace = 30;

        internal const string SettingFolderPath = "Assets/Settings";
        internal const string SettingFullFileName = "PedSettings.asset";
        internal const string SettingsFileName = "PedSettings";
        
        internal const string SerializerSerializeMethodName = "Serialize";
        internal const string SerializerDeserializeMethodName = "Deserialize";
        internal const string SerializerInterfaceName = nameof(IPedSerializer);

        internal const string PackageFolderName = "persistenceeasytodelete";

        internal static Type[] BuildInTypes =
        {
            typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float),
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(string)
        };
        
    }
}
