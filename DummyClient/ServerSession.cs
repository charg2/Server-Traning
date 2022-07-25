using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient;

class ServerSession : Session
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );
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