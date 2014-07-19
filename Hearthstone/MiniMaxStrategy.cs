using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class MiniMaxStrategy : Strategy
    {
        public MiniMaxStrategy(GameEvaluator evaluationHeuristic, int numTimeSteps)
        {
            this.GameEvaluator = evaluationHeuristic;
            this.NumTimeSteps = numTimeSteps;
        }

        public GameEffect ChooseBestAction(GameChoice choice, Game game)
        {
            game = game.Clone();
            // Inform the game and its descendents that it is a hypothetical game, so all players use the strategy of the player doing the imagining
            game.Strategy = game.GetStrategy(game.Get_ReadableSnapshot(choice.ControllerID));
            IEnumerable<GameEffect> effects = choice.Options;
            Readable_GamePlayer chooser = game.Get_ReadableSnapshot(choice.ControllerID);
            // create the base game
            Analyzed_GameState rootState = new Analyzed_GameState(game, null, null, chooser.GetID((Readable_GamePlayer)null), this.GameEvaluator.EstimateWinProbabilities(game));
            // Put the first set of choices onto the starting gameState
            this.PutGameOptions(rootState, choice);
            // loop until we run out of time
            while (rootState.NumDescendents < this.NumTimeSteps)
            {
                // Find the current best path and explore it for one more time unit
                this.ProcessOnce(rootState);
            }
            return rootState.FavoriteChild.SourceEffect;
        }
        private void ProcessOnce(Analyzed_GameState gameState)
        {
            // follow the series of game states that we predict to take place in the actual game
            while (gameState.FavoriteChild != null)
            {
                gameState = gameState.FavoriteChild;
            }
            // Generate a child game for each possibility
            Game currentGame = gameState.Game;
            this.PutGameOptions(gameState, currentGame.Get_NextChoice());
        }
        private void PutGameOptions(Analyzed_GameState gameState,  GameChoice choice)
        {
            foreach (GameEffect effect in choice.Options)
            {
                // quickly do a lazy clone of the game (we just use pointers to the other game until anything actually changes)
                Game newGame = gameState.Game.Clone();
                // make a new effect and execute it
                GameEffect clonedEffect = effect.Clone((GameEffect)null);
                clonedEffect.Process(newGame);
                new Analyzed_GameState(newGame, gameState, effect, choice.ControllerID, this.GameEvaluator.EstimateWinProbabilities(newGame));
            }
        }
        public double EstimateWinProbability(Game game, ID<Readable_GamePlayer> playerID)
        {
            // TODO: have this step recurse
            return this.GameEvaluator.EstimateWinProbability(game, playerID);
        }
        /*public Readable_LifeTarget Choose_LifeTarget(Game game, GameEffect effect)
        {
            return game.Get_LifeTargets();
        }*/
        public GameEvaluator GameEvaluator { get; set; }
        public int NumTimeSteps { get; set; }

    }

    public class Analyzed_GameState
    {
        public Analyzed_GameState(Game resultantGame, Analyzed_GameState parent, GameEffect sourceEffect, ID<Readable_GamePlayer> choosingPlayerID, Dictionary<ID<Readable_GamePlayer>, double> winProbabilities)
        {
            this.Game = resultantGame;
            this.SourceEffect = sourceEffect;
            this.ChoosingPlayerID = choosingPlayerID;
            this.winProbabilities = winProbabilities;
            this.Parent = parent;
        }
        public Game Game; // the state of the game
        public GameEffect SourceEffect; // the latest effect that happened to bring us to this game state
        public Dictionary<ID<Readable_GamePlayer>, double> WinProbabilities // our latest estimate of each player's probability of winning
        {
            get
            {
                this.EnsureAggregatesAreUpdated();
                return this.winProbabilities;
            }
        }
        private Dictionary<ID<Readable_GamePlayer>, double> winProbabilities = new Dictionary<ID<Readable_GamePlayer>, double>(); // our latest estimate of each player's probability of winning
        public ID<Readable_GamePlayer> ChoosingPlayerID;
        public HashSet<Analyzed_GameState> Children = new HashSet<Analyzed_GameState>(); // which states to which we can jump to directly from here
        public int NumDescendents
        {
            get
            {
                this.EnsureAggregatesAreUpdated();
                return this.numDescendents;
            }
        }
        private int numDescendents;
        public Analyzed_GameState Parent // the game state that this one came from
        {
            get
            {
                return this.parent;
            }
            set
            {
                if (this.parent != null)
                    this.parent.RemoveChild(this);
                this.parent = value;
                if (this.parent != null)
                    this.parent.AddChild(this);
            }
        }
        public void AddChild(Analyzed_GameState child)
        {
            this.Children.Add(child);
            this.InvalidateAggregates();
        }
        public void RemoveChild(Analyzed_GameState child)
        {
            this.Children.Remove(child);
            this.InvalidateAggregates();
        }
        private void InvalidateAggregates()
        {
            if (this.AreAggregatesValid())
            {
                this.numDescendents = -1;
                this.favoriteChild = null;
                this.winProbabilities = null;
                if (this.parent != null)
                    this.parent.InvalidateAggregates();
            }
        }
        private bool AreAggregatesValid()
        {
            return (this.numDescendents >= 0);
        }
        private void EnsureAggregatesAreUpdated()
        {
            if (!this.AreAggregatesValid())
                this.UpdateAggregatesFromChildren();
        }
        public void UpdateAggregatesFromChildren()
        {
            // update score based on children
            this.winProbabilities = this.FavoriteChild.WinProbabilities;
            // update this.NumDescendents
            int numDescendents = 0;
            foreach (Analyzed_GameState child in this.Children)
            {
                numDescendents += 1 + child.NumDescendents;
            }
            this.numDescendents = numDescendents;
            // update parent too
            if (this.parent != null)
                this.parent.UpdateAggregatesFromChildren();
        }
        private Analyzed_GameState parent;
        public Analyzed_GameState FavoriteChild // The child game that the chooser most prefers
        {
            get
            {
                if (this.favoriteChild == null)
                {
                    double bestScore = -1;
                    foreach (Analyzed_GameState child in this.Children)
                    {
                        Dictionary<ID<Readable_GamePlayer>, double> childProbabilities = child.WinProbabilities;
                        double childScore = childProbabilities[this.ChoosingPlayerID];
                        if (childScore > bestScore)
                        {
                            favoriteChild = child;
                            bestScore = childScore;
                        }
                        if (bestScore >= 1)
                            break;
                    }
                }
                return this.favoriteChild;
            }
        }
        private Analyzed_GameState favoriteChild;

    }
}
