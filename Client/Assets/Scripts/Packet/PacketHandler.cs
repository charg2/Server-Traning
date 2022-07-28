using System.Diagnostics;
using DummyClient;
using ServerCore;
using UnityEngine;

class PacketHandler
{
    public static void S_ChatHandler( PacketSession session, IPacket packet )
    {
        var chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        //Console.WriteLine( chatPacket.chat );

        UnityEngine.Debug.Log( chatPacket.chat );
    }
}
