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
        public List<Shot> shots = new List<Shot>();
        public string address { get; }
        public string Name { get; }
        public int Id;
        public bool Loser = false;
        public bool ReadyToPlay = false;
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

        public static void MakeActiveBoard(Player P)
        {
            PointF OldLocation = Player.PlayerLocations[P];
            Player OldActivePlayer = Player.PlayerLocations.FirstOrDefault(x => (new PointF(370, 20).Equals(x.Value))).Key;
            Player.PlayerLocations.Remove(P);
            Player.PlayerLocations.Remove(OldActivePlayer);
            Player.PlayerLocations.Add(OldActivePlayer, OldLocation);
            Player.PlayerLocations.Add(P, new PointF(370, 20));
            P.gameBoard.MoveTo(Player.PlayerLocations[P], Player.LocationScales[Player.PlayerLocations[P]]);
            OldActivePlayer.gameBoard.MoveTo(Player.PlayerLocations[OldActivePlayer], Player.LocationScales[Player.PlayerLocations[OldActivePlayer]]);
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

        public static List<string> GetAllPlayerIPs()
        {
            return Playerlist.Select(s => s.address).ToList();
        }

        public static List<string> GetAllPlayerIPsExcept(string IP)
        {
            return Playerlist.Select(s => s.address)
                .Where(s => s != IP).ToList();
        }

        public static Player CheckLosers()
        {
            foreach (Player P in Playerlist)
            {
                int CountShipPieces = 0;
                foreach (Gameboard.GamePiece G in P.gameBoard.pieces)
                {
                    if (G.drawState == 2)
                    {
                        CountShipPieces++;
                    }
                }
                if (CountShipPieces == 0)
                {
                    P.Loser = true;
                    return P;
                }
            }
            return null;
        }

        public static bool IsEveryoneReadyToPlay()
        {
            foreach (Player P in Playerlist)
            {
                if (P.ReadyToPlay == false)
                    return false;
            }
            return true;
        }

        public bool Equals(Player P2)
        {
            if (this.address == P2.address)
                return true;
            else
                return false;
        }
    }
}
