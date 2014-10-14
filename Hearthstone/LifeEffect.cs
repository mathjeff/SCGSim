using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class LifeEffect : TriggeredGameEffect<GameEffect>
    {
        // Returns a LifeEffect that knows to target one particular target among the available options
        public static LifeEffect Targeted(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            return new LifeEffect(amountToGain_provider, targetsProvider, chooserProvider, false);
        }
        // Returns a LifeEffect that knows to target one particular target
        public static LifeEffect Targeted(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<Readable_LifeTarget, Controlled> targetProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            return new LifeEffect(amountToGain_provider, new ListProvider<Readable_LifeTarget, Controlled>(targetProvider), chooserProvider, false);
        }
        // Returns a LifeEffect that knows to affect each target
        public static LifeEffect Blanket(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider)
        {
            return new LifeEffect(amountToGain_provider, targetsProvider, chooserProvider, true);
        }

        // A LifeEffect that hasn't yet chosen its exact details (what it targets, where it comes from, or how strong it is) except that it targets exactly one thing
        private LifeEffect(ValueProvider<int, Controlled> amountToGain_provider, ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider, ValueProvider<Readable_GamePlayer, Controlled> chooserProvider, bool affectAll)
        {
            this.Initialize(amountToGain_provider, targetsProvider, chooserProvider, affectAll);
        }
        private LifeEffect()
        {
        }
        private void Initialize(ValueProvider<int, Controlled> amountToGain_provider, 
            ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider,
            ValueProvider<Readable_GamePlayer, Controlled> chooserProvider,
            bool affectAll)
        {
            this.amountToGain_provider = amountToGain_provider;
            this.targetsProvider = targetsProvider;
            this.chooserProvider = chooserProvider;
            this.TargetRequired = true;
            this.affectAll = affectAll;
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType)
        {
            LifeEffect clone = new LifeEffect();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(LifeEffect original)
        {
            base.CopyFrom(original);
            this.amountToGain_provider = original.amountToGain_provider;
            this.targetsProvider = original.targetsProvider;
            this.chooserProvider = original.chooserProvider;
            this.affectAll = original.affectAll;
            this.TargetRequired = original.TargetRequired;
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
            if (this.affectAll)
            {
                foreach (GameEffect option in options)
                {
                    option.Process(game);
                }
            }
            else
            {
                if (options.Count == 0 || !this.TargetRequired)
                    options.Add(new EmptyEffect());
                // ask the game to choose one of these options
                game.AddChoice(new GameChoice(options, controller.GetID((Readable_GamePlayer)null)));
            }
        }
        public override string ToString(Game game)
        {
            int amountToGain = this.amountToGain_provider.GetValue(this, game, default(int));
            Readable_GamePlayer controller = this.chooserProvider.GetValue(this, game, (Readable_GamePlayer)null);
            string result = controller.ToString();
            if (amountToGain > 0)
                result += " heals a target for " + amountToGain;
            else
                result += " damages a target for " + (amountToGain * -1);
            return result;
        }
        public bool TargetRequired { get; set; } // whether it is valid to choose no target
        private ValueProvider<int, Controlled> amountToGain_provider;
        private ValueProvider<IList<Readable_LifeTarget>, Controlled> targetsProvider;
        private ValueProvider<Readable_GamePlayer, Controlled> chooserProvider;
        private bool affectAll; // whether we affect all valid targets rather than just one
    }
}
