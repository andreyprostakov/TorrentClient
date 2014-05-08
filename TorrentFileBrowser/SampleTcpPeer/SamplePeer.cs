using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SampleTcpPeer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world!");
            IPAddress ip = IPAddress.Parse("127.21.5.99");
            int port = 45746;
            TcpListener listener = new TcpListener(ip, port);
            Console.WriteLine("{0}:{1}", ip, port);
            listener.Start();
            Socket socket = listener.AcceptSocket();
            Console.WriteLine("Socket created");
            byte[] buffer = new byte[256];
            int result = socket.Receive(buffer);
            Console.WriteLine("Received {0}: {1}", result, Encoding.ASCII.GetString(buffer, 0, result));
            String response = "OKA!";
            socket.Send(Encoding.ASCII.GetBytes(response));
            socket.Close();
            listener.Stop();
            Console.ReadKey();
            return;
        }
    }
}
