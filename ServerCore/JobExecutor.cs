using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public interface IJobExecutor
{
    void DoAsyncJob( Action job );
}

public class JobExecutor : IJobExecutor
{
    private Queue< Action > _jobQueue = new();
    private object          _lock     = new();

    private bool _flush = false;

    public void DoAsyncJob( Action job )
    {
        bool flush = false;

        lock ( _lock )
        {
            _jobQueue.Enqueue( job );
            if ( _flush == false )
            {
                _flush = true;
                flush  = _flush;
            }
        }

        if ( flush )
            Flush();
    }

    private void Flush()
    {
        while ( TryPop( out var job ) )
            job.Invoke();
    }


    public Action Pop()
    {
        lock ( _lock )
        {
            if ( _jobQueue.Count == 0 )
            {
                _flush = false;
                return null;
            }

            return _jobQueue.Dequeue();
        }
    }

    public bool TryPop( out Action job )
    {
        lock ( _lock )
        {
            if ( _jobQueue.Count == 0 )
            {
                _flush = false;
                job    = default;
                return false;
            }

            job = _jobQueue.Dequeue();
            return true;
        }
    }
}