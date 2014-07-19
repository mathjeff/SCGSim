using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Represents a choice that a player can make
namespace Games
{
    public class GameChoice
    {
        public GameChoice(IEnumerable<GameEffect> options, ID<Readable_GamePlayer> chooserID)
        {
            this.options = options;
            this.ControllerID = chooserID;
        }
        public IEnumerable<GameEffect> Options
        {
            get
            {
                return this.options;
            }
        }
        public ID<Readable_GamePlayer> ControllerID;
        IEnumerable<GameEffect> options;
    }
}
