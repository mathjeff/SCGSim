using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Knows the rules of Hearthstone
namespace Games
{
    class HearthstoneReferee : Referee
    {
        
        public HearthstoneReferee()
        {
            this.StartingHealth = 30;
            this.Starting_HandSize = 3;
            this.Max_HandSize = 10;
            this.legalCards = new List<ReadableCard>{LeperGnome, ArgentSquire, ZombieChow, ElvenArcher,
                BloodfenRaptor, HauntedCreeper, LootHorder, Wrath,
                EarthenRingFarseer, IronfurGrizzly, ArcaneIntellect, FanOfKnives,
                ChillwindYeti, SenjinShieldMasta, Fireball, Hellfire,
                AzureDrake, SludgeBelcher, Starfall, SilverHandKnight,
                Sunwalker, CairneBloodhoof, FireElemental, Starfire,
                WarGolem, Sprint, Flamestrike,
                IronbarkProtector,
                Cenarius};
            //this.legalCards = new List<ReadableCard>{LeperGnome, ElvenArcher};
        }
        public List<GameEffect> Get_AvailableGameActions(Game game, Readable_GamePlayer player)
        {
            // This function only gets called when there are no effects in progress (like choosing the target of a triggered effect).
            List<GameEffect> options = new List<GameEffect>();
            // So, a player has these types of options: 1. Play a card. 2. Attack with a monster. 3. Activate their special ability. 4. End their turn
            // Let the player play any card
            foreach (ID<ReadableCard> cardId in player.Get_ReadableHand())
            {
                ReadableCard card = game.Get_ReadableSnapshot(cardId);
                if (card.IsPlayable(game))
                    options.Add(new PlayCard_Effect(card.GetID((ReadableCard)null)));
            }
            // Let the player attack with any monster
            IEnumerable<ID<Readable_MonsterCard>> availableAttacker_IDs = player.Get_MonsterIDsInPlay();
            // first figure out which monsters can be attacked (if any monsters have Taunt, they are the only ones that may be attacked)
            foreach (ID<Readable_GamePlayer> playerId in game.TurnOrder)
            {
                // make sure this is a different player
                if (!playerId.Equals(player.GetID((Readable_GamePlayer)null)))
                {
                    LinkedList<ID<Readable_LifeTarget>> requiredTarget_IDs = new LinkedList<ID<Readable_LifeTarget>>();
                    LinkedList<ID<Readable_LifeTarget>> allTarget_Ids = new LinkedList<ID<Readable_LifeTarget>>();
                    Readable_GamePlayer controller = game.Get_ReadableSnapshot(playerId);
                    foreach (ID<Readable_MonsterCard> monsterId in controller.Get_MonsterIDsInPlay())
                    {
                        Readable_MonsterCard monster = game.Get_ReadableSnapshot(monsterId);
                        ID<Readable_LifeTarget> convertedID = monster.GetID((Readable_LifeTarget)null);
                        allTarget_Ids.AddLast(convertedID);
                        if (monster.Get_MustBeAttacked())
                            requiredTarget_IDs.AddLast(convertedID);
                    }
                    if (requiredTarget_IDs.Count != 0)
                    {
                        // There is a monster with taunt, so the only valid targets are the monsters with taunt
                        allTarget_Ids = requiredTarget_IDs;
                    }
                    else
                    {
                        // There are no monsters with taunt, so the valid targets are all monsters and the opponent too
                        allTarget_Ids.AddLast(controller.GetID((Readable_LifeTarget)null));
                    }
                    // Now allow each monster to attack each available target
                    foreach (ID<Readable_MonsterCard> attackerId in availableAttacker_IDs)
                    {
                        if (game.Get_ReadableSnapshot(attackerId).Get_CanAttack())
                        {
                            foreach (ID<Readable_LifeTarget> targetId in allTarget_Ids)
                            {
                                options.Add(new AttackEffect(attackerId.AsType((Readable_LifeTarget)null), targetId));
                            }
                        }
                    }
                }
            }

            // Let the player end their turn
            options.Add(new EndTurn_Effect(player.GetID((Readable_GamePlayer)null)));
            return options;
        }
        public Game NewGame(TournamentPlayer player1, TournamentPlayer player2)
        {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            players.Add(player1);
            players.Add(player2);
            foreach (ReadableCard card in player1.MainDeck)
            {
                if (player2.MainDeck.Contains(card))
                {
                    Console.WriteLine("Error: two players have the same instance of a card in their deck");
                }
            }
            return this.NewGame(players);
        }
        public Game NewGame(List<TournamentPlayer> players)
        {
            if (players.Count != 2)
            {
                throw new ArgumentException("Hearthstone games must have exactly 2 players");
            }
            Game game = new Game(this);
            int numBonusCards = 0;
            foreach (TournamentPlayer templatePlayer in players)
            {
                if (templatePlayer.MainDeck.Count() != this.Starting_DeckSize)
                    throw new ArgumentException("Hearthstone decks must start with exactly 30 cards");
                Writable_GamePlayer newPlayer = new Writable_GamePlayer(templatePlayer);
                newPlayer.InitializeHealth(30);
                game.AddPlayer(newPlayer);
                // draw a bunch of cards from the player's deck
                DrawEffect drawEffect = new DrawEffect(new ConstantValueProvider<Writable_GamePlayer, Controlled>(newPlayer), DrawFromDeck_Provider.FromController(),
                    new ConstantValueProvider<int, Controlled>(this.Starting_HandSize + numBonusCards));
                drawEffect.ControllerID = newPlayer.GetID((Readable_GamePlayer)null);
                drawEffect.Process(game);
                // potentially draw a Coin too
                DrawEffect bonusCards = new DrawEffect(new ConstantValueProvider<Writable_GamePlayer, Controlled>(newPlayer), new ConstantValueProvider<ReadableCard, Controlled>(HearthstoneReferee.Coin),
                    new ConstantValueProvider<int, Controlled>(numBonusCards));
                bonusCards.ControllerID = newPlayer.GetID((Readable_GamePlayer)null);
                bonusCards.Process(game);
                // each player gets one more card than the previous and one additional Coin
                numBonusCards++;
            }
            return game;
        }
        public void AddHealth(int amount, Writable_GamePlayer player)
        {
            int newHealth = player.Health + amount;
            if (newHealth > this.MaxHealth)
                newHealth = this.MaxHealth;
            player.Health = newHealth;
            // we'll check afterward for game losses, in case multiple players lose during the same effect
        }

