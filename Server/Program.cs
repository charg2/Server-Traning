﻿// See https://aka.ms/new-console-template for more information

using System.Net.Sockets;
using System.Net;
using System.Text;
using ServerCore;


Listener _listener = new Listener();

string      host     = Dns.GetHostName();
IPHostEntry ipHost   = Dns.GetHostEntry(host);
IPAddress   ipAddr   = ipHost.AddressList[0];
IPEndPoint  endPoint = new IPEndPoint(ipAddr, 7777);

// www.rookiss.com -> 127.1.2.3
/* 서버 */
_listener.Init( endPoint, () => { return new GameSession(); } );
Console.WriteLine( "Listening..." );

while ( true )
{
    // 손님을 입장시킨다.
    //Socket clientSocket = _listener.Accept();               
    ;
}

class Packet
{
    public ushort size;
    public ushort packetId;
}

class GameSession : PacketSession
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        //Packet packet = new Packet() { size = 100, packetId = 10 };

        //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        //byte[] buffer = BitConverter.GetBytes(packet.size);
        //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
        //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
        //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
        //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

        // 100명
        // 1 -> 이동패킷이 100명
        // 100 -> 이동 패킷이 100 * 100 = 1만
        //Send(sendBuff);
        Thread.Sleep( 5000 );
        Disconnect();
    }

    public override void OnRecvPacket( ArraySegment<byte> buffer )
    {
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id   = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        Console.WriteLine( $"RecvPacketId : {id}, Size {size}" );
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
