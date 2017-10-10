using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Shot
    {
        public Player source;
        public Player target;
        public int index;
        public bool success;

        public Shot(Player source, Player target, int index, bool success)
        {
            this.source = source;
            this.target = target;
            this.index = index;
            this.success = success;
            this.source.shots.Add(this);
        }

    }
}
