
using DummyClient;

namespace Server;
// ServerCore에서 작업해도 괜찮다. => 단순 취향 차이
class SessionManager
{
    static        SessionManager _instance = new();
    public static SessionManager Instance => _instance;

    private List< ServerSession > _sessions = new();
    private object                _lock     = new();
    Random _random = new();

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
                var movePacket = new C_Move
                {
                    posX = _random.Next( -50, 50 ),
                    posY = 0,
                    posZ = _random.Next( -50, 50 )
                };

                session.Send( movePacket.Write() );
            }
        }
    }
}
