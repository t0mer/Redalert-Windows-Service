using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedAlert
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception exeption { get; set; }

        public string MethodName { get; set; }
    }
}
