using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace Harjoitus_4_8
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999);
            EndPoint ep = (EndPoint)iep;
            EndPoint Pep = (IPEndPoint)ep;

            palvelin.SendTo(Encoding.ASCII.GetBytes("JOIN Iisakin asiakas"), Pep);
            bool on = true;
            String TILA = "JOIN";
            byte[] data = new byte[256];
            while(on)
            {
                palvelin.Receive(data);
                String vastaus = Encoding.ASCII.GetString(data);
                String[] palat = vastaus.Split(' ');
                switch(TILA)
                {
                    case "JOIN":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    case "201":
                                        Console.WriteLine("Odotetaan toista pelaajaa...");
                                        break;
                                    case "202":
                                        Console.WriteLine("Vastustajasi on {0}.", palat[2]);
                                        Console.WriteLine("Anna numero");
                                        String luku = Console.ReadLine();
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("DATA " + luku), Pep);
                                        TILA = "GAME";
                                        break;
                                    case "203":
                                        Console.WriteLine("Vastustaja {0} saa aloittaa.", palat[2]);
                                        TILA = "GAME";
                                        break;
                                    default:
                                        //Console.WriteLine("Virhe " + palat[1]);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe " + palat[0]);
                                break;
                        }
                        break;
                    case "GAME":
                        switch (palat[0])
                        {
                            case "ACK":
                                switch (palat[1])
                                {
                                    case "300":
                                        Console.WriteLine("OK, odotetaan vastustajaa...");
                                        break;
                                }
                                break;
                            case "DATA":
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 300"), Pep);
                                Console.WriteLine("Anna numero");
                                String luku = Console.ReadLine();
                                palvelin.SendTo(Encoding.ASCII.GetBytes("DATA " + luku), Pep);
                                break;
                            default:
                                Console.WriteLine("Virhe " + palat[0] + " " + palat[1]);
                                break;
                            case "QUIT":
                                switch (palat[1])
                                {
                                    case "501":
                                        Console.WriteLine("Voitit pelin!");
                                        break;
                                    case "502":
                                        Console.WriteLine("Vastustajasi voitti pelin!");
                                        break;
                                    default:
                                        Console.WriteLine("Virhe " + palat[0] + " " + palat[1]);
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
