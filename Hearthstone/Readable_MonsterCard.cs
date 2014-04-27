using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface Readable_MonsterCard : ReadableCard, Readable_LifeTarget
    {
        int GetDamage();
        int Get_OriginalDamage();
        int Get_OriginalHealth();
        List<GameTrigger<Specific_LifeEffect>> Get_BeforeReceivingDamage_Triggers();
        bool Get_MustBeAttacked();
        bool Get_CanAttack();
    }
}
