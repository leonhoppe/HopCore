using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HopCore.Server.Database {
    public interface IDbTable<T> : IEnumerable<T> {

        T Get(in object key);

        void Add(T item);

        void Update(T item);

        void Clear();

        bool Contains(in T item);

        Task<bool> ContainsAsync(T item);

        bool Remove(in T item);

        Task<bool> RemoveAsync(T item);

        int Count { get; }

        void Insert(in int index, in T item);
        
        T this[object key] { get; set; }
    }
}