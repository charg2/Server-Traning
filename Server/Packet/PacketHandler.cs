using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;


class PacketHandler
{
    public static void C_ChatHandler( PacketSession session, IPacket arg2 )
    {
        var chatPacket    = arg2 as C_Chat;
        var clientSession = session as ClientSession;

        if ( clientSession?.Room == null )
            return;

        GameRoom room = clientSession.Room;
        room.DoAsyncJob( 
            () => room.Broadcast( clientSession, chatPacket.chat ) );
    }
}
