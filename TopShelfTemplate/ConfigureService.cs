using System;

using Topshelf;

using Autofac;
using Topshelf.Autofac;
using Serilog;

namespace TopShelfTemplate
{
    public class ConfigureService
    {
        internal static TopshelfExitCode Configure( )
        {
            TopshelfExitCode exitCode = TopshelfExitCode.Ok;

            try
            {
                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterType<RealWork>().As<IAmDoingRealWork>();
                containerBuilder.RegisterType<MyService>();

                var container = containerBuilder.Build();

                exitCode = HostFactory.Run( x =>
                {
                    x.UseAutofacContainer( container );

                    x.Service<MyService>( hostSettings =>
                    {
                        hostSettings.ConstructUsing( () => container.Resolve<MyService>() );

                        hostSettings.WhenStarted( s => s.StartService() ); 
                        hostSettings.WhenStopped( s => s.StopService() );
                    } );

                    x.RunAsLocalSystem();
                    x.StartAutomatically();

                    x.SetServiceName( "TopshelfTemplateService" );
                    x.SetDisplayName( "TopshelfTemplateService" );
                    x.SetDescription( "TopshelfTemplateService" );
                } );
            }
            catch ( Exception ex )
            {
                Log.Information( ex, "Error in ConfigureService.Configure()" );
                throw;
            }

            return exitCode;
        }
    }
}
