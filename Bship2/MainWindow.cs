using ConsoleRedirection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using tcpNet;

namespace Battleship
{
    public partial class MainWindow : Form
    {

        //Game Components
        public int gamePart = 1; //setup
        public Gameboard MyBoard;
        static Queue<Region> RenderQueue = new Queue<Region>();
        public bool gameready = false;
        public Timer CheckReadyTimer;
        public Queue<string> PlayerOrder = new Queue<string>();
        public int underCursor;
        public Rectangle CursorRect = new Rectangle();
        public Queue<Color> ColorQueue = new Queue<Color>();
        //public object PlayerOneSpot = new {point = new PointF(20,20),}

        //Setup
        public Queue<int> ShipSetupQueue = new Queue<int>(5);
        public int activeSquare = 1;
        public int activeSquare_prev = 0;
        public int shipRotation = 1;

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
            this.CheckTcpListener.Interval = (500);
            this.CheckTcpListener.Tick+=(CheckTcpListener_Tick);

            this.CheckReadyTimer = new System.Windows.Forms.Timer();
            this.CheckReadyTimer.Interval = (10000);
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
            remote.SendData(remote.CList.GetAllClientIPs(), "", "D");
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
            else if (remote.CList.GetAllClientIPs().Count != 0)
            {
                Console.WriteLine("You: {0}", this.txtMsg.Text);
                remote.SendData(remote.CList.GetAllClientIPs(), this.txtMsg.Text, "M");
                this.txtMsg.Clear();
            }
            else
            {
                Console.WriteLine("Players must join before sending messages.");
            }
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
            remote.SendData(remote.CList.GetAllClientIPs(), "R", "B");
        }
        private void ReadyOrNot(string IP)
        {
            remote.SendData(IP, gameready.ToString(), "RoN");
        }

        private void SendChangeTurn()
        {
            remote.SendData(remote.CList.GetAllClientIPs(), gameready.ToString(), "CT");
        }

        //CHECK FOR NEW MESSAGES
        private void CheckTcpListener_Tick(object sender, EventArgs e)
        {
            if (server.IsClientWaiting())
            {
                Get_Data();
            }
            PlayGame();
        }

