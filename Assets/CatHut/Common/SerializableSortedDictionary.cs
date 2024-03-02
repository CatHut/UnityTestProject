using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CatHut
{
    /// <summary>
    /// シリアル化可能なSortedDictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    // SerializableDictionaryを継承する
    [System.Serializable]
    public class SerializableSortedDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>
    {
        // キーの比較器を保持するフィールド
        private IComparer<TKey> comparer;

        // コンストラクターで比較器を受け取る
        public SerializableSortedDictionary(IComparer<TKey> comparer)
        {
            this.comparer = comparer;
        }

        // デフォルトの比較器を使用するコンストラクター
        public SerializableSortedDictionary() : this(Comparer<TKey>.Default) { }

        // 要素を追加するときにキーの順序を保つようにオーバーライドする
        public override void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
            {
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
            }

            // キーの挿入位置を二分探索で求める
            int index = keyValuePairs.BinarySearch(new SerializableKeyValuePair<TKey, TValue>(key, value), new KeyComparer(comparer));

            // 負の値が返された場合は、補数を取って挿入位置とする
            if (index < 0)
            {
                index = ~index;
            }

            // 指定した位置に要素を挿入する
            keyValuePairs.Insert(index, new SerializableKeyValuePair<TKey, TValue>(key, value));
        }

        public new void Add(System.Object obj)
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

        // キーの比較に使用する内部クラス
        private class KeyComparer : IComparer<SerializableKeyValuePair<TKey, TValue>>
        {
            private IComparer<TKey> comparer;

            public KeyComparer(IComparer<TKey> comparer)
            {
                this.comparer = comparer;
            }

            public int Compare(SerializableKeyValuePair<TKey, TValue> x, SerializableKeyValuePair<TKey, TValue> y)
            {
                return comparer.Compare(x.Key, y.Key);
            }
        }

    }
}