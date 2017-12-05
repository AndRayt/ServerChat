using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat
{
    class Program
    {
        private const int PORT = 8000;

        static void Main(string[] args)
        {
            try
            {
                Server server = new Server(PORT);
                //запускаем сервер
                server.Listen();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
