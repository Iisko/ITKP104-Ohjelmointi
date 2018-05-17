using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Harjoitus_3_6
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = null;

            int port = 9999;
            Console.WriteLine("Käytössä oleva portti: {0}", port);
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, port);

            List<EndPoint> asiakkaat = new List<EndPoint>();

            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Bind(iep);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Virhe: " + ex.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Odotetaan asiakasta...");

            while(!Console.KeyAvailable)
            {
                s.ReceiveFrom();
            }

            Console.ReadKey();
            s.Close();

        }
    }
}
