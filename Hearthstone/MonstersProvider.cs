using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Returns all monsters in the game
namespace Games
{
    class AllMonsters_Provider : ValueProvider<IList<Readable_LifeTarget>, Controlled>
    {
        public IList<Readable_MonsterCard> GetValue(Controlled caller, Game game, IList<Readable_MonsterCard> outputType)
        {
            return game.Get_MonstersInPlay();
        }
        public IList<Readable_LifeTarget> GetValue(Controlled caller, Game game, IList<Readable_LifeTarget> outputType)
        {
            List<Readable_LifeTarget> targets = new List<Readable_LifeTarget>();
            foreach (Readable_MonsterCard monster in game.Get_MonstersInPlay())
            {
                targets.Add(monster);
            }
            return targets;
        }
    }

    class EnemyMonsters_Provider : ValueProvider<IList<Readable_LifeTarget>, Controlled>
    {
        public IList<Readable_LifeTarget> GetValue(Controlled caller, Game game, IList<Readable_LifeTarget> outputType)
        {
            IEnumerable<Readable_MonsterCard> monsters = new AllMonsters_Provider().GetValue(caller, game, (IList<Readable_MonsterCard>)null);
            List<Readable_LifeTarget> enemyMonsters = new List<Readable_LifeTarget>();
            foreach (Readable_MonsterCard monster in monsters)
            {
                if (!monster.Get_ControllerID().Equals(caller.Get_ControllerID()))
                {
                    enemyMonsters.Add(monster);
                }
            }
            return enemyMonsters;
        }
    }

}
