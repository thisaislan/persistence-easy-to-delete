using Thisaislan.PersistenceEasyToDelete.PedSerialize.ScriptableObjects;
using Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo(Metadata.AssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    [InitializeOnLoad]
    internal static class PedEditor
    {
        
        private static PedSettings pedSettings;
        private static PedData pedData => pedSettings.pedData;

        #region StartRegion
        
        static PedEditor()
        {                        
            EditorApplication.playModeStateChanged -= EditorChangeStageEvent;
            EditorApplication.playModeStateChanged += EditorChangeStageEvent;
        }

        internal static void CheckInitialization()
        {
            CheckFileSettings();
            CheckFileData();
        }


        [InitializeOnEnterPlayMode]
        internal static void CheckCustomSerializer()
        {
            if (IsDataFileAccessible())
            {
                if (pedSettings.WasCustomSerializerChanged())
                {
                    if (pedSettings.HasCustomSerializerFile())
                    {
                        pedSettings.ResetCustomSerializerChangeFile();

                        PedSerializeSettings.instance.SetAssemblyData(
                            new PedSerializeSettings.CustomSerializerData(
                                pedSettings.GetCustomSerializerClassName(),
                                pedSettings.GetCustomSerializerAssemblyName()
                            )
                        );
                    }
                    else
                    {
                        PedSerializeSettings.instance.CleanAssemblyData();
                    }
                
                    pedSettings.CleanCustomSerializerChangeFlag();
                    PersistAsset(PedSerializeSettings.instance);
                }
            }
        }
        
        [InitializeOnEnterPlayMode]
        internal static void DataInitialization()
        {
            if (IsDataFileAccessible())
            {
                pedData.SetNewRunInfo();
            }
        }
        
        [InitializeOnLoadMethod]
        private static void PedEditorInitializer()
        {
            if (!SessionState.GetBool(Consts.SessionStartFlag, false))
            {
                ResetDataFlags();
                
                SessionState.SetBool(Consts.SessionStartFlag, true);
            }
            
            SessionState.SetBool(Consts.SessionOnScriptReloadFlag, true);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        internal static void PedEditorDidReloadScripts()
        {
            SessionState.SetBool(Consts.SessionOnScriptReloadFlag, false);
        }

        private static void CheckDataBackupInitialization()
        {
            if (pedData != null && pedData.ShouldAvoidChanges() && !Application.isPlaying)
            {
                pedData.CreateBackup();
            }
        }

        private static void EditorChangeStageEvent(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                CheckInitialization();
                CheckDataBackupInitialization();

                if (pedData != null)
                {
                    if (pedData.ShouldAvoidChanges())
                    {
                        pedData.SetBackup();
                    }
                    pedData.CleanBackup();

                    PersistAsset(pedData);
                }

            }
        }

        #endregion //StartRegion
        
        #region SetRegion
        
        private static void CheckFileSettings()
        {
            var settingsPath = $"{Metadata.SettingFolderPath}/{Metadata.SettingFullFileName}";
          
            pedSettings = GetFile<PedSettings>(settingsPath);

            if (pedSettings == null)
            {
                pedSettings = CreateSettingsScriptableObjectAsset(Metadata.SettingFolderPath, settingsPath);
                PersistAsset(pedSettings);
            }

            pedSettings.ResetCustomSerializerChangeFile();
        }

        private static PedSettings CreateSettingsScriptableObjectAsset(string folderPath, string settingsPath) =>
            CreateScriptableObjectAsset<PedSettings>(folderPath, settingsPath);
        
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

            if (pedSettings.pedData == null)
            {
                var pedData = GetFileData(defaultDataPath);
                
                if (pedData == null)
                {
                    pedData = CreateDataScriptableObjectAsset(Metadata.DataFolderPath, defaultDataPath);
                }
                pedSettings.pedData = pedData; 

                PersistAsset(pedSettings);
            }
        }

        private static void InvalidateFileData(string defaultDataPath)
        {
            pedSettings.pedData = null;
            
            if (GetFileData(defaultDataPath) != null)
            {
                RenameFileDataWithDefaultName(defaultDataPath);
            }
        }

        private static PedData GetFileData(string dataPath) =>
            GetFile<PedData>(dataPath);

        private static PedData CreateDataScriptableObjectAsset(string directoryPath, string filePath)
        {
            var data = CreateScriptableObjectAsset<PedData>(directoryPath, filePath);
            
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

            PersistAsset(GetFile<PedData>($"{Metadata.DataFolderPath}/{name}"));
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
        
        internal static void SetPlayerPrefs<T>(string key, T value, IPedSerializer serializer) =>
            pedData.SetPlayerPrefs(key, value, serializer);

        internal static void GetPlayerPrefs<T>(
                string key,
                Action<T> actionIfHasResult,
                Action actionIfHasNotResult,
                IPedSerializer serializer,                
                bool destroyAfter
            ) =>
            pedData.GetPlayerPrefs(key, actionIfHasResult, serializer, actionIfHasNotResult, destroyAfter);

        internal static void DeletePlayerPrefs<T>(string key) =>
            pedData.DeletePlayerPrefs<T>(key);

        internal static void DeleteAllPlayerPrefs() =>
            pedData.DeleteAllPlayerPrefs();

        internal static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult) =>
            pedData.HasPlayerPrefsKey<T>(key, actionWithResult);

        #endregion //PlayerPrefsRegion
        
        #region FileRegion

        internal static void SetFile<T>(string key, T value, IPedSerializer serializer) =>
            pedData.SetFile(key, value, serializer);

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedSerializer serializer,
            bool destroyAfter) =>
            pedData.GetFile(key, actionIfHasResult, serializer, actionIfHasNotResult, destroyAfter);

        internal static void DeleteFile<T>(string key) =>
            pedData.DeleteFile<T>(key);

        internal static void DeleteAllFiles() =>
            pedData.DeleteAllFiles();

        internal static void HasFileKey<T>(string key, Action<bool> actionWithResult) =>
            pedData.HasFileKey<T>(key, actionWithResult);

        #endregion //FileRegion

        #region UtilsRegion
        
        internal static void DeleteAll() =>
            pedData.DeleteAll();

        internal static void CreateAnotherDataFile()
        {
            CheckFileData(true);
            pedData.ResetDataChanged();
        }

        internal static void SelectDataFile()
        {
            if (IsDataFileAccessible()) { Selection.activeObject = pedSettings.pedData; }
        }
        
        internal static void SelectSettingsFile()
        {
            if (IsDataFileAccessible()) { Selection.activeObject = pedSettings; }
        }

        internal static bool IsDataFileAccessible()
        {
            if (pedSettings != null)
            {
                return pedSettings.pedData != null &&
                       pedSettings.name == Metadata.SettingsFileName;
                
            }
            else
            {
                CheckInitialization();
                
                return pedSettings.pedData != null;
            }
        }

        internal static void DeleteDataFile()
        {
            if (IsDataFileAccessible())
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(pedSettings.pedData));
                CreateAnotherDataFile();
            }
        }

        internal static bool IsDataValid(PedData.ValidationDataErrorHandler validationDataErrorHandler)
        {
            CheckCustomSerializer();
            
            return IsDataFileAccessible() &&
                pedData.IsDataValid(validationDataErrorHandler, pedSettings.GetCustomSerializer());
        }

        internal static bool HasCustomSerializerFile() =>
            pedSettings.HasCustomSerializerFile();
        
        internal static bool IsCustomSerializerFileValid(
            PedSettings.ValidationSerializerErrorHandler validationSerializerErrorHandler) => 
            pedSettings.IsCustomSerializerFileValid(validationSerializerErrorHandler);
        
        internal static bool ShouldVerifyDataOnRunStart() =>
            IsDataFileAccessible() && pedSettings.ShouldVerifyDataOnRunStart();

        internal static bool ShouldRunAValidation()
        {
            if (pedSettings != null && pedData != null)
            {
                if (pedSettings.WasCustomSerializerChanged())
                {
                    return true;
                }

                return pedSettings.WasDataChanged() || pedData.WasDataChanged();
            }
            
            return false;
        }

        internal static void CleanDataChangFlag()
        {
            if (pedSettings != null && pedData != null)
            {
                pedSettings.CleanDataChangeFlag();
                pedData.CleanDataChangeFlag();
            }
        }
        
        internal static void ResetDataFlags()
        {
            if (pedSettings != null && pedData != null)
            {
                pedSettings.ResetDataFlag();
                pedData.ResetDataFlag();
            }
        }

        internal static void PersistAsset(Object scriptableObject)
        {

            if (EditorUtility.IsDirty(scriptableObject))
            {
                EditorUtility.SetDirty(scriptableObject);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion //UtilsRegion

    }
}