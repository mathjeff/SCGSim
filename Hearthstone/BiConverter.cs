using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// supports two-way conversion between two types
namespace Games
{
    public interface BiConverter<T1, T2>
    {
        T2 Convert(T1 t1);
        T1 ConvertBack(T2 t2);
    }
}
