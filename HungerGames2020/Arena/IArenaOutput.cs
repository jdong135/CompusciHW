using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arena
{
    public interface IArenaOutput
    {
        void Process(CompleteTurn turn);
    }
}
