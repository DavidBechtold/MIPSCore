using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Util.MIPSEventArgs
{
    public class MIPSEventArgs : EventArgs
    {
        public string Message { get; }

        public MIPSEventArgs(string message)
        { 
            this.Message = message; 
        }
    }
}
