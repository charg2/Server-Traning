using System.Diagnostics;
using DummyClient;
using ServerCore;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

class PacketHandler
{
    // 이미 입장을 한 상태에서 다른 플레이어가 입장을 한다면 여기서 추가를 하면 된다
    public static void S_BroadcastEnterGameHandler( PacketSession session, IPacket packet )
    {
        var pkt           = packet as S_BroadcastEnterGame;
        //var serverSession = session as ServerSession;

        PlayerManager.Instance.EnterGame( pkt );
    }
    // 누군가 나갔을 때
    public static void S_BroadcastLeaveGameHandler( PacketSession session, IPacket packet )
    {
        var pkt           = packet as S_BroadcastLeaveGame;
        //var serverSession = session as ServerSession;

        PlayerManager.Instance.LeaveGame( pkt );

    }
    // GameRoom에 접속했을 때 접속한 플레이어 리스트를 알려준다.
    public static void S_PlayerListHandler( PacketSession session, IPacket packet )
    {
        var pkt           = packet as S_PlayerList;
        //var serverSession = session as ServerSession;

        PlayerManager.Instance.Add( pkt );
    }

    // 누군가가 이동했을 때
    public static void S_BroadcastMoveHandler( PacketSession session, IPacket packet )
    {
        var pkt           = packet as S_BroadcastMove;
        //var serverSession = session as ServerSession;

        PlayerManager.Instance.Move( pkt );
    }
}
