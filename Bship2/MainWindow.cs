using ConsoleRedirection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using tcpNet;

namespace Battleship
{
    public partial class MainWindow : Form
    {

        //Game Components
        public int gamePart = 1; //setup
        public int firstReady = 1;
        public Player MyPlayer;
        public Gameboard MyBoard;
        static Queue<Region> RenderQueue = new Queue<Region>();
        public bool gameready = false;
        public Timer CheckReadyTimer;
        public Queue<string> PlayerOrder = new Queue<string>();
        public int underCursor;
        public Rectangle CursorRect = new Rectangle();
        public Queue<Color> ColorQueue = new Queue<Color>();
        public int Ammo = 0;

        //Setup
        public Queue<int> ShipSetupQueue = new Queue<int>(5);
        public int activeSquare = 1;
        public int activeSquare_prev = 0;
        public int shipRotation = 1;
        public Player CurrentPlayer;
        public static List<Panel> PlayerLocations = new List<Panel>();

        //Join or host?
        public bool IsHost;
        public string JoinIP;
        public string HostName;
        public string MyIPAddress;
        private List<string> AckWaitList = new List<string>();

        //REMOTE COMPONENTS
        public tcpNetwork remote;
        public Timer CheckTcpListener;
        tcpNetwork.Listener server;

        //public console output
        public TextBoxStreamWriter _writer;

        public MainWindow()
        {
            SetupWindow setup = new SetupWindow();
            setup.ShowDialog();
            if (setup.StopApplication) { Environment.Exit(0); } // quit if setup was not completed
            IsHost = setup.Host;
            JoinIP = setup.JoinIP;
            HostName = setup.HostName;
            InitializeComponent();
        }

        //INITIATE TIMER AND SERVER ON LOAD
        private void MainWindow_Load(object sender, EventArgs e)
        {
            
            // Instantiate the writer
            _writer = new ConsoleRedirection.TextBoxStreamWriter(txtConsole);
            // Redirect the out Console stream
            Console.SetOut(_writer);
            this.CheckTcpListener = new System.Windows.Forms.Timer();
            this.CheckTcpListener.Interval = (300);
            this.CheckTcpListener.Tick+=(CheckTcpListener_Tick);

            this.CheckReadyTimer = new System.Windows.Forms.Timer();
            this.CheckReadyTimer.Interval = (6000);
            this.CheckReadyTimer.Tick += (WaitForReady);

            this.remote = new tcpNetwork(8888,true,HostName, _writer);
            this.server = new tcpNetwork.Listener(this.CheckTcpListener, 8888, 2048);
            this.MyIPAddress = tcpNetwork.GetMyIP().ToString();
            this.Text = "Battleship -- " + MyIPAddress;
            PlayGame();
        }
        
