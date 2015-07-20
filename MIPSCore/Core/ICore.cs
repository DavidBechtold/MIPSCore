using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MIPSCore;

namespace MIPSCore
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
        void singleClock();

        /* programms the core with an objdump file 
         * the registers are flushed and the sp gets initialised if it is declared in the config file 
         */
        void programObjdump(string path);

        /* this event is called if an exit systemcall occured */
        event EventHandler completed;

        /* clock performed */
        event EventHandler clocked;

        /* exception event */
        event EventHandler exception;

        string getExceptionString();

        /* get actual Instruction */
        string actualInstruction();

        /* get program counter */
        string programCounter();

        /* get data memory size */
        UInt32 dataMemorySize();

        /* read all register values decimal signed*/
        Int32[] readAllRegisters();
        string toStringAllRegisters();

        /* read registervalue decimal signed */
        Int32 readRegister(UInt16 number);
        string toStringRegister(UInt16 number);

        /*read registervalue decimal unsigned */
        UInt32 readRegisterUnsigned(UInt16 number);
        string toStringRegisterUnsigned(UInt16 number);

        /* read registervalue hex */
        string toStringRegisterHex(UInt16 number);

        /* read control unit signals */
        string readControlUnitSignals();
    }
}
