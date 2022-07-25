
using System;
using System.Collections.Generic;
using ServerCore;

class PacketManager
{
    public static PacketManager Instance { get; } = new();

    PacketManager()
    {
        Register();
    }

    Dictionary< ushort, Action< PacketSession, ArraySegment< byte > > > _onRecv = new();
    Dictionary< ushort, Action< PacketSession, IPacket > > _handler = new();

    public void Register()
    {
     _onRecv.Add( (ushort)PacketID.C_Chat, MakePacket<C_Chat> );
        _handler.Add( (ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler );

    }

    public void OnRecvPacket( PacketSession session, ArraySegment< byte > buffer )
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16( buffer.Array, buffer.Offset );
        count += 2;
        ushort packetId = BitConverter.ToUInt16( buffer.Array, buffer.Offset + count );
        count += 2;

        if ( _onRecv.TryGetValue( packetId, out var action ) )
            action.Invoke( session, buffer );
    }
    
    void MakePacket< T >( PacketSession session, ArraySegment< byte > buffer ) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read(buffer);
        
        if ( _handler.TryGetValue( packet.Protocol, out var action ) )
            action.Invoke( session, packet );
    }
}
