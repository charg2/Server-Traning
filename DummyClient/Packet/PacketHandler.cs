using DummyClient;
using ServerCore;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler( PacketSession session, IPacket packet )
    {
        var chatPacket    = packet as S_BroadcastEnterGame;
        var serverSession = session as ServerSession;
    }

    public static void S_BroadcastLeaveGameHandler( PacketSession session, IPacket packet )
    {
        var chatPacket    = packet as S_PlayerList;
        var serverSession = session as ServerSession;
    }

    public static void S_PlayerListHandler( PacketSession session, IPacket packet )
    {
        var chatPacket    = packet as S_PlayerList;
        var serverSession = session as ServerSession;
    }

    public static void S_BroadcastMoveHandler( PacketSession session, IPacket packet )
    {
        var chatPacket    = packet as S_BroadcastMove;
        var serverSession = session as ServerSession;
    }
}
