using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.ScriptableObjects;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using System.Runtime.CompilerServices;
using System;
using System.IO;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo(Metadata.AssemblyNameInternalsVisibleTo)]
namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor
{
    [InitializeOnLoad]
    internal static class PedeEditor
    {
        
        private static PedeSettings pedeSettings;
        private static PedeData pedeData => pedeSettings.pedeData;

        #region StartRegion
        
        static PedeEditor()
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
                if (pedeSettings.WasCustomSerializerChanged())
                {
                    if (pedeSettings.HasCustomSerializerFile())
                    {
                        pedeSettings.ResetCustomSerializerChangeFile();

                        PedeSerializeSettings.instance.SetAssemblyData(
                            new PedeSerializeSettings.CustomSerializerData(
                                pedeSettings.GetCustomSerializerClassName(),
                                pedeSettings.GetCustomSerializerAssemblyName()
                            )
                        );
                    }
                    else
                    {
                        PedeSerializeSettings.instance.CleanAssemblyData();
                    }
                
                    pedeSettings.CleanCustomSerializerChangeFlag();
                    PersistAsset(PedeSerializeSettings.instance);
                }
            }
        }
        
        [InitializeOnEnterPlayMode]
        internal static void DataInitialization()
        {
            if (IsDataFileAccessible())
            {
                pedeData.SetNewRunInfo();
            }
        }
        
        [InitializeOnLoadMethod]
        private static void PedeEditorInitializer()
        {
            if (!SessionState.GetBool(Consts.SessionStartFlag, false))
            {
                ResetDataFlags();
                
                SessionState.SetBool(Consts.SessionStartFlag, true);
            }
            
            SessionState.SetBool(Consts.SessionOnScriptReloadFlag, true);
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        internal static void PedeEditorDidReloadScripts()
        {
            SessionState.SetBool(Consts.SessionOnScriptReloadFlag, false);
        }

        private static void CheckDataBackupInitialization()
        {
            if (pedeData != null && pedeData.ShouldAvoidChanges() && !Application.isPlaying)
            {
                pedeData.CreateBackup();
            }
        }

        private static void EditorChangeStageEvent(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                CheckInitialization();
                CheckDataBackupInitialization();

                if (pedeData != null)
                {
                    if (pedeData.ShouldAvoidChanges())
                    {
                        pedeData.SetBackup();
                    }
                    pedeData.CleanBackup();

                    PersistAsset(pedeData);
                }

            }
        }

        #endregion //StartRegion
        
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

            pedeSettings.ResetCustomSerializerChangeFile();
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
        
        internal static void SetPlayerPrefs<T>(string key, T value, IPedeSerializer serializer) =>
            pedeData.SetPlayerPrefs(key, value, serializer);

        internal static void GetPlayerPrefs<T>(
                string key,
                Action<T> actionIfHasResult,
                Action actionIfHasNotResult,
                IPedeSerializer serializer,                
                bool destroyAfter
            ) =>
            pedeData.GetPlayerPrefs(key, actionIfHasResult, serializer, actionIfHasNotResult, destroyAfter);

        internal static void DeletePlayerPrefs<T>(string key) =>
            pedeData.DeletePlayerPrefs<T>(key);

        internal static void DeleteAllPlayerPrefs() =>
            pedeData.DeleteAllPlayerPrefs();

        internal static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult) =>
            pedeData.HasPlayerPrefsKey<T>(key, actionWithResult);

        #endregion //PlayerPrefsRegion
        
        #region FileRegion

        internal static void SetFile<T>(string key, T value, IPedeSerializer serializer) =>
            pedeData.SetFile(key, value, serializer);

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedeSerializer serializer,
            bool destroyAfter) =>
            pedeData.GetFile(key, actionIfHasResult, serializer, actionIfHasNotResult, destroyAfter);

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

        internal static void CreateAnotherDataFile()
        {
            CheckFileData(true);
            pedeData.ResetDataChanged();
        }

        internal static void SelectDataFile()
        {
            if (IsDataFileAccessible()) { Selection.activeObject = pedeSettings.pedeData; }
        }
        
        internal static void SelectSettingsFile()
        {
            if (IsDataFileAccessible()) { Selection.activeObject = pedeSettings; }
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
                CheckInitialization();
                
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

        internal static bool IsDataValid(PedeData.ValidationDataErrorHandler validationDataErrorHandler)
        {
            CheckCustomSerializer();
            
            return IsDataFileAccessible() &&
                pedeData.IsDataValid(validationDataErrorHandler, pedeSettings.GetCustomSerializer());
        }

        internal static bool HasCustomSerializerFile() =>
            pedeSettings.HasCustomSerializerFile();
        
        internal static bool IsCustomSerializerFileValid(
            PedeSettings.ValidationSerializerErrorHandler validationSerializerErrorHandler) => 
            pedeSettings.IsCustomSerializerFileValid(validationSerializerErrorHandler);
        
        internal static bool ShouldVerifyDataOnRunStart() =>
            IsDataFileAccessible() && pedeSettings.ShouldVerifyDataOnRunStart();

        internal static bool ShouldRunAValidation()
        {
            if (pedeSettings != null && pedeData != null)
            {
                if (pedeSettings.WasCustomSerializerChanged())
                {
                    return true;
                }

                return pedeSettings.WasDataChanged() || pedeData.WasDataChanged();
            }
            
            return false;
        }

        internal static void CleanDataChangFlag()
        {
            if (pedeSettings != null && pedeData != null)
            {
                pedeSettings.CleanDataChangeFlag();
                pedeData.CleanDataChangeFlag();
            }
        }
        
        internal static void ResetDataFlags()
        {
            if (pedeSettings != null && pedeData != null)
            {
                pedeSettings.ResetDataFlag();
                pedeData.ResetDataFlag();
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