using System;
using System.Collections.Generic;

public static class EnumExtensions
{
    /// <summary>Whether the given value is defined on its enum type.</summary>
    public static bool IsDefined<T>(this T enumValue) where T : Enum
    {
        return EnumValueCache<T>.DefinedValues.Contains(enumValue);
    }
    
    private static class EnumValueCache<T> where T : Enum
    {
        public static readonly HashSet<T> DefinedValues = new HashSet<T>((T[])Enum.GetValues(typeof(T)));
    }
}