using Server_BD.BDConnection;
using Server_BD.Model;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;



//Server
const int PORT = 4444;
IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(endPoint);
socket.Listen();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Server start...");


var clientSocket = await socket.AcceptAsync();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"Client connect... {clientSocket.RemoteEndPoint}");

while (true)
{
    //Receive all byte from clientSocket
    byte[] firstData = new byte[256];
    int bytes = clientSocket.Receive(firstData);
    int equal = bytes + clientSocket.Available;
    byte[] buffer = new byte[equal];
    if (clientSocket.Available > 0)
    {
        byte[] secondData = new byte[clientSocket.Available];
        clientSocket.Receive(secondData);
        buffer = firstData.Concat(secondData).ToArray();
    }
    else
        buffer = firstData;

    //Conver received client data from byte[] to class MyNote and get command
    string strAns = Encoding.Unicode.GetString(buffer, 0, equal);
    //List<MyNote> MyNotes = new List<MyNote>();
    if (strAns.StartsWith("#READ#"))
    {
        using (BDContext context = new BDContext())
        {
            List<MyNote> MyNotes = new List<MyNote>(context.Notes.ToList());
            //MyNotes = new List<MyNote>(MyNotes.Reverse());
            string json = JsonSerializer.Serialize(MyNotes);
            byte[] bjson = Encoding.Unicode.GetBytes(json);
            clientSocket.Send(bjson);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{MyNotes.Count} Notes read from SQL-Base");
        }
    }
    else if (strAns.StartsWith("#WRITE#"))
    {
        strAns = strAns.Substring(7);
        using (BDContext context = new BDContext())
        {
            context.Add(JsonSerializer.Deserialize<MyNote>(strAns));
            if (context.SaveChanges() > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Note writted to SQL-Base");
            }
        }
    }
}
//
clientSocket.Close();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Client disconnect...");
socket.Close();
Console.WriteLine("Server end...");
Console.ReadLine();

