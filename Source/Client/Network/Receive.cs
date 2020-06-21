using Lidgren.Network;
using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace Client.Network
{
    static class Receive
    {
        public static void ServeData(NetIncomingMessage data)
        {
            // Manuseia os dados recebidos
            switch ((ServerPackets)data.ReadByte())
            {
                case ServerPackets.Files: Files(data); break;
                case ServerPackets.FileOwner: FileOwner(data); break;
            }
        }

        public static void PeerData(IPEndPoint peer, NetIncomingMessage data)
        {
            // Manuseia os dados recebidos
            switch ((PeerPackets)data.ReadByte())
            {
                case PeerPackets.Alert: Alert(data); break;
                case PeerPackets.DownloadFile: DownloadFile(peer, data); break;
                case PeerPackets.FileData: FileData(peer, data); break;
                case PeerPackets.File: File(data); break;
            }
        }

        private static void Alert(NetIncomingMessage data)
        {
            // Demonstra uma mensagem na tela
            MessageBox.Show(data.ReadString());
        }

        private static void Files(NetIncomingMessage data)
        {
            // Adiciona todos os arquivos na lista
            byte count = data.ReadByte();
            Window.Form.lstFiles.Items.Clear();
            for (byte i = 0; i < count; i++)
            {
                string fileName = data.ReadString();
                Window.Form.lstFiles.Items.Add(fileName);
            }
        }

        private static void FileOwner(NetIncomingMessage data)
        {
            // Mostra a mensagem
            IPEndPoint owner = data.ReadIPEndPoint();
            Send.DownloadFile(owner, data.ReadString());
        }

        private static void DownloadFile(IPEndPoint peer, NetIncomingMessage data)
        {
            string fileName = data.ReadString();

            // Cria uma thread nova para enviar os dados do arquivo
            object parameters = new Tuple<IPEndPoint, string>(peer, fileName);
            var fileThread = new Thread(new ParameterizedThreadStart(Send.File));
            fileThread.Start(parameters);
        }

        private static void FileData(IPEndPoint peer, NetIncomingMessage data)
        {
            // Adiciona o arquivo no mapa
            string fileName = data.ReadString();
            long fileSize = data.ReadInt64();
            var fileData = new FileData(fileName, fileSize, peer);
            Program.Files.Add(fileName, fileData);
        }

        private static void File(NetIncomingMessage data)
        {
            // Monta o arquivo com os dados recebidos
            string fileName = data.ReadString();
            int partNumber = data.ReadInt32();
            int bufferSize = data.ReadInt32();
            byte[] buffer = data.ReadBytes(bufferSize);
            Program.Files[fileName].Make(partNumber, buffer);
        }
    }
}