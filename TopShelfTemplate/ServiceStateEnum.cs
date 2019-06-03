using System;
using System.Collections.Generic;
using System.Text;

namespace TopShelfTemplate
{
    public enum ServiceStateEnum : int
    {
        Stopped = 1,
        Running,
        Runaway,
        Dead
    }
}
