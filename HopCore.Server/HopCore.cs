using System;
using System.Collections.Generic;
using HopCore.Server.Database;

namespace HopCore.Server {
    public static class HopCore {

        public static readonly IDictionary<Type, ITypeConverter> Converters = new Dictionary<Type, ITypeConverter>();

        public static void AddTypeConvertor<T>() where T : ITypeConverter {
            var instance = Activator.CreateInstance<T>() as ITypeConverter;
            var baseType = typeof(T).BaseType;
            if (baseType != null) {
                var type = baseType.GetGenericArguments()[0];
                Converters.Add(type, instance);
            }
        }

        public static TypeConvertor<T> GetTypeConvertor<T>() {
            return Converters[typeof(T)] as TypeConvertor<T>;
        }

    }
}