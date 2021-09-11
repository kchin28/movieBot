using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace dbot.Persistence
{
    public interface IRepository<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>
    {
        bool IsEmpty { get; }
        void Clear();
        TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory);
        bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue);
        bool TryAdd(TKey key, TValue value);
        bool TryRemove(TKey key, out TValue value);
        TValue GetOrAdd(TKey key, TValue value);
        TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory);
    }

}