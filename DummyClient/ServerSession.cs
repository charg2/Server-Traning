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
        Packet packet = new Packet() { size = 4, packetId = 7 };

        // 보낸다
        for ( int i = 0; i < 5; i++ )
        {
            ArraySegment< byte > openSegment = SendBufferHelper.Open( 4096 );
            byte[]               buffer      = BitConverter.GetBytes( packet.size );
            byte[]               buffer2     = BitConverter.GetBytes( packet.packetId );

            Array.Copy( buffer,  0, openSegment.Array, openSegment.Offset,                 buffer.Length );
            Array.Copy( buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length );

            ArraySegment< byte > sendBuff = SendBufferHelper.Close( packet.size );

            Send( sendBuff );
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