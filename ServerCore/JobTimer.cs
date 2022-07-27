using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class JobTimer
{
    public static JobTimer Instance = new();


    private PriorityQueue< Action, long > _jobs = new();
    private object                        _lock = new();

    public void DoAsyncJobAfter( Action job, int tickAfter = 0 )
    {
        lock ( _lock )
        {
            _jobs.Enqueue( job, tickAfter + System.Environment.TickCount64 );
        }
    }

    public void Flush()
    {
        while ( true )
        {
            long now = System.Environment.TickCount64;

            lock ( _lock )
            {
                if ( _jobs.Count == 0 )
                    break;

                if ( !_jobs.TryPeek( out var job, out var afterTick ) )
                    break;

                if ( afterTick > now )
                    break;

                job.Invoke();
                _jobs.Dequeue();
            }
        }
    }

}