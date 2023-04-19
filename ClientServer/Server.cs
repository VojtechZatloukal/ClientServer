﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace ClientServer
{
    public  class Server
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
        /// Create handle to connected tcp client. 	
        /// </summary> 	
        private TcpClient connectedTcpClient;
        #endregion

        // Use this for initialization
        public void Start()
        {
            // Start TcpServer background thread 		
            tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
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
        private void ListenForIncommingRequests()
        {
            try
            {
                // Create listener on localhost port 8052. 			
                tcpListener = new TcpListener(IPAddress.Parse("172.30.0.1"), 8052);
                tcpListener.Start();
                Console.WriteLine("Server is listening");
                Byte[] bytes = new Byte[1024];
                while (true)
                {
              
                    
                    using (connectedTcpClient = tcpListener.AcceptTcpClient())
                    {
                       
                        // Get a stream object for reading 					
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
                                Console.WriteLine("client message received as: " + clientMessage);
                            }
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("SocketException " + socketException.ToString());
            }
        }
        /// <summary> 	
        /// Send message to client using socket connection. 	
        /// </summary> 	
        private void SendMessage()
        {
            if (connectedTcpClient == null)
            {
                return;
            }

            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = connectedTcpClient.GetStream();
                if (stream.CanWrite)
                {
                    string serverMessage = "This is a message from your server.";
                    // Convert string message to byte array.                 
                    byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(serverMessage);
                    // Write byte array to socketConnection stream.               
                    stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                    Debug.WriteLine("Server sent his message - should be received by client");
                }
            }
            catch (SocketException socketException)
            {
                Debug.WriteLine("Socket exception: " + socketException);
            }
        }

    }
}
