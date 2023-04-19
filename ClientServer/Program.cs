

using ClientServer;

Console.WriteLine("Hello, World!");


Client client = new Client();


client.Start();

while (true)
{

    client.Update();
    Thread.Sleep(1000);
}