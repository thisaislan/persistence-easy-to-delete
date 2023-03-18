using System;
using Thisaislan.PersistenceEasyToDelete.PedComposition;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.ScriptableObjects;

#if UNITY_EDITOR
using Thisaislan.PersistenceEasyToDelete.Editor;
#endif

namespace Thisaislan.PersistenceEasyToDelete
{
    /// <summary>
    /// Provides static methods for saving and deleting data.
    /// It can store string, integer, float, char, boolean and non-engine object types as PlayerPrefs and
    /// non-engine object types as files.
    /// </summary>
    public static class Ped
    {
        /// <summary> Represents the get possibilities of a PlayerPrefs </summary>
        public enum PlayerPrefsGetMode
        {
            /// <summary> Indicates a normal get, without any action after </summary>
            Normal,
            /// <summary> Indicates a destructive get. PlayerPrefs will be destroyed after getting it. </summary>
            Destructive,
            /// <summary> Indicates a destructive get and persist action on disk after getting it. </summary>
            DestructiveAndPersistent
        }
        
        /// <summary> Represents the set possibilities of a PlayerPrefs </summary>
        public enum PlayerPrefsSetMode
        {
            /// <summary> Indicates a normal set, without any action after </summary>
            Normal,
            /// <summary> Indicates a persist action on disk after setting it. </summary>
            Persistent
        }

        private static readonly IPedSerializer serializer = PedSerializeSettings.instance.GetSerializer();

        #region PlayerPrefsRegion
        
        /// <summary> Saves PlayerPrefs with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the PlayerPrefs. Keys in Ped uses pair key and type. </param>
        /// <param name="value"> Value to be saved. </param>
        /// <param name="playerPrefsSetMode">
        ///   Specifies the set mode
        ///   <see cref="T:Thisaislan.PersistenceEasyToDelete.Ped.PlayerPrefsSetMode" />. 
        ///  Default  PlayerPrefsSetMode.Normal
        /// </param>
        /// <remarks>
        ///   By default Ped uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To use a custom serializer class, define that class in the Custom Serializer field in PedSettings
        ///   in the Settings folder.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and value cannot be null.
        /// </exception>
        public static void SetPlayerPrefs<T>(
            string key,
            T value,
            PlayerPrefsSetMode playerPrefsSetMode = PlayerPrefsSetMode.Normal
        )
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
#if UNITY_EDITOR
            PedEditor.SetPlayerPrefs(key, value, serializer);
#else
            PedPlayerPrefs.SetPlayerPrefs(GetFormattedKey(key, typeof(T)), value, playerPrefsSetMode, serializer);
#endif
        }

        /// <summary> Loads PlayerPrefs with a specific key and type. </summary>
        /// <param name="key"> Key used to get the PlayerPrefs. Keys in Ped uses pair key and type. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the PlayerPrefs exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the PlayerPrefs does not exist. Default is null
        /// </param>
        /// <param name="playerPrefsGetMode">
        ///   Specifies the get mode
        ///   <see cref="T:Thisaislan.PersistenceEasyToDelete.Ped.PlayerPrefsGetMode" />. 
        ///  Default PlayerPrefsGetMode.Normal
        /// </param>
        /// <remarks>
        ///   By default Ped uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To use a custom serializer class, define that class in the Custom Serializer field in PedSettings
        ///   in the Settings folder.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionIfHasResult cannot be null.
        /// </exception>
        public static void GetPlayerPrefs<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult = null,
            PlayerPrefsGetMode playerPrefsGetMode = PlayerPrefsGetMode.Normal)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);

#if UNITY_EDITOR
            PedEditor.GetPlayerPrefs(
                    key,
                    actionIfHasResult,
                    actionIfHasNotResult,
                    serializer,
                    playerPrefsGetMode != PlayerPrefsGetMode.Normal
                );
#else
            PedPlayerPrefs.GetPlayerPrefs(
                    GetFormattedKey(key, typeof(T)), 
                        actionIfHasResult, 
                        actionIfHasNotResult, 
                        playerPrefsGetMode,
                        serializer
                    );  
#endif
        }

        /// <summary> Deletes PlayerPrefs with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the PlayerPrefs. Keys in Ped uses pair key and type. </param>
        /// <param name="shouldSaveImmediately"> If true, it saves PlayerPrefs after deletion. Default is false. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public static void DeletePlayerPrefs<T>(string key, bool shouldSaveImmediately = false)
        {
            CheckKeyAsNull(key);
            
#if UNITY_EDITOR
            PedEditor.DeletePlayerPrefs<T>(key);
#else
            PedPlayerPrefs.DeletePlayerPrefs(GetFormattedKey(key, typeof(T)), shouldSaveImmediately);
#endif
        }

        /// <summary> Deletes all PlayerPrefs key and values by calling PlayerPrefs.DeleteAll() </summary>
        /// <param name="shouldSaveImmediately"> If true, it saves PlayerPrefs after deletion. Default is false. </param>
        public static void DeleteAllPlayerPrefs(bool shouldSaveImmediately = false)
        {
#if UNITY_EDITOR
            PedEditor.DeleteAllPlayerPrefs();
#else
            PedPlayerPrefs.DeleteAllPlayerPrefs(shouldSaveImmediately);
#endif
        }

        /// <summary> Returns true if the given key exists in PlayerPrefs, otherwise returns false. </summary>
        /// <param name="key"> Key used to check the PlayerPrefs. Keys in Ped uses pair key and type. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
