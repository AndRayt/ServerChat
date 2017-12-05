using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

        public void Start()
        {
            try
            {
                Stream = client.GetStream();
                String msg = "";
                //По соглашению первое сообщение отправленное 
                //на сервер - это имя пользователя
                userName = GetMessage();

                //Выводим список пользователей
                server.CastMsg(server.GetUserOnline());
                Thread.Sleep(100);

                //выводим предыдущие сообщения
                server.GetHistory(Id);
                
                msg = "connected";
                server.CastMsg(userName, msg, false);

                msg = String.Format("{0} {1} || ID: {2}",userName, msg, Id);
                Console.WriteLine(msg);

                //Просматриваем все остальные сообщения
                while (true)
                {
                    msg = GetMessage();
                    if (msg.Equals("")) continue;
                    server.CastMsg(userName, msg, true);
                    msg = String.Format("{0}: {1}", userName, msg);
                    Console.WriteLine(msg);
                }
            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            } finally
            {
                server.DeleteConnect(this);
                server.CastMsg(server.GetUserOnline());
                Close();
            }
        }

        public void Close()
        {
            if (Stream != null) Stream.Close();
            if (client != null) client.Close();
        }

        private string GetMessage()
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
