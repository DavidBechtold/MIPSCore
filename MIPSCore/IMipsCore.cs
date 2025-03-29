using System;

namespace MIPSCore
{
    public interface IMipsCore
    {
        /* starts the core */
        void StartCore();

        /* stops the core */
        void StopCore();

        /* start the loaded programm from new */
        void ResetCore();

        /* set the execution mode to 
         * singleStep: core performs clock only if function singleStep is called 
         * runToCompletion: core runs to the exit systemcall
        */
        void SetMode(ExecutionMode executionMode);
        
        /* enables the branch delay slot */
        void SetBranchDelaySlot(bool branchDelay);

        bool GetBranchDelaySlot();

        /* perform one clock in singleStep mode */ 
        void SingleClock();

        /* programms the core with an objdump file 
         * the registers are flushed and the sp gets initialised if it is declared in the config file 
         */
        void ProgramObjdump(string path);

        /* this event is called if an exit systemcall occured */
        event EventHandler Completed;

        /* clock performed */
        event EventHandler Clocked;

        /* exception event */
        event EventHandler Exception;

        string GetExceptionString();

        /* notification event */
        event EventHandler Notification;
        string GetNotificationMessage();

        /* get actual Instruction */
        string ActualInstruction();

        /* get program counter */
        string ProgramCounter();

        /* get data memory size */
        uint DataMemorySizeBytes();

        int ReadWordDataMemory(uint byteAddr);
        uint ReadWordDataMemoryUnsigned(uint byteAddr);

        /* read all register values decimal signed*/
        int[] ReadAllRegisters();
        string ToStringAllRegisters();

        /* read registervalue decimal signed */
        int ReadRegister(ushort number);
        string ToStringRegister(ushort number);

        /*read registervalue decimal unsigned */
        uint ReadRegisterUnsigned(ushort number);
        string ToStringRegisterUnsigned(ushort number);

        /* read registervalue hex */
        string ToStringRegisterHex(ushort number);

        /* read control unit signals */
        string ReadControlUnitSignals();


    }
}
