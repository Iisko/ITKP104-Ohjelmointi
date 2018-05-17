using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Harjoitus_1_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 25000);

            palvelin.Bind(iep);
            palvelin.Listen(5);

            
            Socket asiakas = palvelin.Accept();

            IPEndPoint iap = (IPEndPoint)asiakas.RemoteEndPoint;
            Console.WriteLine("Yhteys osoitteesta: {0} portista {1}", iep.Address, iep.Port);
            NetworkStream ns = new NetworkStream(asiakas);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            String message = sr.ReadLine();

            sw.WriteLine("Iisakin palvelin;" + message);

            asiakas.Close();
           
            Console.ReadKey();
            palvelin.Close();

        }
    }
}
