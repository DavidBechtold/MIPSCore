﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MIPSCore.Core;
using MIPSCore.Clock;

namespace MIPSCore
{
    class MIPSCore
    {
        static void Main(string[] args)
        {
            CCore core = new CCore();

            core.programObjdump("C:\\Users\\david\\Dropbox\\Bachelor Arbeit\\Testcode\\testcode.s");

            core.startCore();
            while (true) ;
        }
    }
}
