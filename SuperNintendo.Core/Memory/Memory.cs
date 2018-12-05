using System;
using System.IO;

namespace SuperNintendo.Core.Memory
{
    public static class Memory
    {
        public static DirectMemorySetLink FillRAM; //maps to rom
        private static MemorySetLink BWRAM;
        private static MemorySetLink C4RAM;
        private static MemorySetLink OBC1RAM;
        private static MemorySetLink BSRAM;
        private static MemorySetLink BIOSROM;
        private static byte ROMRegion;
        private static byte ROMSpeed;
        private static byte ROMType;
        private static byte ROMSize;
        private static byte SRAMSize = 3;
        private static uint SRAMMask;

        private static MappingData[] Map = new MappingData[Constants.NUM_BLOCKS];
        private static readonly MappingData[] WriteMap = new MappingData[Constants.NUM_BLOCKS];
        private static readonly bool[] BlockIsRAM = new bool[Constants.NUM_BLOCKS];
        private static readonly bool[] BlockIsROM = new bool[Constants.NUM_BLOCKS];
        private static int ROMFramesPerSecond;

        private static bool LoROM;
        private static int CalculatedSize;

        public static Action PostRomInitFunc;

        public static MemorySet RAM = new MemorySet(0x20000);
        public static MemorySet SRAM = new MemorySet(0x20000);
        public static MemorySet VRAM = new MemorySet(0x10000);
        public static MemorySet ROM = new MemorySet(Constants.MAX_ROM_SIZE + 0x200 + 0x8000);

        public static byte OpenBus;

        public static void Init()
        {
            FillRAM = new DirectMemorySetLink(ROM, 0); //is basically a constant just for filling ROM
            ROM.Position += 0x8000;
            C4RAM = new MemorySetLink(ROM, 0x400000 + 8192 * 8);
            OBC1RAM = new MemorySetLink(ROM, 0x400000);
            BIOSROM = new MemorySetLink(ROM, 0x300000);
            BSRAM = new MemorySetLink(ROM, 0x400000);

            //SuperFX.pvRegisterPosition = FillRAMPosition + 0x3000;
            //SuperFX.nRamBanks = 2; // Most only use 1.  1=64KB=512Mb, 2=128KB=1024Mb
            //SuperFX.pvRamPosition = 0; //link to SRAM
            //SuperFX.nRomBanks = 2 * 1024 * 1024 / (32 * 1024);
            //SuperFX.pvRomPosition = 0; //link to ROM

            PostRomInitFunc = null;
        }

        public static void LoadSRAM(string fileName)
        {
            ClearSRAM();
            var size = SRAMSize > 0 ? (1 << (SRAMSize + 3)) * 128 : 0;
            var fileInfo = new System.IO.FileInfo(fileName);
            if (fileInfo.Exists)
            {
                var len = fileInfo.OpenRead().Read(RAM.bytes, 0, 0x20000);
                //if (len - size == 512)
                //    Array.Copy(SRAM, SRAM + 512, size);
            }
        }

        private static void ClearSRAM()
        {
            for (var i = 0; i < 0x20000; i++)
                SRAM.bytes[i] = 96;
        }

