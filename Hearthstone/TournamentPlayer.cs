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
            this.Random = random;
            this.Referee = referee;
            // make a random deck
            this.Strategy = new MiniMaxStrategy(new DefaultHearthstoneGameEvaluator(), 1000);
            int i;
            LinkedList<ReadableCard> cards = new LinkedList<ReadableCard>();
            IList<ReadableCard> legalCards = referee.LegalCards;
            for (i = 0; i < referee.Starting_DeckSize; i++)
            {
                WritableCard newCard = legalCards[this.Random.Next(legalCards.Count)].Clone((WritableCard)null);
                //WritableCard newCard = legalCards[0].Clone((WritableCard)null);
                newCard.ID = IDFactory.NewID();
                cards.AddLast(newCard);
            }
            this.mainDeck = cards;
            // set up some mutation strategies
            this.MutationCounts = new Dictionary<DeckMutationStrategy, double>();
            this.MutationCounts[new DeckMutationStrategy()] = 6;
        }
        private TournamentPlayer()
        {
        }
        public TournamentPlayer Clone()
        {
            TournamentPlayer clone = new TournamentPlayer();
            clone.Random = this.Random;
            clone.Referee = this.Referee;
            clone.Strategy = this.Strategy;
            clone.mainDeck = new LinkedList<ReadableCard>();
            foreach (ReadableCard card in this.mainDeck)
            {
                clone.mainDeck.AddLast(card.Clone((WritableCard)null));
            }
            clone.NumWins = this.NumWins;
            clone.NumLosses = this.NumLosses;
            return clone;
        }

        public GameStrategy Strategy { get; set; }
        public IEnumerable<ReadableCard> MainDeck { get { return this.mainDeck; } }
        private LinkedList<ReadableCard> mainDeck;
        public void AddCard(ReadableCard card)
        {
            if (this.mainDeck.Contains(card))
            {
                Console.WriteLine("Error: cannot add the same instance of a card that is already in the deck");
            }
            this.mainDeck.AddLast(card);
            this.sortedDeck = null;
            this.NumWins = this.NumLosses = 0;
        }
        public void RemoveCard(ReadableCard card)
        {
            this.mainDeck.Remove(card);
            this.sortedDeck = null;
            this.NumWins = this.NumLosses = 0;
        }
        public IEnumerable<ReadableCard> SortedDeck
        {
            get
            {
                if (this.sortedDeck == null)
                {
                    List<ReadableCard> cards = new List<ReadableCard>(this.MainDeck);
                    cards.Sort(new CardSorter());
                    this.sortedDeck = cards;
                }
                return this.sortedDeck;
            }
        }
        public Referee Referee { get; set; }
        public Dictionary<DeckMutationStrategy, double> MutationCounts; // how many times this particular mutation strategy should be applied to this player
        public TournamentPlayer Mutate(Tournament tournament)
        {
            // determine approximately how many mutations we want to do
            double previousNumMutations = 0;
            foreach (KeyValuePair<DeckMutationStrategy, double> entry in this.MutationCounts)
            {
                previousNumMutations += entry.Value;
            }
            // Choose an exponential random number of mutations to actually do, with mean equal to numMutations
            double randFraction = this.Random.NextDouble();
            double desiredNumMutations = Math.Exp(randFraction * 2 * Math.Log(previousNumMutations));
            if (desiredNumMutations < 2)
                desiredNumMutations = 2;
            // keep track of which mutations are done this time
            Dictionary<DeckMutationStrategy, double> currentMutationCounts = new Dictionary<DeckMutationStrategy, double>(this.MutationCounts.Count);
            foreach (DeckMutationStrategy strategy in this.MutationCounts.Keys)
            {
                currentMutationCounts[strategy] = 0;
            }
            TournamentPlayer player = this.Clone();
            // Now determine which mutations to do
            double numMutations;
            for (numMutations = 0; numMutations < desiredNumMutations; numMutations++)
            {
                // Choose a mutation strategy
                double randomNumber = this.Random.NextDouble() * previousNumMutations;
                foreach (KeyValuePair<DeckMutationStrategy, double> entry in this.MutationCounts)
                {
                    if (randomNumber <= entry.Value)
                    {
                        // mutate using this strategy
                        DeckMutationStrategy strategy = entry.Key;
                        player = strategy.DesignNewDeck(player, tournament);
                        currentMutationCounts[strategy]++;
                        break;
                    }
                    else
                    {
                        // keep looking
                        randomNumber -= entry.Value;
                    }
                }
            }
            // update the mutation counts of the new player so that it knows how it got to its state and it can continue to mutate at a similar rate
            Dictionary<DeckMutationStrategy, double> childMutations = new Dictionary<DeckMutationStrategy, double>();
            foreach (DeckMutationStrategy strategy in currentMutationCounts.Keys)
            {
                // add a little to make sure no strategy gets completely forgotten, and then normalize the counts again
                childMutations[strategy] = (currentMutationCounts[strategy] + 1) * (numMutations / (numMutations + currentMutationCounts.Count()));
            }
            player.MutationCounts = childMutations;
            return player;
        }
        public int NumWins;
        public Random Random;
        public int NumLosses;
        private List<ReadableCard> sortedDeck;
    }

}
