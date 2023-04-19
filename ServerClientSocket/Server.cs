using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServerClientSocket
{
    public class Server
    {


        /// <summary> 
        /// Background thread for TcpServer workload. 	
        /// </summary> 	
        private Thread tcpListenerThread;
        
      

        public void Start()
        {
            // Start TcpServer background thread 		
            tcpListenerThread = new Thread(new ThreadStart(ListenForConnection));
            tcpListenerThread.IsBackground = true;
            tcpListenerThread.Start();
        }


        private byte[] buffer = new byte[1024];

        // Nastavení adresy a portu pro naslouchání

        

        private IPAddress ipAddressOfServer ;
        private int port = 8000;
        private IPEndPoint localEndPoint;
        private Socket listener;

        public Server(string IpAddress, int Port)
        {
            ipAddressOfServer = IPAddress.Parse(IpAddress);
            port = Port;
            localEndPoint = new IPEndPoint(ipAddressOfServer, port);
            listener = new Socket(ipAddressOfServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Console.WriteLine("Server čeká na spojení...");
        }

        

        private void ListenForConnection()
        {
            while (true)
            {
                // Přijetí příchozího spojení
                
                Socket handler = listener.Accept();

                string data = null;

                while (true)
                {
                    buffer = new byte[1024];
                    int bytesRec = handler.Receive(buffer);



                    data += Encoding.ASCII.GetString(buffer, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

                Console.WriteLine("Přijato: {0}", data);

                // Odeslání zprávy zpět klientovi
                byte[] msg = Encoding.ASCII.GetBytes("Potvrzení o přijetí: " + data);
                handler.Send(msg);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
    }
}
