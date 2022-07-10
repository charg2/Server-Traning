using System.Xml;

XmlReaderSettings settings = new XmlReaderSettings()
{
    IgnoreComments   = true,
    IgnoreWhitespace = true
};

using ( XmlReader r = XmlReader.Create( "PDL.xml", settings ) )
{
    r.MoveToContent();

    while ( r.Read() )
    {
        if ( r.Depth == 1 && r.NodeType == XmlNodeType.Element )
            ParsePacket( r );
        //Console.WriteLine(r.Name + " " + r["name"]);
    }
}

/// <summary>
/// 패킷을 파싱한다
/// </summary>
static void ParsePacket( XmlReader r )
{
    if ( r.NodeType == XmlNodeType.EndElement )
        return;

    if ( r.Name.ToLower() != "packet" )
    {
        Console.WriteLine( "Invalid packet node" );
        return;
    }

    string packetName = r["name"];
    if ( string.IsNullOrEmpty( packetName ) )
    {
        Console.WriteLine( "Packet without name" );
        return;
    }

    ParseMembers( r );
}

/// <summary>
/// 패킷 멤버를 파싱한다
/// </summary>
static void ParseMembers( XmlReader r )
{
    string packetName = r[ "name" ];

    int depth = r.Depth + 1;
    while ( r.Read() )
    {
        if ( r.Depth != depth )
            break;

        string memberName = r["name"];
        if ( string.IsNullOrEmpty( memberName ) )
        {
            Console.WriteLine( "Member without name" );
            return;
        }

        string memberType = r.Name.ToLower();
        switch ( memberType )
        {
            case "bool":
            case "byte":
            case "short":
            case "ushort":
            case "int":
            case "long":
            case "float":
            case "double":
            case "string":
            case "list":
                break;
            default:
                break;
        }
    }
}