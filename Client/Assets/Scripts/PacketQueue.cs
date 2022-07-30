using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue
{
    public static PacketQueue      Instance { get; } = new();
    private       Queue< IPacket > _packetQueue = new();
    object                         _lock        = new();

    public void Push( IPacket packet )
    {
        lock ( _lock )
        {
            _packetQueue.Enqueue( packet );
        }
    }

    public IPacket Pop()
    {
        lock ( _lock )
        {
            if ( _packetQueue.Count == 0 )
            {
                return null;
            }
            return _packetQueue.Dequeue();
        }
    }
}