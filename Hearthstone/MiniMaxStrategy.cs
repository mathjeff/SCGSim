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

        public GameEffect ChooseBestAction(IEnumerable<GameEffect> effects, Readable_GamePlayer chooser, Game game)
        {
            return this.ChooseBestAction(effects, chooser, game, this.NumTimeSteps).EffectToChoose;
        }
        public Analyzed_GameMove ChooseBestAction(IEnumerable<GameEffect> effects, Readable_GamePlayer chooser, Game game, int remainingTime)
        {
            // Step 1: Clone the game once for each effect
            // Step 2: Compute the favorability of each scenario
            // Step 3: Remove any scenarios that we don't have time to check (based on their depth and their heuristic score)
            // Step 4a: If multiple scenarios remain, recurse into them
            // Step 4b: If one scenario remains, choose it
            // Step 4c: If scenarios remain, choose the most favorable scenario found
            Analyzed_GameMove bestMove = null;
            foreach (GameEffect effect in effects)
            {
                // quickly do a lazy clone of the game (we just use pointers to the other game until anything actually changes)
                //game.DebugCheck();
                Game newGame = game.Clone();
                //game.DebugCheck();
                if (newGame.Strategy == null)
                {
                    // inform the hypothetical game that all players in that game will use the strategy of the player doing the thinking
                    newGame.Strategy = chooser.GetStrategy(game);
                }
                // make a new effect and execute it
                GameEffect clonedEffect = effect.Clone((GameEffect)null);
                //game.DebugCheck();
                clonedEffect.Process(newGame);
                //game.DebugCheck();
                Analyzed_GameMove option = new Analyzed_GameMove(newGame, effect);
                // if we have enough time, then recurse

                if (remainingTime > 0 && !(effect is EndTurn_Effect))
                {
                    GameChoice nextChoice = newGame.Get_NextChoice();

                    Analyzed_GameMove subOption = this.ChooseBestAction(nextChoice.Options, newGame.Get_ReadableSnapshot(nextChoice.ControllerID), newGame, remainingTime / nextChoice.Options.Count());
                    option.WinProbabilities = subOption.WinProbabilities;
                }
                else
                {
                    // If we don't have much time left for thinking, then just use the provided heuristic
                    option.WinProbabilities = this.GameEvaluator.EstimateWinProbabilities(newGame);
                }
                double myProbability = option.WinProbabilities[chooser.GetID((Readable_GamePlayer)null)];
                // check whether this move is better than any previously found
                if (bestMove == null || myProbability > bestMove.WinProbabilities[chooser.GetID((Readable_GamePlayer)null)])
                    bestMove = option;
                if (myProbability >= 1)
                    break;
                //game.DebugCheck();
            }
            return bestMove;
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

    public class Analyzed_GameMove
    {
        public Analyzed_GameMove(Game resultantGame, GameEffect effectToChoose)
        {
            this.ResultantGame = resultantGame;
            this.EffectToChoose = effectToChoose;
        }
        public Game ResultantGame;
        public GameEffect EffectToChoose;
        public Dictionary<ID<Readable_GamePlayer>, double> WinProbabilities;
    }
}
