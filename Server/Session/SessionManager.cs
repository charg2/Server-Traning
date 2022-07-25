
namespace Server;
// ServerCore에서 작업해도 괜찮다. => 단순 취향 차이
class SessionManager
{
    static        SessionManager _instance = new();
    public static SessionManager Instance => _instance;

    // 티켓 아이디
    int                                      _sessionId = 0;
    private Dictionary< int, ClientSession > _sessions  = new();
    private object                           _lock      = new();

    // Session 생성 및 ID 발급
    public ClientSession Generate()
    {
        lock ( _lock )
        {
            int sessionId = ++_sessionId;

            var session = new ClientSession { SessionId = sessionId };
            _sessions.Add( sessionId, session );

            System.Console.WriteLine( $"Connected : { sessionId }" );

            return session;
        }
    }

    public ClientSession Find( int id )
    {
        lock ( _lock )
        {
            _sessions.TryGetValue( id, out var session );
            return session;
        }
    }
    public void Remove( ClientSession session )
    {
        lock ( _lock )
        {
            _sessions.Remove( session.SessionId );
        }
    }
}
