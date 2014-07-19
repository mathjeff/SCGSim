using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A GameEffect does something specific and contains all of the knowledge necessary to execute itself
namespace Games
{
    // Declares that it can run itself
    public abstract class GameEffect : Controlled
    {
        public virtual bool IsProcessable(Game game) { return true; }
        public abstract void Process(Game game);
        public abstract GameEffect Clone(GameEffect outputType);
        public ID<Readable_GamePlayer> Get_ControllerID()
        {
            return new ID<Readable_GamePlayer>(this.controllerID);
        }
        public ID<Readable_GamePlayer> ControllerID
        {
            get
            {
                return new ID<Readable_GamePlayer>(this.controllerID);
            }
            set
            {
                this.controllerID = value.ToInt();
            }
        }
        private int controllerID;
    }

    // Declares that it knows what caused it
    public abstract class TriggeredGameEffect<TTriggerType> : GameEffect
    {
        public TTriggerType Cause { get; set; }
        public abstract TriggeredGameEffect<TTriggerType> Clone(TriggeredGameEffect<TTriggerType> outputType);
        public override GameEffect Clone(GameEffect outputType) { return this.Clone((TriggeredGameEffect<TTriggerType>)null); }
        public void CopyFrom(TriggeredGameEffect<TTriggerType> other)
        {
            this.Cause = other.Cause;
        }
    }

    // Returns the cause of a TriggeredGameEffect
    public class TriggerProvider<TTriggerType> : ValueProvider<TTriggerType, TriggeredGameEffect<TTriggerType>>
    {
        public TTriggerType GetValue(TriggeredGameEffect<TTriggerType> effect, Game game, TTriggerType outputType)
        {
            return effect.Cause;
        }
    }

}
