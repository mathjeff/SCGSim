using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Game
    {
        static int numActions = 0;
        static int nextID;
        private int ID;
        public Game(Referee referee)
        {
            this.Initialize();
            this.TurnOrder = new LinkedList<ID<Readable_GamePlayer>>();
            this.Referee = referee;
        }
        private Game()
        {
            this.Initialize();
        }
        public Game(Game original)
        {
            this.CopyFrom(original);
        }
        private void Initialize()
        {
            this.ID = ++nextID;
            /*this.cardsDrawn.PutWritable(new WriteControlled_Item<Dictionary<ID<ReadableCard>, ReadableCard>, Dictionary<ID<ReadableCard>, WritableCard>>
                (new DictionaryConverter<ID<ReadableCard>, ReadableCard, WritableCard>(new CardConverter())));
            this.cardsDrawn.GetWritable().PutWritable(new Dictionary<ID<ReadableCard>, WritableCard>());
            */
        }
        public void AddPlayer(Readable_GamePlayer player)
        {
            WriteControlled_Item<Readable_GamePlayer, Writable_GamePlayer> playerPair = new WriteControlled_Item<Readable_GamePlayer, Writable_GamePlayer>(new PlayerConverter());
            playerPair.PutReadonly(player);
            this.players.PutReadonly(player);
            this.TurnOrder.AddLast(player.GetID((Readable_GamePlayer)null));
        }
        public LinkedList<ID<Readable_GamePlayer>> TurnOrder { get; set; }
        public void EndTurn()
        {
            ID<Readable_GamePlayer> playerId = this.TurnOrder.First();
            this.TurnOrder.RemoveFirst();
            this.TurnOrder.AddLast(playerId);
            this.StartTurn();
        }
        private void StartTurn()
        {
            this.Referee.NewTurn(this.TurnOrder.First(), this);
        }
        public IEnumerable<Readable_GamePlayer> Players 
        {
            get
            {
                LinkedList<Readable_GamePlayer> players = new LinkedList<Readable_GamePlayer>();
                foreach (ID<Readable_GamePlayer> playerId in this.TurnOrder)
                {
                    players.AddLast(this.players.GetReadable(playerId));
                }
                return players;
            }
        }
        public Readable_GamePlayer ActivePlayer 
        { 
            get 
            {
                return this.players.GetReadable(this.TurnOrder.First());
            }
        }
        public int NumPlayers
        {
            get
            {
                return this.players.Count();
            }
        }
        /*public IEnumerable<Readable_GamePlayer> Players
        {
            get
            {
                List<Readable_GamePlayer> players = new List<Readable_GamePlayer>();
                foreach (WriteControlled_Item<Readable_GamePlayer, Writable_GamePlayer> playerPair in this.players.Values)
                {
                    players.Add(playerPair.GetReadable());
                }
                return players;
            }
        }*/
        public Referee Referee { get; set; } // in charge of which type of game (Hearthstone, Magic, Hearts, or whatever) is being played
        public List<GameEffect> Get_AvailableGameActions(Readable_GamePlayer player)
        {
            return this.Referee.Get_AvailableGameActions(this, player);
        }
        public Readable_GamePlayer GetWinner()
        {
            return this.Referee.GetWinner(this);
        }
        public List<Readable_GamePlayer> GetLosers()
        {
            return this.Referee.GetLosers(this);
        }
        // returns all valid LifeTargets in play (monsters and players)
        public IList<Readable_LifeTarget> Get_LifeTargets()
        {
            List<Readable_LifeTarget> targets = new List<Readable_LifeTarget>();
            foreach (ID<Readable_GamePlayer> playerId in this.players.GetKeys())
            {
                Readable_GamePlayer player = this.players.GetReadable(playerId);
                targets.Add(player);
            }
            foreach (Readable_MonsterCard monster in this.Get_MonstersInPlay())
            {
                targets.Add(monster);
            }
            return targets;
        }
        // returns all monsters in play
        public IList<Readable_MonsterCard> Get_MonstersInPlay()
        {
            List<Readable_MonsterCard> targets = new List<Readable_MonsterCard>();
            foreach (ID<Readable_GamePlayer> playerId in this.players.GetKeys())
            {
                Readable_GamePlayer player = this.players.GetReadable(playerId);
                foreach (ID<Readable_MonsterCard> monsterId in player.Get_MonsterIDsInPlay())
                {
                    Readable_MonsterCard monster = this.Get_ReadableSnapshot(monsterId);
                    targets.Add(monster);
                }
            }
            return targets;
        }

        public Game Clone()
        {
            Game clone = new Game();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(Game source)
        {
            this.cardsDrawn = source.cardsDrawn.Clone();
            this.players = source.players.Clone();
            this.Strategy = source.Strategy;
            this.TurnOrder = new LinkedList<ID<Readable_GamePlayer>>(source.TurnOrder);
            this.Referee = source.Referee;
            this.pendingEffects = new LinkedList<GameChoice>(source.pendingEffects);
        }

        // For hypothetical games that only exist in the heads of certain players, this is that player's strategy to use for simulating the other players
        public GameStrategy Strategy { get; set; }
        public bool ShouldPrint { get; set; }

        public void Play()
        {
            //this.Print();
            // starting the first turn
            this.Referee.NewTurn(this.TurnOrder.First(), this);
            while (this.Referee.GetLosers(this).Count == 0)
            {
                if (this.ShouldPrint)
                {
                    Console.WriteLine("----------------------");
                    this.Print();
                }
                this.PlayOneAction();
                Console.Write("-");
            }
            Console.WriteLine("");
            Console.WriteLine("Game " + numActions + " over");
            numActions++;
            if (numActions % 10 == 0)
            {
                Console.WriteLine("Num Actions: =" + numActions);
                Console.WriteLine("Time = " + DateTime.Now);
            }
            /* // Someone lost; game over
            Console.WriteLine("Game Over. Losing players:");
            foreach (Readable_GamePlayer player in this.Referee.GetLosers(this))
            {
                player.Print(this);
            }
            Readable_GamePlayer winner = this.Referee.GetWinner(this);
            if (winner != null)
            {
                Console.WriteLine("Winner:");
                winner.Print(this);
            }
            else
            {
                Console.WriteLine("No winners");
            }*/
        }
        public void PlayOneAction()
        {
            GameChoice choice = this.Get_NextChoice();
            GameStrategy strategy = this.GetStrategy(this.Get_ReadableSnapshot(choice.ControllerID));
            GameEffect effect = strategy.ChooseBestAction(choice, this);
            if (!(choice.Options.Contains(effect)))
            {
                Console.WriteLine("GameStrategy " + strategy + " made an invalid choice: " + effect);
                effect = strategy.ChooseBestAction(choice, this);
            }
            if (this.ShouldPrint)
            {
                Console.WriteLine(effect.ToString(this));
            }
            effect.Process(this);
        }
        public GameStrategy GetStrategy(Readable_GamePlayer player)
        {
            if (this.Strategy != null)
            {
                // This is a hypothetical game, so all players use the same strategy as the player doing the imagining
                return this.Strategy;
            }
            // This is a real game, so each player can use their own strategy
            return player.Strategy;
        }
        public void AddChoice(GameChoice choice)
        {
            if (choice.Options.Count() == 1)
            {
                // if there's only one option, we might as well resolve it now rather than queuing up a decision to ask the player about later
                GameEffect effect = choice.Options.First();
                effect.Process(this);
            }
            else
            {
                this.pendingEffects.AddLast(choice);
            }
        }
        public GameChoice Get_NextChoice()
        {
            if (this.pendingEffects.Count > 0)
            {
                // a previous effect still requires a player to make a choice, so that must be done before moving on to normal options (like playing more cards, attacking, or ending the turn)
                GameChoice choice = this.pendingEffects.First();
                this.pendingEffects.RemoveFirst();
                return choice;
            }
            // There aren't any effects still requiring user input, so now the active player can choose on of the usual choices (playing a card, attacking, ending the turn or whatever)
            Readable_GamePlayer activePlayer = this.ActivePlayer;
            return new GameChoice(this.Referee.Get_AvailableGameActions(this, activePlayer), activePlayer.GetID((Readable_GamePlayer)null));
        }
        public void DebugCheck()
        {
            foreach (GameChoice choice in this.pendingEffects)
            {
                foreach (GameEffect effect in choice.Options)
                {

                    if (!effect.IsProcessable(this))
                    {
                        throw new ArgumentException("Invalid effect in the pending queue" + effect);

                    }
                }
            }
        }
        public void Print()
        {
            foreach (Readable_GamePlayer player in this.Players)
            {
                player.Print(this);
            }
        }


        #region Retrieving Read-Only Snapshots of Entities

        public ReadableCard Get_ReadableSnapshot(ID<ReadableCard> cardId)
        {
            // request read-only access to a particular card
            ReadableCard card = this.cardsDrawn.GetReadable(cardId);
            return card;
        }
        public Readable_MonsterCard Get_ReadableSnapshot(ID<Readable_MonsterCard> cardId)
        {
            // We don't store a collection of just the monsters, but we do store a collection of all the cards.
            // So, find the card with that ID and convert it to a Readable_MonsterCard
            return this.Get_ReadableSnapshot(cardId.AsType((ReadableCard)null)) as Readable_MonsterCard;
        }

        public Readable_GamePlayer Get_ReadableSnapshot(ID<Readable_GamePlayer> playerId)
        {
            return this.players.GetReadable(playerId);
        }

        public Readable_LifeTarget Get_ReadableSnapshot(ID<Readable_LifeTarget> targetId)
        {
            // First check whether this is a player
            Readable_GamePlayer player;
            bool isPlayer = this.players.TryGetReadable(new ID<Readable_GamePlayer>(targetId.ToInt()), out player);
            if (isPlayer)
                return player;

            // If a life target isn't a player, then it's a card
            ReadableCard card = this.cardsDrawn.GetReadable(targetId.AsType((ReadableCard)null));
            bool isCard = this.cardsDrawn.TryGetReadable(targetId.AsType((ReadableCard)null), out card);
            if (isCard)
                return card as Readable_LifeTarget;
            // error: not found
            return null;
        }

        #endregion

        #region Retrieving Modifiable Entities

        public Writable_MonsterCard GetWritable(ID<Readable_MonsterCard> monsterId)
        {
            WritableCard card;
            this.cardsDrawn.TryGetWritable(monsterId.AsType((ReadableCard)null), out card);
            return card as Writable_MonsterCard;
        }
        // The reason we keep track of both read-only and modifiable entities is to decrease how much we have to clone if we want to copy the game
        public Writable_LifeTarget GetWritable(ID<Readable_LifeTarget> targetId)
        {
            // First check whether this is a player
            Writable_GamePlayer player;
            bool isPlayer = this.players.TryGetWritable(new ID<Readable_GamePlayer>(targetId.ToInt()), out player);
            if (isPlayer)
                return player;
            
            // If a life target isn't a player, then it's a card
            // Get a writable dictionary of writable cards, and from it fetch this particular writable card
            // Request write-access to a particular card
            WritableCard card;
            card = this.cardsDrawn.GetWritable(targetId.AsType((ReadableCard)null));
            bool isCard = this.cardsDrawn.TryGetWritable(targetId.AsType((ReadableCard)null), out card);
            if (isCard)
                return card as Writable_LifeTarget;
            // error: not found
            return null;
        }

        public Writable_GamePlayer GetWritable(ID<Readable_GamePlayer> playerId)
        {
            return this.players.GetWritable(playerId);
        }
        public void AddCard(ReadableCard card)
        {
            this.cardsDrawn.PutReadonly(card);
        }
        #endregion

        // This is a readonly collection or a read-write collection of readable or read-write objects
        /*private WriteControlled_Item<IReadOnlyDictionary<ID<ReadableCard>, ReadableCard>, Dictionary<ID<ReadableCard>, WriteControlled_Item<ReadableCard, WritableCard>>> cardsDrawn 
            = new WriteControlled_Item<IReadOnlyDictionary<ID<ReadableCard>,ReadableCard>,Dictionary<ID<ReadableCard>,WriteControlled_Item<ReadableCard,WritableCard>>>(
                new DictionaryConverter<ID<ReadableCard>, ReadableCard, WritableCard>(new CardConverter()));*/

        private WriteControlled_Set<ReadableCard, WritableCard> cardsDrawn = new WriteControlled_Set<ReadableCard, WritableCard>(new CardConverter());
        /*private WriteControlled_Item<IReadOnlyDictionary<ID<ReadableCard>, ReadableCard>, WriteControlled_Item<Dictionary<ID<ReadableCard>, ReadableCard>, Dictionary<ID<ReadableCard>, WritableCard>>> cardsDrawn
            = new WriteControlled_Item<IReadOnlyDictionary<ID<ReadableCard>, ReadableCard>, WriteControlled_Item<Dictionary<ID<ReadableCard>, ReadableCard>, Dictionary<ID<ReadableCard>, WritableCard>>>
                (new DictionaryConverter<ID<ReadableCard>, ReadableCard, WritableCard>(new CardConverter()));
        */
        // A dictionary of players by id, which keeps track of which players are modified. The dictionary is so small that we shallow-clone it in every cloned game
        private WriteControlled_Set<Readable_GamePlayer, Writable_GamePlayer> players = new WriteControlled_Set<Readable_GamePlayer, Writable_GamePlayer>(new PlayerConverter());
        /*private Dictionary<ID<Readable_GamePlayer>, WriteControlled_Item<Readable_GamePlayer, Writable_GamePlayer>> players 
            = new Dictionary<ID<Readable_GamePlayer>,WriteControlled_Item<Readable_GamePlayer,Writable_GamePlayer>>();*/
        private LinkedList<GameChoice> pendingEffects = new LinkedList<GameChoice>(); // effects that have triggered and require a choice from a player



    }
}
