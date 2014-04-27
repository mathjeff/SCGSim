using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    public interface ValueProvider<TOutput, TInput1>
    {
        TOutput GetValue(TInput1 input1, Game game, TOutput outputType);
    }
}
