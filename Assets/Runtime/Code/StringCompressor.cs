using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal static class StringCompressor
    {
        internal static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }

                memoryStream.Position = 0;
                byte[] compressedData = memoryStream.ToArray();
                byte[] gZipBuffer = new byte[compressedData.Length + SizeOfInt32];

                Buffer.BlockCopy(
                        BitConverter.GetBytes(buffer.Length), 0,
                        gZipBuffer, 0, SizeOfInt32
                    );

                Buffer.BlockCopy(
                        compressedData, 0,
                        gZipBuffer, SizeOfInt32, compressedData.Length
                    );

                return Convert.ToBase64String(gZipBuffer);
            }
        }

        internal static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);

            if (gZipBuffer.Length <= SizeOfInt32)
            {
                return string.Empty;
            }

            using (MemoryStream memoryStream = new MemoryStream(gZipBuffer, SizeOfInt32, gZipBuffer.Length - SizeOfInt32))
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
            using (MemoryStream decompressedStream = new MemoryStream())
            {
                gZipStream.CopyTo(decompressedStream);

                return Encoding.UTF8.GetString(decompressedStream.ToArray());
            }
        }

        private const int SizeOfInt32 = sizeof(int);

    }
}
