
using DummyClient;

namespace Server;
// ServerCore에서 작업해도 괜찮다. => 단순 취향 차이
class SessionManager
{
    static        SessionManager _instance = new();
    public static SessionManager Instance => _instance;

    private List< ServerSession > _sessions = new();
    private object                _lock     = new();

    /// <summary>
    /// Session 생성 
    /// </summary>
    /// <returns></returns>
    public ServerSession Generate()
    {
        lock ( _lock )
        {
            var session = new ServerSession();
            _sessions.Add( session );

            return session;
        }
    }

    //public ServerSession Find( int id )
    //{
    //    lock ( _lock )
    //    {
    //        _sessions.TryGetValue( id, out var session );
    //        return session;
    //    }
    //}

    public void Remove( ServerSession session )
    {
        lock ( _lock )
        {
            _sessions.Remove( session );
        }
    }

    public void SendForEach()
    {
        lock ( _lock )
        {
            foreach ( var session in _sessions )
            {
                var chatPacket = new C_Chat();
                chatPacket.chat = "Hello Server";
                ArraySegment< byte > segment = chatPacket.Write();

                session.Send( segment );
            }

            Thread.Sleep( 250 );
        }
    }
}
