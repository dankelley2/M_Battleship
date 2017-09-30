using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Player
    {
        public static List<Player> Playerlist = new List<Player>();
        public static Dictionary<Player, PointF> PlayerLocations = 
            new Dictionary<Player, PointF>();
        public static Dictionary<PointF, float> LocationScales = 
            new Dictionary<PointF, float>();
        public List<Data.Shot> shots = new List<Data.Shot>();
        public string address { get; }
        public string Name { get; }
        public int Id;
        public Gameboard gameBoard { get; set; }

        public Player(string address, string name)
        {
            this.address = address;
            this.Name = name;
            Playerlist.Add(this);
            Id = Playerlist.Count;
            switch (Id)
            {
                case 2: { Player.PlayerLocations.Add(this, new PointF(370, 20)); break; }
                case 3: { Player.PlayerLocations.Add(this, new PointF(355, 330)); break; }
                case 4: { Player.PlayerLocations.Add(this, new PointF(460, 330)); break; }
                case 5: { Player.PlayerLocations.Add(this, new PointF(465, 330)); break; }
            }
        }

        public static Player GetPlayerByAddress(string address)
        {
            foreach(Player P in Playerlist)
            {
                if (address == P.address)
                {
                    return P;
                }
            }
            return null;
        }

        public static Player AddPlayer(string address, string name)
        {
            Player P = new Player(address, name);
            return P;
        }

        public static IEnumerable<Player> GetAllPlayersExcept(string IP)
        {
            return Playerlist.Where(s => s.address != IP);
        }
    }
}
