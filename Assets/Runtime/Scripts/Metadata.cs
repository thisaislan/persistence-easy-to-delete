using System;

namespace Thisaislan.PersistenceEasyToDelete.Metas
{
    internal static class Metadata
    {
        
        internal static  int ByteOffset = 0;
        internal static int ByteDistOffset = 4;

        internal static Type[] BuildInTypes =
        {
            typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float),
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(short), typeof(ushort), typeof(string)
        };
        
    }
}
