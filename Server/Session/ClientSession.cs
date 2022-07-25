using System.Net;
using ServerCore;

namespace Server;

public class ClientSession : PacketSession
{
    public int       SessionId { get; set; }
    public GameRoom? Room      { get; set; }

    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        GameRoom.Instance.Enter( this );
    }

    public override void OnRecvPacket( ArraySegment<byte> buffer )
    {
        PacketManager.Instance.OnRecvPacket( this, buffer );
    }

    public override void OnDisConnected( EndPoint endPoint )
    {
        SessionManager.Instance.Remove( this );
        if ( Room != null )
        {
            Room.Leave( this );
            Room = null;
        }

        Console.WriteLine( $"OnDisConnected : {endPoint}" );
    }


    public override void OnSend( int numOfBytes )
    {
        Console.WriteLine( $"Transferred byte : {numOfBytes}" );
    }
}