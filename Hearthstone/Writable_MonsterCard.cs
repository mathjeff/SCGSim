using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public class Writable_MonsterCard : WritableCard, Readable_MonsterCard, Writable_LifeTarget
    {
        #region Constructors etc

        public Writable_MonsterCard(string name, Resource cost, int damage, int health) : base(name, cost)
        {
            this.Damage = this.originalDamage = damage;
            this.Health = this.originalHealth = health;
        }
        private Writable_MonsterCard()
        {
        }

        public void Add_BeforeReceiveDamage_Trigger(GameTrigger<Specific_LifeEffect> trigger)
        {
            this.beforeReceivingDamage_Triggers.Add(trigger);
        }

        // Puts Divine Shield on this unit, which absorbs all damage from the next source to attempt to damage it
        public void Add_SingleUse_Shield()
        {
            // Add a trigger for before receiving damage, which multiplies the quantity by 0 of the effect that triggered this effect
            this.Add_BeforeReceiveDamage_Trigger(new GameTrigger<Specific_LifeEffect>(new LifeSourceMultiplierEffect(0, new TriggerProvider<Specific_LifeEffect>())));
        }

        public void Add_AfterDeath_Trigger(GameTrigger<GameEffect> trigger)
        {
            this.afterDeath_triggers.Add(trigger);
        }
        #endregion

        #region Cloning etc

        public Writable_MonsterCard Clone(Writable_MonsterCard outputType)
        {
            Writable_MonsterCard clone = new Writable_MonsterCard(this.Name, this.Cost, this.Damage, this.Health);
            clone.CopyFrom(this);
            return clone;
        }
        public override WritableCard Clone(WritableCard outputType)
        {
            return this.Clone((Writable_MonsterCard)null);
        }
        public void CopyFrom(Readable_MonsterCard original)
        {
            this.Damage = this.originalDamage = original.GetDamage();
            this.Health = original.GetHealth();
            this.originalHealth = original.GetMaxHealth();
            this.beforeReceivingDamage_Triggers = new List<GameTrigger<Specific_LifeEffect>>(original.Get_BeforeReceivingDamage_Triggers());
            this.afterDeath_triggers = new List<GameTrigger<GameEffect>>(original.Get_AfterDeath_Triggers());
            this.MustBeAttacked = original.Get_MustBeAttacked();
            base.CopyFrom(original);
        }
        public override bool IsPlayable(Game game)
        {
            // check that we have resources to play this card, and that there aren't already too many monsters in play
            return base.IsPlayable(game) && game.Referee.IsPlayable(this, game);
        }
        public override void Play(Game game)
        {
            // Put this card in play
            Writable_GamePlayer controller = game.GetWritable(this.ControllerID);
            controller.MonsterIDsInPlay.GetWritable().Add(this.GetID((Readable_MonsterCard)null));
            // Pay for this card, remove it from hand, and trigger its abilities
            base.Play(game);
        }
        public override String ToString(Game game)
        {
            String output = base.ToString(game);
            output += "  " + this.Damage + "/" + this.Health;
            if (this.beforeReceivingDamage_Triggers.Count > 0)
            {
                output += " (";
                if (this.beforeReceivingDamage_Triggers.Count > 1)
                    output += this.beforeReceivingDamage_Triggers.Count() + "S";
                output += ")";
            }
            return output;
        }
        public int NumAttacksRemaining { get; set; }
        public bool Get_CanAttack()
        {
            return (this.NumAttacksRemaining > 0 && this.Damage > 0);
        }
        public void Attack(ID<Readable_LifeTarget> targetID, Game game)
        {
            Writable_LifeTarget target = game.GetWritable(targetID);
            this.AddHealth(new Specific_LifeEffect(null, -1 * target.GetDamage()), game);
            target.AddHealth(new Specific_LifeEffect(null, -1 * this.GetDamage()), game);
            this.NumAttacksRemaining--;
        }

        #endregion

        #region Properties etc

        public ID<Readable_MonsterCard> GetID(Readable_MonsterCard outputType) { return new ID<Readable_MonsterCard>(this.ID); }
        public ID<Readable_LifeTarget> GetID(Readable_LifeTarget outputType) { return new ID<Readable_LifeTarget>(this.ID); }
        public int Damage { get; set; }
        public int GetDamage() { return this.Damage; }
        private int originalDamage;
        public int Get_OriginalDamage() { return this.originalDamage; }
        public int Health { get; set; }
        public int GetHealth() { return this.Health; }
        private int originalHealth;
        public int Get_OriginalHealth() { return this.originalHealth; }
        public int GetMaxHealth() { return this.originalHealth; }
        public void AddHealth(Specific_LifeEffect effect, Game game) 
        { 
            if (effect.AmountToGain < 0)
            {
                GameTrigger_Factory.TriggerAll<Specific_LifeEffect>(this.beforeReceivingDamage_Triggers, effect, this.ControllerID, game);
            }
            this.Health += effect.AmountToGain;
            // check for death
            if (this.Health <= 0)
                this.Destroy(effect, game);
            this.Health = Math.Min(this.Health, this.originalHealth);
        }
        public void Destroy(GameEffect cause, Game game)
        {
            // inform the controller
            Writable_GamePlayer controller = game.GetWritable(this.ControllerID);
            controller.MonsterIDsInPlay.GetWritable().Remove(this.GetID((Readable_MonsterCard)null));
            // process any on-death effects
            GameTrigger_Factory.TriggerAll<GameEffect>(this.afterDeath_triggers, cause, this.ControllerID, game);
        }
        public bool MustBeAttacked { get; set; }
        public bool Get_MustBeAttacked() { return this.MustBeAttacked; }

        public List<GameTrigger<Specific_LifeEffect>> Get_BeforeReceivingDamage_Triggers() { return this.beforeReceivingDamage_Triggers; }
        private List<GameTrigger<Specific_LifeEffect>> beforeReceivingDamage_Triggers = new List<GameTrigger<Specific_LifeEffect>>();

        public List<GameTrigger<GameEffect>> Get_AfterDeath_Triggers() { return this.afterDeath_triggers; }
        private List<GameTrigger<GameEffect>> afterDeath_triggers = new List<GameTrigger<GameEffect>>();

        #endregion



    }
}
