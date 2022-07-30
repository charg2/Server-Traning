using System;
using System.Net;
using System.Text;
using ServerCore;
using UnityEngine;

namespace DummyClient
{

    class ServerSession : PacketSession
    {
        public override void OnConnected( EndPoint endPoint )
        {
            Console.WriteLine( $"OnConnected : {endPoint}" );
        }

        public override void OnDisConnected( EndPoint endPoint )
        {
            Console.WriteLine( $"OnDisConnected : {endPoint}" );
        }

        public override void OnRecvPacket( ArraySegment< byte > buffer )
        {
            PacketManager.Instance.OnRecvPacket( this, 
                                                 buffer, 
                                                 ( session, pkt ) => PacketQueue.Instance.Push( pkt ) );
        }

        public override void OnSend( int numOfBytes )
        {
            //Console.WriteLine( $"Transferred byte : {numOfBytes}" );
        }
    }
}