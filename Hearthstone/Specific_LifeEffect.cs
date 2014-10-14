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
        public override string ToString(Game game)
        {
            Readable_LifeTarget target = game.Get_ReadableSnapshot(this.TargetID);
            int amount = this.AmountToGain;
            string result = target.ToString(game);
            if (amount > 0)
            {
                result += " gains ";
            }
            else
            {
                result += " loses ";
                amount *= -1;
            }
            result += amount.ToString() + " hitpoint";
            if (amount != 1)
                result += "s";
            return result;
        }

    }
}
