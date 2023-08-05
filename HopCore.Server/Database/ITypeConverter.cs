namespace HopCore.Server.Database {
    public interface ITypeConverter {
        string ConvertForDatabaseInternal(object obj);
        object ConvertFromDatabaseInternal(string raw);
    }

    public abstract class TypeConvertor<T> : ITypeConverter {
        public abstract string ConvertForDatabase(T obj);
        public abstract T ConvertFromDatabase(string raw);
        
        public string ConvertForDatabaseInternal(object obj) {
            return ConvertForDatabase((T)obj);
        }

        public object ConvertFromDatabaseInternal(string raw) {
            return ConvertFromDatabase(raw);
        }
    }
}