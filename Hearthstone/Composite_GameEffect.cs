using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class Composite_GameEffect : GameEffect
    {
        public Composite_GameEffect(IEnumerable<GameEffect> subEffects)
        {
            this.subEffects = new LinkedList<GameEffect>(subEffects);
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            return new Composite_GameEffect(this.subEffects);
        }
        public override void Process(Game game)
        {
            foreach (GameEffect effect in this.subEffects)
            {
                effect.Process(game);
            }
        }
        public override string ToString(Game game)
        {
            string result = "";
            foreach (GameEffect effect in this.subEffects)
            {
                if (result.Length > 0)
                    result += " and ";
                result += effect.ToString(game);
            }
            return result;

        }
        private IEnumerable<GameEffect> subEffects;
    }
}
