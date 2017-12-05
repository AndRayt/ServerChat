using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerChat
{
    class Server
    {
        TcpListener tcpListener;
        List<Client> clients = new List<Client>();

        //список всех сообщений
        StringBuilder msgList = new StringBuilder("");

        //База данных с сообщениями
        DBClass dbMsg = new DBClass();

        int PORT;

        public Server (int PORT)
        {
            this.PORT = PORT;
        }

        public void AddConnect(Client client)
        {
            clients.Add(client);
        }

        public void DeleteConnect (Client cl)
        {
            if (cl != null) clients.Remove(cl);
        }

        public void Listen()
        {
            tcpListener = new TcpListener(IPAddress.Any, PORT);
            tcpListener.Start();
            Console.WriteLine("--- Server is started. Wait connections ---");

            Thread listenThread = new Thread(new ThreadStart(ListenProcess));
            listenThread.Start();
            listenThread.IsBackground = true;

            Console.WriteLine("___ Press any button to stop the server ___");
            Console.ReadLine();
            //завершение работы
            tcpListener.Stop();
            foreach (Client cl in clients)
            {
                cl.Close();
            }
            Environment.Exit(0);
        }

        public void CastMsg(string name, string msg, bool addToDB)
        {
            //добавляем сообщение в список
            //msgList.Append(msg);
            //msgList.Append("\n");
            if (addToDB) dbMsg.AddMessage(name, msg);
            else msg += "#";
            //делаем широковещательную рассылку
            byte[] data = Encoding.Unicode.GetBytes(String.Format("{0}: {1}", name, msg));
            foreach (Client cl in clients)
            {
                cl.Stream.Write(data, 0, data.Length);
            }
        }

        public void CastMsg(string msg)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            foreach (Client cl in clients)
            {
                cl.Stream.Write(data, 0, data.Length);
            }
        }

        public void GetHistory(string id)
        {
            /*byte[] data = Encoding.Unicode.GetBytes(msgList.ToString());
            foreach (Client cl in clients)
            {
                if (cl.Id.Equals(id)) cl.Stream.Write(data, 0, data.Length);
            }*/
            byte[] data = Encoding.Unicode.GetBytes(dbMsg.GetAllMessage());
            foreach (Client cl in clients)
            {
                if (cl.Id.Equals(id)) cl.Stream.Write(data, 0, data.Length);
            }
        }

        public string GetUserOnline()
        {
            StringBuilder str = new StringBuilder();
            str.Append("#");
            foreach (Client cl in clients)
            {
                str.Append(cl.userName);
                str.Append("\n");
            }
            return str.ToString();
        }

        private void ListenProcess()
        {
            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                Client client = new Client(tcpClient, this);
                clients.Add(client);
                Thread clientThread = new Thread(new ThreadStart(client.Start));
                clientThread.Start();
            }
        }
    }
}
