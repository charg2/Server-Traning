// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Net;
using System.Text;
using Server;
using ServerCore;




Listener _listener = new();

// DNS (Domain Name System)
string      host     = Dns.GetHostName();
IPHostEntry ipHost   = Dns.GetHostEntry(host);
IPAddress   ipAddr   = ipHost.AddressList[0];
IPEndPoint  endPoint = new IPEndPoint(ipAddr, 7777);

// www.rookiss.com -> 127.1.2.3
/* 서버 */
_listener.Init( endPoint, () => SessionManager.Instance.Generate() );
Console.WriteLine( "Listening..." );

// FlushRoom();

JobTimer.Instance.DoAsyncJobAfter( FlushRoom, 250 );

while ( true )
{
    JobTimer.Instance.Flush();
}

void FlushRoom()
{
    GameRoom.Instance.DoAsyncJob( () => GameRoom.Instance.Flush() );
    JobTimer.Instance.DoAsyncJobAfter( FlushRoom, 250 );
}