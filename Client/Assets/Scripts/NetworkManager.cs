using System.Collections;
using ServerCore;
using System.Net;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DummyClient
{
    public class NetworkManager : MonoBehaviour
    {
        private ServerSession _session = new();

        public void Start()
        { 
            // DNS (Domain Name System)
            string      host     = Dns.GetHostName();
            IPHostEntry ipHost   = Dns.GetHostEntry( host );
            IPAddress   ipAddr   = ipHost.AddressList[ 0 ];
            IPEndPoint  endPoint = new IPEndPoint( ipAddr, 7777 );

            Connector connector = new Connector();
            connector.Connect( endPoint,
                               () => _session,
                               1 );
        }

        public void Update()
        {
            List<IPacket> list = PacketQueue.Instance.PopAll();

            foreach ( IPacket packet in list )
                PacketManager.Instance.HandlePacket( _session, packet );
        }


        public void Send( ArraySegment< byte > sendBuff )
        {
            _session.Send( sendBuff );
        }
    }
}