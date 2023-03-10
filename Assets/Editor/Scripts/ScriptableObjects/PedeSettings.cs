using System;
using System.Reflection;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Constants;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.Metas;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.PropertyAttributes;
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Data;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Thisaislan.PersistenceEasyToDeleteInEditor.Editor.ScriptableObjects.Settings
{
    [ClassTooltip(Consts.PedeSettingsClassTipAttr)]
    internal class PedeSettings : ScriptableObject
    {
        [Space(Metadata.SettingsDataTopSpace)]
        [Header(Consts.PedeSettingsDataHeaderAttr)]
        [Tooltip(Consts.PedeSettingsDataTooltipAttr)]
        [Space]
        [SerializeField]
        internal PedeData pedeData;
        
        [Space(Metadata.SettingsSerializerTopSpace)]
        [Header(Consts.PedeSettingsSerializerHeaderAttr)]
        [Tooltip(Consts.PedeSettingsSerializerTooltipAttr)]
        [Space]
        [SerializeField]
        private MonoScript customSerializer;

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
                       CheckSerializeMethod(validationSerializerErrorHandler, customSerializer) &&
                       CheckDeserializerMethod(validationSerializerErrorHandler, customSerializer);
            }
            catch
            {
                validationSerializerErrorHandler.HandleSerializerClassError();
                return false;
            }
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
                return !string.IsNullOrEmpty(customSerializer.InvokeCustomSerializeMethod(DateTime.Now));
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
                    customSerializer.InvokeCustomDeserializeMethod<DateTime>(
                        customSerializer.InvokeCustomSerializeMethod(DateTime.Now)
                        ) != null;
            }
        }

        internal class CustomSerializer
        {
            private MonoScript customSerializer;

            internal CustomSerializer(MonoScript customSerializer) =>
                this.customSerializer = customSerializer;

            internal string InvokeCustomSerializeMethod(object obj)
            {
                var method = GetSerializerMethod(Metadata.SerializerSerializeMethodName); 
                return (string)method.Invoke(GetSerializerObject(), new object[] { obj});
            }
        
            internal T InvokeCustomDeserializeMethod<T>(string value) where T: new()
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
            private readonly Action ActionOnValidationSerializerClassError;
            private readonly Action ActionOnValidationSerializerInterfaceError;

            internal ValidationSerializerErrorHandler(
                Action<bool> actionOnValidationMethodNotFoundError,
                Action actionOnValidationSerializerClassError,
                Action actionOnValidationSerializerInterfaceError
            )
            {
                this.ActionOnValidationMethodNotFoundError = actionOnValidationMethodNotFoundError;
                this.ActionOnValidationSerializerClassError = actionOnValidationSerializerClassError;
                this.ActionOnValidationSerializerInterfaceError = actionOnValidationSerializerInterfaceError;
            }

            internal void HandleMethodNotFoundError(bool isSerializerMethod) =>
                ActionOnValidationMethodNotFoundError.Invoke(isSerializerMethod);
            
            internal void HandleSerializerClassError() =>
                ActionOnValidationSerializerClassError.Invoke();
            
            internal void HandleSerializerInterfaceError() =>
                ActionOnValidationSerializerInterfaceError.Invoke();

        }

    }
}
