﻿using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore;

public class Connector
{
    private Func< Session > _sessionFactory;

    public void Conncect( IPEndPoint endPoint, Func< Session > sessionFactory )
    {
        // 휴대폰 설정
        var socket = new Socket( endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp );
        _sessionFactory = sessionFactory;

        var args = new SocketAsyncEventArgs();
        args.Completed      += OnConnectCompleted;
        args.RemoteEndPoint =  endPoint;
        args.UserToken      =  socket;

        RegisterConnect( args );
    }

    void RegisterConnect( SocketAsyncEventArgs args )
    {
        Socket socket = args.UserToken as Socket;
        if ( socket == null )
            return;

        bool pending = socket.ConnectAsync( args );
        if ( !pending )
            OnConnectCompleted( null, args );
    }

    void OnConnectCompleted( object sender, SocketAsyncEventArgs args )
    {
        if ( args.SocketError == SocketError.Success )
        {
            Session session = _sessionFactory.Invoke();
            session.Start( args.ConnectSocket );
            session.OnConnected( args.RemoteEndPoint );
        }
        else
        {
            Console.WriteLine( $"OnConnectCompeted Fail: {args.SocketError}" );
        }
    }
}