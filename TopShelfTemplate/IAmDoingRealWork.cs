using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TopShelfTemplate
{
    public interface IAmDoingRealWork
    {
        void Start( ManualResetEvent externalResetEvent );

        void Stop();
    }
}
