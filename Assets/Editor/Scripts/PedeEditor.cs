using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Settings;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo(Metadata.AssemblyName)]
namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor
{
    internal static class PedeEditor
    {
        
        private static PedeSettings pedeSettings;
        internal static PedeData pedeData => pedeSettings.pedeData;

        static PedeEditor()
        {
            if (Application.isPlaying)
            {
                CheckFileSettings();
                CheckFileData();
            }
        }

        #region SetRegion
        
        private static void CheckFileSettings()
        {
            var settingsPath = $"{Metadata.SettingFolderPath}/{Metadata.SettingFullFileName}";
          
            pedeSettings = GetFile<PedeSettings>(settingsPath);

            if (pedeSettings == null)
            {
                pedeSettings = CreateSettingsScriptableObjectAsset(Metadata.SettingFolderPath, settingsPath);
                PersistAsset(pedeSettings);
            }
        }

        private static void PersistAsset(Object scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static PedeSettings CreateSettingsScriptableObjectAsset(string folderPath, string settingsPath) =>
            CreateScriptableObjectAsset<PedeSettings>(folderPath, settingsPath);
        
        private static T CreateScriptableObjectAsset<T>(string directoryPath, string filePath) 
            where T : ScriptableObject
        {
            var scriptableObject = ScriptableObject.CreateInstance<T>();
            
            Directory.CreateDirectory(directoryPath);
            
            CreatAsset(scriptableObject, filePath);
            PersistAsset(scriptableObject);

            return scriptableObject;
        }
        
        private static void CreatAsset(Object asset, string path) =>
            AssetDatabase.CreateAsset(asset, path);

        private static void CheckFileData(bool isASubstitution = false)
        {
            var defaultDataPath = $"{Metadata.DataFolderPath}/{Metadata.DataFullFileName}";

            if (isASubstitution)
            {
                CheckFileSettings();
                InvalidateFileData(defaultDataPath);
            }

            if (pedeSettings.pedeData == null)
            {
                var pedeData = GetFileData(defaultDataPath);
                
                if (pedeData == null)
                {
                    pedeData = CreateDataScriptableObjectAsset(Metadata.DataFolderPath, defaultDataPath);
                }
                
                pedeSettings.pedeData = pedeData;
                
                PersistAsset(pedeSettings);
            }
        }

        private static void InvalidateFileData(string defaultDataPath)
        {
            pedeSettings.pedeData = null;
            
            if (GetFileData(defaultDataPath) != null)
            {
                RenameFileDataWithDefaultName(defaultDataPath);
            }
        }

        private static PedeData GetFileData(string dataPath) =>
            GetFile<PedeData>(dataPath);

        private static PedeData CreateDataScriptableObjectAsset(string directoryPath, string filePath)
        {
            var data = CreateScriptableObjectAsset<PedeData>(directoryPath, filePath);
            
            PersistAsset(data);
            
            return data;
        }

        private static void RenameFileDataWithDefaultName(string defaultDataPath)
        {
            var name = "";
            var result = "";
            var index = 1;

            do
            {
                name = GetWantedName(index);
                result = AssetDatabase.RenameAsset(defaultDataPath, name);
                index++;
                
            } while (!string.IsNullOrEmpty(result));

            PersistAsset(GetFile<PedeData>($"{Metadata.DataFolderPath}/{name}"));
        }

        private static string GetWantedName(int index) =>
            $"{Metadata.DataOldFilePrefix}-" +
            $"{Metadata.DataFileName}" +
            $"({index})." +
            $"{Metadata.DataFilExtension}";

        private static T GetFile<T>(string path) where T : Object =>
            AssetDatabase.LoadAssetAtPath<T>(path);

        #endregion //SetRegion
        
        #region PlayerPrefsRegion
        
        internal static void SetPlayerPrefs<T>(string key, T value) =>
            pedeData.SetPlayerPrefs(key, value);

        internal static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            bool destroyAfter) =>
            pedeData.GetPlayerPrefs(key, actionIfHasResult, actionIfHasNotResult, destroyAfter);

        internal static void DeletePlayerPrefs<T>(string key) =>
            pedeData.DeletePlayerPrefs<T>(key);

        internal static void DeleteAllPlayerPrefs() =>
            pedeData.DeleteAllPlayerPrefs();

        internal static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult) =>
            pedeData.HasPlayerPrefsKey<T>(key, actionWithResult);

        #endregion //PlayerPrefsRegion
        
        #region FileRegion

        internal static void SetFile<T>(string key, T value) =>
            pedeData.SetFile(key, value);

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            bool destroyAfter) =>
            pedeData.GetFile(key, actionIfHasResult, actionIfHasNotResult, destroyAfter);

        internal static void DeleteFile<T>(string key) =>
            pedeData.DeleteFile<T>(key);

        internal static void DeleteAllFiles() =>
            pedeData.DeleteAllFiles();

        internal static void HasFileKey<T>(string key, Action<bool> actionWithResult) =>
            pedeData.HasFileKey<T>(key, actionWithResult);

        #endregion //FileRegion

        #region UtilsRegion
        
        internal static void DeleteAll() =>
            pedeData.DeleteAll();
        
        internal static void CreateAnotherDataFile() =>
            CheckFileData(true);
        
        internal static void SelectDataFile()
        {
            if (IsDataFileAccessible()) { Selection.activeObject = pedeSettings.pedeData; }
        }

        internal static bool IsDataFileAccessible()
        {
            if (pedeSettings != null)
            {
                return pedeSettings.pedeData != null &&
                       pedeSettings.name == Metadata.SettingsFileName;
                
            }
            else
            {
                CheckFileSettings();
                
                return pedeSettings.pedeData != null;
            }
        }

        internal static void DeleteDataFile()
        {
            if (IsDataFileAccessible())
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(pedeSettings.pedeData));
                CreateAnotherDataFile();
            }
        }

        internal static bool IsDataValid(PedeData.ValidationErrorHandler validationErrorHandler) =>
            IsDataFileAccessible() && pedeData.IsDataValid(validationErrorHandler);

        #endregion //UtilsRegion

    }
}