using Lidgren.Network;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Client.Network
{
    static class Send
    {
        private static void ToServer(NetOutgoingMessage data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendMessage(data, NetDeliveryMethod.ReliableOrdered);
        }

        private static void ToPeer(IPEndPoint peer, NetOutgoingMessage data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendUnconnectedMessage(data, peer);
        }

        public static void Join()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Obtém todos os arquivos que existem no diretório que o programa está sendo executado
            FileInfo[] files = new DirectoryInfo(Application.StartupPath).GetFiles();

            // Empacota e envia os dados
            data.Write((byte)ClientPackets.Join);
            data.Write((byte)files.Length);
            foreach (FileInfo file in files) data.Write(file.Name);
            ToServer(data);
        }

        public static void RequestOwner(string fileName)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Empacota e envia os dados
            Data.Write((byte)ClientPackets.RequestOwner);
            Data.Write(fileName);
            ToServer(Data);
        }

        public static void DownloadFile(IPEndPoint peer, string fileName)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Empacota e envia os dados
            Data.Write((byte)PeerPackets.DownloadFile);
            Data.Write(fileName);
            ToPeer(peer, Data);
        }

        public static void FileData(IPEndPoint peer, string fileName, long fileSize)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Empacota e envia os dados
            Data.Write((byte)PeerPackets.FileData);
            Data.Write(fileName);
            Data.Write(fileSize);
            ToPeer(peer, Data);
        }

        public static void File(object parameters)
        {
            var data = (Tuple<IPEndPoint, string>)parameters;
            var peer = data.Item1;
            var fileName = data.Item2;

            // Tenta abrir um fluxo para a leitura do arquivo
            try
            {
                using (var file = new FileStream($"{Application.StartupPath}\\{fileName}", FileMode.Open))
                {
                    // Envia os dados do arquqivo para quem está baixando
                    FileData(peer, fileName, file.Length);

                    // Envia o arquivo fragmento em partes
                    int sendSize = 8000;
                    int currentPart = 0;
                    while (file.Position < file.Length)
                    {
                        // Evita que o último pacote tenha tamanho maior que o necessário
                        if (file.Position > file.Length - sendSize) sendSize = (int)(file.Length - file.Position);

                        // Envia um pedaço do arquivo
                        byte[] buffer = new byte[sendSize];
                        file.Read(buffer, 0, sendSize);
                        File(peer, fileName, currentPart++, sendSize, buffer);

                        // Evita que a thread consuma todos os recursos
                        Thread.Sleep(1);
                    }
                }
            }
            // Caso o arquivo já esteja aberto vai gerar um erro
            catch
            {
                Alert(peer, "O proprietário não pode enviar o arquivo agora pois está com ele aberto.");
            }
        }

        private static void File(IPEndPoint peer, string fileName, int currentPart, int bufferSize, byte[] buffer)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Empacota e envia os dados
            Data.Write((byte)PeerPackets.File);
            Data.Write(fileName);
            Data.Write(currentPart);
            Data.Write(bufferSize);
            Data.Write(buffer);
            ToPeer(peer, Data);
        }


        public static void Alert(IPEndPoint peer, string text)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Empacota e envia o alerta
            Data.Write((byte)PeerPackets.Alert);
            Data.Write(text);
            ToPeer(peer, Data);
        }
    }
}