        //GET NEW MESSAGES
        private void Get_Data()
        {
            tcpNetwork.Message message = remote.ParseData(server.Listen());
            if (message != null)
            {
                if (message.Type == "I") //Introduction
                {
                    string[] newClient = message.Data.Split(';');
                    if (IsHost)
                    {
                        //Send Everyone an introduciton
                        remote.SendData(remote.CList.GetAllClientIPs(), message.Data , "I");
                    }
                    remote.CList.AddNewClient(newClient[1], newClient[0]);
                    ShareMyContact(newClient[1]); //Introduction
                    Console.WriteLine("Welcome {0}!", newClient[0]);
                }
                if (message.Type == "SC") //Shared Contact
                {
                    string[] newClient = message.Data.Split(';');
                    remote.CList.AddNewClient(newClient[1], newClient[0]);
                    Console.WriteLine("Welcome {0}!", newClient[0]);
                }
                if (message.Type == "B") //Broadcast
                {
                    if (message.Data == "R")
                    {
                        ReadyOrNot(message.Sender);
                    }
                }
                if (message.Type == "GS") //GAME START
                {
                    gamePart = 4;
                }
                if (message.Type == "RoN") //Ready or not Response
                {
                    if (IsHost)
                    {
                        string clientresponse = message.Data.ToLower();
                        remote.CList.GetClientByIP(message.Sender).ClientAttr[0] = Convert.ToBoolean(clientresponse);
                    }
                }
                if (message.Type == "R") //Response
                {
                    string[] newClient = message.Data.Split(';');
                    remote.CList.AddNewClient(newClient[1], newClient[0]);
                    Console.WriteLine("Welcome {0}!", newClient[0]);
                }
                if (message.Type == "D") //Messages
                {
                    Console.WriteLine("{0} has disconnected.", remote.CList.GetClientByIP(message.Sender).NAME);
                    remote.CList.Clients.Remove(remote.CList.GetClientByIP(message.Sender));
                }
                if (message.Type == "M") //Messages
                {
                    DisplayConsoleMessage(message);
                }
                if (message.Type == "A") //Acknowledgement
                {
                    if (AckWaitList.Contains(message.Data))
                    {
                        AckWaitList.Remove(message.Data);
                    }
                }
                
            }
            ContactList.Items.Clear();
            foreach (string IP in remote.CList.GetAllClientIPs())
            {
                ContactList.Items.Add(remote.CList.GetClientByIP(IP).NAME + "@" + IP);
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
            AppendConsoleText(remote.CList.GetClientByMessage(M).NAME + "@" + remote.CList.GetClientByMessage(M).IP + ": " + M.Data);
        }

        /*******************
         * GAME FUNCTIONS
         *******************/

        private int GetPieceAtCursor()
        {
            PointF CursorPos = this.PointToClient(Cursor.Position);
            if (MyBoard.currentArea.Contains(CursorPos))
            {
                return (int)((CursorPos.X - MyBoard.origin.X) / (MyBoard.scale * 10) + 1) +
                    (int)((CursorPos.Y - MyBoard.origin.Y) / (MyBoard.scale * 10)) * 10;
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
                case 5: { DoTurn(); break; }
                //case 5: { SetShip5(); break; }
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
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Red));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Orange));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Yellow));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Green));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Blue));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Indigo));
            ColorQueue.Enqueue(Color.FromArgb(150, Color.Violet));

            //Initiate My game Board
            MyBoard = new Gameboard(MyIPAddress,GetNewBoardColor());
            MyBoard.MoveTo(20,20, 3F);
            ShipSetupQueue.Enqueue(4);
            ShipSetupQueue.Enqueue(3);
            ShipSetupQueue.Enqueue(2);
            ShipSetupQueue.Enqueue(2);
            ShipSetupQueue.Enqueue(1);
            remote.CList.AddNewClient(MyIPAddress, HostName);

            if (IsHost)
            {
                remote.CList.AddNewClient(MyIPAddress, HostName);
            }
            else
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
                    if (G.state == 1)
                    {
                        G.state = 0;
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
                        if (G.state != 0)
                        {
                            Console.WriteLine("Ship cannot be placed here");
                            return 1;
                        }
                    }
                    //Highlight new
                    foreach (Gameboard.GamePiece G in ShipList)
                    {
                        G.state = 1;
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
                foreach (Gameboard.GamePiece G in MyBoard.pieces)
                {
                    if (G.state == 1)
                    {
                        G.state = 2;
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
                        remote.CList.GetClientByIP(MyIPAddress).ClientAttr[0] = true;
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
                if (remote.CList.GetAllClientIPsByAttr(false, 0).Count > 0)
                {
                    Console.Write("Waiting on:");
                    foreach (string IP in remote.CList.GetAllClientIPsByAttr(false, 0))
                    {
                        Console.Write(" {0},", remote.CList.GetClientByIP(IP).NAME);
                    }
                    Console.Write(" To start the game.");
                    Console.WriteLine("");
                }
                if (remote.CList.GetAllClientIPs().Count > 0)
                {
                    if (remote.CList.GetAllClientIPsByAttr(false, 0).Count == 0)
                    {
                        CheckReadyTimer.Stop();
                        gamePart += 1;
                        remote.SendData(remote.CList.GetAllClientIPs(), "Players are Ready. Game starting.", "M");
                        Console.WriteLine("Players are Ready. Game starting.");
                        remote.SendData(remote.CList.GetAllClientIPs(), "GAME START", "GS");

                        //Set Play Order:
                        Console.Write("The Play order is: ");
                        string PlayOrderMessage = "The Play order is: ";
                        foreach (tcpNetwork.Client C in remote.CList)
                        {
                            PlayerOrder.Enqueue(C.IP); // Play order
                            Console.Write(" => {0}", C.NAME);
                            PlayOrderMessage += " => " + C.NAME;
                        }
                        Console.WriteLine("");
                        //send order message queue
                        remote.SendData(remote.CList.GetAllClientIPs(), PlayOrderMessage, "M");
                    }
                    else
                    {
                        AskReady();
                    }
                }
                else
                {
                    Console.WriteLine("Players must exist before the game starts, loner.");
                }
            }
        }

        private void CreateEnemyGrids()
        {

            Player.LocationScales.Add(new PointF(20, 20), 3F);   //Player1
            Player.LocationScales.Add(new PointF(370, 20), 3F);  //ActiveOpponent
            Player.LocationScales.Add(new PointF(355, 330), 1F); //mini
            Player.LocationScales.Add(new PointF(460, 330), 1F); //mini
            Player.LocationScales.Add(new PointF(565, 330), 1F); //mini

            Player MyPlayer = Player.AddPlayer(MyIPAddress, HostName);
            MyPlayer.gameBoard = MyBoard;
            //Add Locations
            Player.PlayerLocations.Add(MyPlayer, MyBoard.origin);

            foreach(tcpNetwork.Client C in remote.CList)
            {
                if (C.IP == MyIPAddress)
                {
                    continue;
                }

                Player NewPlayer = Player.AddPlayer(C.IP, C.NAME);
                Gameboard newBoard = new Gameboard(C.IP, GetNewBoardColor());
                NewPlayer.gameBoard = newBoard;
                
            }
            foreach (Player P in Player.GetAllPlayersExcept(MyIPAddress))
            {
                P.gameBoard.MoveTo(Player.PlayerLocations[P],Player.LocationScales[Player.PlayerLocations[P]]);
            }
            gamePart += 1;
            foreach (GameArt G in GameArt.Drawable)
            {
                G.needsRedraw = true;
            }
            Refresh();
            PlayGame();
        }

        public void DoTurn()
        {
            this.Refresh();
            if (IsHost)
            {
                
            }
        }

        /*******************
         * PAINT FUNCTIONS
         *******************/

        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.Low;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            //Graphics g = this.CreateGraphics();
            foreach (GameArt G in GameArt.Drawable)
            {
                G.Draw(e.Graphics);
            }
            if (gamePart == 2)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100,Color.White)), CursorRect);
            }
            //g.Dispose();
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
                    RotateShips();
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