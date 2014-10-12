using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// a GameTrigger is something that activates based on a particular condition
namespace Games
{
    public class GameTrigger<TTriggerType>
    {
        public GameTrigger()
        {
        }
        public GameTrigger(TriggeredGameEffect<TTriggerType> effectWhenTriggered)
        {
            this.EffectToTrigger = effectWhenTriggered;
        }
        public void Trigger(TTriggerType cause, ID<Readable_GamePlayer> triggerController_ID, Game game)
        {
            TriggeredGameEffect<TTriggerType> effect = this.EffectToTrigger.Clone((TriggeredGameEffect<TTriggerType>)null);
            effect.Cause = cause;
            effect.ControllerID = triggerController_ID;
            effect.Process(game);
        }
        public GameTrigger<TTriggerType> Clone(GameTrigger<TTriggerType> outputType)
        {
            GameTrigger<TTriggerType> clone = new GameTrigger<TTriggerType>();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(GameTrigger<TTriggerType> original)
        {
            this.EffectToTrigger = original.EffectToTrigger;
            this.Repeat = original.Repeat;
        }
        public bool Repeat { get; set; }
        public TriggeredGameEffect<TTriggerType> EffectToTrigger { get; set; }

    }
}
