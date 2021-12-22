using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
namespace Client
{
    class Program {
        static string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;
        
        //mencatat pesan yang masuk
        static void Main(string[] args) 
        {
            Console.Write("Enter your name: ");
            userName = Console.ReadLine(); 
            client = new TcpClient(); 
            try {
                client.Connect(host, port); 
                stream = client.GetStream(); 

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message); 
                stream.Write(data, 0, data.Length); 

                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage)); 
                receiveThread.Start(); 
                Console.WriteLine("Welcome, " + userName);
                SendMessage(); 
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message); 
            }
        }
        
        //mengirim pesan ke server
        static void SendMessage() {
            Console.WriteLine("Type Your message: ");

            while (true) {
                string message = Console.ReadLine(); 
                byte[] data = Encoding.Unicode.GetBytes(message); 
                stream.Write(data, 0, data.Length); 
            }
        }

        //memeriksa apakah ada data yang masuk
        static void ReceiveMessage()
        {
            while (true) {
                try {
                    byte[] data = new byte[64]; 
                    StringBuilder builder = new StringBuilder(); 
                    int bytes = 0; 
                    
                    do {
                        bytes = stream.Read(data, 0, data.Length); 
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes)); 
                        break;
                    }
                    while (stream.DataAvailable); 
                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch (Exception e) {
                    Console.Write(e.Message);
                    break;
                }
            }
        }
    }
}