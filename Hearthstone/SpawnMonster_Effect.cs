using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A SpawnMonster_Effect will create monsters for the appropriate player
namespace Games
{
    class SpawnMonster_Effect : TriggeredGameEffect<GameEffect>
    {
        public SpawnMonster_Effect(Readable_MonsterCard monsterToSpawn)
        {
            this.Initialize(monsterToSpawn, new WritableController_Provider(), new ConstantValueProvider<int, Controlled>(1));
        }
        public SpawnMonster_Effect(Readable_MonsterCard monsterToSpawn, ValueProvider<Writable_GamePlayer, Controlled> monsterController_provider)
        {
            this.Initialize(monsterToSpawn, monsterController_provider, new ConstantValueProvider<int, Controlled>(1));
        }
        public SpawnMonster_Effect(Readable_MonsterCard monsterToSpawn, ValueProvider<Writable_GamePlayer, Controlled> monsterController_provider, ValueProvider<int, Controlled> countProvider)
        {
            this.Initialize(monsterToSpawn, monsterController_provider, countProvider);
        }
        private SpawnMonster_Effect()
        {
        }
        private void Initialize(Readable_MonsterCard monsterToSpawn, ValueProvider<Writable_GamePlayer, Controlled> monsterController_provider, ValueProvider<int, Controlled> countProvider)
        {
            this.monsterToSpawn = monsterToSpawn;
            this.monsterController_provider = monsterController_provider;
            this.countProvider = countProvider;
        }
        public override void Process(Game game)
        {
            Writable_GamePlayer monsterController = this.monsterController_provider.GetValue(this, game, (Writable_GamePlayer)null);
            int numberToSpawn = this.countProvider.GetValue(this, game, default(int));
            //monsterController.get
            List<ID<Readable_MonsterCard>> territory = monsterController.MonsterIDsInPlay.GetWritable();

            for (int i = 0; i < numberToSpawn; i++)
            {
                Writable_MonsterCard monster = this.monsterToSpawn.Clone((Writable_MonsterCard)null);
                monster.ID = IDFactory.NewID();
                monster.ControllerID = monsterController.GetID((Readable_GamePlayer)null);
                territory.Add(monster.GetID((Readable_MonsterCard)null));
                game.AddCard(monster);
            }
        }
        public void CopyFrom(SpawnMonster_Effect original)
        {
            base.CopyFrom(this);
            this.monsterToSpawn = original.monsterToSpawn;
            this.monsterController_provider = original.monsterController_provider;
            this.countProvider = original.countProvider;
        }
        public SpawnMonster_Effect Clone(SpawnMonster_Effect outputType)
        {
            SpawnMonster_Effect clone = new SpawnMonster_Effect();
            clone.CopyFrom(this);
            return clone;
        }
        public override string ToString(Game game)
        {
            int numberToSpawn = this.countProvider.GetValue(this, game, default(int));
            return "Spawn " + numberToSpawn.ToString() + " " + ((ReadableCard)this.monsterToSpawn).ToString(game);
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType) { return this.Clone((SpawnMonster_Effect)null); }
        Readable_MonsterCard monsterToSpawn;
        ValueProvider<Writable_GamePlayer, Controlled> monsterController_provider; // Who controls and receives the monsters, which isn't necessarily the same as who controls the effect
        ValueProvider<int, Controlled> countProvider;
    }
}
