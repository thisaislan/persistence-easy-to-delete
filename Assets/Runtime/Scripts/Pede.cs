using System;
using Thisaislan.PersistenceEasyToDeleteInEditor.Interfaces;
using Thisaislan.PersistenceEasyToDeleteInEditor.PedeComposition;

#if UNITY_EDITOR
using Thisaislan.PersistenceEasyToDeleteInEditor.Editor;
#endif

namespace Thisaislan.PersistenceEasyToDeleteInEditor
{
    /// <summary>
    /// Provides static methods for saving and deleting data.
    /// It can store string, integer, float, char, boolean and non-engine object types as PlayerPrefs and
    /// non-engine object types as files.
    /// </summary>
    public static class Pede
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
        
        private static ISerializer serializer = new PedeDefaultSerializer();
        
        #region SerializerRegion

        /// <summary> Sets a custom serializer to be used on Pede . </summary>
        /// <param name="customSerializer"> Serialize class to be used. That class should implements
        /// <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Interfaces.ISerializer" /> interface. </param>
        /// <remarks>
        ///   By default Pede uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To set a new serializer class use
        /// <see cref="Thisaislan.PersistenceEasyToDeleteInEditor.Pede.SetCustomSerializer(ISerializer)"/> method.
        ///   If the project uses a custom serializer class maybe will be useful set that class at PedeSettings to be
        ///   also used on the validation flow.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   CustomSerializer cannot be null.
        /// </exception>
        public static void SetCustomSerializer(ISerializer customSerializer)
        {
            CheckArgumentAsNull(customSerializer, nameof(customSerializer));
            
            Pede.serializer = customSerializer;
        }

        #endregion //SerializerRegion

        #region PlayerPrefsRegion
        
        /// <summary> Saves PlayerPrefs with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the PlayerPrefs. </param>
        /// <param name="value"> Value to be saved. </param>
        /// <param name="playerPrefsSetMode">
        ///   Specifies the set mode
        ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Pede.PlayerPrefsSetMode" />. 
        ///  Default  PlayerPrefsSetMode.Normal
        /// </param>
        /// <remarks>
        ///   By default Pede uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To set a new serializer class use
        /// <see cref="Thisaislan.PersistenceEasyToDeleteInEditor.Pede.SetCustomSerializer(ISerializer)"/> method.
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
            PedeEditor.SetPlayerPrefs(key, value, serializer);
#else
            PedePlayerPrefs.SetPlayerPrefs(GetFormattedKey(key, typeof(T)), value, playerPrefsSetMode, serializer);
#endif
        }

        /// <summary> Loads PlayerPrefs with a specific key and type. </summary>
        /// <param name="key"> Key used to get the PlayerPrefs. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the PlayerPrefs exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the PlayerPrefs does not exist. Default is null
        /// </param>
        /// <param name="playerPrefsGetMode">
        ///   Specifies the get mode
        ///   <see cref="T:Thisaislan.PersistenceEasyToDeleteInEditor.Pede.PlayerPrefsGetMode" />. 
        ///  Default PlayerPrefsGetMode.Normal
        /// </param>
        /// <remarks>
        ///   By default Pede uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To set a new serializer class use
        /// <see cref="Thisaislan.PersistenceEasyToDeleteInEditor.Pede.SetCustomSerializer(ISerializer)"/> method.
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
            PedeEditor.GetPlayerPrefs(
                    key,
                    actionIfHasResult,
                    actionIfHasNotResult,
                    serializer,
                    playerPrefsGetMode != PlayerPrefsGetMode.Normal
                );
#else
            PedePlayerPrefs.GetPlayerPrefs(
                    GetFormattedKey(key, typeof(T)), 
                        actionIfHasResult, 
                        actionIfHasNotResult, 
                        playerPrefsGetMode,
                        serializer
                    );  
#endif
        }

        /// <summary> Deletes PlayerPrefs with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the PlayerPrefs. </param>
        /// <param name="shouldSaveImmediately"> If true, it saves PlayerPrefs after deletion. Default is false. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public static void DeletePlayerPrefs<T>(string key, bool shouldSaveImmediately = false)
        {
            CheckKeyAsNull(key);
            
#if UNITY_EDITOR
            PedeEditor.DeletePlayerPrefs<T>(key);
#else
            PedePlayerPrefs.DeletePlayerPrefs(GetFormattedKey(key, typeof(T)), shouldSaveImmediately);
#endif
        }

        /// <summary> Deletes all PlayerPrefs key and values by calling PlayerPrefs.DeleteAll() </summary>
        /// <param name="shouldSaveImmediately"> If true, it saves PlayerPrefs after deletion. Default is false. </param>
        public static void DeleteAllPlayerPrefs(bool shouldSaveImmediately = false)
        {
#if UNITY_EDITOR
            PedeEditor.DeleteAllPlayerPrefs();
#else
            PedePlayerPrefs.DeleteAllPlayerPrefs(shouldSaveImmediately);
#endif
        }

        /// <summary> Returns true if the given key exists in PlayerPrefs, otherwise returns false. </summary>
        /// <param name="key"> Key used to check the PlayerPrefs. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public static void HasPlayerPrefsKey<T>(string key, Action<bool> actionWithResult)
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
#if UNITY_EDITOR
            PedeEditor.HasPlayerPrefsKey<T>(key, actionWithResult);
