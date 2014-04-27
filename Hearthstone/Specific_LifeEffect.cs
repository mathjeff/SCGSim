using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A Specific_LifeEffect adjusts the life total of specific LifeTarget
namespace Games
{
    public class Specific_LifeEffect : GameEffect
    {
        public Specific_LifeEffect(ID<Readable_LifeTarget> targetID, int amountToGain)
        {
            this.TargetID = targetID;
            this.AmountToGain = amountToGain;
        }
        public ID<Readable_LifeTarget> TargetID { get; set; }
        public int AmountToGain { get; set; }

        public override GameEffect Clone(GameEffect outputType)
        {
            return new Specific_LifeEffect(this.TargetID, this.AmountToGain);
        }
        // Make this effect happen
        public override void Process(Game game)
        {
            Writable_LifeTarget target = game.GetWritable(this.TargetID);
            target.AddHealth(this, game);
        }

    }
}
