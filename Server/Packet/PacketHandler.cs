using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class PacketHandler
{
    public static void C_PlayerInfoReqHandler( PacketSession arg1, IPacket arg2 )
    {
        //var packet = arg2 as C_PlayerInfoReq;

        //Console.WriteLine( $"PlayerInfoReq : {packet.playerId} {packet.name}" );

        //foreach ( C_PlayerInfoReq.Skill skill in packet.skills )
        //    Console.WriteLine( $"Skill({skill.id})({skill.level})({skill.duration})" );
    }

    public static void C_ChatHandler( PacketSession arg1, IPacket arg2 )
    {
        throw new NotImplementedException();
    }
}
