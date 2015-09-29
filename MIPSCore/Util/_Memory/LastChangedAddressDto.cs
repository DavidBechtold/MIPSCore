namespace MIPSCore.Util._Memory
{
    public class LastChangedAddressDto
    {
        public bool Changed { get; set; }
        public uint Address { get; set; }

        public LastChangedAddressDto(bool changed, uint address)
        {
            Changed = changed;
            Address = address;
        }
    }
}
