using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class SpellCard : WritableCard
    {
        public SpellCard(String name, Resource cost) : base(name, cost)
        {
        }
        private SpellCard()
        {
        }
        public GameEffect PlayEffect;

        public SpellCard Clone()
        {
            SpellCard clone = new SpellCard();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(SpellCard other)
        {
            this.PlayEffect = other.PlayEffect;
            base.CopyFrom(other);
        }

    }
}
