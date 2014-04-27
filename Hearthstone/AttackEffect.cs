using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class AttackEffect : GameEffect
    {
        public AttackEffect(ID<Readable_LifeTarget> attackerID, ID<Readable_LifeTarget> defenderID)
        {
            this.attackerID = attackerID;
            this.defenderID = defenderID;
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new AttackEffect(this.attackerID, this.defenderID);
        }
        public override void Process(Game game)
        {
            Writable_LifeTarget attacker = game.GetWritable(this.attackerID);
            attacker.Attack(this.defenderID, game);
        }
        private ID<Readable_LifeTarget> attackerID;
        private ID<Readable_LifeTarget> defenderID;
    }
}
