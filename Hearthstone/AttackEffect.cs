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
        public override string ToString(Game game)
        {
            Readable_LifeTarget attacker = game.Get_ReadableSnapshot(this.attackerID);
            Readable_LifeTarget defender = game.Get_ReadableSnapshot(this.defenderID);
            return attacker.ToString(game) + " attacks " + defender.ToString(game);
        }
        private ID<Readable_LifeTarget> attackerID;
        private ID<Readable_LifeTarget> defenderID;
    }
}
