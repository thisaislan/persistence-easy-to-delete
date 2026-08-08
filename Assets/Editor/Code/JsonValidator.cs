namespace Thisaislan.PersistenceEasyToDelete.Editor
{
    /// <summary>
    /// Result of a JSON syntax verification attempt made by the
    /// <see cref="JsonValidator" /> validation system.
    /// </summary>
    internal enum JsonValidationResult
    {
        /// <summary>
        /// The value is a well-formed JSON document.
        /// </summary>
        Valid,

        /// <summary>
        /// The value looks like JSON but is malformed.
        /// </summary>
        Invalid,

        /// <summary>
        /// The value does not look like JSON at all, so it cannot be
        /// validated as JSON (for example a non-JSON serialization format).
        /// </summary>
        NotValidatable
    }

    /// <summary>
    /// Static JSON syntax verification used by the PedData validation system.
    /// Verifies that a given string is a well-formed JSON document without
    /// needing an <see cref="Thisaislan.PersistenceEasyToDelete.Interfaces.IPedSerializer" />.
    /// </summary>
    internal static class JsonValidator
    {
        internal static JsonValidationResult IsValidJson(string value)
        {
            if (value == null)
            {
                return JsonValidationResult.NotValidatable;
            }

            int index = 0;
            SkipWhiteSpace(value, ref index);

            if (index >= value.Length)
            {
                return JsonValidationResult.NotValidatable;
            }

            if (!IsJsonValueStarter(value[index]))
            {
                return JsonValidationResult.NotValidatable;
            }

            bool isValid = ParseValue(value, ref index);

            if (isValid)
            {
                SkipWhiteSpace(value, ref index);
                isValid = index == value.Length;
            }

            return isValid ? JsonValidationResult.Valid : JsonValidationResult.Invalid;
        }

        private static bool IsJsonValueStarter(char character) =>
            character == '{' || character == '[' || character == '"' ||
            character == 't' || character == 'f' || character == 'n' ||
            character == '-' || (character >= '0' && character <= '9');

        private static bool ParseValue(string value, ref int index)
        {
            if (index >= value.Length)
            {
                return false;
            }

            switch (value[index])
            {
                case '{': return ParseObject(value, ref index);
                case '[': return ParseArray(value, ref index);
                case '"': return ParseString(value, ref index);
                case 't': return ParseLiteral(value, ref index, "true");
                case 'f': return ParseLiteral(value, ref index, "false");
                case 'n': return ParseLiteral(value, ref index, "null");
                default: return ParseNumber(value, ref index);
            }
        }

        private static bool ParseObject(string value, ref int index)
        {
            index++;

            SkipWhiteSpace(value, ref index);

            if (Consume(value, ref index, '}'))
            {
                return true;
            }

            while (true)
            {
                if (!ParseString(value, ref index))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);

                if (!Consume(value, ref index, ':'))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);

                if (!ParseValue(value, ref index))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);

                if (Consume(value, ref index, '}'))
                {
                    return true;
                }

                if (!Consume(value, ref index, ','))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);
            }
        }

        private static bool ParseArray(string value, ref int index)
        {
            index++;

            SkipWhiteSpace(value, ref index);

            if (Consume(value, ref index, ']'))
            {
                return true;
            }

            while (true)
            {
                if (!ParseValue(value, ref index))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);

                if (Consume(value, ref index, ']'))
                {
                    return true;
                }

                if (!Consume(value, ref index, ','))
                {
                    return false;
                }

                SkipWhiteSpace(value, ref index);
            }
        }

        private static bool ParseString(string value, ref int index)
        {
            if (!Consume(value, ref index, '"'))
            {
                return false;
            }

            while (index < value.Length)
            {
                char current = value[index];

                if (current == '"')
                {
                    index++;
                    return true;
                }

                if (current == '\\')
                {
                    index++;

                    if (index >= value.Length)
                    {
                        return false;
                    }

                    char escape = value[index];

                    if (escape == 'u')
                    {
                        if (index + HexEscapeLength >= value.Length)
                        {
                            return false;
                        }

                        for (int i = 1; i <= HexEscapeLength; i++)
                        {
                            if (!IsHexDigit(value[index + i]))
                            {
                                return false;
                            }
                        }

                        index += HexEscapeLength + 1;
                    }
                    else
                    {
                        if (ValidEscapeCharacters.IndexOf(escape) < 0)
                        {
                            return false;
                        }

                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }

            return false;
        }

        private static bool ParseNumber(string value, ref int index)
        {
            int start = index;

            if (index < value.Length && value[index] == '-')
            {
                index++;
            }

            if (index >= value.Length)
            {
                return false;
            }

            if (value[index] == '0')
            {
                index++;
            }
            else if (value[index] >= '1' && value[index] <= '9')
            {
                while (index < value.Length && value[index] >= '0' && value[index] <= '9')
                {
                    index++;
                }
            }
            else
            {
                return false;
            }

            if (index < value.Length && value[index] == '.')
            {
                index++;

                int fractionDigits = 0;

                while (index < value.Length && value[index] >= '0' && value[index] <= '9')
                {
                    index++;
                    fractionDigits++;
                }

                if (fractionDigits == 0)
                {
                    return false;
                }
            }

            if (index < value.Length && (value[index] == 'e' || value[index] == 'E'))
            {
                index++;

                if (index < value.Length && (value[index] == '+' || value[index] == '-'))
                {
                    index++;
                }

                int exponentDigits = 0;

                while (index < value.Length && value[index] >= '0' && value[index] <= '9')
                {
                    index++;
                    exponentDigits++;
                }

                if (exponentDigits == 0)
                {
                    return false;
                }
            }

            return index > start;
        }

        private static bool ParseLiteral(string value, ref int index, string literal)
        {
            foreach (char character in literal)
            {
                if (index >= value.Length || value[index] != character)
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        private static bool Consume(string value, ref int index, char character)
        {
            if (index < value.Length && value[index] == character)
            {
                index++;
                return true;
            }

            return false;
        }

        private static void SkipWhiteSpace(string value, ref int index)
        {
            while (index < value.Length &&
                (value[index] == ' ' || value[index] == '\t' ||
                 value[index] == '\n' || value[index] == '\r'))
            {
                index++;
            }
        }

        private static bool IsHexDigit(char character) =>
            (character >= '0' && character <= '9') ||
            (character >= 'a' && character <= 'f') ||
            (character >= 'A' && character <= 'F');

        private const int HexEscapeLength = 4;
        private const string ValidEscapeCharacters = "\"\\/bfnrt";

    }
}