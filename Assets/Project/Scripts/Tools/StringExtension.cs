using System;

namespace LWFlo.Tools
{
    public static class StringExtension
    {
        public static TEnum TryConvertToEnumString<TEnum>(this string enumProp) where TEnum : struct, Enum
        {
            if (Enum.TryParse(enumProp, true, out TEnum parsedEnum))
            {
                return parsedEnum;
            }

            throw new ArgumentException($"'{enumProp}' is not a valid value for enum type '{typeof(TEnum).Name}'.");
        }
    }
}