#else
            PedePlayerPrefs.HasPlayerPrefsKey(GetFormattedKey(key, typeof(T)), actionWithResult);
#endif
        }
        
        /// <summary> Writes all modified preferences to disk by calling PlayerPrefs.Save(). </summary>
        public static void SavePlayerPrefs()
        {
#if !UNITY_EDITOR
            PedePlayerPrefs.SavePlayerPrefs();
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
            
            PedeFile.Serialize(value, actionAfterSerialize, serializer);
        }
        
        /// <summary> Decompress and deserialize a object. </summary>
        /// <param name="value"> array of bytes to be deserialized. </param>
        /// <param name="actionAfterDeserialize">  Action that will be performed after the deserialize process </param>
        public static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize)
        {
            CheckValueAsNull(value);
            CheckActionAsNull(actionAfterDeserialize);
            
            PedeFile.Deserialize<T>(value, actionAfterDeserialize, serializer);
        }

        /// <summary> Saves file with a specific key, type and value. </summary>
        /// <param name="key"> Key used to set the file. </param>
        /// <param name="value"> Object to be saved or a engine type. </param>
        /// <remarks>
        ///   By default Pede uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To set a new serializer class use
        /// <see cref="Thisaislan.PersistenceEasyToDeleteInEditor.Pede.SetCustomSerializer(ISerializer)"/> method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and value cannot be null.
        /// </exception>
        public static void SetFile<T>(string key, T value) where T: new()
        {
            CheckKeyAsNull(key);
            CheckValueAsNull(value);
            
#if UNITY_EDITOR
            PedeEditor.SetFile(key, value, serializer);
#else
            PedeFile.SetFile(GetFormattedKey(key, typeof(T)), value, serializer);
#endif
        }
        
        /// <summary> Loads file with a specific key and type. </summary>
        /// <param name="key"> Key used to get the file. </param>
        /// <param name="actionIfHasResult">
        ///   Action that will be performed if the file exists.
        /// </param>
        /// <param name="actionIfHasNotResult">
        ///   Action that will be performed if the file does not exist. Default is null
        /// </param>
        /// <param name="destroyAfter"> If true, deletes file after. Default is false </param>
        /// <remarks>
        ///   By default Pede uses <see cref="T:UnityEngine.JsonUtility" />, so it may have its limitations.
        ///   To set a new serializer class use
        /// <see cref="Thisaislan.PersistenceEasyToDeleteInEditor.Pede.SetCustomSerializer(ISerializer)"/> method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionIfHasResult cannot be null.
        /// </exception>
        public static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult = null,
            bool destroyAfter = false) where T: new()
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionIfHasResult);
            
#if UNITY_EDITOR
            PedeEditor.GetFile(key, actionIfHasResult, actionIfHasNotResult, serializer, destroyAfter);
#else
            PedeFile.GetFile(
                    GetFormattedKey(key, typeof(T)),
                    actionIfHasResult,
                    actionIfHasNotResult,
                    serializer,
                    destroyAfter
                );
#endif
        }

        /// <summary> Deletes file with a specific key and type. </summary>
        /// <param name="key"> Key used to delete the file. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key cannot be null.
        /// </exception>
        public static void DeleteFile<T>(string key) where T: new()
        {
            CheckKeyAsNull(key);
            
#if UNITY_EDITOR
            PedeEditor.DeleteFile<T>(key);
#else
            PedeFile.DeleteFile(GetFormattedKey(key, typeof(T)));
#endif
        }
        
        /// <summary> Deletes all files saved. </summary>
        public static void DeleteAllFiles()
        {
#if UNITY_EDITOR
            PedeEditor.DeleteAllFiles();
#else
            PedeFile.DeleteAllFiles();
#endif   
        }

        /// <summary> Checks if exists a file with a specific key and type. </summary>
        /// <param name="key"> Key used to save the file. </param>
        /// <param name="actionWithResult"> Action that will be performed with the result. </param>
        /// <exception cref="ArgumentNullException">
        ///   Key and actionWithResult cannot be null.
        /// </exception>
        public static void HasFileKey<T>(string key, Action<bool> actionWithResult) where T: new()
        {
            CheckKeyAsNull(key);
            CheckActionAsNull(actionWithResult);
            
#if UNITY_EDITOR
            PedeEditor.HasFileKey<T>(key, actionWithResult);
#else
            PedeFile.HasFileKey(GetFormattedKey(key, typeof(T)), actionWithResult);
#endif
        }
        
        #endregion //FileRegion

        /// <summary> Deletes all data, both files and PlayerPrefs. </summary>
        /// <param name="shouldSaveImmediately"> If true, it  PlayerPrefs after deletion. Default is false. </param>
        public static void DeleteAll(bool shouldSaveImmediately = false)
        {
#if UNITY_EDITOR
            PedeEditor.DeleteAll();
#else
            PedePlayerPrefs.DeleteAllPlayerPrefs(shouldSaveImmediately);
            PedeFile.DeleteAllFiles();
#endif
        }

        private static string GetFormattedKey(string key, Type type) =>
            String.Format(Constants.Consts.PedeKeyFormat, key, type).GetHashCode().ToString();

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