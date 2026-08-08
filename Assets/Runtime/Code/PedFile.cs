using System;
using System.IO;
using System.Text;
using Thisaislan.PersistenceEasyToDelete.Constants;
using Thisaislan.PersistenceEasyToDelete.Interfaces;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal class PedFile : IPedFile
    {
        internal static void Serialize<T>(T value, Action<byte[]> actionAfterSerialize, IPedSerializer serializer)
        {
            string compressedValue = GetCompressedStringValue(value, serializer);
            byte[] bytes = SerializeBytes(compressedValue);

            actionAfterSerialize(bytes);
        }

        internal static void Deserialize<T>(byte[] value, Action<T> actionAfterDeserialize, IPedSerializer serializer)
        {
            string decompressedValue = StringCompressor.DecompressString(DeserializeBytes(value));
            T obj = serializer.Deserialize<T>(decompressedValue);

            actionAfterDeserialize(obj);
        }

        public void Set<T>(string key, T value, IPedSerializer serializer)
        {
            string filePath = GetFullPath(GetFormattedKey(key, typeof(T)));

            Directory.CreateDirectory(Consts.PedFileRootFolderName);

            File.WriteAllBytes(filePath, SerializeBytes(GetCompressedStringValue(value, serializer)));
        }

        public void Get<T>(
            string key,
            Action<T> actionIfHasResult,
            Action actionIfHasNotResult,
            IPedSerializer serializer,
            bool destroyAfter
        )
        {
            string filePath = GetFullPath(GetFormattedKey(key, typeof(T)));

            if (File.Exists(filePath))
            {
                string decompressedValue = StringCompressor.DecompressString(
                    DeserializeBytes(File.ReadAllBytes(filePath))
                );

                T obj = serializer.Deserialize<T>(decompressedValue);

                if (obj != null)
                {
                    actionIfHasResult.Invoke(obj);
                }

                if (destroyAfter)
                {
                    Delete<T>(key);
                }
            }
            else
            {
                actionIfHasNotResult?.Invoke();
            }
        }

        public void Delete<T>(string key)
        {
            string filePath = GetFullPath(GetFormattedKey(key, typeof(T)));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public void DeleteAll()
        {
            if (Directory.Exists(Consts.PedFileRootFolderName))
            {
                Directory.Delete(Consts.PedFileRootFolderName, true);
            }
        }

        public void HasKey<T>(string key, Action<bool> actionWithResult) =>
            actionWithResult.Invoke(File.Exists(GetFullPath(GetFormattedKey(key, typeof(T)))));

        private static string GetCompressedStringValue<T>(T value, IPedSerializer serializer)
        {
            string serializedValue = serializer.Serialize(value);

            return StringCompressor.CompressString(serializedValue);
        }

        private static byte[] SerializeBytes(string value) =>
            Encoding.UTF8.GetBytes(value);

        private static string DeserializeBytes(byte[] bytes) =>
            Encoding.UTF8.GetString(bytes, 0, bytes.Length);

        private static string GetFullPath(string key) =>
            string.Format(Consts.PedFilePathFormat, Consts.PedFileRootFolderName, key);

        private static string GetFormattedKey(string key, Type type) =>
            KeyFormatter.Format(key, type);

    }
}