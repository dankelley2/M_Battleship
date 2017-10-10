using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Battleship
{
    public class Player
    {
        public static List<Player> Playerlist = new List<Player>();
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
            PointF OldLocation = P.gameBoard.origin;
            float OldScale = P.gameBoard.scale;
            Player OldActivePlayer = Player.GetPlayerByPanel(MainWindow.PlayerLocations[1]);
            P.gameBoard.MoveTo(MainWindow.PlayerLocations[1].Location, 3);
            OldActivePlayer.gameBoard.MoveTo(OldLocation,OldScale);
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

        public static Player GetPlayerByPanel(Panel P)
        {
            foreach (Player player in Player.Playerlist)
            {
                if (Point.Round(player.gameBoard.origin).Equals(P.Location))
                return player;
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
