namespace Server;

public class GameRoom
{
    static        GameRoom _instance = new();
    public static GameRoom Instance => _instance;

    private List< ClientSession > _sessions = new();
    private object                _lock     = new();

    public void Enter( ClientSession session )
    {
        lock ( _lock )
        {
            _sessions.Add( session );
            session.Room = this;
        }
    }

    public void Leave( ClientSession session )
    {
        lock ( _lock )
        {
            _sessions.Remove( session );
        }
    }

    public void Broadcast( ClientSession clientSession, string packetChat )
    {
        var packet = new S_Chat
        {
            playerId = clientSession.SessionId,
            chat     = $"{ packetChat } I am { clientSession.SessionId }"
        };

        ArraySegment< byte > segment = packet.Write();

        lock ( _lock )
        {
            foreach ( var session in _sessions )
                session.Send( segment );
        }
    }
}