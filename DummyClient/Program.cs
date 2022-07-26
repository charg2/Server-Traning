using System.Net;
using DummyClient;
using Server;
using ServerCore;

// DNS (Domain Name System)
string      host     = Dns.GetHostName();
IPHostEntry ipHost   = Dns.GetHostEntry( host );
IPAddress   ipAddr   = ipHost.AddressList[ 0 ];
IPEndPoint  endPoint = new IPEndPoint( ipAddr, 7777 );

Connector connector = new Connector();

connector.Connect( endPoint,
                   () => SessionManager.Instance.Generate(),
                   500 );

while ( true )
{
    try
    {
        SessionManager.Instance.SendForEach();
    }
    catch ( Exception e )
    {
        Console.WriteLine( e.ToString() );
    }

    Thread.Sleep( 250 );
}

