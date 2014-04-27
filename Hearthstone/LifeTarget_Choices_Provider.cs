using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Asks someone to choose a valid LifeTarget (player or monster), and returns the chosen monster
namespace Games
{
    class LifeTarget_Choices_Provider : ValueProvider<IList<Readable_LifeTarget>, Controlled>
    {
        public IList<Readable_LifeTarget> GetValue(Controlled caller, Game game, IList<Readable_LifeTarget> outputType)
        {
            return game.Get_LifeTargets();
        }
    }
}
