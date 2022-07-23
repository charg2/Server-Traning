using System.Net;
using ServerCore;

namespace Server;

class Packet
{
    public ushort size;
    public ushort packetId;
}

class ClientSession : PacketSession
{
    public override void OnConnected( EndPoint endPoint )
    {
        Console.WriteLine( $"OnConnected : {endPoint}" );

        Thread.Sleep( 5000 );
        Disconnect();
    }

    public override void OnRecvPacket( ArraySegment<byte> buffer )
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16( buffer.Array, buffer.Offset );
        count += 2;
        ushort id = BitConverter.ToUInt16( buffer.Array, buffer.Offset + count );
        count += 2;

        switch ( (PacketID)id )
        {
            case PacketID.PlayerInfoReq:
            {
                var p = new PlayerInfoReq();
                p.Read( buffer );
                Console.WriteLine( $"PlayerInfoReq : {p.playerId} {p.name}" );

                foreach ( PlayerInfoReq.SkillInfo skill in p.skills )
                    Console.WriteLine( $"Skill({skill.id})({skill.level})({skill.duration})" );
            } 
            break;
            default:
            {
                Console.WriteLine( "error id : " + id.ToString() );
            }
            break;
        }
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