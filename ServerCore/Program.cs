// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;


Listener _listener = new Listener();

// DNS 
string      host     = Dns.GetHostName();
IPHostEntry ipHost   = Dns.GetHostEntry( host );
IPAddress   ipAddr   = ipHost.AddressList[ 0 ];
IPEndPoint  endPoint = new IPEndPoint( ipAddr, 7777 );

_listener.Init( endPoint, () => new GameSession() );
Console.WriteLine( "Listening..." );

while ( true )
    ;

class GameSession : Session
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
        Send( sendBuff );
        Thread.Sleep( 1000 );
        Disconnect();
    }

    public override void OnDisConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnDisConnected : {endPoint}" );
    }

    public override void OnRecv( ArraySegment<byte> buffer )
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine( $"[From Client] {recvData}" );
    }

    public override void OnSend( int numOfBytes )
    {
        Console.WriteLine( $"Transferred byte : {numOfBytes}" );
    }
}