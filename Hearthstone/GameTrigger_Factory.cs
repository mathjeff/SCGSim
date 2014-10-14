using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// GameTriggers is the interface in charge of asking certain effects to happen based on certain events
namespace Games
{
    class GameTrigger_Factory
    {
        /*public GameTrigger_Factory()
        {

        }

        #region Creating Triggers

        // Creates a GameTrigger that triggers after playing the given card
        public GameTrigger<GameEffect> New_AfterPlay_Trigger(WritableCard card)
        {
            GameTrigger<GameEffect> trigger = new GameTrigger<GameEffect>();
            card.Add_AfterPlayCard_Trigger(trigger);
            this.Trigger = trigger;
            return trigger;
        }

        // Creates a GameTrigger that triggers before the given Monster receives damage
        public LifeTrigger New_BeforeReceiveDamage_Trigger(Writable_MonsterCard card)
        {
            LifeTrigger trigger = new LifeTrigger(card);
            card.Add_BeforeReceiveDamage_Trigger(trigger);
            this.Trigger = trigger;
            return trigger;
        }
        #endregion

        #region Creating Effects

        // Creates a GameEffect that allows the given player to choose a target and then add the given (signed) health to it
        public LifeEffect New_LifeEffect(ValueProvider<Double> quantityProvider, ValueProvider<IList<Readable_LifeTarget>> targetOptions_provider, ValueProvider<Readable_GamePlayer> controllerProvider)
        {
            LifeEffect effect = LifeEffect.Targeted(quantityProvider, targetOptions_provider, controllerProvider);
            this.connect(effect);
            return effect;
        }

        // Creates a GameEffect that intercepts and scales a LifeEffect
        public LifeSourceMultiplierEffect New_LifeSourceMultiplierEffect(double scale, ValueProvider<LifeEffect> effectProvider)
        {
            LifeSourceMultiplierEffect effect = new LifeSourceMultiplierEffect(scale, effectProvider);
            this.connect(effect);
            return effect;
        }

        // Creates a GameEffect that provides resources to the given player
        public ResourceEffect New_ResourceEffect(double resourcesToGain, ValueProvider<Readable_GamePlayer> controllerProvider)
        {
            ResourceEffect effect = new ResourceEffect(new ConstantValueProvider<Resource>(new Resource(resourcesToGain)), controllerProvider);
            this.connect(effect);
            return effect;
        }


        // Creates a GameEffect that draws cards
        public DrawEffect New_DrawEffect(GameTrigger t)
        {
            ValueProvider<Readable_GamePlayer> playerProvider = new TriggerController_Provider(t);
            return this.New_DrawEffect(playerProvider, new DrawFromDeck_Provider(playerProvider), new ConstantValueProvider<Double>(1));
        }

        // Creates a GameEffect that draws cards
        public DrawEffect New_DrawEffect(ValueProvider<Readable_GamePlayer> playerProvider, ValueProvider<ReadableCard> cardProvider, ValueProvider<double> quantityProvider)
        {
            DrawEffect effect = new DrawEffect(playerProvider, cardProvider, quantityProvider);
            this.connect(effect);
            return effect;
        }

        #endregion

        #region ValueProviders

        // returns the controller of the given trigger
        public ValueProvider<Readable_GamePlayer> New_TriggerController_Provider(GameTrigger trigger)
        {
            return new TriggerController_Provider(trigger);
        }

        // provides a list of things that have hitpoints (Monster and GamePlayer)
        //public ValueProvider<IList<ID<Readable_LifeTarget>>> New_LifeTarget_Choices_Provider()
        public ValueProvider<IList<Readable_LifeTarget>> New_LifeTarget_Choices_Provider()
        {
            return new LifeTarget_Choices_Provider();
        }

        #endregion
        */



        public static void TriggerAll<TTriggerType>(ICollection<GameTrigger<TTriggerType>> triggers, TTriggerType cause, ID<Readable_GamePlayer> controllerID, Game game)
        {
            // copy the trigger list for later in this method
            LinkedList<GameTrigger<TTriggerType>> allTriggers = new LinkedList<GameTrigger<TTriggerType>>(triggers);
            // figure out which triggers should remain attached
            LinkedList<GameTrigger<TTriggerType>> repeatingTriggers = new LinkedList<GameTrigger<TTriggerType>>();
            foreach (GameTrigger<TTriggerType> trigger in allTriggers)
            {
                if (trigger.Repeat)
                    repeatingTriggers.AddLast(trigger);
            }
            // detach any triggers that don't repeat
            triggers.Clear();
            foreach (GameTrigger<TTriggerType> trigger in repeatingTriggers)
            {
                triggers.Add(trigger);
            }
            // finally trigger all of the triggers
            foreach (GameTrigger<TTriggerType> trigger in allTriggers)
            {
                trigger.Trigger(cause, controllerID, game);
            }
        }
        /*
        private GameTrigger Trigger
        {
            get
            {
                if (this.trigger == null)
                {
                    throw new InvalidOperationException("GameTrigger_Factory must create a trigger before creating a GameEffect");
                }
                return this.trigger;
            }
            set
            {
                if (this.trigger != null)
                {
                    throw new InvalidOperationException("GameTrigger_Factory must be initialized once per GameTrigger it creates");
                }
                this.trigger = value;
            }
        }

        private void connect(GameEffect effect)
        {
            this.connect(this.Trigger, effect);
        }

        private void connect(GameTrigger trigger, GameEffect effect)
        {
            trigger.Effect = effect;
        }

        private GameTrigger trigger;
        */
    }

}
