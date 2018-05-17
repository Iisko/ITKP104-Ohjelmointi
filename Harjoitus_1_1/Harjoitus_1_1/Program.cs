using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
namespace ITKP104
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket soketti = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            soketti.Connect("localhost", 25000);
            String message = "GET / HTTP/1.1\r\nHost: localhost\r\nConnection: Close\r\n\r\n";
            byte[] rec = new byte[128];
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray(0, message.Length), 0, message.Length);

            soketti.Send(data);
            int paljon = soketti.Receive(rec);
            String vastaus = "";
            while (paljon > 0)
            {
                vastaus += System.Text.Encoding.ASCII.GetString(rec, 0, paljon);
                paljon = soketti.Receive(rec);
            }
            Console.WriteLine(vastaus);
        }
    }
}
