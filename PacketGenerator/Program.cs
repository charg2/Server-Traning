using PackGenerator;
using System.Xml;

public class PacketGenerator
{
    private static string _genPackets = string.Empty;
    private static ushort _packetId;
    private static string _packetEnums = string.Empty;

    static void Main( string[] args )
    {
        string pdlPath = "../PDL.xml";

        var settings = new XmlReaderSettings()
        {
            IgnoreComments = true, IgnoreWhitespace = true
        };

        if ( args.Length > 0 )
            pdlPath = args[ 0 ];

        using XmlReader reader = XmlReader.Create( pdlPath, settings );

        reader.MoveToContent();

        while ( reader.Read() )
        {
            if ( reader.Depth == 1 && reader.NodeType == XmlNodeType.Element )
                ParsePacket( reader );
        }

        string fileText = string.Format( PacketFormat.fileFormat, _packetEnums, _genPackets );
        File.WriteAllText( "GenPackets.cs", fileText );
    }


    /// <summary>
    /// 패킷을 파싱한다
    /// </summary>
    static void ParsePacket( XmlReader reader )
    {
        if ( reader.NodeType == XmlNodeType.EndElement )
            return;

        if ( reader.Name.ToLower() != "packet" )
        {
            Console.WriteLine( "Invalid packet node" );
            return;
        }

        string packetName = reader[ "name" ];
        if ( string.IsNullOrEmpty( packetName ) )
        {
            Console.WriteLine( "Packet without name" );
            return;
        }

        Tuple< string, string, string > t = ParseMembers( reader );
        _genPackets += string.Format( PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3 );
        _packetEnums += string.Format( PacketFormat.packetEnumFormat, packetName, ++_packetId ) + Environment.NewLine + "\t";
    }

    /// <summary>
    /// 패킷 멤버를 파싱한다
    /// </summary>
    static Tuple<string, string, string> ParseMembers( XmlReader reader )
    {
        string packetName = reader[ "name" ];

        string memberCode = "";
        string readCode   = "";
        string writeCode  = "";

        int depth = reader.Depth + 1;
        while ( reader.Read() )
        {
            if ( reader.Depth != depth )
                break;

            string memberName = reader[ "name" ];
            if ( string.IsNullOrEmpty( memberName ) )
            {
                Console.WriteLine( "Member without name" );
                return null;
            }

            if ( string.IsNullOrEmpty( memberCode ) == false )
                memberCode += Environment.NewLine;
            if ( string.IsNullOrEmpty( readCode ) == false )
                readCode += Environment.NewLine;
            if ( string.IsNullOrEmpty( writeCode ) == false )
                writeCode += Environment.NewLine;

            string memberType = reader.Name.ToLower();
            switch ( memberType )
            {
                case "bool":
                case "short":
                case "ushort":
                case "int":
                case "long":
                case "float":
                case "double":
                    memberCode += string.Format( PacketFormat.memberFormat, memberType, memberName );
                    readCode   += string.Format( PacketFormat.readFormat, memberName, ToMemberType( memberType ), memberType );
                    writeCode  += string.Format( PacketFormat.writeFormat, memberName, memberType );
                    break;
                case "string":
                    memberCode += string.Format( PacketFormat.memberFormat,      memberType, memberName );
                    readCode   += string.Format( PacketFormat.readStringFormat,  memberName );
                    writeCode  += string.Format( PacketFormat.writeStringFormat, memberName );
                    break;
                case "list":
                    Tuple< string, string, string > t = ParseList( reader );
                    memberCode += t.Item1;
                    readCode   += t.Item2;
                    writeCode  += t.Item3;
                    break;
                default:
                    break;
            }
        }

        memberCode = memberCode.Replace( "\n", "\n\t" );
        readCode   = readCode.Replace( "\n", "\n\t\t" );
        writeCode  = writeCode.Replace( "\n", "\n\t\t" );

        return new( memberCode, readCode, writeCode );
    }

    /// <summary>
    /// 
    /// </summary>
    static Tuple< string, string, string > ParseList( XmlReader reader )
    {
        string listName = reader[ "name" ];
        if ( string.IsNullOrEmpty( listName ) )
        {
            Console.WriteLine( "List without name" );
            return null;
        }

        Tuple<string, string, string> t = ParseMembers(reader);

        string memberCode = string.Format( PacketFormat.memberListFormat,
                                           FirstCharToUpper( listName ),
                                           FirstCharToLower( listName ),
                                           t.Item1,
                                           t.Item2,
                                           t.Item3 );

        string readCode = string.Format( PacketFormat.readListFormat,
                                         FirstCharToUpper( listName ),
                                         FirstCharToLower( listName ) );

        string writeCode = string.Format( PacketFormat.writeListFormat,
                                          FirstCharToUpper( listName ),
                                          FirstCharToLower( listName ) );

        return new Tuple< string, string, string >( memberCode, readCode, writeCode );
    }


    static string ToMemberType( string memberType )
    {
        switch ( memberType )
        {
            case "bool":
                return "ToBoolean";
            case "short":
                return "ToInt16";
            case "ushort":
                return "ToUInt16";
            case "int":
                return "ToInt32";
            case "long":
                return "ToInt64";
            case "float":
                return "ToSingle";
            case "double":
                return "ToDouble";
            default:
                return "";
        }
    }

    static string FirstCharToUpper( string input )
    {
        if ( string.IsNullOrEmpty( input ) )
            return "";

        return input[ 0 ].ToString().ToUpper() + input.Substring( 1 );
    }

    static string FirstCharToLower( string input )
    {
        if ( string.IsNullOrEmpty( input ) )
            return "";

        return input[ 0 ].ToString().ToLower() + input.Substring( 1 );
    }


}
