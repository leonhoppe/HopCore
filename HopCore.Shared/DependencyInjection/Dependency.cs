using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HopCore.Shared.DependencyInjection {
    public static class Dependency {
        private static IDictionary<Type, object> Dependencies { get; } = new Dictionary<Type, object>();

        public static T Inject<T>() {
            var type = typeof(T);
            if (Dependencies.ContainsKey(type))
                return (T)Dependencies[type];

            return Provide<T>();
        }

        public static T Provide<T>(T instance = default) {
            return Provide<T, T>(instance);
        }

        public static TAccessor Provide<TAccessor, TImplementor>(TImplementor instance = default) where TImplementor : TAccessor {
            var type = typeof(TAccessor);
            if (Dependencies.ContainsKey(type)) {
                throw new ArgumentException($"Dependency {type.Name} already registered!");
            }

            if (EqualityComparer<TImplementor>.Default.Equals(instance, default)) 
                instance = (TImplementor)Activator.CreateInstance(typeof(TImplementor));
        
            var dependencyType = DependencyType.Singleton;
            if (type.GetCustomAttributes().SingleOrDefault(attr => attr is DependencyAttribute) is DependencyAttribute attribute)
                dependencyType = attribute.Type;

            switch (dependencyType) {
                case DependencyType.Scoped:
                    break;
            
                case DependencyType.Singleton:
                default:
                    Dependencies.Add(type, instance);
                    break;
            }
        
            return instance;
        }

        public static void DisposeDependencies() {
            foreach (var dependency in Dependencies.Values) {
                (dependency as IDisposable)?.Dispose();
            }
        }
    }
}