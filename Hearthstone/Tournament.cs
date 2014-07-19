using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class Tournament
    {
        public Tournament(Referee referee)
        {
            this.referee = referee;
        }
        public void Run(int numRounds)
        {
            //int i;
            //for (i = numRounds; i > 0; i--)
            while (true)
            {
                this.PlayGame();
                if (this.bestPlayer != this.Players[0])
                { 
                    Console.WriteLine("Best deck:");
                    this.bestPlayer = this.Players[0];
                    foreach (ReadableCard card in this.bestPlayer.MainDeck)
                    {
                        Console.WriteLine(card.Name);
                    }
                }
                Console.WriteLine("Win ratio: " + this.bestPlayer.NumWins + "/" + (this.bestPlayer.NumWins + this.bestPlayer.NumLosses));
            }
        }
        public void PlayGame()
        {
            // Choose two players randomly, weighting towards the higher-ranked players
            int player1Index = this.Players.Count - 1 - (int)Math.Floor(Math.Sqrt(this.random.Next(this.Players.Count * this.Players.Count)));
            int player2Index = this.Players.Count - 2 - (int)Math.Floor(Math.Sqrt(this.random.Next((this.Players.Count - 1) * (this.Players.Count - 1))));
            if (player2Index >= player1Index)
                player2Index++;
            List<TournamentPlayer> players = new List<TournamentPlayer>();
            TournamentPlayer player1 = this.Players[player1Index];
            TournamentPlayer player2 = this.Players[player2Index];
            players.Add(this.Players[player1Index]);
            players.Add(this.Players[player2Index]);
            Game game = this.referee.NewGame(players);
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
        public TournamentPlayer bestPlayer;
        private Referee referee;
    }
}
