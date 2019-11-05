using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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

    public class AutoSerializedDictionary<TKey, TValue> : IRepository<TKey, TValue>
    {
        private readonly Lazy<ConcurrentDictionary<TKey, TValue>> _dictionary;
        private readonly string _filepath;

        public AutoSerializedDictionary(string filepath)
        {
            _filepath = filepath;
            _dictionary = new Lazy<ConcurrentDictionary<TKey, TValue>>(Deserialize);
        }

        private void StartSerialization()
        {
            Task.Factory.StartNew(Serialize, TaskCreationOptions.AttachedToParent);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Serialize()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filepath);
                if (directory != "")
                {
                    Directory.CreateDirectory(directory);
                }
                using (var writer = new StreamWriter(_filepath, false))
                {
                    var serializer = new XmlSerializer(typeof(KeyValuePair<TKey, TValue>[]));
                    serializer.Serialize(writer, _dictionary.Value.ToArray());
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Failed to create directory: {e}");
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"Failed to serialize dictionary: {e}");
            }
        }

        private ConcurrentDictionary<TKey, TValue> Deserialize()
        {
            try
            {
                using (var reader = new StreamReader(_filepath))
                {
                    var serializer = new XmlSerializer(typeof(KeyValuePair<TKey, TValue>[]));
                    var values = (KeyValuePair<TKey, TValue>[]) serializer.Deserialize(reader);
                    return new ConcurrentDictionary<TKey, TValue>(values);
                }
            }
            catch (Exception e) when (e is FileNotFoundException || e is IOException || e is DirectoryNotFoundException)
            {
                Console.WriteLine($"Failed to open file: {e}");
                return new ConcurrentDictionary<TKey, TValue>();
            }
            catch (InvalidOperationException e)
            {
                // This should be closer to the exception source (XmlSerializer.Deserialize), but meh.
                Console.WriteLine($"Failed to read file: {e}");
                return new ConcurrentDictionary<TKey, TValue>();
            }
        }

        public bool IsEmpty => _dictionary.Value.IsEmpty;
        public int Count => _dictionary.Value.Count;

        public IEnumerable<TKey> Keys => _dictionary.Value.Keys;

        public IEnumerable<TValue> Values => _dictionary.Value.Values;

        public TValue this[TKey key] { get { return _dictionary.Value[key]; } }

        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            var val = _dictionary.Value.AddOrUpdate(key, addValue, updateValueFactory);
            StartSerialization();
            return val;
        }

        public void Clear()
        {
            _dictionary.Value.Clear();
            StartSerialization();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.Value.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue GetOrAdd(TKey key, TValue value)
        {
            var val = _dictionary.Value.GetOrAdd(key, value);
            StartSerialization();
            return val;
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var val = _dictionary.Value.GetOrAdd(key, valueFactory);
            StartSerialization();
            return val;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            var val = _dictionary.Value.TryAdd(key, value);
            StartSerialization();
            return val;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.Value.TryGetValue(key, out value);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            return _dictionary.Value.TryRemove(key, out value);
        }

        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            return _dictionary.Value.TryUpdate(key, newValue, comparisonValue);
        }
    }
}
