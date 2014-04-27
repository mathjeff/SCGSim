using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Provides the ID of the controller of a particular effect
namespace Games
{
    public interface Controlled
    {
        ID<Readable_GamePlayer> Get_ControllerID();
    }

    public class ReadableController_Provider : ValueProvider<Readable_GamePlayer, Controlled>
    {
        public Readable_GamePlayer GetValue(Controlled item, Game game, Readable_GamePlayer outputType)
        {
            return game.GetWritable(item.Get_ControllerID());
        }
    }

    class WritableController_Provider : ValueProvider<Writable_GamePlayer, Controlled>
    {
        public Writable_GamePlayer GetValue(Controlled item, Game game, Writable_GamePlayer outputType)
        {
            return game.GetWritable(item.Get_ControllerID());
        }
    }
}
