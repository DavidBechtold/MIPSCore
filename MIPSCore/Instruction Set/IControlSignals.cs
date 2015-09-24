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
        AluSource AluSource { get; }
        AluControl AluControl { get; }
        bool RegisterWrite { get; }
        bool MemoryWrite { get; }
        bool MemoryRead { get; }
        RegisterFileInput RegisterFileInput { get; }
        ProgramCounterSource ProgramCounterSource { get; }
        DataMemoryWordSize DataMemoryWordSize { get; }
        bool Systemcall { get; }
    }
}