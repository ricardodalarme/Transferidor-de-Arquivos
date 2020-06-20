using Lidgren.Network;
using System;
using System.Windows.Forms;

namespace Client.Network
{
    static class Socket
    {
        // Dispositivo de controle da transmissão de dados
        public static NetClient Device;

        // Dados para a conexão com o servidor
        public const string IP = "localhost";
        public const short Port = 7000;

        public static void Init()
        {
            // Configura o dispositivo de rede
            var configuration = new NetPeerConfiguration("dalaTorrent");
            configuration.MaximumTransmissionUnit = 8191;
            configuration.EnableMessageType(NetIncomingMessageType.UnconnectedData);

            // Cria o dispositivo com as devidas configurações
            Device = new NetClient(configuration);
            Device.Start();
        }

        public static void HandleData()
        {
            NetIncomingMessage data;

            // Lê e direciona todos os dados recebidos enquanto o aplicativo estiver aberto
            while ((data = Device.ReadMessage()) != null)
            {
                switch (data.MessageType)
                {
                    // Manuseia as mensagens vindas de algum peer
                    case NetIncomingMessageType.UnconnectedData:
                        Receive.PeerData(data.SenderEndPoint, data);
                        break;
                    // Manuseia as mensagens vindas do servidor
                    case NetIncomingMessageType.Data:
                        Receive.ServeData(data);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        // Desconecta o jogador caso o servidor seja desligado
                        if (data.ReadByte() == (byte)NetConnectionStatus.Disconnected)
                        {
                            MessageBox.Show("O servidor não está mais disponível.");
                            Program.Exit();
                        }
                        break;
                }

                Device.Recycle(data);
            }
        }

        // Retorna um valor de acordo com o estado da conexão do jogador
        public static bool IsConnected => Device.ConnectionStatus == NetConnectionStatus.Connected;

        public static bool TryConnect()
        {
            // Tenta se conectar com o servidor
            Device.Connect(IP, Port);

            // Espera um segundo para verificar se o jogador conseguiu se conectar
            int waitTimer = Environment.TickCount;
            while (!IsConnected && Environment.TickCount < waitTimer + 1000) Device.ReadMessage();

            // Retorna uma mensagem caso não conseguir se conectar
            return IsConnected;
        }
    }
}
