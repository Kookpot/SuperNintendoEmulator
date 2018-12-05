namespace SuperNintendo.Core.DMA
{
    public class SDMA
    {
        public bool ReverseTransfer = true;
        public bool HDMAIndirectAddressing = true;
        public bool UnusedBit43x0 = true;
        public bool AAddressFixed = true;
        public bool AAddressDecrement = true;
        public byte TransferMode = 7;
        public byte BAddress = 0xff;
        public ushort AAddress = 0xffff;
        public byte ABank;
        public ushort DMACount_Or_HDMAIndirectAddress = 0xffff;
        public byte IndirectBank = 0xff;
        public ushort Address;
        public byte Repeat;
        public byte LineCount = 0x7f;
        public byte UnknownByte = 0xff;
        public byte DoTransfer;
    }
}