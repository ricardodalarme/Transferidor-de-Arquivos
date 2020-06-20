using Lidgren.Network;

namespace Server.Network
{
    static class Send
    {
        public static void Files()
        {
            // Empacota os dados do arquivo
            NetOutgoingMessage data = Socket.Device.CreateMessage();
            data.Write((byte)ServerPackets.Files);
            data.Write((byte)Program.Files.Count);
            foreach (string file in Program.Files.Keys) data.Write(file);

            // Envia o pacote para todos os clientes conectados
            Socket.Device.SendToAll(data, NetDeliveryMethod.ReliableOrdered);
        }

        public static void FileOwner(NetConnection client, string fileName)
        {

            // Empacota o dado de quem possui o arquivo
            NetOutgoingMessage data = Socket.Device.CreateMessage();
            data.Write((byte)ServerPackets.FileOwner);
            data.Write(Program.Files[fileName]);
            data.Write(fileName);

            // Envia o pacote para o cliente
            Socket.Device.SendMessage(data, client, NetDeliveryMethod.ReliableOrdered);
        }
    }
}