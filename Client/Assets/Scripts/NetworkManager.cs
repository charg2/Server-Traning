using System.Collections;
using ServerCore;
using System.Net;
using UnityEngine;

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
            IPacket packet = PacketQueue.Instance.Pop();
            if ( packet != null )
                PacketManager.Instance.HandlePacket( _session, packet );
        }
        IEnumerator CoSendPacket()
        {
        }
    }
}