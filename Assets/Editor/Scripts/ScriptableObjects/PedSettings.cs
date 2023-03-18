using System;
using System.Reflection;
using Thisaislan.PersistenceEasyToDelete.Editor.Constants;
using Thisaislan.PersistenceEasyToDelete.Editor.Metas;
using Thisaislan.PersistenceEasyToDelete.Editor.PropertyAttributes;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Thisaislan.PersistenceEasyToDelete.Editor.ScriptableObjects
{
    [ClassTooltip(Consts.PedSettingsClassTipAttr)]
    internal class PedSettings : ScriptableObject
    {

        [Space(Metadata.SettingsFirstFieldTopSpace)]
        [Header(Consts.PedSettingsDataHeaderAttr)]
        [Tooltip(Consts.PedSettingsDataTooltipAttr)]
        [Space]
        [SerializeField]
        [OnChanged(nameof(ResetDataFlag))]
        internal PedData pedData;
        
        [Space(Metadata.SettingsFieldTopSpace)]
        [Header(Consts.PedSettingsSerializerHeaderAttr)]
        [Tooltip(Consts.PedSettingsSerializerTooltipAttr)]
        [Space]
        [SerializeField]
        [OnChanged(nameof(CustomSerializerWasChanged))]
        private MonoScript customSerializer;

        [Tooltip(Consts.PedSettingsSerializerPathTooltipAttr)]
        [PedSerialize.PropertyAttributes.ReadOnly]
        [Space]
        [SerializeField]
        private string customSerializerPath;

        [Space(Metadata.SettingsFieldTopSpace)]
        [Header(Consts.PedSettingsSettingsHeaderAttr)]
        [Tooltip(Consts.PedSettingsVerifyOnStartTooltipAttr)]
        [Space]
        [SerializeField]
        private bool verifyDataOnRunStart = true;

        private bool wasCustomSerializerChanged = true;
        
        private bool wasDataChanged = true;

        #region OverrideRegion

        private void OnValidate()
        {
            if (wasCustomSerializerChanged)
            {
                customSerializerPath = customSerializer != null ?
                AssetDatabase.GetAssetPath(customSerializer) : null;

                PersistAsset();
            }
        }

        #endregion //OverrideRegion

        internal void ResetCustomSerializerChangeFile()
        {
            if (!string.IsNullOrEmpty(customSerializerPath))
            {
                customSerializer = AssetDatabase.LoadAssetAtPath<MonoScript>(customSerializerPath);
                PersistAsset();
            }
        }

        internal void CleanCustomSerializerChangeFlag() =>
            wasCustomSerializerChanged = false;
        
        internal void CleanDataChangeFlag() =>
            ChangeDataFlag(false);

        internal void ResetDataFlag() =>
            ChangeDataFlag(true);
        
        internal bool WasCustomSerializerChanged() =>
            wasCustomSerializerChanged;
        
        internal bool WasDataChanged() =>
            wasDataChanged;

        internal string GetCustomSerializerClassName() =>
            customSerializer.GetClass().ToString();

        internal string GetCustomSerializerAssemblyName() =>
            Assembly.GetAssembly(customSerializer.GetClass()).GetName().Name;

        internal bool ShouldVerifyDataOnRunStart() =>
            verifyDataOnRunStart;

        internal bool HasCustomSerializerFile() => 
            customSerializer != null;

        internal CustomSerializer GetCustomSerializer() =>
            HasCustomSerializerFile() ? new CustomSerializer(customSerializer) : null;

        internal bool IsCustomSerializerFileValid(ValidationSerializerErrorHandler validationSerializerErrorHandler)
        {
            try
            {
                var customSerializer = GetCustomSerializer();
                
                return CheckSerializeInterface(validationSerializerErrorHandler) &&
                       CheckSerializeEncapsulation(validationSerializerErrorHandler) &&
                       CheckSerializeMethod(validationSerializerErrorHandler, customSerializer) &&
                       CheckDeserializerMethod(validationSerializerErrorHandler, customSerializer);
            }
            catch
            {
                validationSerializerErrorHandler.HandleSerializerClassError(false);
                return false;
            }
        }
        
        private void CustomSerializerWasChanged() =>
            wasCustomSerializerChanged = true;

        private void ChangeDataFlag(bool value)
        {
            wasDataChanged = value;

            PersistAsset();
        }
        
        private void PersistAsset()
        {
            PedEditor.PersistAsset(this);
        }

        private bool CheckSerializeEncapsulation(ValidationSerializerErrorHandler validationSerializerErrorHandler)
        {
            var result = customSerializer.GetClass().IsPublic;
            
            if (!result)
            {
                validationSerializerErrorHandler.HandleSerializerClassError(true);
            }

            return result;
        }

        private bool CheckSerializeInterface(ValidationSerializerErrorHandler validationSerializerErrorHandler)
        {
            var result = customSerializer.GetClass().GetInterface(Metadata.SerializerInterfaceName) != null;

            if (!result)
            {
                validationSerializerErrorHandler.HandleSerializerInterfaceError();
            }

            return result;
        }

        private bool CheckSerializeMethod(
            ValidationSerializerErrorHandler validationSerializerErrorHandler,
            CustomSerializer customSerializer
        )
        {
            if (customSerializer.GetSerializerMethod(Metadata.SerializerSerializeMethodName) == null)
            {
                validationSerializerErrorHandler.HandleMethodNotFoundError(true);
                return false;
            }
            else
            {
                return !string.IsNullOrEmpty(customSerializer.InvokeCustomSerializeMethod(new object()));
            }
        }
        
        private bool CheckDeserializerMethod(
            ValidationSerializerErrorHandler validationSerializerErrorHandler,
            CustomSerializer customSerializer
        )
        {
            if (customSerializer.GetSerializerMethod(Metadata.SerializerDeserializeMethodName) == null)
            {
                validationSerializerErrorHandler.HandleMethodNotFoundError(false);
                return false;
            }
            else
            {
                return 
                    customSerializer.InvokeCustomDeserializeMethod<object>(
                        customSerializer.InvokeCustomSerializeMethod(new object())
                        ) != null;
            }
        }

        internal class CustomSerializer
        {
            private readonly MonoScript customSerializer;

            internal CustomSerializer(MonoScript customSerializer) =>
                this.customSerializer = customSerializer;

            internal string InvokeCustomSerializeMethod(object obj)
            {
                var method = GetSerializerMethod(Metadata.SerializerSerializeMethodName); 
                return (string)method.Invoke(GetSerializerObject(), new object[] { obj});
            }
        
            internal T InvokeCustomDeserializeMethod<T>(string value)
            {
                var method = GetSerializerMethod(Metadata.SerializerDeserializeMethodName); 
                var genericMethod = method.MakeGenericMethod(typeof(T));

                return (T)genericMethod.Invoke(GetSerializerObject(), new object[] { value });
            }
            
            internal MethodInfo GetSerializerMethod(string serializeMethodName) =>
                customSerializer.GetClass().GetMethod(serializeMethodName);

            internal Object GetSerializerObject() =>
                Activator.CreateInstance(customSerializer.GetClass());

        }
        
        internal class ValidationSerializerErrorHandler
        {
            private readonly Action<bool> ActionOnValidationMethodNotFoundError;
            private readonly Action <bool>ActionOnValidationSerializerClassError;
            private readonly Action ActionOnValidationSerializerInterfaceError;

            internal ValidationSerializerErrorHandler(
                Action<bool> actionOnValidationMethodNotFoundError,
                Action <bool>actionOnValidationSerializerClassError,
                Action actionOnValidationSerializerInterfaceError
            )
            {
                this.ActionOnValidationMethodNotFoundError = actionOnValidationMethodNotFoundError;
                this.ActionOnValidationSerializerClassError = actionOnValidationSerializerClassError;
                this.ActionOnValidationSerializerInterfaceError = actionOnValidationSerializerInterfaceError;
            }

            internal void HandleMethodNotFoundError(bool isSerializerMethod) =>
                ActionOnValidationMethodNotFoundError.Invoke(isSerializerMethod);
            
            internal void HandleSerializerClassError(bool isEncapsulationError) =>
                ActionOnValidationSerializerClassError.Invoke(isEncapsulationError);
            
            internal void HandleSerializerInterfaceError() =>
                ActionOnValidationSerializerInterfaceError.Invoke();

        }

    }
}
