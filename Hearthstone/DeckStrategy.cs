using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface DeckStrategy
    {
        // Make a new deck (typically similar to the player's existing deck)
        TournamentPlayer DesignNewDeck(TournamentPlayer player, Tournament tournament);
    }
}
