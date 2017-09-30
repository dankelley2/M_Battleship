using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Battleship
{
    public class tcpNetwork
    {
        public NetworkContainer ParentForm;
        public int port;
        private Random identity;

        //Basic Constructor
        public tcpNetwork(NetworkContainer owner, int port)
        {
            this.ParentForm = owner;
            this.port = port;
            this.identity = new Random();
        }

        //Constructor with Console Output
        public tcpNetwork(NetworkContainer owner, int port, System.IO.TextWriter ConsoleOUT)
        {
            this.ParentForm = owner;
            this.port = port;
            this.identity = new Random();
            Console.SetOut(ConsoleOUT);
        }

        public class Listener
        {
            public int port;
            public TcpListener tcpl;
            public Timer refreshTimer;
            
            //Ad Hoc Constructor
            public Listener(int port)
            {
                this.port = port;
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                    this.tcpl = new TcpListener(endPoint);
                    this.tcpl.Server.ReceiveTimeout = 3000;
                    this.tcpl.Start();
                    Console.WriteLine("Listener Started");
                    refreshTimer = null;

                }
                catch (SocketException se)
                {
                    Console.WriteLine("Failed to Connect: " + se.Message);
                }
            }

            //Constructor with Timer
            public Listener(Timer refreshTimer, int port)
            {
                this.port = port;
                try
                {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                    this.tcpl = new TcpListener(endPoint);
                    this.tcpl.Server.ReceiveTimeout = 3000;
                    this.tcpl.Start();
                    Console.WriteLine("Listener Started");
                    refreshTimer.Start();

                }
                catch (SocketException se)
                {
                    Console.WriteLine("Failed to Connect: " + se.Message);
                }
            }

            public void Close()
            {
                if (tcpl != null)
                {
                    if (refreshTimer != null)
                    {
                        refreshTimer.Stop();
                    }
                    tcpl.Stop();
                    Console.WriteLine("Listener Stopped");
                }
            }

            public bool IsClientWaiting()
            {
                return this.tcpl.Pending();
            }

            
            public string Listen()
            {
                string rawdata = null;
                TcpClient Client = null;
                byte[] Buffer = new byte[4096];
                Client = this.tcpl.AcceptTcpClient();
                string sender = Client.Client.RemoteEndPoint.ToString();

                if (Client.Client.Available != 0)
                {
                    NetworkStream stream = Client.GetStream();
                    int byteCount = stream.Read(Buffer,0,Buffer.Length);
                    rawdata = Encoding.UTF8.GetString(Buffer, 0, byteCount);
                }
                return rawdata;
            }
        }

        //Parse Raw Data
        public Message ParseData(string rawdata)
        {
            try
            {
                if (rawdata.Substring(0, 6) == "START|" && rawdata.Substring(rawdata.Length - 4, 4) == "|END")
                {
                    string[] payload = rawdata.Split('|');
                    Message retMessage = new Message(payload[1], payload[2], payload[3], payload[4]);
                    return retMessage;
                }
                else
                {
                    Console.WriteLine("Error-Message did not contain START/END headers: \"{0}\"",rawdata);
                    return null;
                }
            }
            catch(ArgumentOutOfRangeException oor)
            {
                Console.WriteLine("Error-Data packet recieved was too short to parse: \"{0}\"", rawdata);
                return null;
            }


        }

        public void SendData(string IP, string data, string type)
        {
            TcpClient T = null;
            IPEndPoint IPE = null;
            NetworkStream N = null;

            int msgId = identity.Next(9999999);

            string str_msgId = msgId.ToString().PadLeft(7, '0');

            string message = "START|" + str_msgId + "|" + type + "|" + GetMyIP().ToString() + "|" + data.Replace("|", "") + "|END";

            try
            {
                T = new TcpClient();
                IPE = new IPEndPoint(IPAddress.Parse(IP), this.port);
                try
                {
                    T.Connect(IPE);
                    N = T.GetStream();
                    byte[] Buffer = Encoding.UTF8.GetBytes(message);
                    N.Write(Buffer, 0, Buffer.Length);
                    N.Flush();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("Error: " + se.Message);
                }
            }
            finally
            {
                if (N != null)
                {
                    N.Dispose();
                }
                if (T != null)
                {
                    T.Dispose();
                }
            }

        }

        public void SendData(string[] IPList, string data, string type)
        {
            TcpClient T = null;
            IPEndPoint IPE = null;
            NetworkStream N = null;
            foreach (string IP in IPList) {
                int msgId = identity.Next(9999999);
                string str_msgId = msgId.ToString().PadLeft(7, '0');
                string message = "START|" + str_msgId + "|" + type + "|" + GetMyIP().ToString() + "|" + data.Replace("|", "") + "|END";

                try
                {
                    T = new TcpClient();
                    IPE = new IPEndPoint(IPAddress.Parse(IP), this.port);
                    try
                    {
                        T.Connect(IPE);
                        N = T.GetStream();
                        byte[] Buffer = Encoding.UTF8.GetBytes(message);
                        N.Write(Buffer, 0, Buffer.Length);
                        N.Flush();
                    }
                    catch (SocketException se)
                    {
                        Console.WriteLine("Error: " + se.Message);
                    }
                }
                finally
                {
                    if (N != null)
                    {
                        N.Dispose();
                    }
                    if (T != null)
                    {
                        T.Dispose();
                    }
                }
            }

        }

        public void SendAck(Message M)
        {
            SendData(M.Sender,M.Id,"A");
        }
        
        public IPAddress GetMyIP()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address;
            }
        }

        public class Message
        {
            public string Id     { get; set; }
            public string Type   { get; set; }
            public string Sender { get; set; }
            public string Data   { get; set; }

            public Message(string Id, string Type, string Sender, string Data)
            {
                this.Id = Id;
                this.Type = Type;
                this.Sender = Sender;
                this.Data = Data;
            }
        }
    }
}

