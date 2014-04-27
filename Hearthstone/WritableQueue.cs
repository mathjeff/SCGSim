using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A Deck that can be modified. The read/write methods are split to allow for a Game to be cloned and not have to deep-copy every element
namespace Games
{
    public class WritableQueue<TValue> : ReadableQueue<TValue> where TValue : Identifiable<TValue>
    {
        #region Constructors etc

        public static WritableQueue<TValue> ShuffledQueue(IEnumerable<TValue> items)
        {
            List<TValue> clonedItems = new List<TValue>(items);
            shuffle<TValue>(clonedItems);
            WritableQueue<TValue> deck = new WritableQueue<TValue>(clonedItems);
            return deck;
        }

        private static void shuffle<T>(IList<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                SwapitemIndices(i, random.Next(i), items);
            }
        }

        private static void SwapitemIndices<T>(int index1, int index2, IList<T> items)
        {
            T temp = items[index1];
            items[index1] = items[index2];
            items[index2] = temp;
        }

        public WritableQueue()
        {
        }

        public WritableQueue(IReadOnlyList<TValue> items)
        {
            List<ID<TValue>> keys = new List<ID<TValue>>();
            Dictionary<ID<TValue>, KeyValuePair<int, TValue>> values = new Dictionary<ID<TValue>, KeyValuePair<int, TValue>>();

            foreach (TValue value in items)
            {
                ID<TValue> key = value.GetID(default(TValue));
                keys.Add(key);
                values[key] = new KeyValuePair<int, TValue>(keys.Count, value);
            }
            this.backingItems = values;
            this.keys = keys;
        }

        public WritableQueue(ReadableQueue<TValue> original)
        {
            this.CopyFrom(original);
        }

        #endregion

        #region Cloning etc

        public WritableQueue<TValue> Clone()
        {
            WritableQueue<TValue> clone = new WritableQueue<TValue>();
            clone.CopyFrom(this);
            return clone;
        }

        public void CopyFrom(ReadableQueue<TValue> other)
        {
            this.backingItems = other.BackingItems;
            this.keys = other.Keys;
            this.nextDrawIndex = other.NextDrawIndex;
        }

        #endregion

        #region Properties etc

        public int NextDrawIndex { get { return this.nextDrawIndex; } }
        public int CurrentNumItems { get { return this.keys.Count - this.nextDrawIndex; } }
        public IReadOnlyDictionary<ID<TValue>, KeyValuePair<int, TValue>> BackingItems { get { return this.backingItems; } }
        public IReadOnlyList<ID<TValue>> Keys { get { return this.keys; } }

        #endregion

        #region Other Methods

        public TValue Dequeue()
        {
            if (this.nextDrawIndex >= this.keys.Count)
                return default(TValue);
            ID<TValue> key = this.keys[this.nextDrawIndex];
            this.nextDrawIndex++;
            return this.backingItems[key].Value;
        }

        public bool TryGetValue(ID<TValue> key, out TValue value)
        {
            // first check whether we ever contained the item
            KeyValuePair<int, TValue> entry;
            bool found = this.backingItems.TryGetValue(key, out entry);
            if (found && entry.Key >= this.nextDrawIndex)
            {
                // the item was found and we haven't removed it yet
                value = entry.Value;
                return true;
            }
            // not found
            value = default(TValue);
            return false;
        }

        #endregion

        private static Random random
        {
            get
            {
                return Randomness.Random;
            }
        }

        private IReadOnlyList<ID<TValue>> keys;
        private int nextDrawIndex = 0;
        private IReadOnlyDictionary<ID<TValue>, KeyValuePair<int, TValue>> backingItems;

    }

    public class QueueConverter<T> : BiConverter<ReadableQueue<T>, WritableQueue<T>> where T:Identifiable<T>
    {
        public WritableQueue<T> Convert(ReadableQueue<T> original)
        {
            return original.Clone();
        }
        public ReadableQueue<T> ConvertBack(WritableQueue<T> original)
        {
            return original;
        }
    }
}
