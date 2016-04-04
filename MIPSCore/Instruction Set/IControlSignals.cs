namespace MIPSCore.Instruction_Set
{
    public interface IControlSignals
    {
        string GetInstructionFriendlyName { get; }
        string GetInstructionAssemblerName { get; }
        string GetInstructionExample { get; }
        string GetInstructionMeaning { get; }
        string GetInstructionFormat { get; }
        string GetInstructionOpCode { get; }
        string GetInstructionFunction { get; }

        InstructionFormat InstructionFormat { get; }
        RegisterDestination RegisterDestination { get; }
        AluSource1 AluSource1 { get; }
        AluSource2 AluSource2 { get; }
        AluControl AluControl { get; }
        bool RegisterWrite { get; }
        bool MemoryWrite { get; }
        bool MemoryRead { get; }
        bool MemorySignExtend { get; }
        RegisterFileInput RegisterFileInput { get; }
        ProgramCounterSource ProgramCounterSource { get; }
        DataMemoryWordSize DataMemoryWordSize { get; }
        bool Systemcall { get; }
    }
}