using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public abstract class PacketSession : Session
{
    public static readonly int HeaderSize = 2;

    // [size(2)][packetId(2)][...][size(2)][packetId(2)][...]
    public sealed override int OnRecv( ArraySegment< byte > buffer )
    {
        int processLen = 0;

        while ( true )
        {
            // 최소한 헤더는 파싱할 수 있는지 확인
            if ( buffer.Count < HeaderSize )
                break;

            // 패킷이 완전체로 도착했는지 확인
            ushort dataSize = BitConverter.ToUInt16( buffer.Array, buffer.Offset );
            if ( buffer.Count < dataSize )
                break;

            // 여기까지 왔으면 패킷 조립이 가능하다
            OnRecvPacket( new ArraySegment< byte >( buffer.Array, buffer.Offset, dataSize ) );
            processLen += dataSize;
            buffer     =  new ArraySegment< byte >( buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize );
        }
        return processLen;
    }

    public abstract void OnRecvPacket( ArraySegment<byte> buffer );
}

public abstract class Session
{
    private Socket _socket;
    private int    _disconnected = 0;

    private RecvBuffer _recvBuffer = new RecvBuffer( 1024 );

    private object                        _lock        = new();
    private Queue< ArraySegment< byte > > _sendQueue   = new();
    private List< ArraySegment< byte > >  _pendingList = new();
    private SocketAsyncEventArgs          _sendArgs    = new();
    private SocketAsyncEventArgs          _recvArgs    = new();

    
    public abstract void OnConnected( EndPoint endPoint );

    public abstract int  OnRecv( ArraySegment< byte > buffer );
    
    public abstract void OnSend( int numOfBytes );

    public abstract void OnDisConnected( EndPoint endPoint );


    public void Start( Socket socket )
    {
        _socket = socket;

        _recvArgs.Completed += OnRecvCompleted;
        _sendArgs.Completed += OnSendCompleted;

        RegisterRecv();
    }

    void Clear()
    {
        lock ( _lock )
        {
            _sendQueue.Clear();
            _pendingList.Clear();
        }
    }

    public void Send( ArraySegment< byte > sendBuff )
    {
        lock ( _lock )
        {
            _sendQueue.Enqueue( sendBuff );
            if ( _pendingList.Count == 0 )
                RegisterSend();
        }
    }

    public void Disconnect()
    {
        if ( Interlocked.Exchange( ref _disconnected, 1 ) == 1 )
            return;

        OnDisConnected( _socket.RemoteEndPoint );

        _socket.Shutdown( SocketShutdown.Both );
        _socket.Close();
    }

    #region Netowkr IO Method

    void RegisterSend()
    {
        if ( _disconnected == 1 )
            return;

        while ( _sendQueue.Count > 0 )
        {
            ArraySegment< byte > buff = _sendQueue.Dequeue();
            _pendingList.Add( buff );
        }
        _sendArgs.BufferList = _pendingList;

        try
        {
            bool pending = _socket.SendAsync( _sendArgs );
            if ( !pending )
                OnSendCompleted( null, _sendArgs );
        }
        catch ( Exception ex )
        {
            Console.WriteLine( $"RegisterSend Fail { ex } " );
        }
    }

    void OnSendCompleted( object sender, SocketAsyncEventArgs args )
    {
        lock ( _lock )
        {
            if ( args.BytesTransferred > 0 && args.SocketError == SocketError.Success )
            {
                try
                {
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    OnSend( _sendArgs.BytesTransferred );

                    if ( _sendQueue.Count > 0 )
                        RegisterSend();
                }
                catch ( Exception e )
                {
                    Console.WriteLine( $"OnSendCompleted Failed {e}" );
                }
            }
            else
            {
                Disconnect();
            }
        }
    }

    void RegisterRecv()
    {
        if ( _disconnected == 1 )
            return;

        _recvBuffer.Clean();
        ArraySegment< byte > segment = _recvBuffer.WriteSegment;
        _recvArgs.SetBuffer( segment.Array, segment.Offset, segment.Count );

        try
        {
            bool pending = _socket.ReceiveAsync( _recvArgs );
            if ( !pending )
                OnRecvCompleted( null, _recvArgs );
        }
        catch (  Exception e )
        {
            Console.WriteLine( $"RegisterRecv Fail{ e }" );
            throw;
        }

    }


    void OnRecvCompleted( object sender, SocketAsyncEventArgs args )
    {
        if ( args.BytesTransferred > 0 && args.SocketError == SocketError.Success )
        {
            try
            {
                // Write 커서 이동
                if ( !_recvBuffer.OnWrite( args.BytesTransferred ) )
                {
                    Disconnect();
                    return;
                }

                // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지를 받는다.
                int processLen = OnRecv( _recvBuffer.ReadSegment );
                if ( processLen < 0 || _recvBuffer.DataSize < processLen )
                {
                    Disconnect();
                    return;
                }

                // Read 커서 이동
                if ( !_recvBuffer.OnRead( processLen ) )
                {
                    Disconnect();
                    return;
                }

                RegisterRecv();
            }
            catch ( Exception e )
            {
                Console.WriteLine( $"OnRecvCompleted Failed {e}" );
            }
        }
        else
        {
            Disconnect();
        }
    }
    #endregion
}