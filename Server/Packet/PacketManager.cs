using ServerCore;
using System;

namespace Server;

public class PacketManager
{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if ( _instance == null )
                _instance = new PacketManager();

            return _instance;
        }
        
    }
    #endregion


    private Dictionary< ushort, Action< PacketSession, ArraySegment< byte > > > _onRecv  = new();
    private Dictionary< ushort, Action< PacketSession, IPacket > >              _handler = new();

    public void Register()
    {
        _onRecv.Add( (ushort)PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq> );
        _handler.Add( (ushort)PacketID.PlayerInfoReq, PacketHandler.P );
    }

    public void OnRecvPacket( PacketSession session, ArraySegment< byte > buffer )
    {
        ushort count = 0;
        ushort size  = BitConverter.ToUInt16( buffer.Array, buffer.Offset );
        count += 2;
        ushort id = BitConverter.ToUInt16( buffer.Array, buffer.Offset + count );
        count += 2;

        if ( _onRecv.TryGetValue( id, out var action ) ) 
            action.Invoke( session, buffer );
    }


    void MakePacket< T >( PacketSession session, ArraySegment< byte > buffer ) where T : IPacket, new()
    {
        var packet = new T();
        packet.Read( buffer );

        if ( _handler.TryGetValue( packet.Protocol, out var action ) )
            action.Invoke( session, packet );
    }


}