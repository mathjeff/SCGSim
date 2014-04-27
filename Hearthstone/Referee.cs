using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// knows, explains, and enforces the rules of the game
namespace Games
{
    public interface Referee
    {
        List<GameEffect> Get_AvailableGameActions(Game game, Readable_GamePlayer player);
        Game NewGame(TournamentPlayer player1, TournamentPlayer player2);
        Game NewGame(List<TournamentPlayer> players);
        void AddHealth(int amount, Writable_GamePlayer player);
        Readable_GamePlayer GetWinner(Game game);
        List<Readable_GamePlayer> GetLosers(Game game);
        void AddCardToHand(ReadableCard card, Writable_GamePlayer player, Game game);
        int Starting_DeckSize { get; }
        IList<ReadableCard> LegalCards { get; }
        bool IsPlayable(Readable_MonsterCard card, Game game);
        void NewTurn(ID<Readable_GamePlayer> playerId, Game game);
    }

}
