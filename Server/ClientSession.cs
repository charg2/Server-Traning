using System.Net;
using ServerCore;

namespace Server;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class ClientSession : PacketSession
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        Thread.Sleep( 5000 );
        Disconnect();
    }

    public override void OnRecvPacket( ArraySegment<byte> buffer )
    {
        PacketManager.Instance.OnRecvPacket( this, buffer );
    }

    public override void OnDisConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnDisConnected : {endPoint}" );
    }


    public override void OnSend( int numOfBytes )
    {
        Console.WriteLine( $"Transferred byte : {numOfBytes}" );
    }
}