        public bool IsPlayable(Readable_MonsterCard card, Game game)
        {
            Readable_GamePlayer controller = game.Get_ReadableSnapshot(card.Get_ControllerID());
            IEnumerable<ID<Readable_MonsterCard>> cardsInPlay = controller.Get_MonsterIDsInPlay();
            if (cardsInPlay.Count() < 7)
                return true;
            return false;
        }
        public void NewTurn(ID<Readable_GamePlayer> playerID, Game game)
        {
            Writable_GamePlayer player = game.GetWritable(playerID);
            // gain 1 crystal
            if (player.ResourcesPerTurn.ToNumber() < 10)
                player.ResourcesPerTurn = player.ResourcesPerTurn.Plus(new Resource(1));
            // replenish existing crystals
            player.CurrentResources = player.ResourcesPerTurn;
            // give one attack to each monster
            foreach (ID<Readable_MonsterCard> monsterId in player.MonsterIDsInPlay.GetReadable())
            {
                game.GetWritable(monsterId).NumAttacksRemaining = 1;
            }
            // draw a card if there's room
            this.AddCardToHand(player.Deck.Dequeue(), player, game);
        }

        public Readable_GamePlayer GetWinner(Game game)
        {
            List<Readable_GamePlayer> losers = this.GetLosers(game);
            List<Readable_GamePlayer> winners = new List<Readable_GamePlayer>();
            foreach (Readable_GamePlayer player in game.Players)
            {
                if (!losers.Contains(player))
                {
                    winners.Add(player);
                }
            }
            if (winners.Count == 1)
            {
                return winners[0];
            }
            return null;
        }
        public List<Readable_GamePlayer> GetLosers(Game game)
        {
            List<Readable_GamePlayer> losers = new List<Readable_GamePlayer>();
            foreach (Readable_GamePlayer player in game.Players)
            {
                if (player.GetHealth() <= 0)
                {
                    losers.Add(player);
                }
            }
            return losers;
        }
        public void AddCardToHand(ReadableCard card, Writable_GamePlayer player, Game game)
        {
            if (player.Get_ReadableHand().Count >= this.Max_HandSize)
                return;
            if (card == null)
            {
                // take damage for having no cards left
                player.NumDrawsSkipped++;
                player.AddHealth(new Specific_LifeEffect(null, -player.NumDrawsSkipped), game);
            }
            else
            {
                player.DrawCard(card, game);
            }
        }
        public int StartingHealth { get; set; }
        public int MaxHealth
        {
            get
            {
                return this.StartingHealth;
            }
        }
        public int Starting_HandSize { get; set; }
        public int Max_HandSize { get; set; }
        public int Starting_DeckSize { get { return 30; } }
        private List<ReadableCard> legalCards;


