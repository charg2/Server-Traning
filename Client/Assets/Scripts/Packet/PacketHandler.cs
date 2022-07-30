using System.Diagnostics;
using DummyClient;
using ServerCore;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

class PacketHandler
{
    public static void S_ChatHandler( PacketSession session, IPacket packet )
    {
        var chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        UnityEngine.Debug.Log( chatPacket.chat );

        //if ( chatPacket.playerId == 1 )
        {
            GameObject gameObject = GameObject.Find( "Player" );
            if ( gameObject == null )
                UnityEngine.Debug.Log( "Player Not Found" );
            else
                UnityEngine.Debug.Log( "Player Found" );
        }
    }
}
