using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface Readable_GamePlayer : Readable_LifeTarget, Identifiable<Readable_GamePlayer>
    {
        TournamentPlayer SourcePlayer { get; }
        ReadableQueue<ReadableCard> GetDeck();
        IReadOnlyList<ID<ReadableCard>> Get_ReadableHand();
        Writable_GamePlayer Clone(Writable_GamePlayer outputType);
        ID<Readable_GamePlayer> Get_NextPlayer_ID();
        Resource Get_CurrentResources();
        Resource Get_ResourcesPerTurn();
        int Get_Total_MonsterDamage(Game game);
        int Get_Total_MonsterHealth(Game game);
        IReadOnlyList<ID<Readable_MonsterCard>> Get_MonsterIDsInPlay();
        Strategy Strategy { get; }
        void Print(Game game);
        int Get_NumDrawsSkipped();
    }
}
