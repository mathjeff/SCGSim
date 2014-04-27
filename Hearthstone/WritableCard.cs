using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class WritableCard : ReadableCard
    {

        #region Constructors etc

        protected WritableCard()
        {
            this.Initialize();
        }
        public WritableCard(String name, Resource cost)
        {
            this.Initialize();
            this.name = name;
            this.cost = cost;
        }
        private void Initialize()
        {
            this.ID = IDFactory.NewID();
        }

        #endregion

        #region Cloning etc

        public virtual WritableCard Clone(WritableCard outputType)
        {
            WritableCard clone = new WritableCard();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(ReadableCard other)
        {
            this.name = other.Name;
            this.cost = other.GetCost();
            this.ControllerID = other.Get_ControllerID();
            this.OwnerID = other.Get_OwnerID();
            this.ID = other.GetID((ReadableCard)null).ToInt();
            this.afterPlay_triggers = new LinkedList<GameTrigger<GameEffect>>();
            foreach (GameTrigger<GameEffect> trigger in other.Get_AfterPlay_Triggers())
            {
                this.afterPlay_triggers.AddLast(trigger.Clone((GameTrigger<GameEffect>)null));
            }
        }


        #endregion

        #region Properties etc

        public int ID { get; set; }
        public ID<ReadableCard> GetID(ReadableCard outputType) { return new ID<ReadableCard>(this.ID); }

        public String Name
        {
            get
            {
                return this.name;
            }
        }
        public Resource Cost
        {
            get
            {
                return this.cost;
            }
        }
        public Resource GetCost() { return this.cost; }
        public ID<Readable_GamePlayer> ControllerID { get; set; }
        public ID<Readable_GamePlayer> Get_ControllerID() { return this.ControllerID; }
        public ID<Readable_GamePlayer> OwnerID { get; set; }
        public ID<Readable_GamePlayer> Get_OwnerID() { return this.OwnerID; }

        public virtual void Play(Game game)
        {
            // pay the resources for this card
            Writable_GamePlayer controller = game.GetWritable(this.ControllerID);
            controller.CurrentResources = controller.CurrentResources.Minus(this.cost);
            // remove this card from hand
            List<ID<ReadableCard>> hand = controller.Get_WritableHand();
            hand.Remove(this.GetID((ReadableCard)null));
            // Trigger anything that happens when playing this card
            this.AfterPlay(game);
        }
        public virtual String ToString(Game game)
        {
            return "  " + this.Name;
        }

        #endregion

        #region Other Methods

        public void Add_AfterPlayCard_Trigger(GameTrigger<GameEffect> trigger)
        {
            this.afterPlay_triggers.AddLast(trigger);
            //this.afterPlay_triggers.GetWritable().Add(trigger);
        }
        public IEnumerable<GameTrigger<GameEffect>> Get_AfterPlay_Triggers()
        {
            return this.afterPlay_triggers;
            //return this.afterPlay_triggers.GetReadable();
        }
        public void AfterPlay(Game game)
        {
            GameEffect playEffect = new PlayCard_Effect(this.GetID((ReadableCard)null));
            playEffect.ControllerID = this.ControllerID;
            foreach (GameTrigger<GameEffect> trigger in this.afterPlay_triggers)
            {
                trigger.Trigger(playEffect, this.ControllerID, game);
            }
            //GameTrigger_Factory.TriggerAll<GameEffect>(this.afterPlay_triggers.GetWritable(), new PlayCard_Effect(this.GetID((ReadableCard)null)), this.ControllerID, game);
            //GameTrigger_Factory.TriggerAll<GameEffect>(this.afterPlay_triggers, new PlayCard_Effect(this.GetID((ReadableCard)null)), this.ControllerID, game);
        }
        public virtual bool IsPlayable(Game game)
        {
            // check that we have enough resources to play this card
            Readable_GamePlayer controller = game.Get_ReadableSnapshot(this.ControllerID);
            return (controller.Get_CurrentResources().Minus(this.cost).IsValid);
        }

        #endregion



        private String name;
        private Resource cost;
        private LinkedList<GameTrigger<GameEffect>> afterPlay_triggers = new LinkedList<GameTrigger<GameEffect>>();
        //private WriteControlled_Item<IReadOnlyList<GameTrigger<GameEffect>>, List<GameTrigger<GameEffect>>> afterPlay_triggers 
        //    = new WriteControlled_Item<IReadOnlyList<GameTrigger<GameEffect>>, List<GameTrigger<GameEffect>>>(new ListConverter<GameTrigger<GameEffect>>());

    }

    public class CardConverter : BiConverter<ReadableCard, WritableCard>
    {
        public CardConverter()
        {
        }

        public WritableCard Convert(ReadableCard readable)
        {
            return readable.Clone((WritableCard)null);
        }

        public ReadableCard ConvertBack(WritableCard writable)
        {
            return writable;
        }
    }
}