        //CLEAN UP ON CLOSING
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "", "D");
            this.CheckTcpListener.Dispose();
            this.server.Close();
        }

        //ADD TEXT TO CONSOLE
        public void AppendConsoleText(string message)
        {
            this.txtConsole.AppendText(message);
            this.txtConsole.AppendText(Environment.NewLine);
        }

        //SEND MESSAGE
        private void SendMessage(object sender, EventArgs e)
        {
            if (this.txtMsg.Text.Trim(' ') == "")
            {
                return;
            }
            else if (Player.GetAllPlayerIPsExcept(MyIPAddress).Count != 0)
            {
                Console.WriteLine("You: {0}", this.txtMsg.Text);
                remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), this.txtMsg.Text, "M");
                this.txtMsg.Clear();
            }
            else
            {
                Console.WriteLine("Players must join before sending messages.");
            }
        }
        private void SendHitMiss(int target, bool HitMiss)
        {
                remote.SendData(Player.GetAllPlayerIPs(), MyIPAddress + ";" + target + ";" + HitMiss, "SR");
        }
        private void SendShot(Player P, int index)
        {
            remote.SendData(P.address, index.ToString(), "S");
            Console.WriteLine("Shot fired on " + P.Name + "!");
        }
        private void SendIntroduction(string IP)
        {
            remote.SendData(IP, remote.HostName + ";" + MyIPAddress, "I"); //Introduction
        }
        private void ShareMyContact(string IP)
        {
            remote.SendData(IP, remote.HostName + ";" + MyIPAddress, "SC");
        }
        private void AskReady()
        {
            remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "R", "B");
        }
        private void ReadyOrNot(string IP)
        {
            remote.SendData(IP, gameready.ToString(), "RoN");
        }

        private void SendChangeTurn(Player CPlayer)
        {
            remote.SendData(Player.GetAllPlayerIPs(), CPlayer.address , "CT");
        }

        //CHECK FOR NEW MESSAGES
        private void CheckTcpListener_Tick(object sender, EventArgs e)
        {
            if (server.IsClientWaiting())
            {
                Get_Data();
            }
        }

        //GET AND PARSE NEW MESSAGES
        private void Get_Data()
        {
            tcpNetwork.Message message = remote.ParseData(server.Listen());
            if (message != null)
            {
                if (message.Type == "I") //Introduction
                {
                    string[] newClient = message.Data.Split(';');
                    //if (gamePart > 2)
                    //{
                    //    remote.SendData(newClient[1], "Sorry, this game is already in session", "M");
                    //    Console.WriteLine("{0}@{1} tried to join but was denied.",newClient[0],newClient[1]);
                    //    return;
                    //}
                    if (IsHost)
                    {
                        //Send Everyone an introduciton
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), message.Data , "I");
                    }
                    Player.AddPlayer(newClient[1], newClient[0]);
                    ShareMyContact(newClient[1]); //Introduction
                    Console.WriteLine("Welcome {0}!", newClient[0]);
                }
                else if (message.Type == "SC") //Shared Contact
                {
                    string[] newClient = message.Data.Split(';');
                    Player.AddPlayer(newClient[1], newClient[0]);
                    Console.WriteLine("Welcome {0}!", newClient[0]);
                }
                else if (message.Type == "B") //Broadcast
                {
                    if (message.Data == "R")
                    {
                        ReadyOrNot(message.Sender);
                    }
                }
                else if (message.Type == "GS") //GAME START
                {
                    gamePart = 4;
                    PlayGame();
                }
                else if (message.Type == "RoN") //Ready or not Response
                {
                    if (IsHost)
                    {
                        string clientresponse = message.Data.ToLower();
                        Player.GetPlayerByAddress(message.Sender).ReadyToPlay = Convert.ToBoolean(clientresponse);
                    }
                }
                else if (message.Type == "S")
                {
                    int target = Convert.ToInt16(message.Data);
                    if (MyBoard.GetGamePieceState(target) == 0) // Miss
                    {
                        MyBoard.SetGamePieceState(MyBoard.GetPieceAtIndex(target), 1);
                        SendHitMiss(target, false);
                    }
                    else if (MyBoard.GetGamePieceState(target) == 2) // Hit!
                    {
                        MyBoard.SetGamePieceState(MyBoard.GetPieceAtIndex(target), 3);
                        SendHitMiss(target, true);
                    }
                    Refresh();
                }
                else if (message.Type == "SR") //Shot Response
                {
                    string[] shot = message.Data.Split(';');
                    Player P = Player.GetPlayerByAddress(shot[0]);
                    int target = Convert.ToInt16(shot[1]);
                    if (message.Sender != MyIPAddress)
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "Shot fired on " + P.Name + "!", "CM");
                    if (Convert.ToBoolean(shot[2]) == true)
                    {
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "It's a Hit","CM");

                        P.gameBoard.SetGamePieceState(P.gameBoard.GetPieceAtIndex(target), 3);
                        Shot thisShot = new Shot(CurrentPlayer, P, target, true);
                        if (IsHost)
                        {
                            SetupTurn(true);
                        }
                    }
                    if (Convert.ToBoolean(shot[2]) == false)
                    {
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "It was a doozy", "CM");
                        P.gameBoard.SetGamePieceState(P.gameBoard.GetPieceAtIndex(target), 1);
                        Shot thisShot = new Shot(CurrentPlayer, P, target, false);
                        if (IsHost)
                        {
                            SetupTurn(false);
                        }
                    }

                    Refresh();
                }
                else if (message.Type == "CT") //Response
                {
                    string WhoIsNext = message.Data;
                    CurrentPlayer = Player.GetPlayerByAddress(WhoIsNext);
                    AnnounceNextPlayer(CurrentPlayer);
                }
                else if (message.Type == "D") //Disconnect Message
                {
                    Console.WriteLine("{0} has disconnected.", Player.GetPlayerByAddress(message.Sender).Name);
                    Player.Playerlist.Remove(Player.GetPlayerByAddress(message.Sender));
                }
                else if (message.Type == "M") //Messages
                {
                    DisplayConsoleMessage(message);
                }
                else if (message.Type == "CM") //Messages
                {
                    Console.WriteLine(message.Data);
                }
                else if (message.Type == "A") //Acknowledgement
                {
                    if (AckWaitList.Contains(message.Data))
                    {
                        AckWaitList.Remove(message.Data);
                    }
                }
                
            }
            ContactList.Items.Clear();
            foreach (string IP in Player.GetAllPlayerIPsExcept(MyIPAddress))
            {
                ContactList.Items.Add(Player.GetPlayerByAddress(IP).Name + "@" + IP);
            }

            
        }

        private void DisplayConsoleMessageInfo(tcpNetwork.Message M)
        {
            AppendConsoleText("Id: " + M.Id);
            AppendConsoleText("Type: " + M.Type);
            AppendConsoleText("From: " + M.Sender);
            AppendConsoleText("data: " + M.Data);
        }
        private void DisplayConsoleMessage(tcpNetwork.Message M)
        {
            AppendConsoleText(Player.GetPlayerByAddress(M.Sender).Name + ": " + M.Data);
        }

        /*******************
         * GAME FUNCTIONS
         *******************/

        private int GetPieceAtCursor()
        {
            PointF CursorPos = this.PointToClient(Cursor.Position);
            foreach (Player P in Player.Playerlist)
            {
                if (P.gameBoard.currentArea.Contains(CursorPos))
                {
                    return (int)((CursorPos.X - P.gameBoard.origin.X) / (P.gameBoard.scale * 10) + 1) +
                        (int)((CursorPos.Y - P.gameBoard.origin.Y) / (P.gameBoard.scale * 10)) * 10;
                }
            }
            return -1;
        }

        private Color GetNewBoardColor()
        {
            Color C = ColorQueue.Dequeue();
            ColorQueue.Enqueue(C);
            return C;
        }

        private Player GetNextPlayer()
        {
            string IP = PlayerOrder.Dequeue();
            Player NextPlayer = Player.GetPlayerByAddress(IP);
            PlayerOrder.Enqueue(IP);
            return NextPlayer;
        }

        public void PlayGame()
        {
            switch (gamePart)
            {
                case 1: { SetupGame(); break; }
                case 2: { PlaceShips(false); break; }
                case 4: { CreateEnemyGrids(); break; }
                case 5: { SetupTurn(false); break; }
                //case 6: { PlayTurn(); break; }
            }

            //INVALIDATE NEEDED REFRESHES////
            foreach (GameArt G in GameArt.Drawable)
            {
                if (G.needsRedraw)
                {
                    this.Invalidate(new Region(G.currentArea));
                }
            }
            if (RenderQueue.Count > 0)
            {
                this.Invalidate(RenderQueue.Dequeue());
            }
            this.Update();
            /////////////////////////////////
            activeSquare_prev = activeSquare;
        }

        public void SetupGame()
        {
            //Get PanelLocations
            PlayerLocations.Add(Player1Panel);
            PlayerLocations.Add(Player2Panel);
            PlayerLocations.Add(Player3Panel);
            PlayerLocations.Add(Player4Panel);
            PlayerLocations.Add(Player5Panel);

            ColorQueue.Enqueue(Color.FromArgb(150, Color.SkyBlue));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Orange));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Yellow));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Green));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Red));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Indigo));
            ColorQueue.Enqueue(Color.FromArgb(75, Color.Violet));

            //Initiate My game Board
            this.MyPlayer = Player.AddPlayer(MyIPAddress, HostName); //Add Player
            MyBoard = new Gameboard(MyIPAddress,GetNewBoardColor(),PlayerLocations[MyPlayer.Id-1]);
            MyPlayer.gameBoard = MyBoard;
            ShipSetupQueue.Enqueue(4);
            ShipSetupQueue.Enqueue(3);
            ShipSetupQueue.Enqueue(2);
            ShipSetupQueue.Enqueue(2);
            ShipSetupQueue.Enqueue(1);

            ////Add Players
            //Player.LocationScales.Add(new PointF(20, 20), 3F);   //Player1
            //Player.LocationScales.Add(new PointF(370, 20), 3F);  //ActiveOpponent
            //Player.LocationScales.Add(new PointF(355, 330), 1F); //mini
            //Player.LocationScales.Add(new PointF(460, 330), 1F); //mini
            //Player.LocationScales.Add(new PointF(565, 330), 1F); //mini
            //
            ////Add Locations
            //Player.PlayerLocations.Add(MyPlayer, MyBoard.origin);

            if (!(IsHost))
            {
                SendIntroduction(JoinIP);
            }
            gamePart += 1;
            Console.WriteLine("Use the mouse to place your ships, R to rotate, and S to set the ships position.");
            Console.WriteLine("Place a {0} block ship", (ShipSetupQueue.Peek() + 1).ToString());
            SetCursorRectangle();
            PlayGame();
        }

        private int PlaceShips(bool force)
        {
            if (activeSquare == activeSquare_prev && !(force))
            {
                return 1;
            }
            if (ShipSetupQueue.Count > 0)
            {

                Invalidate();
                List<Gameboard.GamePiece> ShipList = new List<Gameboard.GamePiece>();
                int squarecount = ShipSetupQueue.Peek();
                switch (shipRotation)
                {
                    case 1: { ShipList = MyBoard.Get_RightPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 2: { ShipList = MyBoard.Get_BelowPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 3: { ShipList = MyBoard.Get_LeftPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 4: { ShipList = MyBoard.Get_AbovePieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                }
                //undo previous
                foreach (Gameboard.GamePiece G in MyBoard.pieces)
                {
                    if (G.drawState == 4)
                    {
                        G.drawState = 0;
                    }
                }
                if (ShipList == null)
                {
                    Console.WriteLine("Ship cannot be placed here");
                    return 1;
                }
                else
                {
                    //Check for collisions
                    foreach (Gameboard.GamePiece G in ShipList)
                    {
                        if (G.drawState != 0)
                        {
                            Console.WriteLine("Ship cannot be placed here");
                            return 1;
                        }
                    }
                    //Highlight new
                    foreach (Gameboard.GamePiece G in ShipList)
                    {
                        G.drawState = 4;
                        MyBoard.needsRedraw = true;
                    }
                }
                return 0;
            }
            return 1;
        }

        private void LockShips()
        {
            if (ShipSetupQueue.Count > 0 && PlaceShips(true) == 0)
            {
                int squarecount = ShipSetupQueue.Peek();
                List<Gameboard.GamePiece> ShipList = new List<Gameboard.GamePiece>();
                switch (shipRotation)
                {
                    case 1: { ShipList = MyBoard.Get_RightPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 2: { ShipList = MyBoard.Get_BelowPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 3: { ShipList = MyBoard.Get_LeftPieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                    case 4: { ShipList = MyBoard.Get_AbovePieces(MyBoard.GetPieceAtIndex(activeSquare), squarecount); break; }
                }
                MyBoard.AddShipLocation(ShipList); // Add to set ships

                foreach (Gameboard.GamePiece G in MyBoard.pieces) // change draw state
                {
                    if (G.drawState == 4)
                    {
                        G.drawState = 2;
                        G.pieceDirection = shipRotation;

                    }
                }


                SetCursorRectangle();
                ShipSetupQueue.Dequeue();
                Refresh();
                if (ShipSetupQueue.Count > 0)
                {
                    Console.WriteLine("Place a {0} block ship", (ShipSetupQueue.Peek() + 1).ToString());
                }
                else
                {
                    gamePart += 1;
                    gameready = true;
                    if (IsHost)
                    {
                        CheckReadyTimer.Start();
                        Player.GetPlayerByAddress(MyIPAddress).ReadyToPlay = true;
                    }
                    RotateShip.Hide();
                    ConfirmShip.Hide();
                    Invalidate();
                    PlayGame();
                }
            }
        }

        private void RotateShips()
        {
            shipRotation++;
            if (shipRotation > 4)
            {
                shipRotation = 1;
            }
            SetCursorRectangle();
            UpdateCursorRectangle(Point.Round(MyBoard.rectPieces[activeSquare - 1].Location));
            PlaceShips(true);
        }

        private void WaitForReady(object sender, EventArgs e)
        {
            if (IsHost)
            {
                if (Player.GetAllPlayerIPsExcept(MyIPAddress).Count > 0)
                {
                    if (Player.IsEveryoneReadyToPlay())
                    {
                        CheckReadyTimer.Stop();
                        gamePart += 1;
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "Players are Ready. Game starting.", "CM");
                        Console.WriteLine("Players are Ready. Game starting.");
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), "GAME START", "GS");

                        //Set Play Order:
                        Console.Write("The Play order is: ");
                        string PlayOrderMessage = "The Play order is: ";
                        foreach (Player P in Player.Playerlist)
                        {
                            PlayerOrder.Enqueue(P.address); // Play order
                            Console.Write(" => {0}", P.Name);
                            PlayOrderMessage += " => " + P.Name;
                        }
                        Console.WriteLine("");
                        //send order message queue
                        remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), PlayOrderMessage, "CM");
                    }
                    else
                    {
                        AskReady();
                        if (firstReady == 0)
                        {
                            Console.Write("Waiting on:");
                            foreach (string IP in Player.GetAllPlayerIPsExcept(MyIPAddress))
                            {
                                Console.Write(" {0},", Player.GetPlayerByAddress(IP).Name);
                            }
                            Console.Write(" To start the game.");
                            Console.WriteLine("");
                        }
                        firstReady = 0;
                    }
                }
                else
                {
                    Console.WriteLine("Players must exist before the game starts, loner.");
                }
            }
            if (gamePart == 4)
            {
                PlayGame();
            }
        }

        private void CreateEnemyGrids()
        {

            foreach(Player P in Player.Playerlist)
            {
                if (P.address == MyIPAddress)
                {
                    continue;
                }
                
                Gameboard newBoard = new Gameboard(P.address, GetNewBoardColor(),PlayerLocations[P.Id-1]);
                P.gameBoard = newBoard;
                
            }
            gamePart += 1;
            foreach (GameArt G in GameArt.Drawable)
            {
                G.needsRedraw = true;
            }
            Refresh();
            PlayGame();
        }

        public void SetupTurn(bool SamePlayer)
        {
            //CheckForLoser
            //Player Loser = Player.CheckLosers();
            //if (Loser != null)
            //{
            //    remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), Loser.Name + " has fallen!", "M");
            //    Console.WriteLine(Loser.Name + " has fallen!");
            //}
            //if (Player.Playerlist.Where(p => p.Loser == false).Count() == 1)
            //{
            //    Player winner = Player.Playerlist.Where(p => p.Loser == false).First();
            //    remote.SendData(Player.GetAllPlayerIPsExcept(MyIPAddress), winner.Name + " has Won! The game will go on because I haven't programmed this part yet!", "M");
            //    Console.WriteLine(winner.Name + " has Won! The game will go on because I haven't programmed this part yet!");
            //    gamePart = 7;
            //    return;
            //}

            if (IsHost)
            {
                if (!(SamePlayer))
                {
                    CurrentPlayer = GetNextPlayer();
                }
                SendChangeTurn(CurrentPlayer);
            }
            StatusLight.Show();
            gamePart = 6;
            PlayGame();
        }

        private void FinishGame()
        {
            throw new NotImplementedException();
        }

        public void AnnounceNextPlayer(Player P)
        {
            if (P.address == MyIPAddress)
            {
                Console.WriteLine("Your Shot {0}! Choose a game board and square!", P.Name);
                StatusLight.Image = Bship2.Properties.Resources.Light_Green;
                Ammo = 1;
            }
            else
            {
                Console.WriteLine("{0}'s Turn.. Good Luck!", P.Name);
                StatusLight.Image = Bship2.Properties.Resources.Light_Red;
            }
            Refresh();
        }

        /*******************
         * PAINT FUNCTIONS
         *******************/

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.Low;
            e.Graphics.CompositingQuality = CompositingQuality.Default;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            Player P1 = Player.GetPlayerByPanel(PlayerLocations[0]);
            Player P2 = Player.GetPlayerByPanel(PlayerLocations[1]);
            foreach (GameArt G in GameArt.Drawable)
            {
                G.Draw(e.Graphics);
            }
            if (gamePart > 2)
            {
                if (P1 != null) //Draw P1 Ships
                {
                    P1.gameBoard.DrawShips(e.Graphics, Player1Ships);
                }
                if (P2 != null) //Draw P2 Ships
                {
                    P2.gameBoard.DrawShips(e.Graphics, Player2Ships);
                }
            }
            if (gamePart == 2 && !CursorRect.Location.Equals(new Point(0,0)))
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(50,Color.White),2), CursorRect);
            }
        }

        private void SetCursorRectangle()
        {
            int RectHeight = (int)MyBoard.scale * 10;
            int RectLength = RectHeight * (ShipSetupQueue.Peek() + 1);
            if (shipRotation % 2 == 1)
            {
                CursorRect.Width = RectLength;
                CursorRect.Height = RectHeight;
            }
            else
            {
                CursorRect.Width = RectHeight;
                CursorRect.Height = RectLength;
            }
            
        }
        private void UpdateCursorRectangle(Point Pos)
        {
            SetCursorRectangle();
            if (shipRotation == 1) //RIGHT
            {
                CursorRect.X = Pos.X;
                CursorRect.Y = Pos.Y;
            }
            else if (shipRotation == 2) //DOWN
            {
                CursorRect.X = Pos.X;
                CursorRect.Y = Pos.Y;
            }
            else if (shipRotation == 3) //LEFT
            {
                CursorRect.X = Pos.X - CursorRect.Width + CursorRect.Height;
                CursorRect.Y = Pos.Y;
            }
            else if (shipRotation == 4) //UP
            {
                CursorRect.X = Pos.X;
                CursorRect.Y = Pos.Y - CursorRect.Height + CursorRect.Width;
            }

        }

        private void MainWindow_MouseMove(object sender, EventArgs e)
        {
        }


        /*******************
         * EVENT FUNCTIONS
         *******************/

        private void MainWindow_Click(object sender, EventArgs e)
        {
            PointF CursorPos = this.PointToClient(Cursor.Position);
            if (gamePart < 5)
            {
                if (GetPieceAtCursor() != -1)
                {
                    activeSquare = GetPieceAtCursor();

                    if (gamePart == 2 && ShipSetupQueue.Count > 0) //Ship Setup
                    {
                        if (activeSquare != underCursor)
                        {
                            underCursor = activeSquare;
                            Point Location = Point.Round(MyBoard.rectPieces[underCursor - 1].Location);
                            UpdateCursorRectangle(Location);
                        }
                    }

                }
            }
            else // take shot
            {
                if (CurrentPlayer.address == MyIPAddress && Ammo > 0) //If It's my turn
                {
                    int target = GetPieceAtCursor(); //Get Piece
                    if (target != -1) //If you clicked a board
                    {
                        foreach (Player P in Player.GetAllPlayersExcept(MyIPAddress))// foreach player
                        {
                            if (P.gameBoard.origin.Equals(new PointF(370,20)) && P.gameBoard.currentArea.Contains(CursorPos))// If you clicked the ACTIVE board
                            {
                                if (P.gameBoard.GetGamePieceState(target) == 0)//And if That piece has not already been fired upon
                                {
                                    Ammo -= 1;
                                    SendShot(P, target); // send a shot
                                }
                            }
                            else if (!(P.gameBoard.origin.Equals(PlayerLocations[1].Location)) && P.gameBoard.currentArea.Contains(CursorPos)) //clicked smaller board
                            {
                                Player.MakeActiveBoard(P);
                                Refresh();
                            }
                        }
                    }
                }
            }
            PlayGame();
        }

        private void txtMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMessage(this, e);
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        { if (GetChildAtPoint(this.PointToClient(Cursor.Position)) == null){
                if (e.KeyCode == Keys.R)
                {
                    if (gamePart == 2)
                    {
                        RotateShips();
                    }
                    else
                    {
                        this.Invalidate();
                    }
                    
                }
                if (e.KeyCode == Keys.S)
                {
                    LockShips();
                }
            }
        }

        private void RotateShip_Click(object sender, EventArgs e)
        {
            RotateShips();
        }

        private void ConfirmShip_Click(object sender, EventArgs e)
        {
            LockShips();
        }
    }


}

namespace ConsoleRedirection
{
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); // When character data is written, append it to the text box.
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}