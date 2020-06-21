namespace Client.Network
{
    // Pacotes do cliente-servidor
    enum ClientPackets
    {
        Join,
        RequestOwner
    }

    // Pacotes cliente-cliente
    enum PeerPackets
    {
        Alert,
        DownloadFile,
        FileData,
        File
    }

    // Pacotes do servidor-cliente
    enum ServerPackets
    {
        Files,
        FileOwner
    }
}