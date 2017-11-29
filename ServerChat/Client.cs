using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerChat
{
    //Класс в котором хранятся данные о клиенте
    class Client
    {
        public string Id
        {
            get;
            private set;
        }
        public NetworkStream Stream
        {
            get;
            private set;
        }
        public string userName;

        TcpClient client;
        Server server;

        public Client(TcpClient client, Server server)
        {
            Id = Guid.NewGuid().ToString();
            this.client = client;
            this.server = server;
        }

        public void start()
        {
            try
            {
                Stream = client.GetStream();
                String msg = "";
                //По соглашению первое сообщение отправленное 
                //на сервер - это имя пользователя
                userName = getMessage();

                msg = userName + " connected";
                server.castMsg(msg, Id);

                msg = msg + " ID: " + Id;
                Console.WriteLine(msg);

                //Просматриваем все остальные сообщения
                while (true)
                {
                    msg = getMessage();
                    if (msg.Equals("")) continue;
                    msg = userName + ": " + msg;
                    Console.WriteLine(msg);
                    server.castMsg(msg, Id);
                }
            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            } finally
            {
                server.deleteConnect(this);
                close();
            }
        }

        public void close()
        {
            if (Stream != null) Stream.Close();
            if (client != null) client.Close();
        }

        private string getMessage()
        {
            byte[] msgBytes = new byte[128];
            StringBuilder msg = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = Stream.Read(msgBytes, 0, msgBytes.Length);
                msg.Append(Encoding.Unicode.GetString(msgBytes, 0, bytes));
            } while (Stream.DataAvailable);

            return msg.ToString();
        }
    }
}
