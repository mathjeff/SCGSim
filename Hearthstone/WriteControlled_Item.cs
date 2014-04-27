using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A WriteControlled_Item is an item that's fast to clone by using references to the original until it is actually modified
// It requires that the caller promises not to modify an elements without first obtaining a writelock (via GetWritable)
// To reduce accidental misuses of this class, the caller can use TReadable to refer to the type of object that doesn't have any setters
namespace Games
{
    public class WriteControlled_Item<TReadable, TWritable>
    {

        #region Constructors etc

        public WriteControlled_Item(BiConverter<TReadable, TWritable> converter)
        {
            this.converter = converter;
        }

        // Simulates a deep copy of the underlying writable object (because that doesn't happen until explicitly requested later)
        public WriteControlled_Item<TReadable, TWritable> Clone()
        {
            WriteControlled_Item<TReadable, TWritable> clone = new WriteControlled_Item<TReadable,TWritable>(this.converter);
            // use the same reference as the readable value for both this item and the clone
            this.readonlyItem = clone.GetReadable();
            // record that this is the same reference and that neither may modify the shared object
            this.Revoke_WriteAccess();
            return clone;
        }

        #endregion

        #region Modifying the stored object

        // put an item and do not allow future callers to modify it
        public void PutReadonly(TReadable item)
        {
            this.writableItem = default(TWritable);
            this.readonlyItem = item;
        }

        // put an item that future callers may modify
        public void PutWritable(TWritable item)
        {
            this.readonlyItem = default(TReadable);
            this.writableItem = item;
        }

        // Disallow future callers from modifying the stored object
        public void Revoke_WriteAccess()
        {
            this.writableItem = default(TWritable);
        }

        #endregion

        #region Requesting read-write access to the object

        // request a modifiable version of the object
        public TWritable GetWritable() 
        {
            if (this.writableItem == null)
            {
                this.writableItem = this.converter.Convert(this.readonlyItem);
                this.readonlyItem = default(TReadable);
            }
            return this.writableItem;
        }

        #endregion

        #region Requesting read-only access to the object
        // request a read-only version of the object
        public TReadable GetReadable()
        {
            if (this.readonlyItem == null)
                this.readonlyItem = this.converter.ConvertBack(this.writableItem);
            return this.readonlyItem;
        }
        #endregion


        private TReadable readonlyItem;
        private TWritable writableItem;
        private BiConverter<TReadable, TWritable> converter;
    }

    class ListConverter<T> : BiConverter<IReadOnlyList<T>, List<T>>
    {
        public List<T> Convert(IReadOnlyList<T> original)
        {
            return new List<T>(original);
        }
        public IReadOnlyList<T> ConvertBack(List<T> original)
        {
            return original;
        }
    }

   
}
