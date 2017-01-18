using System;
using System.Collections.Generic;

namespace linqprovider
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type enumType = FindIEnumerable(seqType);

            return enumType == null ? seqType : enumType.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type enumType = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (enumType.IsAssignableFrom(seqType))
                    {
                        return enumType;
                    }
                }
            }

            foreach (var iface in seqType.GetInterfaces())
            {
                var enumType = FindIEnumerable(iface);

                if (enumType != null)
                {
                    return enumType;
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }
}