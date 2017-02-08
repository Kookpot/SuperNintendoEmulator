using System;

namespace SNESFromScratch
{
    public class SPU
    {
        private int _skip, _setZF;
        public C65816 C65816;

        public void WriteSPU(int address, byte value)
        {
            if (address >= 0x2140 && address <= 0x2147)
            {
                _setZF = 0;
            }
        }

        public byte ReadSPU(int address)
        {
            var temp = _skip;
            if (_skip < 18)
            {
                _skip += 1;
            }
            else
            {
                _skip = 0;
            }
            switch (temp >> 1)
            {
                case 0:
                case 1:
                case 6:
                    _setZF = 2;
                    return 0;
                case 2:
                    if ((temp & 1) != 0)
                    {
                        return (byte) ((C65816.Registers.A & 0xFF00)/0x100);
                    }
                    return (byte) (C65816.Registers.A & 0xFF);
                case 3:
                    if ((temp & 1) != 0)
                    {
                        return (byte) ((C65816.Registers.X & 0xFF00)/0x100);
                    }
                    return (byte) (C65816.Registers.X & 0xFF);
                case 4:
                    if ((temp & 1) != 0)
                    {
                        return (byte)((C65816.Registers.Y & 0xFF00) / 0x100);
                    }
                    return (byte) (C65816.Registers.Y & 0xFF);
                case 5:
                case 7:
                    return (byte) ((temp & 1) != 0 ? 0xBB : 0xAA);
                case 8:
                    return 0x33;
                case 9:
                    return 0;
            }
            throw new Exception("test");
        }
    }
}