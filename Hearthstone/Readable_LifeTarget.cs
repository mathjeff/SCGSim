using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface Readable_LifeTarget : Identifiable<Readable_LifeTarget>
    {
        int GetHealth();
        int GetMaxHealth();
        string ToString(Game game);
    }
}
