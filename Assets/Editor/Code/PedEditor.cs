using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    internal static class PedEditor
    {
        private static PedData activePedData;

        [InitializeOnLoadMethod]
        private static void PedEditorInitializer()
        {
            EditorApplication.playModeStateChanged -= EditorChangeStageEvent;
            EditorApplication.playModeStateChanged += EditorChangeStageEvent;
        }

        internal static void CheckInitialization()
        {
            GetActivePedData();
        }

        internal static PedData GetDataForStorage() =>
            GetActivePedData();

        internal static void SetActivePedData(PedData pedData)
        {
            if (pedData == null)
            {
                return;
            }

            List<PedData> allPedData = GetAllPedData().ToList();

            bool hasChanged = false;

            if (!pedData.IsActivePed())
            {
                pedData.SetActivePed(true);
                hasChanged = true;
            }

            for (int index = 0; index < allPedData.Count; index++)
            {
                PedData data = allPedData[index];

                if (data != pedData && data.IsActivePed())
                {
                    data.SetActivePed(false);
                    hasChanged = true;
                }
            }

            activePedData = pedData;

            if (hasChanged)
            {
                PersistAssets(allPedData);
            }
        }

        internal static void PersistAssets(List<PedData> allPedData)
        {
            for (int index = 0; index < allPedData.Count; index++)
            {
                EditorUtility.SetDirty(allPedData[index]);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        internal static void CreateAnotherDataFile()
        {
            PedData previousActiveData = GetActivePedData();

            PedData newData = CreateNewDataFile();

            if (previousActiveData != null)
            {
                previousActiveData.SetActivePed(false);
                PersistAsset(previousActiveData);
            }

            newData.SetActivePed(true);
            PersistAsset(newData);

            activePedData = newData;

            Selection.activeObject = newData;

            EditorUtility.FocusProjectWindow();
        }

        internal static void SelectDataFile()
        {
            PedData data = GetActivePedData();

            if (data != null)
            {
                Selection.activeObject = data;
                EditorUtility.FocusProjectWindow();
            }
        }

        internal static bool IsDataFileAccessible() =>
            GetActivePedData() != null;

        internal static void PersistAsset(Object scriptableObject)
        {
            EditorUtility.SetDirty(scriptableObject);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static PedData GetActivePedData()
        {
            if (activePedData == null || !activePedData.IsActivePed())
            {
                activePedData = FindActivePedData();

                if (activePedData == null)
                {
                    activePedData = GetOrCreateDefaultPedData();
                }
            }

            return activePedData;
        }

        private static PedData FindActivePedData()
        {
            List<PedData> allPedData = GetAllPedData().ToList();

            List<PedData> activePedData = allPedData
                .Where(ped => ped.IsActivePed())
                .ToList();

            if (activePedData.Count == 0)
            {
                return null;
            }

            if (activePedData.Count > 1)
            {
                return ResolveMultipleActivePedData(activePedData, allPedData);
            }

            return activePedData[0];
        }

        private static PedData ResolveMultipleActivePedData(
            List<PedData> activePedData,
            List<PedData> allPedData
        )
        {
            PedData pedDataToKeepActive = activePedData
                .OrderBy(GetFileCreationTime)
                .First();

            for (int index = 0; index < activePedData.Count; index++)
            {
                if (activePedData[index] != pedDataToKeepActive)
                {
                    activePedData[index].SetActivePed(false);
                }
            }

            PersistAssets(allPedData);

            Debug.LogWarning(
                $"{Consts.DebugMessageSuffix} {Consts.MultipleActivePedDataWarningMessage}"
            );

            return pedDataToKeepActive;
        }

        private static DateTime GetFileCreationTime(PedData pedData) =>
            File.GetCreationTime(AssetDatabase.GetAssetPath(pedData));

        internal static bool IsAnotherPedDataActive(PedData pedData) =>
            GetAllPedData().Any(ped => ped != pedData && ped.IsActivePed());

        internal static IEnumerable<PedData> GetAllPedData() =>
            AssetDatabase.FindAssets($"t:{nameof(PedData)}")
                .Select(guid => GetFile<PedData>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(ped => ped != null)
                .OrderBy(ped => AssetDatabase.GetAssetPath(ped));

        private static PedData GetOrCreateDefaultPedData()
        {
            string defaultDataPath = $"{Metadata.DataFolderPath}/{Metadata.DataFullFileName}";

            PedData defaultData = GetFileData(defaultDataPath);

            if (defaultData == null)
            {
                defaultData = CreateDataScriptableObjectAsset(Metadata.DataFolderPath, defaultDataPath);
            }

            defaultData.SetActivePed(true);
            PersistAsset(defaultData);

            return defaultData;
        }

        private static PedData CreateNewDataFile()
        {
            string dataPath = GetNextAvailableDataPath();

            return CreateDataScriptableObjectAsset(Metadata.DataFolderPath, dataPath);
        }

        private static string GetNextAvailableDataPath()
        {
            int index = 1;
            string candidatePath = GetDataFilePath(Metadata.DataFileName, index, useIndex: false);

            while (GetFileData(candidatePath) != null)
            {
                candidatePath = GetDataFilePath(Metadata.DataFileName, index, useIndex: true);
                index++;
            }

            return candidatePath;
        }

        private static string GetDataFilePath(string fileName, int index, bool useIndex)
        {
            string filename = useIndex
                ? string.Format(Metadata.IndexedDataFileNameFormat, fileName, index)
                : fileName + Metadata.DataFileExtension;

            return $"{Metadata.DataFolderPath}/{filename}";
        }

        private static PedData GetFileData(string dataPath) =>
            GetFile<PedData>(dataPath);

        private static T GetFile<T>(string path) where T : Object =>
            AssetDatabase.LoadAssetAtPath<T>(path);

        private static T CreateScriptableObjectAsset<T>(string directoryPath, string filePath)
            where T : ScriptableObject
        {
            T scriptableObject = ScriptableObject.CreateInstance<T>();

            Directory.CreateDirectory(directoryPath);

            AssetDatabase.CreateAsset(scriptableObject, filePath);
            PersistAsset(scriptableObject);

            return scriptableObject;
        }

        private static PedData CreateDataScriptableObjectAsset(string directoryPath, string filePath) =>
            CreateScriptableObjectAsset<PedData>(directoryPath, filePath);

        private static void EditorChangeStageEvent(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
            {
                CheckInitialization();
            }
            else if (playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                PedData pedData = GetActivePedData();

                CheckDataBackupInitialization(pedData);

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

        private static void CheckDataBackupInitialization(PedData pedData)
        {
            if (pedData != null && pedData.ShouldAvoidChanges() && !Application.isPlaying)
            {
                pedData.CreateBackup();
            }
        }

    }
}