using Server.Network;
using System;
using System.Collections.Generic;
using System.Net;

namespace Server
{
    class Program
    {
        // Mapa para identificar quais usuários tem determinados arquivos
        public static Dictionary<string, IPEndPoint> Files = new Dictionary<string, IPEndPoint>();

        static void Main()
        {
            // Demonstra a logo
            Console.Title = "Server";
            Console.WriteLine(@"
  ____                          _       _    
 / ___|    ___   _ __  __   __ (_)   __| |   ___    _ __ 
 \___ \   / _ \ | '__| \ \ / / | |  / _` |  / _ \  | '__|
  ___) | |  __/ | |     \ V /  | | | (_| | | (_) | | |   
 |____/   \___| |_|      \_/   |_|  \__,_|  \___/  |_|   ");

            // Inicia o dispositivo de rede
            Socket.Init();
            Console.WriteLine($"\n                                              Porta: {Socket.Port}");
            Console.WriteLine("\n[Log]");

            // Manuseia todos os dados recebidos
            while (true) Socket.HandleData();
        }
    }
}