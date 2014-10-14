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
            this.Initialize(new LinkedList<GameEffect>(subEffects));
        }
        public Composite_GameEffect(GameEffect effect1, GameEffect effect2)
        {
            LinkedList<GameEffect> subEffects = new LinkedList<GameEffect>();
            subEffects.AddLast(effect1);
            subEffects.AddLast(effect2);
            this.Initialize(subEffects);
        }
        private Composite_GameEffect()
        {
        }
        private void Initialize(IEnumerable<GameEffect> subEffects)
        {
            this.subEffects = subEffects;
        }
        public override GameEffect Clone(GameEffect outputType)
        {
            Composite_GameEffect clone = new Composite_GameEffect();
            clone.CopyFrom(this);
            return clone;
        }
        public void CopyFrom(Composite_GameEffect original)
        {
            base.CopyFrom(original);
            this.Initialize(new LinkedList<GameEffect>(original.subEffects));
        }
        public override void Process(Game game)
        {
            foreach (GameEffect effect in this.subEffects)
            {
                effect.ControllerID = this.ControllerID;
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
