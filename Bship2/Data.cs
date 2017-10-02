using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Data
    {
        public class Shot
        {
            public Player source;
            public Player target;
            public int index;
            public bool success;
        }
    }
}
