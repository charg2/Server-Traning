namespace Server;

public class GameRoom
{
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
}