        public static IsMemoryLinked GetBasePointer(uint address)
        {
            var addressData = Map[(address & 0xffffff) >> Constants.SHIFT];

            if (addressData.MappingType == null)
                return new MemorySetLink(addressData.MemoryLink, addressData.Position);

            switch (addressData.MappingType)
            {
                case MappingType.MAP_LOROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        return null;

                    return new MemorySetLink(SRAM, (int)(((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask) - (address & 0xffff)));

                case MappingType.MAP_HIROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        return null;

                    return new MemorySetLink(SRAM, (int)((((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask) - (address & 0xffff)));

                case MappingType.MAP_BWRAM:
                    return new MemorySetLink(BWRAM, (int)(-0x6000 - (address & 0x8000)));

                case MappingType.MAP_SA1RAM:
                    return SRAM;

                //case MappingType.MAP_SPC7110_ROM:
                //    return GetBasePointerSPC7110(address);

                //case MappingType.MAP_C4:
                //    return GetBasePointerC4(address & 0xffff);

                //case MappingType.MAP_OBC_RAM:
                //    return GetBasePointerOBC1(address & 0xffff);

                case MappingType.MAP_NONE:
                default:
                    return null;
            }
        }

        public static void LoadROM(Stream stream)
        {
            ROM = new MemorySet((int) (0x8000 + stream.Length));
            stream.Read(ROM.bytes, ROM.Position, (int)stream.Length);
            LoadROM((int)stream.Length);
        }

        public static byte GetByte(uint address)
        {
            var block = (int)((address & 0xffffff) >> Constants.SHIFT);
            var addressData = Map[block];
            var speed = MemorySpeed(address);
            byte data;

            if (addressData.MappingType == null)
            {
                data = ROM[(int)(addressData.Position + (address & 0xffff))];
                AddCyclesInMemoryAccess(speed);
                return data;
            }

            switch (addressData.MappingType)
            {
                //case MappingType.MAP_CPU:
                //    var data = GetCPU(address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_PPU:
                //    if (CPU.CPUState.InDMAorHDMA && (address & 0xff00) == 0x2100)
                //        return OpenBus;

                //    var data2 = GetPPU(address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return data2;

                case MappingType.MAP_LOROM_SRAM:
                case MappingType.MAP_SA1RAM:
                    // Address & 0x7fff   : offset into bank
                    // Address & 0xff0000 : bank
                    // bank >> 1 | offset : SRAM address, unbound
                    // unbound & SRAMMask : SRAM offset
                    data = SRAM[(int) ((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)];
                    AddCyclesInMemoryAccess(speed);
                    return data;

                case MappingType.MAP_HIROM_SRAM:
                case MappingType.MAP_RONLY_SRAM:
                    data = SRAM[(int) (((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)];
                    AddCyclesInMemoryAccess(speed);
                    return data;

                //case MappingType.MAP_BWRAM:
                //    data = *(Memory.BWRAMPosition + ((address & 0x7fff) - 0x6000));
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_DSP:
                //    data = GetDSP(address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_SPC7110_ROM:
                //    data = GetSPC7110Byte(address);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_SPC7110_DRAM:
                //    data = GetSPC7110(0x4800);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_C4:
                //    data = GetC4(address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_OBC_RAM:
                //    data = GetOBC1(address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_SETA_DSP:
                //    data = GetSetaDSP(address);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_SETA_RISC:
                //    data = GetST018(address);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                //case MappingType.MAP_BSX:
                //    data = GetBSX(address);
                //    AddCyclesInMemoryAccess(speed);
                //    return data;

                case MappingType.MAP_NONE:
                default:
                    data = OpenBus;
                    AddCyclesInMemoryAccess(speed);
                    return data;
            }
        }

        public static ushort GetWord(uint address, WrapType w = WrapType.WRAP_NONE)
        {
            ushort word;

            var mask = (uint)(Constants.MASK & (w == WrapType.WRAP_PAGE ? 0xff : (w == WrapType.WRAP_BANK ? 0xffff : 0xffffff)));
            if ((address & mask) == mask)
            {
                word = OpenBus = GetByte(address);

                switch (w)
                {
                    case WrapType.WRAP_PAGE:
                        CPU.Registers.PC.xPBPC = address;
                        CPU.Registers.PCl++;
                        return (ushort)(word | (GetByte(CPU.Registers.PC.xPBPC) << 8));

                    case WrapType.WRAP_BANK:
                        CPU.Registers.PC.xPBPC = address;
                        CPU.Registers.PCw++;
                        return (ushort)(word | (GetByte(CPU.Registers.PC.xPBPC) << 8));

                    case WrapType.WRAP_NONE:
                    default:
                        return (ushort)(word | (GetByte(address + 1) << 8));
                }
            }

            var block = (int)(address & 0xffffff) >> Constants.SHIFT;
            var GetAddressData = Map[block];
            var speed = MemorySpeed(address);

            if (GetAddressData.MappingType == null)
            {
                word = (ushort)(ROM[(int)(GetAddressData.Position + (address & 0xffff))] | 
                    ROM[(int)(GetAddressData.Position + ((address + 1) & 0xffff))] << 8);

                AddCyclesInMemoryAccess_x2(speed);
                return word;
            }

            switch (GetAddressData.MappingType)
            {
                //case MappingType.MAP_CPU:
                // word  = GetCPU(address & 0xffff);
                //          AddCyclesInMemoryAccess(speed);
                // word |= GetCPU((address + 1) & 0xffff) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_PPU:
                // if (CPU.CPUState.InDMAorHDMA)
                // {
                //  word = OpenBus = GetByte(address);
                //  return (ushort) (word | (GetByte(address + 1) << 8));
                // }

                // word  = GetPPU(address & 0xffff);
                //          AddCyclesInMemoryAccess(speed);
                //          word |= GetPPU((address + 1) & 0xffff) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                case MappingType.MAP_LOROM_SRAM:
                case MappingType.MAP_SA1RAM:
                    if (SRAMMask >= Constants.MASK)
                        word = SRAM[(int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)];
                    else
                        word = (ushort)(SRAM[(int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)] |
                            (SRAM[(int)(((((address + 1) & 0xff0000) >> 1) | ((address + 1) & 0x7fff)) & SRAMMask)] << 8));
                    AddCyclesInMemoryAccess_x2(speed);
                    return word;

                case MappingType.MAP_HIROM_SRAM:
                case MappingType.MAP_RONLY_SRAM:
                    if (SRAMMask >= Constants.MASK)
                        word = SRAM[(int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)];
                    else
                        word = (ushort)(SRAM[(int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)] |
                                (SRAM[(int)((((address + 1) & 0x7fff) - 0x6000 + (((address + 1) & 0xf0000) >> 3)) & SRAMMask)] << 8));
                    AddCyclesInMemoryAccess_x2(speed);
                    return word;

                //case MappingType.MAP_BWRAM:
                // word = Memory.BWRAM + ((address & 0x7fff) - 0x6000);
                //          AddCyclesInMemoryAccess_x2(speed);
                //          return word;

                //case MappingType.MAP_DSP:
                // word  = GetDSP(address & 0xffff);
                //          AddCyclesInMemoryAccess(speed);
                //          word |= GetDSP((address + 1) & 0xffff) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_SPC7110_ROM:
                //     word  = GetSPC7110Byte(address);
                //              AddCyclesInMemoryAccess(speed);
                // word |= GetSPC7110Byte(address + 1) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_SPC7110_DRAM:
                // word  = GetSPC7110(0x4800);
                //          AddCyclesInMemoryAccess(speed);
                // word |= GetSPC7110(0x4800) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_C4:
                // word  = GetC4(address & 0xffff);
                //          AddCyclesInMemoryAccess(speed);
                //          word |= GetC4((address + 1) & 0xffff) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_OBC_RAM:
                // word = GetOBC1(address & 0xffff);
                //          AddCyclesInMemoryAccess(speed);
                //          word |= GetOBC1((address + 1) & 0xffff) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_SETA_DSP:
                // word = GetSetaDSP(address);
                //          AddCyclesInMemoryAccess(speed);
                //          word |= GetSetaDSP(address + 1) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_SETA_RISC:
                // word = GetST018(address);
                //          AddCyclesInMemoryAccess(speed);
                // word |= GetST018(address + 1) << 8;
                //          AddCyclesInMemoryAccess(speed);
                //          return word;

                //case MappingType.MAP_BSX:
                // word = GetBSX(address);
                //          AddCyclesInMemoryAccess(speed);
                // word |= GetBSX(address + 1) << 8;
                // AddCyclesInMemoryAccess(speed);
                // return word;

                case MappingType.MAP_NONE:
                default:
                    word = (ushort)(OpenBus | (OpenBus << 8));
                    AddCyclesInMemoryAccess_x2(speed);
                    return word;
            }
        }

        public static void SetByte(byte data, uint address)
        {
            var block = (int)(address & 0xffffff) >> Constants.SHIFT;
            var SetAddressData = WriteMap[block];
            int speed = MemorySpeed(address);

            if (SetAddressData.MappingType == null)
            {
                ROM[(int)(SetAddressData.Position + (address & 0xffff))] = data;
                AddCyclesInMemoryAccess(speed);
                return;
            }

            switch (SetAddressData.MappingType)
            {
                //case MappingType.MAP_CPU:
                //    SetCPU(data, address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_PPU:
                //    if (CPU.CPUState.InDMAorHDMA && (address & 0xff00) == 0x2100)
                //        return;

                //    SetPPU(data, address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                case MappingType.MAP_LOROM_SRAM:
                    if (SRAMMask > 0)
                    {
                        SRAM[(int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)] = data;
                        CPU.CPUState.SRAMModified = true;
                    }
                    AddCyclesInMemoryAccess(speed);
                    return;

                //case MappingType.MAP_LOROM_SRAM_B:
                //    if (Multi.sramMaskB)
                //    {
                //        *(Multi.sramB + ((((Address & 0xff0000) >> 1) | (Address & 0x7fff)) & Multi.sramMaskB)) = Byte;
                //        CPU.SRAMModified = TRUE;
                //    }

                //    AddCyclesInMemoryAccess(speed);
                //    return;

                case MappingType.MAP_HIROM_SRAM:
                    if (SRAMMask > 0)
                    {
                        SRAM[(int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)] = data;
                        CPU.CPUState.SRAMModified = true;
                    }
                    AddCyclesInMemoryAccess(speed);
                    return;

                case MappingType.MAP_BWRAM:
                    BWRAM[(int)((address & 0x7fff) - 0x6000)] = data;
                    CPU.CPUState.SRAMModified = true;
                    AddCyclesInMemoryAccess(speed);
                    return;

                case MappingType.MAP_SA1RAM:
                    SRAM[(int) (address & 0xffff)] = data;
                    AddCyclesInMemoryAccess(speed);
                    return;

                //case MappingType.MAP_DSP:
                //    SetDSP(data, address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_C4:
                //    SetC4(data, address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_OBC_RAM:
                //    SetOBC1(data, address & 0xffff);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_SETA_DSP:
                //    SetSetaDSP(data, address);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_SETA_RISC:
                //    SetST018(data, address);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                //case MappingType.MAP_BSX:
                //    SetBSX(data, address);
                //    AddCyclesInMemoryAccess(speed);
                //    return;

                case MappingType.MAP_NONE:
                default:
                    AddCyclesInMemoryAccess(speed);
                    return;
            }
        }

        public static void SetWord(ushort word, uint address, WrapType w = WrapType.WRAP_NONE, WriteOrder o = WriteOrder.WRITE_01)
        {
            var mask = Constants.MASK & (w == WrapType.WRAP_PAGE ? 0xff : (w == WrapType.WRAP_BANK ? 0xffff : 0xffffff));
            if ((address & mask) == mask)
            {
                if (o == WriteOrder.WRITE_01)
                    SetByte((byte)word, address);

                switch (w)
                {
                    case WrapType.WRAP_PAGE:
                        CPU.Registers.PC.xPBPC = address;
                        CPU.Registers.PCl++;
                        SetByte((byte)(word >> 8), CPU.Registers.PC.xPBPC);
                        break;

                    case WrapType.WRAP_BANK:
                        CPU.Registers.PC.xPBPC = address;
                        CPU.Registers.PCw++;
                        SetByte((byte)(word >> 8), CPU.Registers.PC.xPBPC);
                        break;

                    case WrapType.WRAP_NONE:
                    default:
                        SetByte((byte)(word >> 8), address + 1);
                        break;
                }

                if (o == WriteOrder.WRITE_10)
                    SetByte((byte)word, address);

                return;
            }

            int block = (int)((address & 0xffffff) >> Constants.SHIFT);
            var SetAddressData = WriteMap[block];
            int speed = MemorySpeed(address);

            if (SetAddressData.MappingType == null)
            {
                SetAddressData.MemoryLink[(int)(SetAddressData.Position + (address & 0xffff))] = (byte)(word >> 8);
                SetAddressData.MemoryLink[(int)(SetAddressData.Position + ((address + 1) & 0xffff))] = (byte)word;
                AddCyclesInMemoryAccess_x2(speed);
                return;
            }

            switch (SetAddressData.MappingType)
            {
                //case MappingType.MAP_CPU:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetCPU(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  SetCPU((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetCPU((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //              SetCPU(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //              return;
                // }

                case MappingType.MAP_PPU:
                //if (CPU.CPUState.InDMAorHDMA)
                //{
                // if ((address & 0xff00) != 0x2100)
                //  SetPPU((byte) word, address & 0xffff);
                // if (((address + 1) & 0xff00) != 0x2100)
                //  SetPPU(word >> 8, (address + 1) & 0xffff);
                // return;
                //}

                //if (o == WriteOrder.WRITE_10)
                //{
                // SetPPU(word >> 8, (address + 1) & 0xffff);
                // AddCyclesInMemoryAccess(speed);
                // SetPPU((byte) word, address & 0xffff);
                //             AddCyclesInMemoryAccess(speed);
                //             return;
                //}
                //else
                //{
                // SetPPU((byte) word, address & 0xffff);
                //             AddCyclesInMemoryAccess(speed);
                //             SetPPU(word >> 8, (address + 1) & 0xffff);
                // AddCyclesInMemoryAccess(speed);
                //             return;
                //}

                case MappingType.MAP_LOROM_SRAM:
                    if (SRAMMask > 0)
                    {
                        if (SRAMMask >= Constants.MASK)
                        {
                            SRAM[(int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)] = (byte)(word >> 8);
                            SRAM[(int)(((((address + 1) & 0xff0000) >> 1) | ((address + 1) & 0x7fff)) & SRAMMask)] = (byte)word;
                        }
                        else
                        {

                            SRAM[(int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask)] = (byte)word;
                            SRAM[(int)(((((address + 1) & 0xff0000) >> 1) | ((address + 1) & 0x7fff)) & SRAMMask)] = (byte)(word >> 8);
                        }

                        CPU.CPUState.SRAMModified = true;
                    }

                    AddCyclesInMemoryAccess_x2(speed);
                    return;

                case MappingType.MAP_HIROM_SRAM:
                    if (SRAMMask > 0)
                    {
                        if (SRAMMask >= Constants.MASK)
                        {
                            SRAM[(int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)] = (byte)(word >> 8);
                            SRAM[(int)((((address + 1) & 0x7fff) - 0x6000 + (((address + 1) & 0xf0000) >> 3)) & SRAMMask)] = (byte)word;
                        }
                        else
                        {
                            SRAM[(int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask)] = (byte)word;
                            SRAM[(int)((((address + 1) & 0x7fff) - 0x6000 + (((address + 1) & 0xf0000) >> 3)) & SRAMMask)] = (byte)(word >> 8);
                        }
                        CPU.CPUState.SRAMModified = true;
                    }

                    AddCyclesInMemoryAccess_x2(speed);
                    return;

                case MappingType.MAP_BWRAM:
                    BWRAM[(int) (((address & 0x7fff) - 0x6000))] = (byte)(word >> 8);
                    BWRAM[(int) ((((address + 1) & 0x7fff) - 0x6000))] = (byte)word;
                    CPU.CPUState.SRAMModified = true;
                    AddCyclesInMemoryAccess_x2(2);
                    return;

                case MappingType.MAP_SA1RAM:
                    SRAM[(int)(address & 0xffff)] = (byte)(word >> 8);
                    SRAM[(int)((address + 1) & 0xffff)] = (byte) word;
                    AddCyclesInMemoryAccess_x2(speed);
                    return;

                //case MappingType.MAP_DSP:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetDSP(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  SetDSP((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetDSP((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  SetDSP(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                //case MappingType.MAP_C4:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetC4(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  SetC4((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetC4((byte)word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  SetC4(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                //case MappingType.MAP_OBC_RAM:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetOBC1(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  SetOBC1((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetOBC1((byte) word, address & 0xffff);
                //              AddCyclesInMemoryAccess(speed);
                //  SetOBC1(word >> 8, (address + 1) & 0xffff);
                //  AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                //case MappingType.MAP_SETA_DSP:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetSetaDSP(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  SetSetaDSP((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetSetaDSP((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  SetSetaDSP(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                //case MappingType.MAP_SETA_RISC:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetST018(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  SetST018((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetST018((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  SetST018(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                //case MappingType.MAP_BSX:
                // if (o == WriteOrder.WRITE_10)
                // {
                //  SetBSX(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  SetBSX((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }
                // else
                // {
                //  SetBSX((byte) word, address);
                //              AddCyclesInMemoryAccess(speed);
                //  SetBSX(word >> 8, address + 1);
                //              AddCyclesInMemoryAccess(speed);
                //  return;
                // }

                case MappingType.MAP_NONE:
                default:
                    AddCyclesInMemoryAccess_x2(speed);
                    return;
            }
        }

        public static void SetPCBase(uint address)
        {
            CPU.Registers.PBPC = address & 0xffffff;
            CPU.ICPU.ShiftedPB = address & 0xff0000;

            var addressData = Map[(int)((address & 0xffffff) >> Constants.SHIFT)];

            CPU.CPUState.MemSpeed = MemorySpeed(address);
            CPU.CPUState.MemSpeedx2 = CPU.CPUState.MemSpeed << 1;

            if (addressData.MappingType == null)
            {
                CPU.CPUState.PCBase = addressData;
                return;
            }

            switch (addressData.MappingType)
            {
                case MappingType.MAP_LOROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        CPU.CPUState.PCBase = null;
                    else
                        CPU.CPUState.PCBase = new MappingData { MemoryLink = SRAM, Position = (int)(((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask) - (address & 0xffff)) };
                    return;

                case MappingType.MAP_HIROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        CPU.CPUState.PCBase = null;
                    else
                        CPU.CPUState.PCBase = new MappingData { MemoryLink = SRAM, Position = (int)((((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask) - (address & 0xffff)) };
                    return;

                case MappingType.MAP_BWRAM:
                    CPU.CPUState.PCBase = new MappingData { MemoryLink = BWRAM, Position = (int)(-0x6000 - (address & 0x8000)) };
                    return;

                case MappingType.MAP_SA1RAM:
                    CPU.CPUState.PCBase = new MappingData { MemoryLink = SRAM, Position = 0 };
                    return;

                //case MappingType.MAP_SPC7110_ROM:
                //    CPU.CPUState.PCBasePosition = GetBasePointerSPC7110(address);
                //    return;

                //case MappingType.MAP_C4:
                //    CPU.CPUState.PCBasePosition = GetBasePointerC4(address & 0xffff);
                //    return;

                //case MappingType.MAP_OBC_RAM:
                //    CPU.CPUState.PCBasePosition = GetBasePointerOBC1(address & 0xffff);
                //    return;

                //case MappingType.MAP_BSX:
                //    CPU.CPUState.PCBasePosition = GetBasePointerBSX(address);
                //    return;

                case MappingType.MAP_NONE:
                default:
                    CPU.CPUState.PCBase = null;
                    return;
            }
        }

        private static void LoadROM(int ROMfillSize)
        {
            Settings.DisplayColor = PPU.SPPU.BuildPixel(31, 31, 31);
            PPU.SPPU.SetUIColor(255, 255, 255);

            CalculatedSize = 0;

            var hi_score = ScoreHiROM(false);
            var lo_score = ScoreLoROM(false);
            var score_nonheadered = Math.Max(hi_score, lo_score);
            var score_headered = Math.Max(ScoreHiROM(true), ScoreLoROM(true));

            var size_is_likely_headered = ((ROMfillSize - 512) & 0xFFFF) == 0;
            if (size_is_likely_headered)
                score_headered += 2;
            else
                score_headered -= 2;

            if (First512BytesCountZeroes() >= 0x1E0)
                score_headered += 2;
            else
                score_headered -= 2;

            var headered_score_highest = score_headered > score_nonheadered;

            CalculatedSize = (ROMfillSize / 0x2000) * 0x2000;

            // CalculatedSize is now set, so rescore
            hi_score = ScoreHiROM(false);
            lo_score = ScoreLoROM(false);

            if (lo_score >= hi_score)
                LoROM = true;
            else
                LoROM = false;

            InitROM();
            CPU.CPU.Reset();
        }

        private static int ScoreLoROM(bool skip_header)
        {
            var position = 0x7f00 + (skip_header ? 0x200 : 0);
            var score = 0;

            if ((ROM[position + 0xd5] & 0x1) == 0)
                score += 3;

            // Mode23 is SA-1
            if (ROM[position + 0xd5] == 0x23)
                score += 2;

            if (ROM[position + 0xdc] + (ROM[position + 0xdd] << 8) + ROM[position + 0xde] + (ROM[position + 0xdf] << 8) == 0xffff)
            {
                score += 2;
                if (0 != (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)))
                    score++;
            }

            if (ROM[position + 0xda] == 0x33)
                score += 2;

            if ((ROM[position + 0xd5] & 0xf) < 4)
                score += 2;

            if ((ROM[position + 0xfd] & 0x80) == 0)
                score -= 6;

            if ((ROM[position + 0xfc] + (ROM[position + 0xfd] << 8)) > 0xffb0)
                score -= 2; // reduced per Cowering suggestion

            if (CalculatedSize <= 1024 * 1024 * 16)
                score += 2;

            if ((1 << (ROM[position + 0xd7] - 7)) > 48)
                score -= 1;

            if (!AllASCII(position + 0xb0, 6))
                score -= 1;

            if (!AllASCII(position + 0xc0, Constants.ROM_NAME_LEN - 1))
                score -= 1;

            return score;
        }

        private static int ScoreHiROM(bool skip_header)
        {
            var position = 0xff00 + (skip_header ? 0x200 : 0);
            var score = 0;

            if ((ROM[position + 0xd5] & 0x1) > 0)
                score += 2;

            // Mode23 is SA-1
            if (ROM[position + 0xd5] == 0x23)
                score -= 2;

            if (ROM[position + 0xd4] == 0x20)
                score += 2;

            if (ROM[position + 0xdc] + (ROM[position + 0xdd] << 8) + ROM[position + 0xde] + (ROM[position + 0xdf] << 8) == 0xffff)
            {
                score += 2;
                if (0 != (ROM[position + 0xde] + (ROM[position + 0xdf] << 8)))
                    score++;
            }

            if (ROM[position + 0xda] == 0x33)
                score += 2;

            if ((ROM[position + 0xd5] & 0xf) < 4)
                score += 2;

            if ((ROM[position + 0xfd] & 0x80) == 0)
                score -= 6;

            if ((ROM[position + 0xfc] + (ROM[position + 0xfd] << 8)) > 0xffb0)
                score -= 2; // reduced after looking at a scan by Cowering

            if (CalculatedSize > 1024 * 1024 * 3)
                score += 4;

            if ((1 << (ROM[position + 0xd7] - 7)) > 48)
                score -= 1;

            if (!AllASCII(position + 0xb0, 6))
                score -= 1;

            if (!AllASCII(position + 0xc0, Constants.ROM_NAME_LEN - 1))
                score -= 1;

            return score;
        }

        private static bool AllASCII(int position, int length)
        {
            for (var i = 0; i < length; i++)
                if (ROM[position + i] < 32 || ROM[position + i] > 126)
                    return false;

            return true;
        }

        private static int First512BytesCountZeroes()
        {
            var position = 0;
            var zeroCount = 0;
            for (var i = 0; i < 512; i++)
                if (ROM[position + i] == 0)
                    zeroCount++;

            return zeroCount;
        }

        private static void InitBSX()
        {
            Settings.BSXItself = false;
            Settings.BS = false;
        }

        private static void ParseSNESHeader(int position)
        {
            ROMSize = ROM[position + 0x27];
            SRAMSize = ROM[position + 0x28];
            ROMSpeed = ROM[position + 0x25];
            ROMType = ROM[position + 0x26];
            ROMRegion = ROM[position + 0x29];
        }

        private static void MapInitialize()
        {
            for (var c = 0; c < 0x1000; c++)
            {
                Map[c] = new MappingData { MappingType = MappingType.MAP_NONE };
                WriteMap[c] = new MappingData { MappingType = MappingType.MAP_NONE };
                BlockIsROM[c] = false;
                BlockIsRAM[c] = false;
            }
        }

        private static void MapSpace(uint bank_s, uint bank_e, uint addr_s, uint addr_e, MappingData data)
        {
            for (var c = bank_s; c <= bank_e; c++)
            {
                for (var i = addr_s; i <= addr_e; i += 0x1000)
                {
                    var p = (c << 4) | (i >> 12);
                    Map[p] = data;
                    BlockIsROM[p] = false;
                    BlockIsRAM[p] = true;
                }
            }
        }

        private static void MapIndex(uint bank_s, uint bank_e, uint addr_s, uint addr_e, MappingType index, MapType type)
        {
            var isROM = ((type == MapType.MAP_TYPE_I_O) || (type == MapType.MAP_TYPE_RAM)) ? false : true;
            var isRAM = ((type == MapType.MAP_TYPE_I_O) || (type == MapType.MAP_TYPE_ROM)) ? false : true;

            for (var c = bank_s; c <= bank_e; c++)
            {
                for (var i = addr_s; i <= addr_e; i += 0x1000)
                {
                    var p = (c << 4) | (i >> 12);
                    Map[p] = new MappingData { MappingType = index };
                    BlockIsROM[p] = isROM;
                    BlockIsRAM[p] = isRAM;
                }
            }
        }

        private static void MapWRAM()
        {
            // will overwrite others
            MapSpace(0x7e, 0x7e, 0x0000, 0xffff, new MappingData { MemoryLink = RAM, Position = 0, MappingType = MappingType.MAP_RAM });
            MapSpace(0x7f, 0x7f, 0x0000, 0xffff, new MappingData { MemoryLink = RAM, Position = 0x10000, MappingType = MappingType.MAP_RAM });
        }

        private static void MapSystem()
        {
            // will be overwritten
            MapSpace(0x00, 0x3f, 0x0000, 0x1fff, new MappingData { MemoryLink = RAM, Position = 0, MappingType = MappingType.MAP_RAM });
            MapIndex(0x00, 0x3f, 0x2000, 0x3fff, MappingType.MAP_PPU, MapType.MAP_TYPE_I_O);
            MapIndex(0x00, 0x3f, 0x4000, 0x5fff, MappingType.MAP_CPU, MapType.MAP_TYPE_I_O);
            MapSpace(0x80, 0xbf, 0x0000, 0x1fff, new MappingData { MemoryLink = RAM, Position = 0, MappingType = MappingType.MAP_RAM });
            MapIndex(0x80, 0xbf, 0x2000, 0x3fff, MappingType.MAP_PPU, MapType.MAP_TYPE_I_O);
            MapIndex(0x80, 0xbf, 0x4000, 0x5fff, MappingType.MAP_CPU, MapType.MAP_TYPE_I_O);
        }

        private static uint MapMirror(uint size, uint pos)
        {
            // from bsnes
            if (size == 0)
                return 0;

            if (pos < size)
                return pos;

            var mask = (uint) 1 << 31;
            while ((pos & mask) == 0)
                mask >>= 1;

            if (size <= (pos & mask))
                return MapMirror(size, pos - mask);
            else
                return mask + MapMirror(size - mask, pos - mask);
        }

        private static void MapLoROM(uint bank_s, uint bank_e, uint addr_s, uint addr_e, uint size)
        {
            for (var c = bank_s; c <= bank_e; c++)
            {
                for (var i = addr_s; i <= addr_e; i += 0x1000)
                {
                    var p = (c << 4) | (i >> 12);
                    var addr = (c & 0x7f) * 0x8000;
                    Map[p] = new MappingData { MemoryLink = ROM, Position = (int) (MapMirror(size, addr) - (i & 0x8000)) };
                    BlockIsROM[p] = true;
                    BlockIsRAM[p] = true;
                }
            }
        }

        private static void MapLoROMSRAM()
        {
            uint hi;

            if (SRAMSize == 0)
                return;

            if (ROMSize > 11 || SRAMSize > 5)
                hi = 0x7fff;
            else
                hi = 0xffff;

            MapIndex(0x70, 0x7d, 0x0000, hi, MappingType.MAP_LOROM_SRAM, MapType.MAP_TYPE_RAM);
            MapIndex(0xf0, 0xff, 0x0000, hi, MappingType.MAP_LOROM_SRAM, MapType.MAP_TYPE_RAM);
        }

        private static void MapWriteProtectROM()
        {
            for (var i = 0; i < Map.Length; i++)
                WriteMap[i] = Map[i];

            for (int c = 0; c < 0x1000; c++)
                if (BlockIsROM[c])
                    WriteMap[c] = new MappingData { MappingType = MappingType.MAP_NONE };
        }

        private static void MapLoROMMap()
        {
            MapSystem();

            MapLoROM(0x00, 0x3f, 0x8000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0x40, 0x7f, 0x0000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0x80, 0xbf, 0x8000, 0xffff, (uint) CalculatedSize);
            MapLoROM(0xc0, 0xff, 0x0000, 0xffff, (uint) CalculatedSize);
             
            MapLoROMSRAM();
            MapWRAM();

            MapWriteProtectROM();
        }

        private static void InitROM()
        {
            Settings.SuperFX = false;
            Settings.DSP = 0;
            Settings.SA1 = false;
            Settings.C4 = false;
            Settings.SDD1 = false;
            Settings.SPC7110 = false;
            Settings.SPC7110RTC = false;
            Settings.OBC1 = false;
            Settings.SETA = 0;
            Settings.SRTC = false;
            Settings.BS = false;
            Settings.MSU1 = false;

            //SuperFX.nRomBanks = CalculatedSize >> 15;

            //// Parse ROM header and read ROM informatoin
            InitBSX(); // Set BS header before parsing

            var romHeaderPosition = 0x7FB0;
            ParseSNESHeader(romHeaderPosition);

            //// Detect and initialize chips
            //// detection codes are compatible with NSRT

            // DSP1/2/3/4
            DSP.DSP.SetDSP = null;
            DSP.DSP.GetDSP = null;

            // MSU1
            Settings.MSU1 = false;

            //// Map memory and calculate checksum

            MapInitialize();
            MapLoROMMap();

            Settings.PAL = false;
            Settings.FrameTime = Settings.FrameTimeNTSC;
            ROMFramesPerSecond = 60;

            // SRAM size
            SRAMMask = SRAMSize > 0 ? (uint)((1 << (SRAMSize + 3)) * 128) - 1 : 0;

            //// Initialize emulation

            Timings.Timings.H_Max_Master = Constants.SNES_CYCLES_PER_SCANLINE;
            Timings.Timings.H_Max = Timings.Timings.H_Max_Master;
            Timings.Timings.HBlankStart = Constants.SNES_HBLANK_START_HC;
            Timings.Timings.HBlankEnd = Constants.SNES_HBLANK_END_HC;
            Timings.Timings.HDMAInit = Constants.SNES_HDMA_INIT_HC;
            Timings.Timings.HDMAStart = Constants.SNES_HDMA_START_HC;
            Timings.Timings.RenderPos = Constants.SNES_RENDER_START_HC;
            Timings.Timings.V_Max_Master = Constants.SNES_MAX_NTSC_VCOUNTER;
            Timings.Timings.V_Max = Timings.Timings.V_Max_Master;
            Timings.Timings.DMACPUSync = 18;
            /* If the CPU is halted (i.e. for DMA) while /NMI goes low, the NMI will trigger
               after the DMA completes (even if /NMI goes high again before the DMA
               completes). In this case, there is a 24-30 cycle delay between the end of DMA
               and the NMI handler, time enough for an instruction or two. */
            // Wild Guns, Mighty Morphin Power Rangers - The Fighting Edition
            Timings.Timings.NMIDMADelay = 24;

            PPU.IPPU.TotalEmulatedFrames = 0;

            PostRomInitFunc?.Invoke();
        }

        private static void AddCyclesInMemoryAccess(int speed)
        {
            if (!CPU.CPUState.InDMAorHDMA)
            {
                CPU.CPUState.Cycles += speed;
                while (CPU.CPUState.Cycles >= CPU.CPUState.NextEvent)
                    CPU.CPU.DoHEventProcessing();
            }
        }

        private static void AddCyclesInMemoryAccess_x2(int speed)
        {
            if (!CPU.CPUState.InDMAorHDMA)
            {
                CPU.CPUState.Cycles += speed << 1;
                while (CPU.CPUState.Cycles >= CPU.CPUState.NextEvent)
                    CPU.CPU.DoHEventProcessing();
            }
        }

        private static int MemorySpeed(uint address)
        {
            if ((address & 0x408000) > 0)
            {
                if ((address & 0x800000) > 0)
                    return CPU.CPUState.FastROMSpeed;

                return CPU.Constants.SLOW_ONE_CYCLE;
            }

            if (((address + 0x6000) & 0x4000) > 0)
                return CPU.Constants.SLOW_ONE_CYCLE;

            if (((address - 0x4000) & 0x7e00) > 0)
                return CPU.Constants.ONE_CYCLE;

            return CPU.Constants.TWO_CYCLES;
        }

        private static IsMemoryLinked GetMemPointer(uint address)
        {
            var GetAddressData = Map[(address & 0xffffff) >> Constants.SHIFT];

            if (GetAddressData.MappingType == null)
                return new MemorySetLink(GetAddressData.MemoryLink, (int)(GetAddressData.Position + (address & 0xffff)));

            switch (GetAddressData.MappingType)
            {
                case MappingType.MAP_LOROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        return null;

                    return new MemorySetLink(SRAM, (int)((((address & 0xff0000) >> 1) | (address & 0x7fff)) & SRAMMask));

                case MappingType.MAP_HIROM_SRAM:
                    if ((SRAMMask & Constants.MASK) != Constants.MASK)
                        return null;

                    return new MemorySetLink(SRAM, (int)(((address & 0x7fff) - 0x6000 + ((address & 0xf0000) >> 3)) & SRAMMask));

                case MappingType.MAP_BWRAM:
                    return new MemorySetLink(BWRAM, (int)(-0x6000 + (address & 0x7fff)));

                case MappingType.MAP_SA1RAM:
                    return new MemorySetLink(SRAM, (int)address & 0xffff);

                //case MappingType.MAP_SPC7110_ROM:
                //    return GetBasePointerSPC7110(address) + (address & 0xffff);

                //case MappingType.MAP_C4:
                //    return GetMemPointerC4(address & 0xffff);

                //case MappingType.MAP_OBC_RAM:
                //    return GetMemPointerOBC1(address & 0xffff);

                case MappingType.MAP_NONE:
                default:
                    return null;
            }
        }
    }
}