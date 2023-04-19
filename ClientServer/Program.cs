

// See https://aka.ms/new-console-template for more information
using ClientServer;

Console.WriteLine("Hello, World!");

Server server = new Server();
Client client = new Client();

server.Start();
client.Start();

while (true)
{
    server.Update();
    client.Update();
}