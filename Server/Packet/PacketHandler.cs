using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Server;


class PacketHandler
{
    
    public static void C_LeaveGameHandler( PacketSession session, IPacket packet )
    {
        var clientSession = session as ClientSession;

        if ( clientSession?.Room == null )
            return;

        GameRoom room = clientSession.Room;
        room.DoAsyncJob( () => room.Leave( clientSession ) );
    }

    public static void C_MoveHandler( PacketSession session, IPacket packet )
    {
        var movePacket    = packet as C_Move;
        var clientSession = session as ClientSession;

        if ( clientSession?.Room == null )
            return;

        Console.WriteLine( $"{ movePacket.posX }, { movePacket.posY }, { movePacket.posZ }" );

        GameRoom room = clientSession.Room;
        room.DoAsyncJob( () => room.Move( clientSession, movePacket ) );
    }
}
