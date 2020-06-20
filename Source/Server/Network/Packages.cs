namespace Server.Network
{
    // Pacotes do cliente-servidor
    enum ClientPackets
    {
        Join,
        RequestOwner
    }

    // Pacotes do servidor-cliente
    enum ServerPackets
    {
        Files,
        FileOwner
    }
}