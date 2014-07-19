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
            double activePlayerBonus = 1;
            foreach (Readable_GamePlayer candidate in game.Players)
            {
                // Exchange rates:
                // 1 card is worth 2 mana
                // Being the active player is worth 1 card
                // n power and n toughness is worth n mana
                // losing 25% of your life is worth a card
                // 1 Mana per turn is worth 2 mana now.
                // This results in this score:
                // (mana/2)+(manaPerTurn)+(activePlayer?)+((power+toughness)/2)+(log(life)/log(4/3))
                /*
                double score = candidate.Get_CurrentResources().ToNumber() / 2 + candidate.Get_ResourcesPerTurn().ToNumber() + activePlayerBonus +
                    (candidate.Get_Total_MonsterDamage(game) + candidate.Get_Total_MonsterHealth(game)) / 2 + Math.Log(candidate.GetHealth()) / Math.Log(4.0 / 3.0);
                */
                
                double score = candidate.Get_CurrentResources().ToNumber() / 2 + candidate.Get_ResourcesPerTurn().ToNumber() + activePlayerBonus +
                    (candidate.Get_Total_MonsterDamage(game) + candidate.Get_Total_MonsterHealth(game)) / 2 + Math.Log(candidate.GetHealth()) / Math.Log(4.0 / 3.0);
                
                totalScore += score;
                if (candidate == player)
                    playerScore = score;
                activePlayerBonus--;
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
