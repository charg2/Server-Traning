using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class ServerSession : Session
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );
        var packet = new PlayerInfoReq() { playerId = 1, name = "ABCD" };
        packet.skills.Add( new PlayerInfoReq.SkillInfo() { id = 101, level = 1, duration = 3.0f } );
        packet.skills.Add( new PlayerInfoReq.SkillInfo() { id = 201, level = 2, duration = 4.0f } );
        packet.skills.Add( new PlayerInfoReq.SkillInfo() { id = 301, level = 3, duration = 5.0f } );
        packet.skills.Add( new PlayerInfoReq.SkillInfo() { id = 401, level = 4, duration = 6.0f } );

        // 보낸다
        // for (int i = 0; i < 5; i++)
        {
            ArraySegment< byte > s = packet.Write();
            if ( s != null )
                Send( s );
        }
    }

    public override void OnDisConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnDisConnected : {endPoint}" );
    }

    public override int OnRecv( ArraySegment< byte > buffer )
    {
        string recvData = Encoding.UTF8.GetString( buffer.Array, buffer.Offset, buffer.Count );
        Console.WriteLine( $"[From Server] {recvData}" );
        return buffer.Count;
    }

    public override void OnSend( int numOfBytes )
    {
        Console.WriteLine( $"Transferred byte : {numOfBytes}" );
    }
}