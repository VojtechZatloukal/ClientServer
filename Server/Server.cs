using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
  
        public class Server
        {
            #region private members 	
            /// <summary> 	
            /// TCPListener to listen for incomming TCP connection 	
            /// requests. 	
            /// </summary> 	
            private TcpListener tcpListener;
            /// <summary> 
            /// Background thread for TcpServer workload. 	
            /// </summary> 	
            private Thread tcpListenerThread;
            /// <summary> 	
            /// Create list of handles to connected tcp client. 	
            /// </summary> 	
            private List<KeyValuePair<TcpClient, Thread>> connectedClients;
            #endregion

            // Use this for initialization
            public void Start(string listeningIP, int listeningPort)
            {
                connectedClients = new List<KeyValuePair<TcpClient, Thread>>();

                // Start TcpServer background thread 
                tcpListenerThread = new Thread(new ThreadStart(() =>
                {
                    ListenForIncommingRequests(listeningIP, listeningPort);
                }));
                tcpListenerThread.IsBackground = true;
                tcpListenerThread.Start();
            }

            // Update is called once per frame
            public void Update()
            {
                SendMessage();
            }

            /// <summary> 	
            /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
            /// </summary> 	
            private void ListenForIncommingRequests(string listeningIP, int listeningPort)
            {
                try
                {
                    // Get ip address from console
                    // Create listener on localhost port 8052.

                    tcpListener = new TcpListener(IPAddress.Parse(listeningIP), listeningPort);
                    tcpListener.Start();
                    Console.WriteLine($"Server is listening on ip {listeningIP}:{listeningPort}");
                    Byte[] bytes = new Byte[1024];
                    while (true)
                    {
                        TcpClient connectedTcpClient = tcpListener.AcceptTcpClient();

                        // Get a stream object for writing. 			
                        NetworkStream stream = connectedTcpClient.GetStream();
                        if (stream.CanWrite)
                        {
                            string serverMessage = "Connection acknowledged!";
                            // Convert string message to byte array.                 
                            byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                            // Write byte array to socketConnection stream.               
                            stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                            Console.WriteLine("Server sent message - should be received by client");
                        }


                        Thread clientThread = new Thread(new ThreadStart(() => HandleClient(connectedTcpClient)));
                        connectedClients.Add(new KeyValuePair<TcpClient, Thread>(connectedTcpClient, clientThread));
                        clientThread.Start();
                    }
                }
                catch (SocketException socketException)
                {
                    Console.WriteLine("SocketException " + socketException.ToString());
                }
            }

            private void RemoveClient(TcpClient connectedClient)
            {
                KeyValuePair<TcpClient, Thread> client = connectedClients.FirstOrDefault(c => c.Key == connectedClient);
                connectedClients.Remove(client);

            }

            /// <summary>
            /// Handles the client.
            /// </summary>
            /// <param name="connectedTcpClient"></param>
            private void HandleClient(TcpClient connectedTcpClient)
            {
                IPEndPoint remoteIpEndPoint = connectedTcpClient.Client.RemoteEndPoint as IPEndPoint ?? new IPEndPoint(0, 0);
                Console.WriteLine($"Listening to client at {remoteIpEndPoint.Address}:{remoteIpEndPoint.Port}");
                try
                {
                    while (true)
                    {
                        Byte[] bytes = new Byte[1024];
                        using (NetworkStream stream = connectedTcpClient.GetStream())
                        {
                            int length;
                            // Read incomming stream into byte arrary. 						
                            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                var incommingData = new byte[length];
                                Array.Copy(bytes, 0, incommingData, 0, length);
                                // Convert byte array to string message. 							
                                string clientMessage = Encoding.ASCII.GetString(incommingData);

                                //if (remoteIpEndPoint != null)
                                Console.WriteLine($"{remoteIpEndPoint.Address}:{remoteIpEndPoint.Port} - \"{clientMessage}\"");
                                //else
                                //    Console.WriteLine($"Message received: \"{clientMessage}\"");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    Console.WriteLine($"Removing inactive client {remoteIpEndPoint.Address}:{remoteIpEndPoint.Port}");
                    RemoveClient(connectedTcpClient);
                }
            }

            /// <summary> 	
            /// Send message to connected clients using socket connection.
            /// </summary> 	
            private void SendMessage()
            {
                if (connectedClients.Count == 0)
                {
                    return;
                }

                foreach (var item in connectedClients)
                {
                    TcpClient connectedTcpClient = item.Key;
                    try
                    {
                        // Get a stream object for writing. 			
                        NetworkStream stream = connectedTcpClient.GetStream();
                        if (stream.CanWrite)
                        {
                            string serverMessage = "This is a message from server.";
                            // Convert string message to byte array.                 
                            byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                            // Write byte array to socketConnection stream.               
                            stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                            //Console.WriteLine("Server sent his message - should be received by client");
                        }
                    }
                    catch (SocketException socketException)
                    {
                        Console.WriteLine("Socket exception: " + socketException + socketException.HResult);
                        RemoveClient(connectedTcpClient);
                    }
                }
            }
        }
    
}
