using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Harjoitus_5_10_12
{
    class Program
    {
        //enum Field { Viesti, Data };
        static char erotin = ' ';

        static void Main(string[] args)
        {
            Socket palvelin;
            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, 9999);
            try
            {
                palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                palvelin.Bind(iep);
            }
            catch
            {
                return;
            }

            String STATE = "WAIT";

            bool on = true;
            int vuoro = -1;
            int Pelaajat = 0;
            int Quit_ACK = 0;
            int luku = -1;
            EndPoint[] Pelaaja = new EndPoint[2];
            String[] Nimi = new String[2];
            while(on)
            {
                IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)client;
                byte[] data = new byte[256];
                int received = palvelin.ReceiveFrom(data, ref remote);
                String vastaus = Encoding.ASCII.GetString(data,0,received);
                String[] kehys = vastaus.Split(' ');

                switch (STATE)
                {
                    case "WAIT":
                        switch (kehys[0])
                        {
                            case "JOIN":
                                Pelaaja[Pelaajat] = remote;
                                Nimi[Pelaajat] = kehys[1];
                                Pelaajat++;
                                if(Pelaajat == 1)
                                {
                                    palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 201 JOIN OK"), remote);
                                    Console.WriteLine("Uusi pelaaja {0}:{1} {2}", ((IPEndPoint)remote).Address, ((IPEndPoint)remote).Port, kehys[1]);
                                }
                                else if(Pelaajat == 2)
                                {
                                    Random rand = new Random();
                                    int Aloittaja = rand.Next(0,1);
                                    vuoro = Aloittaja;
                                    luku = rand.Next(0, 9);
                                    Console.WriteLine(luku);
                                    palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 202 " + Nimi[Flip(Aloittaja)]), Pelaaja[Aloittaja]);
                                    palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 203 " + Nimi[Aloittaja]), Pelaaja[Flip(Aloittaja)]);
                                    Console.WriteLine("Uusi pelaaja {0}:{1} {2}", ((IPEndPoint)remote).Address, ((IPEndPoint)remote).Port, kehys[1]);
                                    STATE = "GAME";
                                }
                                else
                                {
                                    palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 401 Ei mahdu!"), remote);
                                    //Virheet?
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe " + kehys[0]);
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), remote);
                                
                                break;
                        } // switch (kehys[0])
                        break;

                    case "GAME":
                        switch (kehys[0])
                        {
                            case "JOIN":
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 401 Ei mahdu!"), remote);
                                break;
                            case "DATA":
                                if (((IPEndPoint)remote).Equals(((IPEndPoint)Pelaaja[vuoro])))
                                {
                                    int numero;
                                    int.TryParse(kehys[1], out numero);
                                    if (numero == luku)
                                    {
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 501 QUIT"), Pelaaja[vuoro]);
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 502 QUIT"), Pelaaja[Flip(vuoro)]);
                                        STATE = "END";
                                    }
                                    else
                                    {
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 300"), Pelaaja[vuoro]);
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 300"), Pelaaja[Flip(vuoro)]);
                                        vuoro = Flip(vuoro);
                                        STATE = "WAIT_ACK";
                                    }
                                }
                                else
                                {
                                    palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 403"), Pelaaja[Flip(vuoro)]);
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe " + kehys[0]);
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), Pelaaja[vuoro]);
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), Pelaaja[Flip(vuoro)]);
                                break;
                        }
                        break;

                    case "WAIT_ACK":
                        switch (kehys[0])
                        {
                            case "ACK":
                                switch (kehys[1])
                                {
                                    case "300":
                                        if (((IPEndPoint)remote).Equals(((IPEndPoint)Pelaaja[vuoro])))
                                        {
                                            STATE = "GAME";
                                        }
                                        else
                                        {
                                            palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), Pelaaja[Flip(vuoro)]);
                                        }
                                        break;
                                    default:
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), Pelaaja[vuoro]);
                                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 404"), Pelaaja[Flip(vuoro)]);
                                        Console.WriteLine("Virhe " + kehys[1]);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe " + kehys[0]);
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 403"), Pelaaja[vuoro]);
                                palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 403"), Pelaaja[Flip(vuoro)]);
                                break;
                        }
                        break;

                    case "END":

                        if (Quit_ACK >= Pelaajat)
                        {
                            on = false;
                        }
                        switch (kehys[0])
                        {
                            case "QUIT":
                                Quit_ACK++;
                                break;
                        }
                        break;

                    default:
                        Console.WriteLine("Errors...");
                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 400"), Pelaaja[vuoro]);
                        palvelin.SendTo(Encoding.ASCII.GetBytes("ACK 400"), Pelaaja[Flip(vuoro)]);
                        break;
                }

            }
        }
        static int Flip(int n)
        {
            return (n == 1 ? 0 : 1);
        }
    }
}
