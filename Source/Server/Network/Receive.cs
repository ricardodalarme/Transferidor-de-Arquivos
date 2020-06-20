using Lidgren.Network;
using System;

namespace Server.Network
{
    static class Receive
    {
        public static void Handle(NetConnection client, NetIncomingMessage data)
        {
            // Manuseia os dados recebidos
            switch ((ClientPackets)data.ReadByte())
            {
                case ClientPackets.Join: Join(client, data); break;
                case ClientPackets.RequestOwner: DownloadFile(client, data); break;
            }
        }

        private static void Join(NetConnection client, NetIncomingMessage data)
        {
            // Recebe todos os arquivos do cliente
            Console.WriteLine($"Recebendo arquivos de {client.RemoteEndPoint}");
            byte count = data.ReadByte();
            for (byte i = 0; i < count; i++)
            {
                string fileName = data.ReadString();
                Console.WriteLine($"    {fileName}");

                // Adiciona no mapa caso o arquivo ainda não esteja mapeado, caso já estiver atualiza quem tem
                if (!Program.Files.ContainsKey(fileName))
                    Program.Files.Add(fileName, client.RemoteEndPoint);
            }

            // Envia ao cliente todos os arquivos que estão disponiveis para serem baixados e atualiza a lista dos demais conectados
            Send.Files();
        }

        private static void DownloadFile(NetConnection client, NetIncomingMessage data)
        {
            // Retorna ao cliente quem tem o pacote que ele está procurando
            string fileName = data.ReadString();
            Console.WriteLine($"{client.RemoteEndPoint} está requisitando o arquivo {fileName}");
            Send.FileOwner(client, fileName);
        }
    }
}