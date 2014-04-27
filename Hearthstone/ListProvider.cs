using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Games
{
    class ListProvider<TOutput, TInput1> : ValueProvider<IList<TOutput>, TInput1>
    {
        public ListProvider(ValueProvider<TOutput, TInput1> singletonProvider)
        {
            this.singletonProvider = singletonProvider;
        }
        public IList<TOutput> GetValue(TInput1 input1, Game game, IList<TOutput> outputType)
        {
            IList<TOutput> list = new List<TOutput>();
            list.Add(this.singletonProvider.GetValue(input1, game, default(TOutput)));
            return list;
        }
        private ValueProvider<TOutput, TInput1> singletonProvider;
    }
}
