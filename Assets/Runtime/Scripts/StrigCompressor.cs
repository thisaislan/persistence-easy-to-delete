using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Thisaislan.PersistenceEasyToDelete.Metas;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal static class StringCompressor
    {
        
        internal static string CompressString(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, Metadata.ByteOffset, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, Metadata.ByteOffset, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + Metadata.ByteDistOffset];
            
            Buffer.BlockCopy(
                    compressedData, Metadata.ByteOffset, gZipBuffer,
                    Metadata.ByteDistOffset, compressedData.Length
                );
            
            Buffer.BlockCopy(
                    BitConverter.GetBytes(buffer.Length), Metadata.ByteOffset, 
                    gZipBuffer, Metadata.ByteOffset, Metadata.ByteDistOffset
                );
            
            return Convert.ToBase64String(gZipBuffer);
        }
        
        internal static string DecompressString(string compressedText)
        {
            var gZipBuffer = Convert.FromBase64String(compressedText);
            
            using (var memoryStream = new MemoryStream())
            {
                var dataLength = BitConverter.ToInt32(gZipBuffer, Metadata.ByteOffset);
                memoryStream.Write(gZipBuffer, Metadata.ByteDistOffset, gZipBuffer.Length - Metadata.ByteDistOffset);

                var buffer = new byte[dataLength];
                memoryStream.Position = Metadata.ByteOffset;
                
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, Metadata.ByteOffset, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        
    }
}