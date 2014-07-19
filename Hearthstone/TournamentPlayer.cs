using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class TournamentPlayer
    {
        public TournamentPlayer(Random random, Referee referee)
        {
            this.Referee = referee;
            // currently just makes a random deck
            this.Strategy = new MiniMaxStrategy(new DefaultHearthstoneGameEvaluator(), 100);
            int i;
            LinkedList<ReadableCard> cards = new LinkedList<ReadableCard>();
            IList<ReadableCard> legalCards = referee.LegalCards;
            for (i = 0; i < referee.Starting_DeckSize; i++)
            {
                WritableCard newCard = legalCards[random.Next(legalCards.Count)].Clone((WritableCard)null);
                newCard.ID = IDFactory.NewID();
                cards.AddLast(newCard);
            }
            this.MainDeck = cards;
        }
        public Strategy Strategy { get; set; }
        public IEnumerable<ReadableCard> MainDeck 
        {
            get
            {
                return this.sortedDeck;
            }
            set
            {
                List<ReadableCard> cards = new List<ReadableCard>(value);
                cards.Sort(new CardSorter());
                this.sortedDeck = cards;
            }
        }
        public Referee Referee { get; set; }
        public int NumWins;
        public int NumLosses;
        private List<ReadableCard> sortedDeck;
    }

}
