using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class MiniMaxStrategy : Strategy, GameEvaluator
    {
        public MiniMaxStrategy(GameEvaluator evaluationHeuristic, int numTimeSteps)
        {
            this.GameEvaluator = evaluationHeuristic;
            this.NumTimeSteps = numTimeSteps;
        }


        public GameEffect ChooseBestAction(IEnumerable<GameEffect> effects, Readable_GamePlayer chooser, Game game)
        {
            // Step 1: Clone the game once for each effect
            // Step 2: Compute the favorability of each scenario
            // Step 3: Remove any scenarios that we don't have time to check (based on their depth and their heuristic score)
            // Step 4a: If multiple scenarios remain, recurse into them
            // Step 4b: If one scenario remains, choose it
            // Step 4c: If scenarios remain, choose the most favorable scenario found
            double bestProbability = -1;
            GameEffect bestEffect = null;
            foreach (GameEffect effect in effects)
            {
                // quickly do a lazy clone of the game (we just use pointers to the other game until anything actually changes)
                Game newGame = game.Clone();
                if (newGame.Strategy == null)
                {
                    // inform the hypothetical game that all players in that game will use the strategy of the player doing the thinking
                    newGame.Strategy = chooser.GetStrategy(game);
                }
                // make a new effect and execute it
                GameEffect clonedEffect = effect.Clone((GameEffect)null);
                clonedEffect.Process(newGame);
                // evaluate the desirability of the new situation
                double currentProbability = this.EstimateWinProbability(newGame, chooser.GetID((Readable_GamePlayer)null));
                if (currentProbability > bestProbability)
                {
                    bestEffect = effect;
                    bestProbability = currentProbability;
                }
            }
            return bestEffect;
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
}
