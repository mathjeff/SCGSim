using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class DeckMutationStrategy : DeckStrategy
    {
        public TournamentPlayer DesignNewDeck(TournamentPlayer player, Tournament Tournament)
        {
            TournamentPlayer newPlayer = player.Clone();
            // Choose a new random card
            IList<ReadableCard> legalCards = player.Referee.LegalCards;
            WritableCard newCard = legalCards[player.Random.Next(legalCards.Count)].Clone((WritableCard)null);
            newCard.ID = IDFactory.NewID();

            // replace an existing card with the new card
            newPlayer.RemoveCard(newPlayer.MainDeck.First());
            newPlayer.AddCard(newCard);

            return newPlayer;
        }
    }
}
