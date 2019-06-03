using System;

using Serilog;
using System.Threading;

namespace TopShelfTemplate
{
    public class RealWork : IAmDoingRealWork
    {
        /// <summary>
        /// Manual reset event
        /// </summary>
        private ManualResetEvent _externalResetEvent = null;

        /// <summary>
        /// Internal reset event
        /// </summary>
        private ManualResetEvent _internalResetEvent = new ManualResetEvent( true );

        /// <summary>
        /// Wait handles
        /// </summary>
        private WaitHandle[] _waitHandles = null;

        /// <summary>
        /// Thread
        /// </summary>
        private Thread _worker;

        /// <summary>
        /// Thread name
        /// </summary>
        private readonly string _threadName;

        /// <summary>
        /// Thread scan frequency, in milliseconds
        /// </summary>
        private readonly int _scanFrequency = 10000;

        /// <summary>
        /// Service state
        /// </summary>
        public ServiceStateEnum ServiceState { get; set; }

        /// <summary>
        /// C'tor
        /// </summary>
        public RealWork( )
        {
            // Set internal reset event to non-signalled
            _internalResetEvent.Reset();
            _worker = null;

            _threadName = ConfigurationHelper.Instance.GetConfigurationValue( "Thread: Name" ) ?? "RealWork";
            _scanFrequency = int.Parse(
                ConfigurationHelper.Instance.GetConfigurationValue( "Thread:ScanFrequencyInMilliseconds" ) ?? "10000" );

            ServiceState = ServiceStateEnum.Stopped;
        }

        /// <summary>
        /// Start work
        /// </summary>
        public void Start( ManualResetEvent externalResetEvent )
        {
            _externalResetEvent = externalResetEvent ?? throw new ArgumentNullException( "manualResetEvent" );
            
            try
            {
                _worker = new Thread( new ThreadStart( ThreadFunction ) );
                _worker.Name = _threadName;
                _worker.Start();

                Thread.Sleep( 0 );
                ServiceState = ServiceStateEnum.Running;
            }
            catch
            {
                try
                {
                    Stop();
                    ServiceState = ServiceStateEnum.Stopped;
                }
                catch { } // We really have to stop somewhere
                throw;
            }

            Log.Information( "RealWork is running now." );
        }

        /// <summary>
        /// Stop work
        /// </summary>
        public void Stop()
        {
            // Do some cleanup here, depending on what specifically you are doing
            _internalResetEvent.Set();

            try
            {
                try
                {
                    // Big question here...is thread in blocking or executing state?
                    if (( _worker.ThreadState & ThreadState.WaitSleepJoin ) == 0)
                        _worker.Interrupt();
                    else
                    {
                        _worker.Abort();
                        _worker.Join( 20 );
                    }
                }
                catch( ThreadInterruptedException )
                {
                    ServiceState = ServiceStateEnum.Dead;

                    try
                    {
                        _worker.Abort();
                        _worker.Join( 20 );
                    }
                    catch { }
                }
            }
            catch
            {
                // We should really never get to here
                ServiceState = ServiceStateEnum.Runaway;
                throw;
            }
        }

        /// <summary>
        /// Main thread work here
        /// </summary>
        private void ThreadFunction()
        {
            _waitHandles = new[] { _externalResetEvent, _internalResetEvent };

            while( true )
            {
                if (WaitHandle.WaitAny( _waitHandles, _scanFrequency, false ) != WaitHandle.WaitTimeout)
                {
                    Log.Information( "RealWork received signal to stop." );
                    return;
                }

                // TODO: Now do some real work here, like checking the database table for something,
                //       wait for an event, check the message queue for some meessage, etc.
                Log.Information( "In RealWork, doing some real work here..." );
            }
        }
    }
}
