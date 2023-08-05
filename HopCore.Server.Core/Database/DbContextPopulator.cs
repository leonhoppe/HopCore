using System;
using HopCore.Server.Database;

namespace HopCore.Server.Core.Database {
    public sealed class DbContextPopulator : IDbContextPopulator {
        public DbContextPopulator() {
            AddDefaultConvertors();
        }
        
        public void PopulateTableStores(IDbContext context) {
            var props = context.GetType().GetProperties();

            foreach (var info in props) {
                var genericType = typeof(DbTable<>).MakeGenericType(info.PropertyType.GetGenericArguments());
                var instance = Activator.CreateInstance(genericType, context.GetConnection(), info.Name, context);
                info.SetValue(context, instance);
            }
        }

        private void AddDefaultConvertors() {
            HopCore.AddTypeConvertor<FloatArrayConvertor>();
            HopCore.AddTypeConvertor<Vector3Convertor>();
            HopCore.AddTypeConvertor<Vector4Convertor>();
        }
    }
}