﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class PlayCard_Effect : GameEffect
    {
        public PlayCard_Effect(ID<ReadableCard> cardId)
        {
            this.cardId = cardId;
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new PlayCard_Effect(this.cardId);
        }
        public override void Process(Game game)
        {
            ReadableCard card = game.Get_ReadableSnapshot(this.cardId);
            card.Play(game);
        }
        private ID<ReadableCard> cardId;
    }
}