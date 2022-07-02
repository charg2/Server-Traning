using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;
abstract class Session
{
    private Socket _socket;
    private int    _disconnected = 0;

    private object                       _lock        = new();
    private Queue< byte[] >              _sendQueue   = new();
    private List< ArraySegment< byte > > _pendingList = new();
    private SocketAsyncEventArgs         _sendArgs    = new();
    private SocketAsyncEventArgs         _recvArgs    = new();

    public abstract void OnConnected( EndPoint endPoint );
    public abstract void OnRecv( ArraySegment<byte> buffer );
    public abstract void OnSend( int numOfBytes );
    public abstract void OnDisConnected( EndPoint endPoint );

    public void Start( Socket socket )
    {
        _socket = socket;

        _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>( OnRecvCompleted );
        _recvArgs.SetBuffer( new byte[ 1024 ], 0, 1024 );

        _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>( OnSendComplted );
        RegisterRecv();
    }

    public void Send( byte[] sendBuff )
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
        while ( _sendQueue.Count > 0 )
        {
            byte[] buff = _sendQueue.Dequeue();
            _pendingList.Add( new ArraySegment<byte>( buff, 0, buff.Length ) );
        }
        _sendArgs.BufferList = _pendingList;

        bool pending = _socket.SendAsync(_sendArgs);
        if ( !pending )
            OnSendComplted( null, _sendArgs );
    }

    void OnSendComplted( object sender, SocketAsyncEventArgs args )
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
        bool pending = _socket.ReceiveAsync(_recvArgs);
        if ( !pending )
            OnRecvCompleted( null, _recvArgs );
    }

    void OnRecvCompleted( object sender, SocketAsyncEventArgs args )
    {
        if ( args.BytesTransferred > 0 && args.SocketError == SocketError.Success )
        {
            try
            {
                OnRecv( new ArraySegment<byte>( args.Buffer, args.Offset, args.BytesTransferred ) );

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