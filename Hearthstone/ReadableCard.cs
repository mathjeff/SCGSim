using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface ReadableCard : Identifiable<ReadableCard>, Controlled
    {
        ID<Readable_GamePlayer> Get_OwnerID();
        String Name { get; }
        Resource GetCost();
        WritableCard Clone(WritableCard outputType);
        bool IsPlayable(Game game);
        void Play(Game game); // casts this card or puts it into play
        String ToString(Game game);
        IEnumerable<GameTrigger<GameEffect>> Get_AfterPlay_Triggers();
    }

    public class CardSorter : Comparer<ReadableCard>
    {
        public override int Compare(ReadableCard card1, ReadableCard card2)
        {
            int comparison = card1.GetCost().ToNumber().CompareTo(card2.GetCost().ToNumber());
            if (comparison != 0)
                return comparison;
            return card1.Name.CompareTo(card2.Name);
        }
    }
}
