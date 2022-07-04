using ServerCore;
using System.Text;

public abstract class Packet
{
    public ushort size; // 2
    public ushort packetId; // 2
    public abstract ArraySegment<byte> Write();
    public abstract void Read( ArraySegment<byte> s );
}

class PlayerInfoReq : Packet
{
    public long playerId; // 8
    public string name;
    public struct SkillInfo
    {
        public int id;
        public short level;
        public float duration;
        // 스킬 하나 마다 byte array에 밀어 넣어주기 위한 인터페이스
        // return value가 boolean 타입인 이유 : TryWriteBytes와 인터페이스를 맞추기 위해서
        public bool Write( Span<byte> s, ref ushort count )
        {
            // SkillInfo가 들고 있는 데이터 들을 하나씩 밀어넣어주는 작업을 해준다.
            bool success = true;
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), id );
            count += sizeof( int );
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), level );
            count += sizeof( short );
            success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), duration );
            count += sizeof( float );

            return success;
        }
        public void Read( ReadOnlySpan<byte> s, ref ushort count )
        {
            id = BitConverter.ToInt32( s.Slice( count, s.Length - count ) );
            count += sizeof( int );
            level = BitConverter.ToInt16( s.Slice( count, s.Length - count ) );
            count += sizeof( short );
            // float 타입은 ToSingle이다(double은 ToDouble).
            duration = BitConverter.ToSingle( s.Slice( count, s.Length - count ) );
            count += sizeof( float );
        }
    }
    public List<SkillInfo> skills = new List<SkillInfo>();
    public PlayerInfoReq()
    {
        this.packetId = (ushort)PacketID.PlayerInfoReq;
    }

    public override void Read( ArraySegment<byte> segment )
    {
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof( ushort );
        count += sizeof( ushort );

        this.playerId = BitConverter.ToInt64( s.Slice( count, s.Length - count ) );
        count += sizeof( long );

        ushort nameLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof( ushort );

        this.name = Encoding.Unicode.GetString( s.Slice( count, nameLen ) );
        count += nameLen;

        // skill list
        // ToUInt16 : unsigned short
        // ToInt16 : (signed) short
        // 스킬의 갯수를 추출
        ushort skillLen = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
        count += sizeof( ushort );

        // 혹시 skills에 원치 않는 값이 들어 갔을 경우를 위해 Clear
        skills.Clear();

        for ( int i = 0; i < skillLen; i++ )
        {
            SkillInfo skill = new SkillInfo();
            // 새로 생성한 skill에 전달 받은 패킷의 정보를 역직렬화
            skill.Read( s, ref count );
            // 해당 SkillInfo를 패킷의 List<SkillInfo>에 추가
            skills.Add( skill );
        }
    }

    public override ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof( ushort );

        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), this.packetId );
        count += sizeof( ushort );
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), this.playerId );
        count += sizeof( long );

        ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), nameLen );
        count += sizeof( ushort );
        count += nameLen;

        // 스킬 하나마다 byte array에 밀어 넣어줘야 한다.
        // skill
        // (ushort)skills.Count : 스킬(list)이 들고 있는 갯수의 크기를 byte array에 밀어 넣어준다.
        // Count가 int 타입이기 때문에 2바이트 크기로 변환해 준 다음 밀어 넣어 준다.
        success &= BitConverter.TryWriteBytes( s.Slice( count, s.Length - count ), (ushort)skills.Count );
        count += sizeof( ushort );

        foreach ( SkillInfo skill in skills )
        {
            // ref로 count를 늘려주기 때문에 굳이 한번 더 늘려줄 필요가 없다.
            success &= skill.Write( s, ref count );
        }

        success &= BitConverter.TryWriteBytes( s, count );

        if ( success == false )
        {
            return null;
        }

        return SendBufferHelper.Close( count );
    }
}

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoOk = 2,
}