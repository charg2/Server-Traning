using ServerCore;

namespace Server;

public class GameRoom : IJobExecutor
{
    static        GameRoom _instance = new();
    public static GameRoom Instance => _instance;

    private List< ClientSession > _sessions    = new();
    private object                _lock        = new();
    private JobExecutor           _jobExecutor = new();

    public void Push( Action job )
    {
        _jobExecutor.Push( job );
    }

    public void Enter( ClientSession session )
    {
        _sessions.Add( session );
        session.Room = this;
    }

    public void Leave( ClientSession session )
    {
        _sessions.Remove( session );
    }

    public void Broadcast( ClientSession clientSession, string packetChat )
    {
        var packet = new S_Chat
        {
            playerId = clientSession.SessionId,
            chat     = $"{ packetChat } I am { clientSession.SessionId }"
        };

        ArraySegment< byte > segment = packet.Write();

        foreach ( var session in _sessions )
            session.Send( segment );
    }
}