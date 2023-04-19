using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ServerClientSocket
{
    internal class Client
    {
        byte[] bytes = new byte[1024];

        // Nastavení adresy a portu serveru
        IPAddress ipAddress;
        int port  ;
        IPEndPoint remoteEP;

        // Vytvoření nového soketu
        Socket sender;

        // Připojení soketu k serveru
       


        // Odeslání zprávy serveru
       

      

        public Client()
        {
            try
            {
                ipAddress = IPAddress.Parse("127.0.0.1");
                port = 8000;
                remoteEP = new IPEndPoint(ipAddress, port);
              
               

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
            


       public void SendMSG()
        {
            sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(remoteEP);
            Console.WriteLine("Připojeno k serveru: {0}", sender.RemoteEndPoint.ToString());
            
            string message = "Toto je testovací zpráva <EOF>";
            byte[] msg = Encoding.ASCII.GetBytes(message);
            
            int bytesSent = sender.Send(msg);

            // Přijetí potvrzení od serveru
            int bytesRec = sender.Receive(bytes);
            Console.WriteLine("Potvrzení: {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));

            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

      
    }
}






    