        // Lots of cards
        public IList<ReadableCard> LegalCards
        {
            get
            {
                return this.legalCards;
            }
        }
        public static ReadableCard Coin
        {
            get
            {
                // The card named "Coin" that costs 0
                SpellCard card = new SpellCard("Coin", new Resource(0));
                // Make a trigger that happens after playing this card, which adds 1 resource this turn to the controller of the effect
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new ResourceEffect(new ConstantValueProvider<Resource, Controlled>(new Resource(1)), new WritableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard ZombieChow
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Zombie Chow", new Resource(1), 2, 3);
                ValueProvider<IList<Readable_LifeTarget>, Controlled> controllerProvider = new OpponentsProvider();
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(5), new OpponentsProvider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard ArgentSquire
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Argent Squire", new Resource(1), 1, 1);
                card.Add_SingleUse_Shield();
                return card;
            }
        }

        public static Readable_MonsterCard LeperGnome
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Leper Gnome", new Resource(1), 2, 1);
                // 2 damage to the opponent when it dies
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-2), new OpponentsProvider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard ElvenArcher
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Elven Archer", new Resource(1), 1, 1);
                // Deals 1 damage when it comes into play
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-1), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard BloodfenRaptor
        {
            get
            {
                return new Writable_MonsterCard("Bloodfen Raptor", new Resource(2), 3, 2);
            }
        }

        public static Readable_MonsterCard RiverCrocolisk
        {
            get
            {
                return new Writable_MonsterCard("River Crocolisk", new Resource(2), 2, 3);
            }
        }

