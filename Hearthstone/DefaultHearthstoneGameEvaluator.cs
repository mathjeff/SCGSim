using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class DefaultHearthstoneGameEvaluator : GameEvaluator
    {
        public double EstimateWinProbability(Game game, ID<Readable_GamePlayer> playerId)
        {
            Readable_GamePlayer player = game.Get_ReadableSnapshot(playerId);
            // first check for obvious things like whether one player has already lost
            if (game.GetWinner() != null)
            {
                if (game.GetWinner() == player)
                    return 1;
                else
                    return 0;
            }
            if (game.GetLosers().Count == game.NumPlayers)
            {
                return 1.0 / game.NumPlayers;
            }
            // Now compute a heuristic based on how much stuff each player has
            double totalScore = 0;
            double playerScore = 0;
            double activePlayer = 1;
            foreach (Readable_GamePlayer candidate in game.Players)
            {
                // Exchange rates:
                // n power and n toughness is worth n mana
                // losing 25% of your life is worth 2 mana
                // 1 crystal per turn is worth 2 mana now.
                // n cards in hand is worth n mana (the cards often generate n mana worth of value (plus the mana spent to play them) even though they often cost 2*n to draw them)
                // Being the active player is worth 0.5 cards and 0.5 mana/turn, but the active player has received 1 additional card and 1 additional mana, so these must each be subtracted instead
                // This results in this score (equivalent amount of mana this turn):
                // mana+manaPerTurn*2++((power+toughness)/2)+(log(life)/log(4/3))+(activePlayer?)*-1.5+handSize

                /*
                double score = candidate.Get_CurrentResources().ToNumber() * activePlayer / 2 + candidate.Get_ResourcesPerTurn().ToNumber() - activePlayer * 2 +
                    (candidate.Get_Total_MonsterDamage(game) + candidate.Get_Total_MonsterHealth(game)) / 2 + Math.Log(candidate.GetHealth()) / Math.Log(4.0 / 3.0);
                */
                double score = candidate.Get_CurrentResources().ToNumber() * activePlayer + candidate.Get_ResourcesPerTurn().ToNumber() * 2
                    + (candidate.Get_Total_MonsterDamage(game) + candidate.Get_Total_MonsterHealth(game)) / 2 + Math.Log(candidate.GetHealth()) / Math.Log(4.0 / 3.0) + activePlayer * -1.5
                    + candidate.Get_ReadableHand().Count;
                
                totalScore += score;
                if (candidate == player)
                    playerScore = score;
                activePlayer *= 0;
            }
            return playerScore / totalScore;

        }

        public Dictionary<ID<Readable_GamePlayer>, double> EstimateWinProbabilities(Game game)
        {
            Dictionary<ID<Readable_GamePlayer>, double> probabilities = new Dictionary<ID<Readable_GamePlayer>, double>();
            foreach (Readable_GamePlayer player in game.Players)
            {
                ID<Readable_GamePlayer> playerId = player.GetID((Readable_GamePlayer)null);
                probabilities.Add(playerId, this.EstimateWinProbability(game, playerId));
            }
            return probabilities;
        }

    }
}
