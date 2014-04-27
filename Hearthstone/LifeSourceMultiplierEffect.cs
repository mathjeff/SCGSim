using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class LifeSourceMultiplierEffect : TriggeredGameEffect<Specific_LifeEffect>
    {
        private LifeSourceMultiplierEffect()
        {
        }
        public LifeSourceMultiplierEffect(double scale, ValueProvider<Specific_LifeEffect, TriggeredGameEffect<Specific_LifeEffect>> lifeSource_provider)
        {
            this.scale = scale;
            this.LifeSource_Provider = lifeSource_provider;
        }
        public override void Process(Game game)
        {
            Specific_LifeEffect effectToScale = this.LifeSource_Provider.GetValue(this, game, (Specific_LifeEffect)null);
            effectToScale.AmountToGain = (int)(((double)effectToScale.AmountToGain) * this.scale);
        }
        public override TriggeredGameEffect<Specific_LifeEffect> Clone(TriggeredGameEffect<Specific_LifeEffect> outputType)
        {
            LifeSourceMultiplierEffect clone = new LifeSourceMultiplierEffect(this.scale, this.LifeSource_Provider);
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(LifeSourceMultiplierEffect other)
        {
            this.LifeSource_Provider = other.LifeSource_Provider;
            this.scale = other.scale;
        }
        public ValueProvider<Specific_LifeEffect, TriggeredGameEffect<Specific_LifeEffect>> LifeSource_Provider { get; set; }
        private double scale;
    }

}
