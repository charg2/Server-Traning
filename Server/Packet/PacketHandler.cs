using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketHandler
    {
        public static void P( PacketSession s, IPacket p )
        {
            var packet = p as PlayerInfoReq;

            Console.WriteLine( $"PlayerInfoReq : {packet.playerId} {packet.name}" );

            foreach ( PlayerInfoReq.Skill skill in packet.skills )
                Console.WriteLine( $"Skill({skill.id})({skill.level})({skill.duration})" );
        }
    }
}
