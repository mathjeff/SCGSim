using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Writable_GamePlayer : Writable_LifeTarget, Readable_GamePlayer
    {
        #region Constructors etc

        public Writable_GamePlayer()
        {
            this.Initialize();
        }

        public Writable_GamePlayer(TournamentPlayer source)
        {
            this.Initialize();
            // copy the deck and keep a pointer to the strategy
            LinkedList<ReadableCard> cards = new LinkedList<ReadableCard>();
            foreach (ReadableCard readableCard in source.MainDeck)
            {
                WritableCard writableCard = readableCard.Clone((WritableCard)null);
                writableCard.ControllerID = this.GetID((Readable_GamePlayer)null);
                cards.AddLast(writableCard);
            }
            this.Deck = WritableQueue<ReadableCard>.ShuffledQueue(cards);
            this.sourcePlayer = source;
        }

        private void Initialize()
        {
            this.ID = IDFactory.NewID();
            this.hand.PutWritable(new List<ID<ReadableCard>>());
            this.MonsterIDsInPlay = new WriteControlled_Item<IReadOnlyList<ID<Readable_MonsterCard>>, List<ID<Readable_MonsterCard>>>(new ListConverter<ID<Readable_MonsterCard>>());
            this.MonsterIDsInPlay.PutWritable(new List<ID<Readable_MonsterCard>>());
        }

        #endregion

        #region Cloning etc

        public Writable_GamePlayer(Writable_GamePlayer original)
        {
            this.CopyFrom(original);
        }

        public void CopyFrom(Readable_GamePlayer original)
        {
            // the hand should be small enough that we can just clone it for the moment
            this.hand.PutReadonly(original.Get_ReadableHand());
            this.Deck = original.GetDeck().Clone();
            this.Health = original.GetHealth();
            this.MaxHealth = original.GetMaxHealth();
            this.NumDrawsSkipped = original.Get_NumDrawsSkipped();
            this.sourcePlayer = original.SourcePlayer;
            this.ID = original.GetID((Readable_GamePlayer)null).ToInt();
            this.MonsterIDsInPlay = new WriteControlled_Item<IReadOnlyList<ID<Readable_MonsterCard>>,List<ID<Readable_MonsterCard>>>(new ListConverter<ID<Readable_MonsterCard>>());
            this.MonsterIDsInPlay.PutReadonly(original.Get_MonsterIDsInPlay());
            this.CurrentResources = original.Get_CurrentResources();
            this.ResourcesPerTurn = original.Get_ResourcesPerTurn();
        }
        public Writable_GamePlayer Clone(Writable_GamePlayer outputType)
        {
            return new Writable_GamePlayer(this);
        }

        #endregion


        #region Properties etc

        public int ID { get; set; }
        public ID<Writable_GamePlayer> GetID(Writable_GamePlayer outputType) { return new ID<Writable_GamePlayer>(this.ID); }
        public ID<Readable_GamePlayer> GetID(Readable_GamePlayer outputType) { return new ID<Readable_GamePlayer>(this.ID); }
        public ID<Readable_LifeTarget> GetID(Readable_LifeTarget outputType) { return new ID<Readable_LifeTarget>(this.ID); }
        public TournamentPlayer SourcePlayer { get { return this.sourcePlayer; } }
        public WritableQueue<ReadableCard> Deck { get; set; }
        public int Damage { get; set; }
        public int GetDamage() { return this.Damage; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int GetHealth() { return this.Health; }
        public int GetMaxHealth() { return this.MaxHealth; }
        public void AddHealth(Specific_LifeEffect effect, Game game) { this.Health = Math.Min(this.Health + effect.AmountToGain, this.MaxHealth); } // the game will check for death later
        public void InitializeHealth(int amount) { this.Health = this.MaxHealth = amount; }
        public List<ReadableCard> Territory { get; set; }
        public Resource CurrentResources = new Resource(0);
        public Resource ResourcesPerTurn = new Resource(0);
        public WriteControlled_Item<IReadOnlyList<ID<Readable_MonsterCard>>, List<ID<Readable_MonsterCard>>> MonsterIDsInPlay { get; set; }
        public IReadOnlyList<ID<Readable_MonsterCard>> Get_MonsterIDsInPlay() { return this.MonsterIDsInPlay.GetReadable(); }
        public IReadOnlyList<ID<ReadableCard>> Get_ReadableHand() { return this.hand.GetReadable(); }
        public List<ID<ReadableCard>> Get_WritableHand() { return this.hand.GetWritable(); }
        public ReadableQueue<ReadableCard> GetDeck() { return this.Deck; }
        public ID<Readable_GamePlayer> NextPlayer_ID { get; set; }
        public ID<Readable_GamePlayer> Get_NextPlayer_ID() { return this.NextPlayer_ID; }
        public Resource Get_CurrentResources() { return this.CurrentResources; }
        public Resource Get_ResourcesPerTurn() { return this.ResourcesPerTurn; }
        public int NumDrawsSkipped { get; set; } // number of times that the player has tried to draw but been unable due to having no cards
        public int Get_NumDrawsSkipped() { return this.NumDrawsSkipped; }

        public int Get_Total_MonsterDamage(Game game)
        {
            int total = 0;
            foreach (ID<Readable_MonsterCard> monsterId in this.MonsterIDsInPlay.GetReadable())
            {
                Readable_MonsterCard monster = game.Get_ReadableSnapshot(monsterId);
                total += monster.GetDamage();
            }
            return total;
        }
        public int Get_Total_MonsterHealth(Game game)
        {
            int total = 0;
            foreach (ID<Readable_MonsterCard> monsterId in this.MonsterIDsInPlay.GetReadable())
            {
                Readable_MonsterCard monster = game.Get_ReadableSnapshot(monsterId);
                total += monster.GetHealth();
            }
            return total;
        }

        #endregion

        public Strategy GetStrategy(Game game)
        {
            if (game.Strategy != null)
            {
                // this is a hypothetical game, so we are being simulated by one particular player and all use the same strategy
                return game.Strategy;
            }
            // This is a real game and we use the strategy assigned to the player
            return this.sourcePlayer.Strategy;
        }
        // chooses and executes the most desirable action from a list of options
        public GameEffect ChooseBestAction(IEnumerable<GameEffect> options, Game game)
        {
            return this.GetStrategy(game).ChooseBestAction(options, this, game);
        }
        // Interestingly enough, the player could do fine without understanding what they're choosing. They could just inspect the board state afterward and evaluate it
        /*public Readable_LifeTarget Choose_LifeTarget(GameEffect sourceEffect)
        {
            return null;
            //return this.SourcePlayer.Strategy.Choose_LifeTarget(sourceEffect.Game, sourceEffect);
        }*/

        public void TryToDrawCard(ReadableCard card, Game game)
        {
            this.Referee.AddCardToHand(card, this, game);
        }
        public void DrawCard(ReadableCard card, Game game)
        {
            this.hand.GetWritable().Add(card.GetID((ReadableCard)null));
            game.AddCard(card);
        }

        public int Get_NumCardsInHand()
        {
            return this.hand.GetReadable().Count;
        }
        public void Attack(ID<Readable_LifeTarget> target, Game game)
        {
            throw new NotImplementedException("Players that attack need to consume some durability from their weapon");
        }

        public void Print(Game game)
        {
            Console.WriteLine("Player " + this.ID + " : " + this.Health + " health, " + this.CurrentResources.ToNumber() + "/" + this.ResourcesPerTurn.ToNumber() + " resources");
            IEnumerable<ID<ReadableCard>> hand = this.hand.GetReadable();
            Console.WriteLine(" Hand:(" + hand.Count() + ")");
            foreach (ID<ReadableCard> cardId in this.hand.GetReadable())
            {
                ReadableCard card = game.Get_ReadableSnapshot(cardId);
                Console.WriteLine(card.ToString(game));
            }
            Console.WriteLine(" Field:");
            foreach (ID<Readable_MonsterCard> cardId in this.MonsterIDsInPlay.GetReadable())
            {
                Readable_MonsterCard card = game.Get_ReadableSnapshot(cardId);
                Console.WriteLine(card.ToString(game));
            }
        }

        private Referee Referee
        {
            get
            {
                return this.sourcePlayer.Referee;
            }
        }

        private TournamentPlayer sourcePlayer;


        private WriteControlled_Item<IReadOnlyList<ID<ReadableCard>>, List<ID<ReadableCard>>> hand = 
            new WriteControlled_Item<IReadOnlyList<ID<ReadableCard>>,List<ID<ReadableCard>>>(new ListConverter<ID<ReadableCard>>());
        
    }

    public class PlayerConverter : BiConverter<Readable_GamePlayer, Writable_GamePlayer>
    {
        public Writable_GamePlayer Convert(Readable_GamePlayer original)
        {
            return original.Clone((Writable_GamePlayer)null);
        }
        public Readable_GamePlayer ConvertBack(Writable_GamePlayer original)
        {
            return original;
        }
    }
}
