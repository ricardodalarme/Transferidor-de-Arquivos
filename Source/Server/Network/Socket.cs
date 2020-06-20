using Lidgren.Network;
using System;
using System.Collections.Generic;

namespace Server.Network
{
    static class Socket
    {
        // Dispositivo de controle da transmissão de dados
        public static NetServer Device;

        // Porta para comunicação entre cliente-servidor
        public const int Port = 7000;

        public static void Init()
        {
            // Define algumas configurações da rede
            var configuration = new NetPeerConfiguration("dalaTorrent");
            configuration.Port = Port;

            // Cria o dispositivo com as devidas configurações
            Device = new NetServer(configuration);
            Device.Start();
        }

        public static void HandleData()
        {
            NetIncomingMessage data;

            // Lê e direciona todos os dados recebidos
            while ((data = Device.ReadMessage()) != null)
            {
                switch (data.MessageType)
                {
                    // Alteração de estado da conexão
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)data.ReadByte())
                        {
                            // Nova conexão - Conecta o jogador ao jogo
                            case NetConnectionStatus.Connected:
                                Console.WriteLine($"Nova conexão recebida de {data.SenderConnection.RemoteEndPoint}");
                                break;
                            // Conexão perdida, disconecta o jogador do jogo
                            case NetConnectionStatus.Disconnected:
                                DisconectClient(data.SenderConnection);
                                break;
                        }
                        break;
                    // Manuseia os dados recebidos
                    case NetIncomingMessageType.Data:
                        Receive.Handle(data.SenderConnection, data);
                        break;
                }

                Device.Recycle(data);
            }
        }

        private static void DisconectClient(NetConnection client)
        {
            // Lista todos os arquivos que estão relacionados ao cliente
            var toRemove = new List<string>();
            foreach (var map in Program.Files)
                if (map.Value == client.RemoteEndPoint)
                    toRemove.Add(map.Key);

            // Remove os arquivos listados do mapa
            foreach (var key in toRemove) Program.Files.Remove(key);

            // Atualiza a lista de arquivos de todo mundo conectado
            Send.Files();

            // Demonstra quem saiu
            Console.WriteLine($"{client.RemoteEndPoint} foi desconectado.");
        }
    }
}