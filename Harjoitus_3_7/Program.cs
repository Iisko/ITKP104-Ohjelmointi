using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;
namespace Harjoitus_3_7
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            int port = 9999;
            Console.WriteLine("Käytössä oleva portti: {0}", port);

            IPEndPoint iep = new IPEndPoint(IPAddress.Loopback, port);
            byte[] rec = new byte[256];

            EndPoint ep = (EndPoint)iep;
            s.ReceiveTimeout = 1000;
            String msg;
            bool on = true;
            do
            {
                Console.Write("> ");
                msg = Console.ReadLine();
                if (msg.Equals("QUIT"))
                {
                    on = false;
                }
                else
                {
                    s.SendTo(Encoding.ASCII.GetBytes(msg), ep);

                    while(!Console.KeyAvailable)
                    {
                        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint palvelinep = (EndPoint)remote;
                        int paljon = 0;

                        try
                        {
                            int received = s.ReceiveFrom(rec, ref palvelinep);
                            String rec_string = Encoding.ASCII.GetString(rec, 0, received);
                            char[] delim = { ';' };
                            String[] palat = rec_string.Split(delim, 2);
                            if (palat.Length < 2)
                            {

                            }
                            else
                            {
                                Console.WriteLine("{0}: {1}", palat[0], palat[1]);
                            }
                        }
                        catch
                        {
                            // timeout
                            Console.WriteLine("Connection timeout!");
                        }

                    }
                }
            } while (on);
            Console.ReadKey();
            s.Close();
        }
    }
}
