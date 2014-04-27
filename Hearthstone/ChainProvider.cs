using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// A ChainProvider just connects two ValueProviders
namespace Games
{
    class ChainProvider<TOutput, TIntermediate, TInput> : ValueProvider<TOutput, TInput>
    {
        public ChainProvider(ValueProvider<TOutput, TIntermediate> converter2, ValueProvider<TIntermediate, TInput> converter1)
        {
            this.converter1 = converter1;
            this.converter2 = converter2;
        }

        public TOutput GetValue(TInput input, Game game, TOutput outputType)
        {
            return this.converter2.GetValue(this.converter1.GetValue(input, game, default(TIntermediate)), game, outputType);
        }

        ValueProvider<TIntermediate, TInput> converter1;
        ValueProvider<TOutput, TIntermediate> converter2;
    }
}
