using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.PropertyAttributes;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeSerialize.Interfaces;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects
{
    [
        CreateAssetMenu(
            fileName = Metadata.DataFileName,
            menuName = Metadata.AssetMenuDataName,
            order = Metadata.AssetMenuDataOrder
        )
    ]
    [ClassTooltip(Consts.PedeDataClassTipAttr)]
    internal class PedeData : ScriptableObject
    {

        [Serializable]
        internal struct Data
        {
            [SerializeField]
            internal string key;
            
            [SerializeField]
            internal string type;

            [SerializeField]
            [TextArea(Metadata.TextAreaDataMinLines, Metadata.TextAreaDataMaxLines)]
            internal string value;

            internal bool IsKeyNull() =>
                key == null;
            
            internal bool IsSameValue(string key, string type) =>
                this.key.Equals(key) && this.type.Equals(type);
        }

        [SerializeField]
        [Header(Consts.PedeDataSettingsHeaderAttr)]
        [Tooltip(Consts.PedeDataAvoidChangesTooltipAttr)]
        [Space]
        private bool avoidChanges;
        
        [SerializeField]
        [Space(Metadata.DefaultFieldDataTopSpace)]
        [Tooltip(Consts.PedeDataPlayerPrefsTooltipAttr)]
        [Header(Consts.PedeDataHeaderAttr)]
        [Space]
        internal List<Data> playerPrefData = new List<Data>();
        
        [SerializeField]
        [Space(Metadata.DefaultFieldDataTopSpace)]
        [Tooltip(Consts.PedeDataFileToolTipAttr)]
        internal List<Data> fileData = new List<Data>();

        [SerializeField]
        [Space(Metadata.DefaultFieldDataTopSpace)]
        [Tooltip(Consts.PedeDataFileInfoAttr)]
        private Info info = new Info();

        private List<Data> playerPrefDataBackup;
        
        private List<Data> fileDataBackup;
        
        private bool wasDataChanged = true;
        
        private bool avoidChangesBackup;

        #region OverrideRegion
        
        private void OnEnable()
        {
            if (!IsOnScriptReload())
            {
                SetFixedData();
            }
        }

        private void OnValidate()
        {
            if (!IsOnScriptReload() && IsAvoidChangesFlagsTheSame())
            {
                DataHasChanged();
            }
        }

        #endregion //OverrideRegion

        #region PlayerPrefsRegion

        internal void SetPlayerPrefs<T>(string key, T value, IPedeSerializer serializer)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
            var data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                var index = playerPrefData.IndexOf(data);
                
                playerPrefData.Remove(data);
                playerPrefData.Insert(index, CreatePlayerPrefsData(key, value, serializer));
            }
            else
            {
                playerPrefData.Add(CreatePlayerPrefsData(key, value, serializer));
            }
            info.UpdatePlayerPrefsNumberOfSetsInTheLastRun();
            info.UpdateSize(this);
            PersistAsset();
        }

        internal void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            IPedeSerializer serializer,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false
        )
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
            var data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetPlayerPrefsData(data.value,  actionIfHasResult, serializer);
                
                if (destroyAfter) { DeletePlayerPrefs<T>(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
            
            info.UpdatePlayerPrefsNumberOfGetsInTheLastRun();
            PersistAsset();
        }

        internal void DeletePlayerPrefs<T>(string key)
        {
            RemovePlayerPrefsData<T>(key);
            info.UpdatePlayerPrefsNumberOfDeletesInTheLastRun();
            info.UpdateSize(this);
            PersistAsset();
        }
        
        internal void DeleteAllPlayerPrefs()
        {
            info.UpdatePlayerPrefsNumberOfDeletesInTheLastRun(playerPrefData.Count);
            playerPrefData.Clear();
            PersistAsset();
        }

        internal void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
            actionWithResult.Invoke(ExistsData(playerPrefData, key, GetTypeName(typeof(T))));
            
            info.UpdatePlayerPrefsNumberOfHasKeyInTheLastRun();
            PersistAsset();
        }

        private Data CreatePlayerPrefsData<T>(string key, T value, IPedeSerializer serializer) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value = GetPlayerPrefsValue(value, serializer)
            };
        
        private string GetPlayerPrefsValue<T>(T value, IPedeSerializer serializer)
        {
            if (Metadata.BuildInTypes.Contains(typeof(T))) { return Convert.ToString(value); }
            else { return serializer.Serialize(value); }
        }

        private void GetPlayerPrefsData<T>(string value, Action<T> actionWithResult, IPedeSerializer serializer)
        {
            if (Metadata.BuildInTypes.Contains(typeof(T)))
            {
                GetBuildInTypePlayerPrefs(value, actionWithResult);
            }
            else
            {
                GetObject(serializer.Deserialize<T>(value), actionWithResult);
            }
        }

        private void GetBuildInTypePlayerPrefs<T>(string value, Action<T> actionWithResult) =>
            actionWithResult.Invoke(GetConvertedBuildInType<T>(value));

        private T GetConvertedBuildInType<T>(string value)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            return (T)typeConverter.ConvertFromString(value);
        }

        private Data GetFirstPlayerPrefsDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(playerPrefData, key, GetTypeName(typeof(T)));

        private void RemovePlayerPrefsData<T>(string key) =>
            RemoveData(playerPrefData, key, GetTypeName(typeof(T)));
        
        private bool IsPlayerPrefsDataValuesValid(
            ValidationDataErrorHandler validationDataErrorHandler,
            PedeSettings.CustomSerializer customSerializer
        )
        {
            var dataIsValid = true;

            for (int index = 0; index < playerPrefData.Count; index++)
            {
                var data = playerPrefData[index];

                if (!string.IsNullOrEmpty(data.type))
                {
                    try
                    {
                        var type = Metadata.BuildInTypes.FirstOrDefault(type => GetTypeName(type).Equals(data.type));

                        if (type != default) { TryConvertFromBuiltType(type, data); }
                        else { InvokeDeserializeMethodAsAbstract(customSerializer, data); }
                    }
                    catch
                    {
                        HasError(data.key, index);
                    }
                }
                else { HasError(data.key, index); }
            }
            
            void HasError(string keyInValidation, int index)
            {
                dataIsValid = false;
                validationDataErrorHandler.HandleValueError(keyInValidation, index, false);
            }

            return dataIsValid;
        }

        private void TryConvertFromBuiltType(Type type, Data data)
        {
            if (type == typeof(bool)) { GetConvertedBuildInType<bool>(data.value); }
            else if (type == typeof(byte)) { GetConvertedBuildInType<byte>(data.value); }
            else if (type == typeof(sbyte)) { GetConvertedBuildInType<sbyte>(data.value); }
            else if (type == typeof(char)) { GetConvertedBuildInType<char>(data.value); }
            else if (type == typeof(decimal)) { GetConvertedBuildInType<decimal>(data.value); }
            else if (type == typeof(double)) { GetConvertedBuildInType<double>(data.value); }
            else if (type == typeof(float)) { GetConvertedBuildInType<float>(data.value); }
            else if (type == typeof(int)) { GetConvertedBuildInType<int>(data.value); }
            else if (type == typeof(uint)) { GetConvertedBuildInType<uint>(data.value); }
            else if (type == typeof(long)) { GetConvertedBuildInType<long>(data.value); }
            else if (type == typeof(ulong)) { GetConvertedBuildInType<ulong>(data.value); }
            else if (type == typeof(short)) { GetConvertedBuildInType<short>(data.value); }
            else if (type == typeof(ushort)) { GetConvertedBuildInType<ushort>(data.value); }
            else if (type == typeof(string)) { GetConvertedBuildInType<string>(data.value); }
        }

        private bool IsPlayerPrefsDataKeysValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsKeysValid(playerPrefData, validationDataErrorHandler, false);

        private bool IsPlayerPrefsDataTypesValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsTypesValid(playerPrefData, validationDataErrorHandler, false);

        #endregion //PlayerPrefsRegion
        
        #region FileRegion
        
        internal void SetFile<T>(string key, T value, IPedeSerializer serializer)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
            var data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                var index = fileData.IndexOf(data);
                
                fileData.Remove(data);
                fileData.Insert(index, CreateFileData(key, value, serializer));
            }
            else
            {
                fileData.Add(CreateFileData(key, value, serializer));
            }
            
            info.UpdateFileNumberOfSetsInTheLastRun();
            info.UpdateSize(this);
            PersistAsset();
        }
        
        internal void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            IPedeSerializer serializer,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false
        )
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
            var data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetFileData(data.value,  actionIfHasResult, serializer);
                
                if (destroyAfter) { DeleteFile<T>(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
            
            info.UpdateFileNumberOfGetsInTheLastRun();
            PersistAsset();
        }
        
        internal void DeleteFile<T>(string key)
        {
            CheckKeyAsNull(key);
            RemoveFile<T>(key);
            
            info.UpdateFileNumberOfDeletesInTheLastRun();
            info.UpdateSize(this);
            
            PersistAsset();
        }
        
        internal void DeleteAllFiles()
        {
            info.UpdateFileNumberOfDeletesInTheLastRun(fileData.Count);
            fileData.Clear();
            PersistAsset();
        }

        internal void  HasFileKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
            actionWithResult.Invoke(ExistsData(fileData, key, GetTypeName(typeof(T))));
            
            info.UpdateFileNumberOfHasKeyInTheLastRun();
            PersistAsset();
        }

        private void RemoveFile<T>(string key) =>
            RemoveData(fileData, key, GetTypeName(typeof(T)));
        
        private void GetFileData<T>(string value, Action<T> actionWithResult, IPedeSerializer serializer) =>
            GetObject(serializer.Deserialize<T>(value), actionWithResult);

        private Data GetFirstFileDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(fileData, key, GetTypeName(typeof(T)));
        
        private Data CreateFileData<T>(string key, T value, IPedeSerializer serializer) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value =  serializer.Serialize(value)
            };
        
        private bool IsFileDataValuesValid(
            ValidationDataErrorHandler validationDataErrorHandler,
            PedeSettings.CustomSerializer customSerializer
        )
        {
            var dataIsValid = true;

            for (int index = 0; index < fileData.Count; index++)
            {
                var data = fileData[index];

                if (!string.IsNullOrEmpty(data.type))
                {
                    try { InvokeDeserializeMethodAsAbstract(customSerializer, data); }
                    catch { HasError(data.key, index); }
                }
                else
                {
                    HasError(data.key, index);
                }
            }

            void HasError(string keyInValidation, int index)
            {
                dataIsValid = false;
                validationDataErrorHandler.HandleValueError(keyInValidation, index, true);
            }
            
            return dataIsValid;
        }
        
        private bool IsFileDataKeysValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsKeysValid(fileData, validationDataErrorHandler, true);
        
        private bool IsFileDataTypesValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsTypesValid(fileData, validationDataErrorHandler, false);
        
        #endregion //FileRegion

        #region UtilsRegion

        internal void DeleteAll()
        {
            DeleteAllPlayerPrefs();
            DeleteAllFiles();
        }

        internal bool IsDataValid(
            ValidationDataErrorHandler validationDataErrorHandler,
            PedeSettings.CustomSerializer customSerializer
        )   
        {
            var isPlayerPrefsDataTypesValid = IsPlayerPrefsDataTypesValid(validationDataErrorHandler);
            var isFileDataTypesValid = IsFileDataTypesValid(validationDataErrorHandler);
            
            var isPlayerPrefsValuesValid = IsPlayerPrefsDataValuesValid(validationDataErrorHandler, customSerializer);
            var isFileDataValuesValid = IsFileDataValuesValid(validationDataErrorHandler, customSerializer);
            
            var isPlayerPrefsKeysValid = IsPlayerPrefsDataKeysValid(validationDataErrorHandler);
            var isFileDataKeysValid = IsFileDataKeysValid(validationDataErrorHandler);
            

            return isPlayerPrefsValuesValid &&
                   isFileDataValuesValid &&
                   isPlayerPrefsKeysValid &&
                   isFileDataKeysValid &&
                   isPlayerPrefsDataTypesValid &&
                   isFileDataTypesValid;
        }
        
        internal bool ShouldAvoidChanges() => 
            avoidChanges;

        internal void CreateBackup()
        {
            if (avoidChanges)
            {
                playerPrefDataBackup = new List<Data>(playerPrefData);
                fileDataBackup = new List<Data>(fileData);
            }
        }
        
        internal void SetBackup()
        {
            if (avoidChanges)
            {
                if (playerPrefDataBackup != null) { playerPrefData = playerPrefDataBackup; }
                if (fileDataBackup != null) { fileData = fileDataBackup; }
                info.UpdateSize(this);
            }
        }

        internal void CleanBackup()
        {
            playerPrefDataBackup = null;
            fileDataBackup = null;
        }
        
        internal void CleanDataChangeFlag() =>
            ChangeDataFlag(false);
        
        internal void ResetDataFlag() =>
            ChangeDataFlag(true);

        internal bool WasDataChanged() =>
            wasDataChanged;
        
        internal void ResetDataChanged() =>
            wasDataChanged = false;

        internal void SetFixedData() =>
            info.SetFixedData();

        internal void SetNewRunInfo()
        {
            info.UpdateNumberOfUses();
            info.CleanLastRunData();
            PersistAsset();
        }

        private void DataHasChanged()
        {
            info.UpdateSize(this);
            ResetDataFlag();
        }

        private void ChangeDataFlag(bool value)
        {
            wasDataChanged = value;
            PersistAsset();
        }

        private bool IsKeysValid(
            List<Data> dataList,
            ValidationDataErrorHandler validationDataErrorHandler,
            bool isFileData
        )
        {
            var dataIsValid = true;

            for (int index = 0; index < dataList.Count; index++)
            {
                var data = dataList[index];

                if (string.IsNullOrEmpty(data.key))
                {
                    validationDataErrorHandler.HandleKeyError(data.value, index, isFileData, false);
                    dataIsValid = false;
                }

                if (IsDuplicatedKey(data.key, data.type, dataList))
                {
                    validationDataErrorHandler.HandleKeyError(data.value, index, isFileData, true);
                    dataIsValid = false;
                }
            }

            return dataIsValid;
        }

        private bool IsDuplicatedKey(string key, string type, List<Data> dataList) =>
            dataList.FindAll(innerData => innerData.key == key && innerData.type == type).Count > 1;
        
        private bool IsTypesValid(
            List<Data> dataList,
            ValidationDataErrorHandler validationDataErrorHandler,
            bool isFileData
        )
        {
            var dataIsValid = true;

            for (int index = 0; index < dataList.Count; index++)
            {
                var data = dataList[index];

                if (string.IsNullOrEmpty(data.type))
                {
                    validationDataErrorHandler.HandleTypeError(data.value, index, isFileData);
                    dataIsValid = false;
                }
            }

            return dataIsValid;
        }

        private void GetObject<T>(T value, Action<T> actionIfHasResult) =>
            actionIfHasResult.Invoke(value);
        
        private bool ExistsData(List<Data> dataList, string key, string typeName) =>
            dataList.Exists(data => data.IsSameValue(key, typeName));

        private void RemoveData(List<Data> dataList, string key, string typeName) =>
            dataList.RemoveAll(data => data.IsSameValue(key, typeName));

        private Data GetFirstDataOrDefault(List<Data> daraList, string key, string typeName) =>
            daraList.FirstOrDefault(data => data.IsSameValue(key, typeName));

        private string GetTypeName(Type type) =>
            type.ToString();
        
        private void CheckKeyAsNull(string key) =>
            CheckArgumentAsNull(key, nameof(key));
        
        private void CheckValueAsNull<T>(T value) =>
            CheckArgumentAsNull(value, nameof(value));
        
        private void CheckActionAsNull<T>(Action<T> actionIfHasResult) =>
            CheckArgumentAsNull(actionIfHasResult, nameof(actionIfHasResult));
        
        private void CheckArgumentAsNull<T>(T argument, string argumentName)
        {
            if (argument == null) { throw new ArgumentNullException(nameof(argumentName)); }
        }

        private void PersistAsset()
        {
            if (!avoidChanges) { PedeEditor.PersistAsset(this); }
        }

        private void InvokeDeserializeMethodAsAbstract(PedeSettings.CustomSerializer customSerializer, Data data)
        {
            MethodInfo method = null;
            object obj = null;
            string methodName = null;
            
            var type = GetTypeByName(data.type);

            if (customSerializer != null)
            {
                methodName = nameof(customSerializer.InvokeCustomDeserializeMethod);
                obj = customSerializer;
            }
            else
            {
                var defaultSerializer = new DefaultPedeSerializer();
                
                methodName = nameof(defaultSerializer.Deserialize);
                obj = defaultSerializer;
            }

            method = GetMakeGenericMethod(obj, type, methodName);
            
            method.Invoke(obj, new object[] { data.value });
        }
        
        private static bool IsOnScriptReload() =>
            SessionState.GetBool(Consts.SessionOnScriptReloadFlag, false);

        private MethodInfo GetMakeGenericMethod(object obj, Type type, string methodName)
        {
            var method = obj.GetType().GetMethod(methodName);

            if (method == null)
            {
                method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            }
            
            return method.MakeGenericMethod(type);
        }

        private Type GetTypeByName(string name) =>
                AppDomain.CurrentDomain.GetAssemblies()
                    .Reverse()
                    .Select(assembly => assembly.GetType(name))
                    .FirstOrDefault(type => type != null)
                ??
                AppDomain.CurrentDomain.GetAssemblies()
                    .Reverse()
                    .SelectMany(assembly => assembly.GetTypes())
                    .FirstOrDefault(type => type.Name.Contains(name));
        
        private bool IsAvoidChangesFlagsTheSame()
        {
            var isAvoidChangesFlagTheSame = avoidChanges == avoidChangesBackup;

            if (!isAvoidChangesFlagTheSame)
            {
                avoidChangesBackup = avoidChanges;
            }
            
            return isAvoidChangesFlagTheSame;
        }

        #endregion //UtilsRegion
        
        internal class ValidationDataErrorHandler
        {
            private readonly Action<string, int, bool> ActionOnValidationIndividualValueError;
            private readonly Action<string, int, bool, bool> ActionOnValidationIndividualKeyError;
            private readonly Action<string, int, bool> ActionOnValidationIndividualTypeError;

            internal ValidationDataErrorHandler(
                Action<string, int, bool> actionOnValidationIndividualValueError,
                Action<string, int, bool, bool> actionOnValidationIndividualKeyError,
                Action<string, int, bool> actionOnValidationIndividualTypeError
            )
            {
                this.ActionOnValidationIndividualValueError = actionOnValidationIndividualValueError;
                this.ActionOnValidationIndividualKeyError = actionOnValidationIndividualKeyError;
                this.ActionOnValidationIndividualTypeError = actionOnValidationIndividualTypeError;
            }

            internal void HandleValueError(string key, int index, bool isFileData) =>
                ActionOnValidationIndividualValueError.Invoke(key, index, isFileData);
            
            internal void HandleKeyError(string key, int index, bool isFileData, bool isDuplicity) =>
                ActionOnValidationIndividualKeyError.Invoke(key, index, isFileData, isDuplicity);
            
            internal void HandleTypeError(string key, int index, bool isFileData) =>
                ActionOnValidationIndividualTypeError.Invoke(key, index, isFileData);
        }

        [Serializable]
        private class Info
        {
            
            [Serializable]
            private struct DataInfo
            {
                [SerializeField]
                [PedeSerialize.PropertyAttributes.ReadOnly]
                internal float sizeInBytes;
            
                [SerializeField]
                [PedeSerialize.PropertyAttributes.ReadOnly]
                internal int getsInTheLastRun;

                [SerializeField]
                [PedeSerialize.PropertyAttributes.ReadOnly]
                internal int setsInTheLastRun;
            
                [SerializeField]
                [PedeSerialize.PropertyAttributes.ReadOnly]
                internal int deleteInTheLastRun;
                
                [SerializeField]
                [PedeSerialize.PropertyAttributes.ReadOnly]
                internal int hasKeyInTheLastRun;
                
                internal void CleanLastRunData()
                {
                    getsInTheLastRun = 0;
                    setsInTheLastRun = 0;
                    deleteInTheLastRun = 0;
                    hasKeyInTheLastRun = 0;
                }
                
            }
            
            [SerializeField]
            [PedeSerialize.PropertyAttributes.ReadOnly]
            private string createdIn;
            
            [SerializeField]
            [PedeSerialize.PropertyAttributes.ReadOnly]
            private string createdBy;

            [SerializeField]
            [PedeSerialize.PropertyAttributes.ReadOnly]
            private int numberOfUses;
            
            [SerializeField]
            [PedeSerialize.PropertyAttributes.ReadOnly]
            private float totalSizeInBytes;

            [Space]
            [SerializeField]
            private DataInfo playerPrefsDataInfo;
            
            [Space]
            [SerializeField]
            private DataInfo fileDataInfo;

            internal void SetFixedData()
            {
                if (string.IsNullOrEmpty(createdIn) || string.IsNullOrEmpty(createdBy))
                {
                    createdIn = DateTime.Now.ToString(Consts.InfoDateFormation);
                    createdBy = Environment.UserName;
                }
            }

            internal void CleanLastRunData()
            {
                playerPrefsDataInfo.CleanLastRunData();
                fileDataInfo.CleanLastRunData();
            }

            internal void UpdateNumberOfUses() =>
                numberOfUses++;

            internal void UpdatePlayerPrefsNumberOfGetsInTheLastRun() =>
                playerPrefsDataInfo.getsInTheLastRun++;

            internal void UpdatePlayerPrefsNumberOfSetsInTheLastRun() =>
                playerPrefsDataInfo.setsInTheLastRun++;

            internal void UpdatePlayerPrefsNumberOfDeletesInTheLastRun(int numberOfDeletes = 1) =>
                playerPrefsDataInfo.deleteInTheLastRun += numberOfDeletes;

            internal void UpdatePlayerPrefsNumberOfHasKeyInTheLastRun() =>
                playerPrefsDataInfo.hasKeyInTheLastRun++;

            internal void UpdateFileNumberOfGetsInTheLastRun() =>
                fileDataInfo.getsInTheLastRun++;

            internal void UpdateFileNumberOfSetsInTheLastRun() =>
                fileDataInfo.setsInTheLastRun++;

            internal void UpdateFileNumberOfDeletesInTheLastRun(int numberOfDeletes = 1) =>
                fileDataInfo.deleteInTheLastRun += numberOfDeletes;

            internal void UpdateFileNumberOfHasKeyInTheLastRun() =>
                fileDataInfo.hasKeyInTheLastRun++;
            
            internal void UpdateSize(PedeData pedeData)
            {
                if (pedeData.playerPrefData != null && pedeData.playerPrefData.Count != 0)
                {
                    playerPrefsDataInfo.sizeInBytes = GetDataSizeInBytes(pedeData.playerPrefData);
                }
                else
                {
                    playerPrefsDataInfo.sizeInBytes = 0;
                }
                
                if (pedeData.fileData != null && pedeData.fileData.Count != 0)
                {
                    fileDataInfo.sizeInBytes = GetDataSizeInBytes(pedeData.fileData);
                }
                else
                {
                    fileDataInfo.sizeInBytes = 0;
                }

                totalSizeInBytes = playerPrefsDataInfo.sizeInBytes + fileDataInfo.sizeInBytes;
            }
            
            private float GetDataSizeInBytes(List<Data> data)=>
                data.Sum(internalData => internalData.value.Length);

        }

    }
}