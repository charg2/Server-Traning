
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

    Dictionary< ushort, Func< PacketSession, ArraySegment< byte >, IPacket > > _makeFunc = new();
    Dictionary< ushort, Action< PacketSession, IPacket > > _handler = new();

    public void Register()
    {
     _makeFunc.Add( (ushort)PacketID.S_Chat, MakePacket<S_Chat> );
        _handler.Add( (ushort)PacketID.S_Chat, PacketHandler.S_ChatHandler );

    }

    public void OnRecvPacket( PacketSession session, ArraySegment< byte > buffer, Action< PacketSession, IPacket > onRecvCallback = null )
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16( buffer.Array, buffer.Offset );
        count += 2;
        ushort packetId = BitConverter.ToUInt16( buffer.Array, buffer.Offset + count );
        count += 2;

        if ( _makeFunc.TryGetValue( packetId, out var func ) )
        {
            IPacket packet = func.Invoke( session, buffer );
            if ( onRecvCallback != null )
                onRecvCallback.Invoke( session, packet );
            else
                HandlePacket( session, packet );
        }
    }
    
    T MakePacket< T >( PacketSession session, ArraySegment< byte > buffer ) where T : IPacket, new()
    {
        T packet = new T();
        packet.Read( buffer );
      
        return packet;
    }

    public void HandlePacket( PacketSession session, IPacket packet )
    {
        if ( _handler.TryGetValue( packet.Protocol, out var action ) )
            action.Invoke( session, packet );
    }

}
