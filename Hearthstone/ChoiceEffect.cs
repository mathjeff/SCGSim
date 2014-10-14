using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A ChoiceEffect lets a player make a choice
namespace Games
{
    class ChoiceEffect : TriggeredGameEffect<GameEffect>
    {
        public ChoiceEffect(GameEffect option1, GameEffect option2)
        {
            LinkedList<GameEffect> choices = new LinkedList<GameEffect>();
            choices.AddLast(option1);
            choices.AddLast(option2);
            this.Initialize(choices);
        }
        public ChoiceEffect(IEnumerable<GameEffect> options)
        {
            this.options = options;
        }
        private void Initialize(IEnumerable<GameEffect> options)
        {
            this.options = options;
        }
        public override void Process(Game game)
        {
            foreach (GameEffect option in this.options)
            {
                option.ControllerID = this.ControllerID;
            }
            GameChoice choice = new GameChoice(this.options, this.Get_ControllerID());
            game.AddChoice(choice);
        }
        public override TriggeredGameEffect<GameEffect> Clone(TriggeredGameEffect<GameEffect> outputType)
        {
            return this.Clone((ChoiceEffect)null);
        }
        public ChoiceEffect Clone(ChoiceEffect outputType)
        {
            return new ChoiceEffect(this.options);
        }
        public override string ToString(Game game)
        {
            string result = "";
            foreach (GameEffect option in this.options)
            {
                if (result.Count() > 0)
                {
                    result += " or ";
                }
                result += option.ToString();
            }
            return result;
        }
        private IEnumerable<GameEffect> options;
    }
}
