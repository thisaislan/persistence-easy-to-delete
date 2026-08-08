using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using Thisaislan.PersistenceEasyToDelete.Interfaces;
using RuntimeMetadata = Thisaislan.PersistenceEasyToDelete.Metas.Metadata;
using UnityEditor;
using UnityEngine;

namespace Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects
{
    [
        CreateAssetMenu(
            fileName = Metadata.DataFileName,
            menuName = Metadata.AssetMenuDataName,
            order = Metadata.AssetMenuDataOrder
        )
    ]
    internal class PedData : ScriptableObject
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
        private bool isActive;

        [SerializeField]
        private bool avoidChanges;

        [SerializeField]
        internal List<Data> playerPrefData = new List<Data>();

        [SerializeField]
        internal List<Data> fileData = new List<Data>();

        private List<Data> playerPrefDataBackup;
        private List<Data> fileDataBackup;

        internal void SetPlayerPrefs<T>(string key, T value, IPedSerializer serializer)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);

            Data data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                int index = playerPrefData.IndexOf(data);

                playerPrefData.Remove(data);
                playerPrefData.Insert(index, CreatePlayerPrefsData(key, value, serializer));
            }
            else
            {
                playerPrefData.Add(CreatePlayerPrefsData(key, value, serializer));
            }

            PersistAsset();
        }

        internal void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            IPedSerializer serializer,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false
        )
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);

            Data data = GetFirstPlayerPrefsDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetPlayerPrefsData(data.value, actionIfHasResult, serializer);

                if (destroyAfter)
                {
                    DeletePlayerPrefs<T>(key);
                }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        internal void DeletePlayerPrefs<T>(string key)
        {
            RemovePlayerPrefsData<T>(key);
            PersistAsset();
        }

        internal void DeleteAllPlayerPrefs()
        {
            playerPrefData.Clear();
            PersistAsset();
        }

        internal void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);

            actionWithResult.Invoke(ExistsData(playerPrefData, key, GetTypeName(typeof(T))));
        }

        internal void SetFile<T>(string key, T value, IPedSerializer serializer)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);

            Data data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                int index = fileData.IndexOf(data);

                fileData.Remove(data);
                fileData.Insert(index, CreateFileData(key, value, serializer));
            }
            else
            {
                fileData.Add(CreateFileData(key, value, serializer));
            }

            PersistAsset();
        }

        internal void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            IPedSerializer serializer,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false
        )
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);

            Data data = GetFirstFileDataOrDefault<T>(key);

            if (!data.IsKeyNull())
            {
                GetFileData(data.value, actionIfHasResult, serializer);

                if (destroyAfter)
                {
                    DeleteFile<T>(key);
                }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        internal void DeleteFile<T>(string key)
        {
            CheckKeyAsNull(key);
            RemoveFile<T>(key);

            PersistAsset();
        }

        internal void DeleteAllFiles()
        {
            fileData.Clear();
            PersistAsset();
        }

        internal void HasFileKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);

            actionWithResult.Invoke(ExistsData(fileData, key, GetTypeName(typeof(T))));
        }

        internal void DeleteAll()
        {
            DeleteAllPlayerPrefs();
            DeleteAllFiles();
        }

        internal bool IsDataValid(ValidationDataErrorHandler validationDataErrorHandler)
        {
            bool isPlayerPrefsDataTypesValid = IsPlayerPrefsDataTypesValid(validationDataErrorHandler);
            bool isFileDataTypesValid = IsFileDataTypesValid(validationDataErrorHandler);

            bool isPlayerPrefsValuesValid = IsPlayerPrefsDataValuesValid(validationDataErrorHandler);
            bool isFileDataValuesValid = IsFileDataValuesValid(validationDataErrorHandler);

            bool isPlayerPrefsKeysValid = IsPlayerPrefsDataKeysValid(validationDataErrorHandler);
            bool isFileDataKeysValid = IsFileDataKeysValid(validationDataErrorHandler);

            return isPlayerPrefsValuesValid &&
                   isFileDataValuesValid &&
                   isPlayerPrefsKeysValid &&
                   isFileDataKeysValid &&
                   isPlayerPrefsDataTypesValid &&
                   isFileDataTypesValid;
        }

        internal bool ShouldAvoidChanges() =>
            avoidChanges;

        internal bool IsActivePed() =>
            isActive;

        internal void SetActivePed(bool value) =>
            isActive = value;

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
                if (playerPrefDataBackup != null)
                {
                    playerPrefData = playerPrefDataBackup;
                }

                if (fileDataBackup != null)
                {
                    fileData = fileDataBackup;
                }
            }
        }

        internal void CleanBackup()
        {
            playerPrefDataBackup = null;
            fileDataBackup = null;
        }

        internal class ValidationDataErrorHandler
        {
            private readonly Action<string, int, bool> actionOnValidationIndividualValueError;
            private readonly Action<string, int, bool, bool> actionOnValidationIndividualKeyError;
            private readonly Action<string, int, bool> actionOnValidationIndividualTypeError;

            internal ValidationDataErrorHandler(
                Action<string, int, bool> actionOnValidationIndividualValueError,
                Action<string, int, bool, bool> actionOnValidationIndividualKeyError,
                Action<string, int, bool> actionOnValidationIndividualTypeError
            )
            {
                this.actionOnValidationIndividualValueError = actionOnValidationIndividualValueError;
                this.actionOnValidationIndividualKeyError = actionOnValidationIndividualKeyError;
                this.actionOnValidationIndividualTypeError = actionOnValidationIndividualTypeError;
            }

            internal void HandleValueError(string key, int index, bool isFileData) =>
                actionOnValidationIndividualValueError.Invoke(key, index, isFileData);

            internal void HandleKeyError(string key, int index, bool isFileData, bool isDuplicity) =>
                actionOnValidationIndividualKeyError.Invoke(key, index, isFileData, isDuplicity);

            internal void HandleTypeError(string key, int index, bool isFileData) =>
                actionOnValidationIndividualTypeError.Invoke(key, index, isFileData);
        }

        private Data CreatePlayerPrefsData<T>(string key, T value, IPedSerializer serializer) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value = GetPlayerPrefsValue(value, serializer)
            };

        private string GetPlayerPrefsValue<T>(T value, IPedSerializer serializer)
        {
            if (RuntimeMetadata.BuiltInTypes.Contains(typeof(T)))
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            else
            {
                return serializer.Serialize(value);
            }
        }

        private void GetPlayerPrefsData<T>(string value, Action<T> actionWithResult, IPedSerializer serializer)
        {
            if (RuntimeMetadata.BuiltInTypes.Contains(typeof(T)))
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
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(null, CultureInfo.InvariantCulture, value);
        }

        private Data GetFirstPlayerPrefsDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(playerPrefData, key, GetTypeName(typeof(T)));

        private void RemovePlayerPrefsData<T>(string key) =>
            RemoveData(playerPrefData, key, GetTypeName(typeof(T)));

        private bool IsPlayerPrefsDataValuesValid(ValidationDataErrorHandler validationDataErrorHandler)
        {
            bool dataIsValid = true;

            for (int index = 0; index < playerPrefData.Count; index++)
            {
                Data data = playerPrefData[index];

                if (!string.IsNullOrEmpty(data.type))
                {
                    try
                    {
                        Type type = RuntimeMetadata.BuiltInTypes.FirstOrDefault(type => GetTypeName(type).Equals(data.type));

                        if (type != default)
                        {
                            TryConvertFromBuiltInType(type, data);
                        }
                        else if (!IsSerializedValueValid(data.key, data.type, data.value))
                        {
                            HasError(data.key, index);
                        }
                    }
                    catch
                    {
                        HasError(data.key, index);
                    }
                }
                else
                {
                    HasError(data.key, index);
                }
            }

            void HasError(string keyInValidation, int index)
            {
                dataIsValid = false;
                validationDataErrorHandler.HandleValueError(keyInValidation, index, false);
            }

            return dataIsValid;
        }

        private void TryConvertFromBuiltInType(Type type, Data data)
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

        private bool IsSerializedValueValid(string key, string type, string value)
        {
            JsonValidationResult validationResult = JsonValidator.IsValidJson(value);

            if (validationResult == JsonValidationResult.NotValidatable)
            {
                Debug.Log(string.Format(
                    Consts.UnableToValidateSerializedValueLogMessage,
                    nameof(PedData),
                    key,
                    type
                ));
            }

            return validationResult != JsonValidationResult.Invalid;
        }

        private bool IsPlayerPrefsDataKeysValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsKeysValid(playerPrefData, validationDataErrorHandler, false);

        private bool IsPlayerPrefsDataTypesValid(ValidationDataErrorHandler validationDataErrorHandler) =>
            IsTypesValid(playerPrefData, validationDataErrorHandler, false);

        private Data CreateFileData<T>(string key, T value, IPedSerializer serializer) =>
            new Data
            {
                key = key,
                type = GetTypeName(typeof(T)),
                value = serializer.Serialize(value)
            };

        private void GetFileData<T>(string value, Action<T> actionWithResult, IPedSerializer serializer) =>
            GetObject(serializer.Deserialize<T>(value), actionWithResult);

        private Data GetFirstFileDataOrDefault<T>(string key) =>
            GetFirstDataOrDefault(fileData, key, GetTypeName(typeof(T)));

        private void RemoveFile<T>(string key) =>
            RemoveData(fileData, key, GetTypeName(typeof(T)));

        private bool IsFileDataValuesValid(ValidationDataErrorHandler validationDataErrorHandler)
        {
            bool dataIsValid = true;

            for (int index = 0; index < fileData.Count; index++)
            {
                Data data = fileData[index];

                if (!string.IsNullOrEmpty(data.type))
                {
                    try
                    {
                        if (!IsSerializedValueValid(data.key, data.type, data.value))
                        {
                            HasError(data.key, index);
                        }
                    }
                    catch
                    {
                        HasError(data.key, index);
                    }
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
            IsTypesValid(fileData, validationDataErrorHandler, true);

        private bool IsKeysValid(
            List<Data> dataList,
            ValidationDataErrorHandler validationDataErrorHandler,
            bool isFileData
        )
        {
            bool dataIsValid = true;

            for (int index = 0; index < dataList.Count; index++)
            {
                Data data = dataList[index];

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
            bool dataIsValid = true;

            for (int index = 0; index < dataList.Count; index++)
            {
                Data data = dataList[index];

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

        private Data GetFirstDataOrDefault(List<Data> dataList, string key, string typeName) =>
            dataList.FirstOrDefault(data => data.IsSameValue(key, typeName));

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
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        private void PersistAsset()
        {
            if (!avoidChanges)
            {
                PedEditor.PersistAsset(this);
            }
        }

    }
}