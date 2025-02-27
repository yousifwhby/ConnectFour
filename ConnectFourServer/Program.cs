﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;   
using System.Net.Sockets; 

namespace ConnectFourServer
{
    class Program
    {
        static TcpListener server;
        static public List<Room> rooms = new List<Room>(); 

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ip_ = null;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ip_ = ip;
                }
            }
            return ip_;
        }

        static void Main(string[] args)
        {
            //have a tcpServer that sendes available rooms on connection
            //then wait for a flag response of either new room , join room , spectate 
            //followed by the op details

            Byte[] bt = new byte[] { 172, 16, 13, 113 };
            IPAddress localHost = new IPAddress(bt);
            IPAddress DynamicLocalHost = Program.GetLocalIPAddress();
            server = new TcpListener(DynamicLocalHost, 3000);
            Console.WriteLine(DynamicLocalHost.ToString());
            server.Start(); 

            Task serverTask = Task.Factory.StartNew( () =>{ 

                while (true)
                {
                    Console.WriteLine("waiting for a connection");
                    Socket socketConnection = server.AcceptSocket();
                    Console.WriteLine("connecting ..");

                    Task.Factory.StartNew( (socketConnection_) =>{
                        
                        User nUser = new User((Socket)socketConnection_);
                        nUser.networkController();

                    },socketConnection );
                    
                } 

            } );
            Task.WaitAll(serverTask);
            

        }
    }
}
