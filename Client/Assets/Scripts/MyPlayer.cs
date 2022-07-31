using DummyClient;
using ServerCore;
using System;
using System.Collections;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _networkManager;
    void Start()
    {
        _networkManager = GameObject.Find( "NetworkManager" ).GetComponent<NetworkManager>();
        StartCoroutine( "CoSendPacket" );
    }

    IEnumerator CoSendPacket()
    {
        while ( true )
        {
            yield return new WaitForSeconds( 0.25f );

            C_Move movePacket = new C_Move();
            movePacket.posX = UnityEngine.Random.Range( -50f, 50f );
            movePacket.posY = 0;
            movePacket.posZ = UnityEngine.Random.Range( -50f, 50f );
            _networkManager.Send( movePacket.Write() );
        }
    }
}