using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CatHut
{
    public static class TypeNames
    {
        public static readonly IReadOnlyList<string> ValueTypes = new List<string> {
            "uint",    // UnsignedInt
            "int",     // Int
            "short",   // Short
            "ushort",  // UnsignedShort
            "long",    // Long
            "ulong",   // UnsignedLong
            "float",   // Float
            "double",  // Double
            "char",    // Char
            "string",  // String
            "bool",    // Boolean
            "byte",    // Byte
            "sbyte"    // SByte
        }.AsReadOnly();
    }
}