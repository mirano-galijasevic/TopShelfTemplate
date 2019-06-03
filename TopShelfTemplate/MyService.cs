using System;
using System.Timers;

using Serilog;
using System.Threading;
using Topshelf.Runtime;

namespace TopShelfTemplate
{
    public class MyService : IService
    {
        /// <summary>
        /// Some service that is doing real work
        /// </summary>
        private readonly IAmDoingRealWork _realWorkService;

        /// <summary>
        /// Manual reset event, used for signalling between the threads
        /// </summary>
        private ManualResetEvent _manualResetEvent = new ManualResetEvent( true );

        /// <summary>
        /// C'tor
        /// </summary>
        public MyService( IAmDoingRealWork realWorkService )
        {
            _realWorkService = realWorkService ?? throw new ArgumentNullException( "realWork" );

            _manualResetEvent.Reset(); //Non-signalled
        }

        /// <summary>
        /// On service start
        /// </summary>
        public void StartService()
        {
            Log.Information(  "Service is starting...." );

            _realWorkService.Start( _manualResetEvent );

            Log.Information( "Service has started." );
        }

        /// <summary>
        /// On service stop
        /// </summary>
        public void StopService()
        {
            Log.Information( "Service is stopping...." );

            // set event to signalled
            Log.Information( "Stop is called from SCM, now signalling service to stop..." );
            _manualResetEvent.Set(); 
        }
    }
}
