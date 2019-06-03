using System;

using Topshelf;
using Serilog;
using System.Threading;

namespace TopShelfTemplate
{
    class Program
    {
        /// <summary>
        /// Main entry point of the application
        /// </summary>
        /// <param name="args"></param>
        static void Main( string[] args )
        {
            TopshelfExitCode exitCode;

            try
            {
                string logLocation = ConfigurationHelper.Instance.GetConfigurationValue( "Logging:LogLocation" );

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File( logLocation )
                    .CreateLogger();

                exitCode = ConfigureService.Configure();
            }
            catch( Exception ex)
            {
                Log.Information( ex, "Error in Program.Main()" );
                throw;
            }

            Environment.ExitCode = ( int )Convert.ChangeType( exitCode, exitCode.GetTypeCode() );
        }
    }
}
