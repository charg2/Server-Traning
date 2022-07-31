using ServerCore;

namespace Server;

public class GameRoom : IJobExecutor
{
    static        GameRoom _instance = new();
    public static GameRoom Instance => _instance;

    private List< ClientSession >        _sessions    = new();
    private object                       _lock        = new();
    private JobExecutor                  _jobExecutor = new();
    private List< ArraySegment< byte > > _pendingList = new();

    public void DoAsyncJob( Action job )
    {
        _jobExecutor.DoAsyncJob( job );
    }

    public void Enter( ClientSession session )
    {
        // 플레이어 추가
        _sessions.Add( session );
        session.Room = this;

        // 입장 플레이어에게 기존 플레이어 목록 전송
        var players = new S_PlayerList();
        foreach ( var clientSession in _sessions )
        {
            players.players.Add( new S_PlayerList.Player()
            {
                isSelf = ( clientSession == session),
                playerId = clientSession.SessionId,
                posX = clientSession.PosX,
                posY = clientSession.PosY,
                posZ = clientSession.PosZ,
            } );
            
        }
        
        session.Send( players.Write() );
        //  기존 플레이어에게 입장플레이어 정보 전송
        var enter = new S_BroadcastEnterGame()
        {
            playerId = session.SessionId, posX = 0, posY = 0, posZ = 0,
        };


        Broadcast( enter.Write() );
    }

    public void Leave( ClientSession session )
    {
        _sessions.Remove( session );

        //  기존 플레이어에게 입장플레이어 정보 전송
        var leave = new S_BroadcastLeaveGame()
        {
            playerId = session.SessionId,
        };

        Broadcast( leave.Write() );
    }

    public void Broadcast( ArraySegment<byte> segment )
    {
        _pendingList.Add( segment );
    }

    public void Flush()
    {
        foreach ( var session in _sessions )
            session.Send( _pendingList );

        // Console.WriteLine( $"Flushed { _pendingList.Count } Items" );
        _pendingList.Clear();
    }

    public void Move( ClientSession clientSession, C_Move movePacket )
    {
        clientSession.PosX = movePacket.posX;
        clientSession.PosY = movePacket.posY;
        clientSession.PosZ = movePacket.posZ;

        var move = new S_BroadcastMove()
        {
            playerId = clientSession.SessionId, 
            posX = clientSession.PosX,
            posY = clientSession.PosY, 
            posZ = clientSession.PosZ,
        };

        Broadcast( move.Write() );
    }
}