using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class DrawEffect : TriggeredGameEffect<GameEffect>
    {
        // A DrawEffect that has a particular player draw 1 card from his/her deck
        public DrawEffect(ValueProvider<Writable_GamePlayer, Controlled> playerProvider)
        {
            this.Initialize(playerProvider, DrawFromDeck_Provider.FromController(), new ConstantValueProvider<int, Controlled>(1));
        }
        // A configurable DrawEffect that hasn't yet decided who is drawing, where those cards come from, or how many cards to draw
        public DrawEffect(ValueProvider<Writable_GamePlayer, Controlled> playerProvider,
            ValueProvider<ReadableCard, Controlled> cardProvider,
            ValueProvider<int, Controlled> numCards_provider)
        {
            this.Initialize(playerProvider, cardProvider, numCards_provider);
        }
        private void Initialize(ValueProvider<Writable_GamePlayer, Controlled> playerProvider,
            ValueProvider<ReadableCard, Controlled> cardProvider,
            ValueProvider<int, Controlled> numCards_provider)
        {
            this.playerProvider = playerProvider;
            this.cardProvider = cardProvider;
            this.numCards_provider = numCards_provider;
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType)
        {
            return new DrawEffect(this.playerProvider, this.cardProvider, this.numCards_provider);
        }
        public override void Process(Game game)
        {
            Writable_GamePlayer player = null;
            int numCards = this.numCards_provider.GetValue(this, game, default(int));
            if (numCards >= 1)
                player = this.playerProvider.GetValue(this, game, (Writable_GamePlayer)null);
            for (int i = 0; i < numCards; i++)
            {
                // draw another card
                ReadableCard card = this.cardProvider.GetValue(this, game, (ReadableCard)null);
                if (card != null && card.Get_ControllerID() == null)
                {
                    // This only ever happens if the card isn't in the game yet, so it works to just clone it here
                    WritableCard writable = card.Clone((WritableCard)null);
                    writable.ControllerID = this.ControllerID;
                    card = writable;
                }
                player.TryToDrawCard(card, game);
            }
        }
        public override string ToString(Game game)
        {
            int numCards = this.numCards_provider.GetValue(this, game, default(int));
            string cardSource = this.cardProvider.ToString();
            Readable_GamePlayer player = this.playerProvider.GetValue(this, game, (Writable_GamePlayer)null);
            return "Draw " + numCards + " from " + cardSource + " for " + player;
        }
        private ValueProvider<Writable_GamePlayer, Controlled> playerProvider;
        private ValueProvider<ReadableCard, Controlled> cardProvider;
        private ValueProvider<int, Controlled> numCards_provider;
    }
}