        public static Readable_MonsterCard HauntedCreeper
        {
            get
            {
                // There is a card called "Haunted Creeper" that costs 2 and is a 1/2
                Writable_MonsterCard card = new Writable_MonsterCard("Haunted Creeper", new Resource(2), 1, 2);
                // Add a trigger that triggers after this monster dies, which generates an effect that spawns two 1/1 monsters for the controller of this monster
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(new SpawnMonster_Effect(new Writable_MonsterCard("Insect", new Resource(1), 1, 1), new WritableController_Provider(), new ConstantValueProvider<int, Controlled>(2))));
                return card;
            }
        }

        public static Readable_MonsterCard LootHorder
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Loot Horder", new Resource(2), 2, 1);
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new WritableController_Provider())));
                return card;
            }
        }

        public static SpellCard Wrath
        {
            get
            {
                SpellCard card = new SpellCard("Wrath", new Resource(2));

                GameEffect bigDamage = LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-3), new AllMonsters_Provider(), new ReadableController_Provider());
                
                GameEffect smallDamage = LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-1), new AllMonsters_Provider(), new ReadableController_Provider());
                GameEffect draw = new DrawEffect(new WritableController_Provider());

                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new ChoiceEffect(bigDamage, new Composite_GameEffect(smallDamage, draw))));
                return card;
            }
        }


        public static Readable_MonsterCard EarthenRingFarseer
        {
            get
            {
                // There is a monster called "Earthen Ring Farseer" that costs 3 and is a 3/3
                Writable_MonsterCard card = new Writable_MonsterCard("Earthen Ring Farseer", new Resource(3), 3, 3);
                // Make a trigger that triggers after playing this card, which generates an effect that provides 3 life to the controller of the effect
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(3), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                return card;
            }
        }
        public static Readable_MonsterCard IronfurGrizzly
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Ironfur Grizzly", new Resource(3), 3, 3);
                card.MustBeAttacked = true;
                return card;
            }
        }

        public static SpellCard ArcaneIntellect
        {
            get
            {
                SpellCard card = new SpellCard("Arcane Intellect", new Resource(3));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new WritableController_Provider(), DrawFromDeck_Provider.FromController(), new ConstantValueProvider<int, Controlled>(2))));
                return card;
            }
        }

        public static SpellCard FanOfKnives
        {
            get
            {
                SpellCard card = new SpellCard("Fan of Knives", new Resource(3));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Blanket(new ConstantValueProvider<int, Controlled>(-1), new EnemyMonsters_Provider(), new ReadableController_Provider())));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect()));
                return card;
            }
        }

        /*public static Readable_MonsterCard KingMukla
        { 
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("King Mukla", new Resource(3), 5, 5);
                //card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new OpponentsProvider(), new ConstantValueProvider<ReadableCard, Controlled>(new SpellCard))))
                return card;
            }
        }*/

        public static Readable_MonsterCard ChillwindYeti
        {
            get
            {
                return new Writable_MonsterCard("Chillwind Yeti", new Resource(4), 4, 5);

            }
        }
        public static Readable_MonsterCard SenjinShieldMasta
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Sen'jin SheildMasta", new Resource(4), 3, 5);
                card.MustBeAttacked = true;
                return card;
            }
        }

        public static SpellCard Fireball
        {
            get
            {
                SpellCard card = new SpellCard("Fireball", new Resource(4));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-6), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static SpellCard Hellfire
        {
            get
            {
                SpellCard card = new SpellCard("Hellfire", new Resource(4));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Blanket(new ConstantValueProvider<int, Controlled>(-3), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard AzureDrake
        {
            get
            {
                // There is a card called "Azure Drake" that costs 5 and is a 4/4
                Writable_MonsterCard card = new Writable_MonsterCard("Azure Drake", new Resource(5), 4, 4);
                // Make a trigger that triggers after playing this card, and has the effect's controller draw one card from his/her deck
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new WritableController_Provider())));
                // TODO: add in the spell damage effect from Azure Drake once the engine works
                return card;
            }
        }

        public static Readable_MonsterCard SludgeBelcher
        {
            get
            {
                // There is a card called "Sludge Belcher" that costs 5 and is a 3/5
                Writable_MonsterCard card = new Writable_MonsterCard("Sludge Belcher", new Resource(5), 3, 5);
                card.MustBeAttacked = true; // Taunt
                // Create a 1/2 taunt for referencing in a moment
                Writable_MonsterCard slime = new Writable_MonsterCard("Slime", new Resource(1), 1, 2);
                slime.MustBeAttacked = true;
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(new SpawnMonster_Effect(slime, new WritableController_Provider())));
                return card;
            }
        }

        public static SpellCard Starfall
        {
            get
            {
                SpellCard card = new SpellCard("Starfall", new Resource(5));
                GameEffect blanket = LifeEffect.Blanket(new ConstantValueProvider<int, Controlled>(-2), new EnemyMonsters_Provider(), new ReadableController_Provider());
                GameEffect target = LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-5), new EnemyMonsters_Provider(), new ReadableController_Provider());
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new ChoiceEffect(blanket, target)));
                return card;
            }
        }

        public static Readable_MonsterCard SilverHandKnight
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Silver Hand Knight", new Resource(5), 4, 4);
                Writable_MonsterCard squire = new Writable_MonsterCard("Squire", new Resource(1), 2, 2);
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new SpawnMonster_Effect(squire)));
                return card;
            }
        }

        public static Readable_MonsterCard Sunwalker
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Sunwalker", new Resource(6), 4, 5);
                card.Add_SingleUse_Shield(); // Divine Shield
                card.MustBeAttacked = true;  // Taunt
                return card;
            }
        }

        public static Readable_MonsterCard CairneBloodhoof
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Cairne Bloodhoof", new Resource(6), 4, 5);
                Writable_MonsterCard reincarnation = new Writable_MonsterCard("Baine Bloodhoof", new Resource(4), 4, 5);
                card.Add_AfterDeath_Trigger(new GameTrigger<GameEffect>(new SpawnMonster_Effect(reincarnation, new WritableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard FireElemental
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Fire Elemental", new Resource(6), 6, 5);
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-3), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                return card;
            }
        }

        public static SpellCard Starfire
        {
            get
            {
                SpellCard card = new SpellCard("Starfire", new Resource(6));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Targeted(new ConstantValueProvider<int, Controlled>(-5), new LifeTarget_Choices_Provider(), new ReadableController_Provider())));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new WritableController_Provider())));
                return card;
            }
        }

        public static Readable_MonsterCard WarGolem
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("War Golemn", new Resource(7), 7, 7);
                return card;
            }
        }

        public static SpellCard Sprint
        {
            get
            {
                SpellCard card = new SpellCard("Sprint", new Resource(7));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new DrawEffect(new WritableController_Provider(), DrawFromDeck_Provider.FromController(), new ConstantValueProvider<int, Controlled>(4))));
                return card;
            }
        }

        public static SpellCard Flamestrike
        {
            get
            {
                SpellCard card = new SpellCard("Flamestrike", new Resource(7));
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(LifeEffect.Blanket(new ConstantValueProvider<int, Controlled>(-4), new EnemyMonsters_Provider(), new ReadableController_Provider())));
                return card;
            }

        }


        public static Readable_MonsterCard IronbarkProtector
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Ironbark Protector", new Resource(8), 8, 8);
                card.MustBeAttacked = true;
                return card;
            }
        }

        public static Readable_MonsterCard Cenarius
        {
            get
            {
                Writable_MonsterCard card = new Writable_MonsterCard("Cenarius", new Resource(9), 5, 8);
                Writable_MonsterCard treant = new Writable_MonsterCard("Treant", new Resource(2), 2, 2);
                treant.MustBeAttacked = true;
                card.Add_AfterPlayCard_Trigger(new GameTrigger<GameEffect>(new SpawnMonster_Effect(treant, new WritableController_Provider(), new ConstantValueProvider<int, Controlled>(2))));
                return card;
            }
        }
    }
}
