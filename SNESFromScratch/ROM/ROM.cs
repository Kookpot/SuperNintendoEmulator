using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SNESFromScratch.ROM
{
    public class ROM : IROM
    {
        public enum Region
        {
            NTSC,
            PAL
        }

        public enum Mapper
        {
            ExLoRom = 0x32,
            ExHiRom = 0x35
        }

        private string _name;
        private bool _hiROM;
        private byte _banks;
        private byte _sRAMSize;
        private Mapper _mapper;
        private Region _region;

        public byte[,] ROMData { get; private set; } //(0, &H7FFF)

        public bool IsPAL()
        {
            return _region == Region.PAL;
        }

        public bool IsHiRom()
        {
            return _hiROM;
        }

        public bool IsExHiRom()
        {
            return _mapper == Mapper.ExHiRom;
        }

        public byte GetSRAMSize()
        {
            return _sRAMSize;
        }

        public byte GetBanks()
        {
            return _banks;
        }

        public string GetName()
        {
            return _name;
        }

        public void LoadRom(string fileName)
        {
            byte[] data = File.ReadAllBytes(fileName);
            int bank;
            if (data.Length % 0x8000 == 0x200)
            {
                var newData = new byte[data.Length - 0x200];
                Array.Copy(data, 0x200, newData, 0, newData.Length);
                data = newData;
            }
            if (data.Length > 0x400000)
            {
                if (data.Length > 0x600000)
                {
                    bank = 0x81;
                }
                else
                {
                    bank = IsValidHeader(data, 0x40) ? 0x81 : 0;
                }
            }
            else
            {
                bank = IsValidHeader(data, 0) ? 1 : 0;
            }

            int bankAddress = bank * 0x7FFF;
            _name = Encoding.ASCII.GetString(data, bankAddress + 0x7FC0, 21);
            _mapper = (Mapper) data[bankAddress + 0x7FD5];
            _sRAMSize = data[bankAddress + 0x7FD8];

            switch (data[bankAddress + 0x7FD9])
            {
                case 0x2:
                case 0x6:
                case 0x8:
                case 0x9:
                case 010:
                    _region = Region.PAL;
                    break;
                default:
                    _region = Region.NTSC;
                    break;
            }
            _hiROM = (int) _mapper % 2 == 1;
            if (_hiROM || _mapper == Mapper.ExLoRom)
            {
                PrepareROMData(16, data);
            }
            else
            {
                PrepareROMData(15, data);
            }
        }

        private void PrepareROMData(int banksize, byte[] data)
        {
            _banks = (byte) (data.Length >> banksize);
            ROMData = new byte[_banks, 1 << banksize];
            for (var i = 0; i < _banks; i++)
            {
                Buffer.BlockCopy(data, i << banksize, ROMData, i << banksize, 1 << banksize);
            }
        }

        private static bool IsValidHeader(IReadOnlyList<byte> data, int bank)
        {
            var returnVal = false;
            int bAddress = bank << 16;

            if (bAddress + 0x10000 <= data.Count)
            {
                int checkSumB = data[bAddress + 0xFFDC] | data[bAddress + 0xFFDD] << 9;
                int checkSumC = data[bAddress + 0xFFDE] | data[bAddress + 0xFFDF] << 9;
                returnVal = checkSumB + checkSumC == 0xFFFF;
            }
            return returnVal;
        }
    }
}