using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class LifeEffect : TriggeredGameEffect<GameEffect>
    {
        // A LifeEffect that hasn't yet chosen its exact details (what it targets, where it comes from, or how strong it is)
        public LifeEffect(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            this.Initialize(amountToGain_provider, targetsProvider, chooserProvider);
        }
        // A LifeEffect that hasn't yet chosen its exact details (what it targets, where it comes from, or how strong it is)
        public LifeEffect(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<Readable_LifeTarget, Controlled> targetProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            this.Initialize(amountToGain_provider, new ListProvider<Readable_LifeTarget, Controlled>(targetProvider), chooserProvider);
        }
        private void Initialize(ValueProvider<int, Controlled> amountToGain_provider, 
            ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider,
            ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            this.amountToGain_provider = amountToGain_provider;
            this.targetsProvider = targetsProvider;
            this.chooserProvider = chooserProvider;
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType)
        {
            LifeEffect clone = new LifeEffect(this.amountToGain_provider, this.targetsProvider, this.chooserProvider);
            clone.chooserProvider = this.chooserProvider;
            return clone;
        }
        // Choose a target for this effect, and then spawn a specific effect and process it
        public override void Process(Game game)
        {
            int amountToGain = this.amountToGain_provider.GetValue(this, game, default(int));
            Readable_GamePlayer controller = this.chooserProvider.GetValue(this, game, (Readable_GamePlayer)null);
            IList<Readable_LifeTarget> availableTargets = this.targetsProvider.GetValue(this, game, (IList<Readable_LifeTarget>)null);
            // multiple choices of what to heal/damage, so ask the player what to do
            List<GameEffect> options = new List<GameEffect>();
            foreach (Readable_LifeTarget target in availableTargets)
            {
                options.Add(new Specific_LifeEffect(target.GetID((Readable_LifeTarget)null), amountToGain));
            }
            if (!this.TargetRequired)
                options.Add(new EmptyEffect());
            GameEffect effect = controller.ChooseBestAction(options, game);
            effect.Process(game);
        }
        public bool TargetRequired { get; set; } // whether it is valid to choose no target
        private ValueProvider<int, Controlled> amountToGain_provider;
        private ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider;
        private ValueProvider<Readable_GamePlayer, Controlled> chooserProvider;
    }
}
