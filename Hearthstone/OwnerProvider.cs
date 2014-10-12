using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class OwnerProvider: ValueProvider<Readable_GamePlayer, ID<ReadableCard>>
    {
        public Readable_GamePlayer GetValue(ID<ReadableCard> cardId, Game game, Readable_GamePlayer outputType)
        {
            ReadableCard card = game.Get_ReadableSnapshot(cardId);
            Readable_GamePlayer owner = game.Get_ReadableSnapshot(card.Get_OwnerID());
            return owner;
        }
    }

    class OpponentsProvider : ValueProvider<IList<Readable_LifeTarget>, Controlled>
    {
        public IList<Readable_GamePlayer> GetValue(Controlled controlled, Game game, IList<Readable_GamePlayer> outputType)
        {
            IEnumerable<Readable_GamePlayer> players = game.Players;
            List<Readable_GamePlayer> opponents = new List<Readable_GamePlayer>();
            foreach (Readable_GamePlayer player in players)
            {
                if (!player.GetID((Readable_GamePlayer)null).Equals(controlled.Get_ControllerID()))
                    opponents.Add(player);
            }
            return opponents;
        }

        public IList<Readable_LifeTarget> GetValue(Controlled controlled, Game game, IList<Readable_LifeTarget> outputType)
        {
            IEnumerable<Readable_GamePlayer> players = this.GetValue(controlled, game, (IList<Readable_GamePlayer>)null);
            List<Readable_LifeTarget> targets = new List<Readable_LifeTarget>();
            foreach (Readable_GamePlayer player in players)
            {
                targets.Add(player);
            }
            return targets;
        }

    }
}