using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class OwnerProvider: ValueProvider<Readable_GamePlayer, ID<ReadableCard>>
    {
        public Readable_GamePlayer GetValue(ID<ReadableCard> cardId, Game game, Readable_GamePlayer outputType)
        {
            ReadableCard card = game.Get_ReadableSnapshot(cardId);
            Readable_GamePlayer owner = game.Get_ReadableSnapshot(card.Get_OwnerID());
            return owner;
        }
    }
}