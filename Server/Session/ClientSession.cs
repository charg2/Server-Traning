using System.Net;
using ServerCore;

namespace Server;

public class ClientSession : PacketSession
{
    public int       SessionId { get; set; }
    public GameRoom? Room      { get; set; }
    public float PosX { get; set; }
    public float PosY { get; set; }
    public float PosZ { get; set; }

    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        GameRoom.Instance.DoAsyncJob( () => GameRoom.Instance.Enter( this ) );
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
            GameRoom room = GameRoom.Instance;
            room.DoAsyncJob( () => room.Leave( this ) );
            Room = null;
        }

        Console.WriteLine( $"OnDisConnected : {endPoint}" );
    }

    public override void OnSend( int numOfBytes )
    {
        //Console.WriteLine( $"Transferred byte : {numOfBytes}" );
    }
}