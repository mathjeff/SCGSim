using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A WriteControlled_Set is a collection that's fast to clone by using references to non-modified collections or elements
// It requires that the caller promises not to modify an elements without first obtaining a writelock (via GetWritable)
// To reduce accidental misuses of this class, the caller can use TReadable to refer to the type of object that doesn't have any setters
namespace Games
{
    public class WriteControlled_Set<TReadable, TWritable> : ReadableSet<TReadable, TWritable> where TReadable : Identifiable<TReadable> where TWritable : Identifiable<TReadable>
    {
        #region Constructors etc

        // Constructor
        public WriteControlled_Set(BiConverter<TReadable, TWritable> subConverter)
        {
            this.subConverter = subConverter;
        }
        // Simulates a deep copy of the collection and all its contents (because the actual deep copy is lazy)
        public WriteControlled_Set<TReadable, TWritable> Clone()
        {
            // make a new empty set
            WriteControlled_Set<TReadable, TWritable> clone = new WriteControlled_Set<TReadable, TWritable>(this.subConverter);
            // ensure that we have a read-only collection of read-only elements
            if (this.readableItems == null)
            {
                // Produce another collection that we won't modify, filled with elements that we won't modify either
                Dictionary<ID<TReadable>, TReadable> readables = new Dictionary<ID<TReadable>, TReadable>();
                // First, for each item on which we still have a write-lock, downgrade to a read-lock
                foreach (ID<TReadable> key in this.writableItems.Keys)
                {
                    this.moveableItems[key] = this.subConverter.ConvertBack(this.writableItems[key]);
                }
                this.writableItems.Clear();
                // Now surrender the write-lock on the collection itself, leaving only read-only access to a collection of read-only elements
                this.readableItems = this.moveableItems;
                this.moveableItems = new Dictionary<ID<TReadable>, TReadable>();
            }
            // now the clone can use the same the collection that we've promised not to modify
            clone.readableItems = this.readableItems;

            // Note that the original will also save a reference to the readonly collection, so calling this function repeatedly is fast
            return clone;
        }

        #endregion

        #region Directly modifying the collection
        // Stores a new object that future callers can modify whenever they want
        public void PutWritable(TWritable writable)
        {
            this.BecomeWritable(); // ensure the collection is writable (but not necessarily the items)
            ID<TReadable> id = writable.GetID(default(TReadable));
            this.writableItems[id] = writable;
            this.moveableItems.Remove(id); // we no longer need the read-only version, so remove it to avoid confusion
        }
        // Stores a new object that future callers can't modify without explicitly requesting write access
        public void PutReadonly(TReadable readable)
        {
            this.BecomeWritable(); // ensure the collection is writable (but not necessarily the items)
            ID<TReadable> ID = readable.GetID(default(TReadable));
            // update our read-only copy, and then remove our writable copy
            this.moveableItems[ID] = readable;
            if (this.writableItems.ContainsKey(ID))
                this.writableItems.Remove(ID);
        }

        // Removes everything
        public void Clear()
        {
            this.readableItems = null;
            this.moveableItems.Clear();
            this.writableItems.Clear();
        }

        // Removes one item from the collection
        public void Remove(ID<TReadable> ID)
        {
            this.BecomeWritable();
            this.moveableItems.Remove(ID);
            this.writableItems.Remove(ID);
        }
        // Makes the collection itself writable (which doesn't necessarily make the items themselves writable)
        private void BecomeWritable()
        {
            if (this.readableItems != null)
            {
                // we have a readonly version of the collection
                if (this.moveableItems.Count == 0 && this.writableItems.Count == 0)
                {
                    // Make a writable collection of elements, where we promise not to modify any individual element
                    foreach (ID<TReadable> ID in this.readableItems.Keys)
                    {
                        this.moveableItems[ID] = this.readableItems[ID];
                    }
                }
                // no longer need the readonly version of the collection, if we're about to modify it
                this.readableItems = null;
            }
        }

        #endregion

        #region Requesting read-write access to items in the collection

        // Requests read/write access to an item by its ID
        public TWritable GetWritable(ID<TReadable> id)
        {
            TWritable writable;
            this.TryGetWritable(id, out writable);
            return writable;
        }

        // Requests read/write access to an item by its ID
        public bool TryGetWritable(ID<TReadable> id, out TWritable writable)
        {
            // If we haven't yet asked for a writable version, then we must ask for one now
            if (!this.writableItems.ContainsKey(id))
            {
                TReadable readable;
                bool present = this.TryGetReadable(id, out readable);
                if (!present)
                {
                    writable = default(TWritable);
                    return false;
                }
                this.BecomeWritable(); // the object may soon be modified, so we must use our own writable collection rather than a shared collection (read-only collection of read-only items)
                this.writableItems[id] = this.subConverter.Convert(readable);
                this.moveableItems.Remove(id); // we don't need the read-only version any more, so remove it to avoid confusion
            }

            // After ensuring we have a writable version, we can just return it
            return this.writableItems.TryGetValue(id, out writable);
        }

        public IEnumerable<ID<TReadable>> GetKeys()
        {
            if (this.readableItems != null)
                return this.readableItems.Keys;
            return this.moveableItems.Keys.Concat(this.writableItems.Keys);
        }
        #endregion

        #region Requesting read-only access to items

        // Request readonly access to an item by its ID
        public TReadable GetReadable(ID<TReadable> id)
        {
            TReadable readable;
            this.TryGetReadable(id, out readable);
            return readable;
        }

        // Request readonly access to an item by its ID
        public bool TryGetReadable(ID<TReadable> id, out TReadable readable)
        {
            // if the collection itself is read-only, then just do a lookup
            if (this.readableItems != null)
                return this.readableItems.TryGetValue(id, out readable);
            bool present;
            present = this.moveableItems.TryGetValue(id, out readable);
            // If we haven't modified the given item, then we can just return the read-only version
            if (present)
                return present;
            TWritable writable;
            present = this.writableItems.TryGetValue(id, out writable);
            if (present)
            {
                // If we've modified the given item, then we have to create a Readable from the modified Writable
                readable = this.subConverter.ConvertBack(writable);
                return true;
            }
            return false;
        }

        // Returns a readonly list of readonly items
        public IEnumerable<TReadable> Get_ReadablesList()
        {
            if (this.readableItems != null)
                return this.readableItems.Values;
            List<TReadable> values = new List<TReadable>();
            foreach (ID<TReadable> key in this.moveableItems.Keys.Concat(this.writableItems.Keys))
            {
                values.Add(this.GetReadable(key));
            }
            return values;
        }

        // Tells how many items are in the collection
        public int Count()
        {
            if (this.readableItems != null)
                return this.readableItems.Count();
            return this.moveableItems.Count + this.writableItems.Count;
        }

        #endregion

        // A collection that we promise not to modify and that we expect to not be modified by others
        // If this collection is non-null, then it's the only one that needs to be scanned to determine whether an element is present
        private IReadOnlyDictionary<ID<TReadable>, TReadable> readableItems = new Dictionary<ID<TReadable>, TReadable>();
        // A collection of items, where we promise not to modify any item in it
        private Dictionary<ID<TReadable>, TReadable> moveableItems = new Dictionary<ID<TReadable>, TReadable>();
        // A collection of items that we can modify whenever we want
        private Dictionary<ID<TReadable>, TWritable> writableItems = new Dictionary<ID<TReadable>, TWritable>();
        private BiConverter<TReadable, TWritable> subConverter;
    }
}
