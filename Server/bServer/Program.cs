using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Server
{
    class ClientObject
    {
        public string Id { get; private set; }
        public NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server;
        SaveData save = new SaveData();

        //membuat id baru disetiap koneksi baru
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        //memproses data dari client 
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                userName = message;

                message = userName + " entered the group";
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", userName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);

                        save.writeMessage(message);
                    }
                    catch
                    {
                        message = String.Format("{0}: left the chat", userName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        //Menyimpan data ke dalam file .txt
        class SaveData
        {
            private List<string> saveMessages = new List<string>();
            public void writeMessage(string message)
            {
                saveMessages.Add(message);
                File.WriteAllLines("D:/Pens/Semester 4/Arsitektur Jaringan & Komputer/FP MultiCLient/chats.txt", saveMessages);
            }
        }

        //menerima data dari user client
        private string GetMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;

            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        //close program
        public void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }

    class ServerObject
    {
        static TcpListener tcpListener;
        List<ClientObject> clients = new List<ClientObject>();

        //menyambungkan koneksi 
        public void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        //memutuskan koneksi client
        public void RemoveConnection(string id)
        {
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
                clients.Remove(client);
        }

        //memeriksa client yang terhubung ke server
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Server started");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //mengambil data dari pengirim dan menyebarkan ke penerima
        public void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);

            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].Stream.Write(data, 0, data.Length);
                }
            }
        }
    }

    //mencatat semua client yang masuk di fungsi main
    class Program
    {
        static ServerObject server;
        static Thread listenThread;
        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}