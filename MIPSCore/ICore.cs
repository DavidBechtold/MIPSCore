using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPSCore.Core
{
    public interface ICore
    {
        /* starts the core */
        void startCore();

        /* stopts the core */
        void stopCore();

        /* set the execution mode to 
         * singleStep: core performs clock only if function singleStep is called 
         * runToCompletion: core runs to the exit systemcall
        */
        void setMode(ExecutionMode mode);

        /* perform one clock in singleStep mode */ 
        void singleStep();

        /* programms the core with an objdump file 
         * the registers are flushed and the sp gets initialised if it is declared in the config file 
         */
        void programObjdump(string path);

        /* this event is called if an exit systemcall occured */
        event EventHandler completed;

        /* clock performed */
        event EventHandler clocked;

        /* get actual Instruction */
        string actualInstruction();

        /* get program counter */
        string programCounter();

        /* read registervalue decimal signed */
        string readRegister(UInt16 number);

        /*read registervalue decimal unsigned */
        string readRegisterUnsigned(UInt16 number);

        /* read registervalue hex */
        string readRegisterHex(UInt16 number);

        /* read control unit signals */
        string readControlUnitSignals();
    }
}