#if UNITY_EDITOR
            PedEditor.HasPlayerPrefsKey<T>(key, actionWithResult);
#else
            PedPlayerPrefs.HasPlayerPrefsKey(GetFormattedKey(key, typeof(T)), actionWithResult);
#endif
        }
        
        /// <summary> Writes all modified preferences to disk by calling PlayerPrefs.Save(). </summary>
        public static void SavePlayerPrefs()
        {
#if !UNITY_EDITOR
            PedPlayerPrefs.SavePlayerPrefs();
#endif
        }
        
        #endregion //PlayerPrefsRegion
        
        #region FileRegion

        /// <summary> Compress and serialize a object </summary>
        /// <param name="value"> Object to be serialized. </param>
        /// <param name="actionAfterSerialize">  Action that will be performed after the serialize process </param>
        public static void Serialize<T>(T value, Action<byte[]> actionAfterSerialize)
        {
            CheckValueAsNull(value);
            CheckActionAsNull(actionAfterSerialize);
            
            PedFile.Serialize(value, actionAfterSerialize, serializer);
        }
        
        /// <summary> Decompress and deserialize a object. </summary>
        /// <param name="value"> array of bytes to be deserialized. </param>
        /// <param name="actionAfterDeserialize">  Action that will be performed after the deserialize process </param>
        public static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize)
        {
            CheckValueAsNull(value);
            CheckActionAsNull(actionAfterDeserialize);
            
            PedFile.Deserialize<T>(value, actionAfterDeserialize, serializer);
        }

        /// <summary> Saves file with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the file. Keys in Ped uses pair key and type. </param>
        /// <param name="value"> Object to be saved or a engine type. </param>
        /// <remarks>
        ///   By default Ped uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To use a custom serializer class, define that class in the Custom Serializer field in PedSettings
        ///   in the Settings folder.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and value cannot be null.
        /// </exception>
        public static void SetFile<T>(string key, T value)
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
#if UNITY_EDITOR
            PedEditor.SetFile(key, value, serializer);
#else
            PedFile.SetFile(GetFormattedKey(key, typeof(T)), value, serializer);
#endif
        }
        
        /// <summary> Loads file with a specific key and type. </summary>
        /// <param name="key"> Key used to get the file. Keys in Ped uses pair key and type. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the file exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the file does not exist. Default is null
        /// </param>
        /// <param name="destroyAfter"> If true, deletes file after. Default is false </param>
        /// <remarks>
        ///   By default Ped uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To use a custom serializer class, define that class in the Custom Serializer field in PedSettings
        ///   in the Settings folder.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionIfHasResult cannot be null.
        /// </exception>
        public static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
#if UNITY_EDITOR
            PedEditor.GetFile(key, actionIfHasResult, actionIfHasNotResult, serializer, destroyAfter);
#else
            PedFile.GetFile(
                    GetFormattedKey(key, typeof(T)),
                    actionIfHasResult,
                    actionIfHasNotResult,
                    serializer,
                    destroyAfter
                );
#endif
        }

        /// <summary> Deletes file with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the file. Keys in Ped uses pair key and type. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public static void DeleteFile<T>(string key)
        {
            CheckKeyAsNull(key);
            
#if UNITY_EDITOR
            PedEditor.DeleteFile<T>(key);
#else
            PedFile.DeleteFile(GetFormattedKey(key, typeof(T)));
#endif
        }
        
        /// <summary> Deletes all files saved. </summary>
        public static void DeleteAllFiles()
        {
#if UNITY_EDITOR
            PedEditor.DeleteAllFiles();
#else
            PedFile.DeleteAllFiles();
#endif   
        }

        /// <summary> Checks if exists a file with a specific key and type. </summary>
        /// <param name="key"> Key used to save the file. Keys in Ped uses pair key and type. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public static void HasFileKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
#if UNITY_EDITOR
            PedEditor.HasFileKey<T>(key, actionWithResult);
#else
            PedFile.HasFileKey(GetFormattedKey(key, typeof(T)), actionWithResult);
#endif
        }
        
        #endregion //FileRegion

        /// <summary> Deletes all data, both files and PlayerPrefs. </summary>
        /// <param name="shouldSaveImmediately"> If true, it  PlayerPrefs after deletion. Default is false. </param>
        public static void DeleteAll(bool shouldSaveImmediately = false)
        {
#if UNITY_EDITOR
            PedEditor.DeleteAll();
#else
            PedPlayerPrefs.DeleteAllPlayerPrefs(shouldSaveImmediately);
            PedFile.DeleteAllFiles();
#endif
        }

        private static string GetFormattedKey(string key, Type type) =>
            String.Format(Constants.Consts.PedKeyFormat, key, type).GetHashCode().ToString();

        private static void CheckKeyAsNull(string key) =>
            CheckArgumentAsNull(key, nameof(key));
        
        private static void CheckValueAsNull<T>(T value) =>
            CheckArgumentAsNull(value, nameof(value));
        
        private static void CheckActionAsNull<T>(Action<T> actionIfHasResult) =>
            CheckArgumentAsNull(actionIfHasResult, nameof(actionIfHasResult));
        
        private static void CheckArgumentAsNull<T>(T argument, string argumentName)
        {
            if (argument == null) { throw new ArgumentNullException(nameof(argumentName)); }
        }
        
    }
}