using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S_ChatHandler( PacketSession session, IPacket packet )
    {
        var chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        //Console.WriteLine( chatPacket.chat );
    }
}
