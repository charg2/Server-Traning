// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Net;
using System.Text;
using Server;
using ServerCore;


Listener _listener = new Listener();

// DNS (Domain Name System)
string      host     = Dns.GetHostName();
IPHostEntry ipHost   = Dns.GetHostEntry(host);
IPAddress   ipAddr   = ipHost.AddressList[0];
IPEndPoint  endPoint = new IPEndPoint(ipAddr, 7777);

// www.rookiss.com -> 127.1.2.3
/* 서버 */
_listener.Init( endPoint, () => new ClientSession() );
Console.WriteLine( "Listening..." );

while ( true )
{
    // 손님을 입장시킨다.
    //Socket clientSocket = _listener.Accept();               
    ;
}