using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Represents Mana, Crystals, Money, Ore, Gas, Minerals, or whatever else can be traded as a commodity
// TODO: replace this implementation with a Dictionary of resource types when implementing Magic: the Gathering
namespace Games
{
    public class Resource
    {
        public Resource(int quantity)
        {
            this.quantity = quantity;
        }
        public Resource Minus(Resource other)
        {
            return new Resource(this.quantity - other.quantity);
        }
        public Resource Plus(Resource other)
        {
            return new Resource(this.quantity + other.quantity);
        }
        // Tells whether this resource is non-negative
        public bool IsValid
        {
            get
            {
                return (this.quantity >= 0);
            }
        }
        // compute the magnitude of this quantity of resources
        public int ToNumber()
        {
            return this.quantity;
        }
        private int quantity;
        //public Dictionary<int, double> quantitiesByType; // TODO: uncomment this when implementing Magic: the Gathering
    }
}
