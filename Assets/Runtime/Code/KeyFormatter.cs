using System;
using Thisaislan.PersistenceEasyToDelete.Constants;

namespace Thisaislan.PersistenceEasyToDelete.PedComposition
{
    internal static class KeyFormatter
    {
        internal static string Format(string key, Type type)
        {
            string rawKey = string.Format(Consts.PedKeyFormat, key, type);

            return GetStableHash(rawKey).ToString();
        }

        private static uint GetStableHash(string value)
        {
            unchecked
            {
                uint hash = Consts.FnvHashOffsetBasis;

                foreach (char character in value)
                {
                    hash ^= character;
                    hash *= Consts.FnvHashPrime;
                }

                return hash;
            }
        }
    }
}
