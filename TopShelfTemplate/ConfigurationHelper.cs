using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using System.Threading;
using Microsoft.Extensions.Configuration;

namespace TopShelfTemplate
{
    public sealed class ConfigurationHelper
    {
        /// <summary>
        /// Configuration builder
        /// </summary>
        private IConfigurationRoot _configurationRoot;

        /// <summary>
        /// Static instance of this class
        /// </summary>
        private static readonly Lazy<ConfigurationHelper> _instance =
            new Lazy<ConfigurationHelper>( () => new ConfigurationHelper() );

        /// <summary>
        /// Instance accessor
        /// </summary>
        public static ConfigurationHelper Instance => _instance.Value;

        /// <summary>
        /// Object used for locking
        /// </summary>
        private static object _locker = new object();

        /// <summary>
        /// C'tor
        /// </summary>
        private ConfigurationHelper()
        {
            var pathToOriginalExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName( pathToOriginalExe );

            var builder = new ConfigurationBuilder()
                .SetBasePath( pathToContentRoot )
                .AddJsonFile( "appsettings.json", optional: false, reloadOnChange: true );

            _configurationRoot = builder.Build();
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfigurationValue( string key )
        {
            bool acquiredLock = false;

            try
            {
                Monitor.TryEnter( _locker, TimeSpan.FromMilliseconds( 50 ), ref acquiredLock );

                if (acquiredLock)
                {
                    return _configurationRoot[key] ?? null;
                }
                else
                    return null;
            }
            finally
            {
                if (acquiredLock)
                    Monitor.Exit( _locker );
            }
        }
    }
}
