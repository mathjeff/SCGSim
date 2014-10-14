using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Tournament
    {
        public Tournament(Referee referee, int numPlayers)
        {
            this.referee = referee;
            this.desiredNumPlayers = numPlayers;
            for (int i = 0; i < numPlayers; i++)
            {
                this.Players.Add(this.MakeNewPlayer());
            }
        }
        public void Run()
        {
            while (true)
            {
                TournamentPlayer bestPlayer = this.Players[0];
                // play a bunch of games, giving lower-ranked players the opportunity to rise to the top
                for (int i = this.Players.Count - 2; i >= 0; i--)
                {
                    if (this.random.Next(2) == 0)
                        this.PlayGame(i, i + 1);
                    else
                        this.PlayGame(i + 1, i);
                }
                // If the best players has changed, then print the current best player and continue
                if (this.Players[0] != bestPlayer)
                { 
                    Console.WriteLine("Best deck:");
                    foreach (ReadableCard card in this.Players[0].SortedDeck)
                    {
                        Console.WriteLine(card.Name);
                    }
                    bestPlayer = this.Players[0];
                }
                Console.WriteLine("Win ratio: " + bestPlayer.NumWins + "/" + (bestPlayer.NumWins + bestPlayer.NumLosses));

                // Choose a player with rank better than median, make a mutated version, insert the mutated version at the median, and remove the worst-ranked player
                int middleIndex = this.Players.Count / 2;
                int indexToClone = this.random.Next(middleIndex);
                TournamentPlayer newPlayer = this.Players[indexToClone].Mutate(this);
                this.Players.Insert(middleIndex, newPlayer);
                this.Players.RemoveAt(this.Players.Count - 1);
            }
        }
        public void PlayRandomGame()
        {
            // Choose two players randomly, weighting towards the higher-ranked players
            int player1Index = this.Players.Count - 1 - (int)Math.Floor(Math.Sqrt(this.random.Next(this.Players.Count * this.Players.Count)));
            int player2Index = this.Players.Count - 2 - (int)Math.Floor(Math.Sqrt(this.random.Next((this.Players.Count - 1) * (this.Players.Count - 1))));
            if (player2Index >= player1Index)
                player2Index++;
            if (this.random.Next(2) == 0)
                this.PlayGame(player1Index, player2Index);
            else
                this.PlayGame(player2Index, player1Index);
        }
        public void PlayGame(int player1Index, int player2Index)
        {
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            TournamentPlayer player1 = this.Players[player1Index];
            TournamentPlayer player2 = this.Players[player2Index];
            players.Add(this.Players[player1Index]);
            players.Add(this.Players[player2Index]);
            Game game = this.referee.NewGame(players);
            if (player1Index == 0 || player2Index == 0)
                game.ShouldPrint = true;
            game.Play();
            if (game.GetWinner() != null)
            {
                TournamentPlayer winner = game.GetWinner().SourcePlayer;
                int winnerIndex, loserIndex;
                TournamentPlayer loser = null;
                if (winner == player1)
                {
                    winnerIndex = player1Index;
                    loserIndex = player2Index;
                }
                else
                {
                    if (winner == player2)
                    {
                        winnerIndex = player2Index;
                        loserIndex = player1Index;
                    }
                    else
                    {
                        return;
                    }
                }
                loser = this.Players[loserIndex];
                winner.NumWins++;
                loser.NumLosses++;

                if (winnerIndex > 0)
                {
                    // improve winner's rank by 1
                    this.Players.RemoveAt(winnerIndex);
                    winnerIndex--;
                    this.Players.Insert(winnerIndex, winner);
                    if (winnerIndex == loserIndex)
                        return;
                }
                // worsen losers's rank by 1
                if (loserIndex < players.Count - 1)
                {
                    this.Players.RemoveAt(loserIndex);
                    loserIndex++;
                    this.Players.Insert(loserIndex, loser);
                }

            }
        }


        public TournamentPlayer MakeNewPlayer()
        {
            return new TournamentPlayer(this.random, this.referee);
        }

        private Random random
        {
            get
            {
                return Randomness.Random;
            }
        }
        public List<TournamentPlayer> Players = new List<TournamentPlayer>();
        //public TournamentPlayer bestPlayer;
        private Referee referee;
        private int desiredNumPlayers;
    }
}
