using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
namespace Harjoitus_2_4_5 // Tässä on siis molemmat tehtävät yhdessä
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                s.Connect("localhost", 25000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Virhe: " + ex.Message);
                Console.ReadKey();
                return;
            }

            NetworkStream ns = new NetworkStream(s);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            String email = "Terve, SMTP maailma!";

            String message = "";
            bool online = true;

            while(online)
            {
                message = sr.ReadLine();
                String[] status = message.Split(' ');
                Console.WriteLine(message);
                switch (status[0])
                {
                    case "220":
                        sw.WriteLine("HELO jyu.fi");
                        break;
                    case "250":
                        switch (status[1])
                        {
                            case "2.0.0":
                                sw.WriteLine("QUIT");
                                online = false;
                                break;
                            case "2.1.0":
                                sw.WriteLine("RCPT TO: local");
                                break;
                            case "2.1.5":
                                sw.WriteLine("DATA");
                                break;
                            default:
                                sw.WriteLine("MAIL FROM: Iisakki");
                                break;
                        }   // switch
                        break;
                    case "354":
                        sw.WriteLine(email+"\r\n.\r\n");
                        break;

                    case "221":
                        online = false;
                        break;
    
                    default:
                        Console.WriteLine("Virhe!");
                        sw.WriteLine("QUIT");
                        break;
                }   // switch

                sw.Flush();
            }   // while

            Console.ReadKey();
            sw.Close();
            sr.Close();
            ns.Close();
            s.Close();
        }
    }
}
