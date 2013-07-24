namespace xChatLib
{
    public enum PacketType : ushort
    {
        Message = 0,
        Command = 1,
        Register = 2,
        BinaryTransfer = 3
    }
    public enum DisconnectionReason
    {
        Manual,
        SocketException,
        RemoteHostDisconnected
    }
}
