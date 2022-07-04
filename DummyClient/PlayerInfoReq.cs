using System.Text;
using DummyClient;
using ServerCore;

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoOk  = 2,
};

class PlayerInfoReq : Packet
{
    public long   playerId;
    public string name;

    public struct SkillInfo
    {
        public int   id;
        public short level;
        public float duration;

        public bool Write( Span< byte > s, ref ushort count )
        {
            bool success = true;
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), id );
            count   += sizeof( int );
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), level );
            count   += sizeof( short );
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), duration );
            count   += sizeof( float );
            return success;
        }

        public void Read( ReadOnlySpan< byte > s, ref ushort count )
        {
            id       =  BitConverter.ToInt32( s.Slice( count, s.Length - count ) );
            count    += sizeof( int );
            level    =  BitConverter.ToInt16( s.Slice( count, s.Length - count ) );
            count    += sizeof( short );
            duration =  BitConverter.ToSingle( s.Slice( count, s.Length - count ) );
            count    += sizeof( float );
        }
    }

    public List< SkillInfo > skills = new List< SkillInfo >();

    public void Read( ArraySegment< byte > segment )
    {
        ushort count = 0;

        ReadOnlySpan< byte > s = new ReadOnlySpan< byte >( segment.Array, segment.Offset, segment.Count );

        count         += sizeof( ushort );
        count         += sizeof( ushort );
        this.playerId =  BitConverter.ToInt64( s.Slice( count, s.Length - count ) );
        count         += sizeof( long );

        // string 
        ushort nameLen = BitConverter.ToUInt16( s.Slice( count, s.Length - count ) );
        count     += sizeof( ushort );
        this.name =  Encoding.Unicode.GetString( s.Slice( count, nameLen ) );
        count     += nameLen;

        // skill list
        skills.Clear();
        ushort skillLen = BitConverter.ToUInt16( s.Slice( count, s.Length - count ) );
        count += sizeof( ushort );
        for ( int i = 0; i < skillLen; i++ )
        {
            SkillInfo skill = new SkillInfo();
            skill.Read( s, ref count );
            skills.Add( skill );
        }
    }

    public ArraySegment< byte > Write()
    {
        ArraySegment< byte > segment = SendBufferHelper.Open( 4096 );

        ushort count   = 0;
        bool   success = true;

        Span< byte > s = new Span< byte >( segment.Array, segment.Offset, segment.Count );

        count   += sizeof( ushort );
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), (ushort)PacketID.PlayerInfoReq );
        count   += sizeof( ushort );
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), this.playerId );
        count   += sizeof( long );

        // string
        ushort nameLen = (ushort)Encoding.Unicode.GetBytes( this.name, 0, this.name.Length, segment.Array
                                                            , segment.Offset + count + sizeof( ushort ) );
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), nameLen );
        count   += sizeof( ushort );
        count   += nameLen;

        // skill list
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), (ushort)skills.Count );
        count   += sizeof( ushort );
        foreach ( SkillInfo skill in skills )
        {
            success &= skill.Write( s, ref count );
        }

        success &= BitConverter.TryWriteBytes( s, count );

        if ( success == false )
            return null;

        return SendBufferHelper.Close( count );
    }
}