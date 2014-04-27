using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class DrawFromDeck_Provider : ValueProvider<ReadableCard, Writable_GamePlayer>
    {
        public static ValueProvider<ReadableCard, Controlled> FromController()
        {
            ValueProvider<Writable_GamePlayer, Controlled> controllerProvider = new WritableController_Provider();
            ValueProvider<ReadableCard, Writable_GamePlayer> cardProvider = new DrawFromDeck_Provider();

            return new ChainProvider<ReadableCard, Writable_GamePlayer, Controlled>(cardProvider, controllerProvider);
        }

        public ReadableCard GetValue(Writable_GamePlayer player, Game game, ReadableCard outputType)
        {
            return player.Deck.Dequeue();
        }
    }

}
