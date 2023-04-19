// See https://aka.ms/new-console-template for more information
using ServerClientSocket;

Console.WriteLine("Hello, World!");


Server server = new Server("127.0.0.1", 8000);

server.Start();

Client client = new Client();

while (true)
{
    client.SendMSG();
    Thread.Sleep(5000);
}
