using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif


namespace CatHut
{
    /// <summary>
    /// シリアル化可能なDictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : IEnumerable<SerializableKeyValuePair<TKey, TValue>>
    {
#if UNITY_5_3_OR_NEWER
        [SerializeField]
#endif
        protected List<SerializableKeyValuePair<TKey, TValue>> keyValuePairs = new List<SerializableKeyValuePair<TKey, TValue>>();

        public virtual void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the dictionary. key:" + key.ToString());
            }

            keyValuePairs.Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key)
        {
            for (int i = 0; i < keyValuePairs.Count; i++)
            {
                if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
                {
                    keyValuePairs.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<TKey> Keys
        {
            get { return keyValuePairs.Select(kvp => kvp.Key); }
        }

        public IEnumerable<TValue> Values
        {
            get { return keyValuePairs.Select(kvp => kvp.Value); }
        }

        public int Count
        {
            get { return keyValuePairs.Count; }
        }

        public TValue this[TKey key]
        {
            get
            {
                foreach (var kvp in keyValuePairs)
                {
                    if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                    {
                        return kvp.Value;
                    }
                }

                throw new KeyNotFoundException("The given key was not found in the dictionary. key:" + key.ToString());
            }

            set
            {
                // キーが存在する場合は値を更新する
                for (int i = 0; i < keyValuePairs.Count; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(keyValuePairs[i].Key, key))
                    {
                        keyValuePairs[i] = new SerializableKeyValuePair<TKey, TValue>(key, value);
                        return;
                    }
                }

                // キーが存在しない場合は要素を追加する
                Add(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<SerializableKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return keyValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // XMLシリアライザが要求するAdd(System.Object)メソッドを実装する
        public void Add(System.Object obj)
        {
            // 引数をSerializableKeyValuePair<TKey, TValue>にキャストする
            SerializableKeyValuePair<TKey, TValue> pair = obj as SerializableKeyValuePair<TKey, TValue>;

            // キャストに失敗した場合は例外をスローする
            if (pair == null)
            {
                throw new ArgumentException("Invalid argument type for Add method.");
            }

            // キャストに成功した場合はAdd(TKey key, TValue value)を呼び出す
            Add(pair.Key, pair.Value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var kvp in keyValuePairs)
            {
                if (EqualityComparer<TKey>.Default.Equals(kvp.Key, key))
                {
                    value = kvp.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

    }
}
