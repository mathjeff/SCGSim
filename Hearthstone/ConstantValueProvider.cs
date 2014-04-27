using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class ConstantValueProvider<TOutput, TInput> : ValueProvider<TOutput, TInput>
    {
        public ConstantValueProvider(TOutput valueToProvide)
        {
            this.value = valueToProvide;
        }
        public TOutput GetValue(TInput input1, Game input2, TOutput outputType)
        {
            return this.value;
        }
        private TOutput value;

    }
}
