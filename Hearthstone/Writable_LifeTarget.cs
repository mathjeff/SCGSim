using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A LifeTarget is something that can gain/lose hitpoints
namespace Games
{
    public interface Writable_LifeTarget : Readable_LifeTarget
    {
        void AddHealth(Specific_LifeEffect effect, Game game);
        int GetDamage();
        void Attack(ID<Readable_LifeTarget> target, Game game);
    }
}
