using System;
using System.IO;
using System.Text;
using Thisaislan.PersistenceEasyToDelete.PedSerialize.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal static class PedFile
    {
        
        internal static void Serialize<T>(T value, Action<byte[]> actionAfterSerialize, IPedSerializer serializer)
        {
            var compressedValue = GetCompressedStringValue(value, serializer);
            var bytes = SerializeBytes(compressedValue);

            actionAfterSerialize(bytes);
        }

        public static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize, IPedSerializer serializer)
        {
            var decompressedValue = StringCompressor.DecompressString(DeserializeBytes(value));
            var obj = serializer.Deserialize<T>(decompressedValue);

            actionAfterDeserialize(obj);
        }

        internal static void SetFile<T>(string key, T value, IPedSerializer serializer)
        {
            var filePath = GetFullPath(key);
            
            Directory.CreateDirectory(Constants.Consts.PedFileRootFolderName);
            
            File.Create(filePath).Close();
            
            var compressedValue = GetCompressedStringValue(value, serializer);
            var bytes = SerializeBytes(compressedValue);
            
            File.WriteAllBytes(filePath, bytes);
        }

        internal static void GetFile<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedSerializer serializer,
            bool destroyAfter
        )
        {
            var filePath = GetFullPath(key);
            
            if (File.Exists(filePath))
            {
                File.Open(filePath, FileMode.Open).Close();
                
                var decompressedValue = StringCompressor.DecompressString(
                    DeserializeBytes(File.ReadAllBytes(filePath))
                    );
                
                var obj = serializer.Deserialize<T>(decompressedValue);

                if (obj != null) { actionIfHasResult.Invoke(obj); }
                
                if (destroyAfter) { DeleteFile(key); }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        internal static void DeleteFile(string key)
        {
            var filePath = GetFullPath(key);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        internal static void DeleteAllFiles()
        {
            if (Directory.Exists(Constants.Consts.PedFileRootFolderName))
            {
                Directory.Delete(Constants.Consts.PedFileRootFolderName, true);
            }
        }

        internal static void HasFileKey(string key, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(File.Exists(GetFullPath(key)));

        private static string GetCompressedStringValue<T>(T value, IPedSerializer serializer)
        {
            var strigValue = serializer.Serialize(value);
            
            return StringCompressor.CompressString(strigValue);
        }

        private static byte[] SerializeBytes(string value) =>
            Encoding.UTF8.GetBytes(value);

        private static string DeserializeBytes(byte[]  bytes) =>
            Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        private static string GetFullPath(string key) =>
            String.Format(Constants.Consts.PedFilePethFormat, Constants.Consts.PedFileRootFolderName, key);

    }
}