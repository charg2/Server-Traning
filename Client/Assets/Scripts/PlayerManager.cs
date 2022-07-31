using System.Collections.Generic;
using UnityEngine;

namespace DummyClient
{
    public class PlayerManager
    {
        private MyPlayer                  _myPlayer;
        private Dictionary< int, Player > _players = new();

        public static PlayerManager Instance { get; } = new();

        public void Add( S_PlayerList packet )
        {
            Object obj = UnityEngine.Resources.Load( "Player" );

            foreach ( var player in packet.players )
            {
                var gameObject = UnityEngine.Object.Instantiate( obj ) as GameObject;
                if ( player.isSelf )
                {
                    MyPlayer myPlayer = gameObject.AddComponent< MyPlayer >();
                    myPlayer.PlayerId           = player.playerId;
                    myPlayer.transform.position = new Vector3( player.posX, player.posY, player.posZ );

                    _myPlayer = myPlayer;
                }
                else
                {
                    Player otherPlayer = gameObject.AddComponent< Player >();
                    otherPlayer.PlayerId = player.playerId;
                    otherPlayer.transform.position = new Vector3( player.posX, player.posY, player.posZ );

                    _players.Add( player.playerId, otherPlayer );
                }
            }
        }

        public void LeaveGame( S_BroadcastLeaveGame packet )
        {
            if ( _myPlayer.PlayerId == packet.playerId )
            {
                GameObject.Destroy( _myPlayer.gameObject );
                _myPlayer = null;
            }
            else
            {
                if ( _players.TryGetValue( packet.playerId, out var player ) )
                {
                    GameObject.Destroy( player.gameObject );
                    _players.Remove( packet.playerId );
                }
            }
        }

        public void Move( S_BroadcastMove packet )
        {
            if ( _myPlayer.PlayerId == packet.playerId )
            {
                _myPlayer.transform.position = new Vector3( packet.posX, packet.posY, packet.posZ );
            }
            else
            {
                if ( _players.TryGetValue( packet.playerId, out var player ) )
                    player.transform.position = new Vector3( packet.posX, packet.posY, packet.posZ );
            }
        }

        public void EnterGame( S_BroadcastEnterGame packet )
        {
            if ( packet.playerId == _myPlayer.PlayerId )
                return;

            Object obj = UnityEngine.Resources.Load( "Player" );

            var gameObject = UnityEngine.Object.Instantiate( obj ) as GameObject;

            Player otherPlayer = gameObject.AddComponent< Player >();
            otherPlayer.transform.position = new Vector3( packet.posX, packet.posY, packet.posZ );

            _players.Add( packet.playerId, otherPlayer );
        }
    }

}