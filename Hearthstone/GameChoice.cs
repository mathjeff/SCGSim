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
        public static ID<Readable_GamePlayer> CHOSEN_RANDOMLY = new ID<Readable_GamePlayer>(IDFactory.NewID());

        public GameChoice(IEnumerable<GameEffect> options, ID<Readable_GamePlayer> chooserID)
        {
            this.Initialize(options, chooserID);
        }
        public static GameChoice Random(IEnumerable<GameEffect> options)
        {
            return new GameChoice(options, CHOSEN_RANDOMLY);
        }
        private void Initialize(IEnumerable<GameEffect> options, ID<Readable_GamePlayer> chooserID)
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
