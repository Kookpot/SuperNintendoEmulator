using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SNESFromScratch
{
    public class IOPort
    {
        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);
        //public static KeyStateInfo GetKeyState(Keys key) {
        //    short keyState = GetKeyState((int)key);
        //    byte[] bits = BitConverter.GetBytes(keyState);
        //    bool toggled = bits[0] > 0, pressed = bits[1] > 0;
        //    return new KeyStateInfo(key, pressed, toggled);
        //}

        private struct DMAChannel
        {
            public byte Control;
            public byte Dest;
            public byte SourceBank;
            public int Source;
            public int Size;
            public byte HDMABank;
        }

        private struct HDMAChannel
        {
            public byte SourceBank;
            public int Source;
            public int Count, Repeat;
            public bool First;
            public byte[] Data;
        }

        private readonly DMAChannel[] _dmaChannels = new DMAChannel[8];
        private readonly HDMAChannel[] _hdmaChannels = new HDMAChannel[8];
        private byte _hdmaEnabled;

        public bool NMIEnable;
        public byte IRQEnable;
        private int _multiplicand, _multiplier, _divisor, _dividend;
        private int _multResult, _divResult;

        public bool ControllerReady;
        private int _controllerReadPosition;
        public int HCount, VCount;
        public bool FastROM;

        public C65816 C65816;

        public void ResetIO()
        {
            Array.Clear(_dmaChannels, 0, _dmaChannels.Length);
            Array.Clear(_hdmaChannels, 0, _hdmaChannels.Length);
            for (var channel = 0; channel <= 7; channel++)
            {
                _hdmaChannels[channel].Data = new byte[4];
            }
            _hdmaEnabled = 0;

            NMIEnable = false;
            IRQEnable = 0;

            C65816.VBlank = false;
            ControllerReady = false;
        }

        private static bool KeyPressed(Keys key)
        {
            return GetKeyState((int) key) < 0;
        }

        public byte ReadIO(int address)
        {
            byte value;
            switch (address)
            {
                case 0x4210:
                    if (C65816.VBlank)
                    {
                        C65816.VBlank = false;
                        return 0x80;
                    }
                    return 0;
                case 0X4211:
                    if (C65816.IRQOcurred)
                    {
                        C65816.IRQOcurred = false;
                        return 0x80;
                    }
                    return 0;
                case 0x4212:
                    value = 0;
                    if (ControllerReady)
                    {
                        value = 0x1;
                    }
                    if (C65816.HBlank)
                    {
                        value = (byte) (value | 0x40);
                    }
                    if (C65816.VBlank)
                    {
                        value = (byte) (value | 0x80);
                    }
                    return value;
                case 0x4016: //Input on CPU Pin 32, connected to gameport 1, pin 4 (JOY1) (1=Low)
                    //Note to Mike: On some games the controller don't work properly when reading
                    //this address (from what I understood, this is latched from 4018/4019)
                    return 0xFF;
                    //Dim Temp As Integer = Controller_Read_Position
                    //Controller_Read_Position = (Controller_Read_Position + 1) And 0xF
                    //Select Case Temp
                    //    Case 0 : If Key_Pressed(Keys.Z) Then Return 1
                    //    Case 1 : If Key_Pressed(Keys.X) Then Return 1
                    //    Case 2 : If Key_Pressed(Keys.Tab) Then Return 1
                    //    Case 3 : If Key_Pressed(Keys.Return) Then Return 1
                    //    Case 4 : If Key_Pressed(Keys.Up) Then Return 1
                    //    Case 5 : If Key_Pressed(Keys.Down) Then Return 1
                    //    Case 6 : If Key_Pressed(Keys.Left) Then Return 1
                    //    Case 7 : If Key_Pressed(Keys.Right) Then Return 1
                    //    Case 8 : If Key_Pressed(Keys.A) Then Return 1
                    //    Case 9 : If Key_Pressed(Keys.S) Then Return 1
                    //    Case 10 : If Key_Pressed(Keys.Q) Then Return 1
                    //    Case 11 : If Key_Pressed(Keys.W) Then Return 1
                    //End Select
                case 0x4218:
                    value = 0;
                    if (KeyPressed(Keys.A))
                    {
                        value = 0x80; //A
                    }
                    if (KeyPressed(Keys.S))
                    {
                        value = (byte) (value | 0x40); //X
                    }
                    if (KeyPressed(Keys.Q))
                    {
                        value = (byte) (value | 0x20); //L
                    }
                    if (KeyPressed(Keys.W))
                    {
                        value = (byte) (value | 0x10); //R
                    }
                    return value;
                case 0x4219:
                    value = 0;
                    if (KeyPressed(Keys.Z))
                    {
                        value = 0x80; //B
                    }
                    if (KeyPressed(Keys.X))
                    {
                        value = (byte) (value | 0x40); //Y
                    }
                    if (KeyPressed(Keys.Tab))
                    {
                        value = (byte) (value | 0x20); //Select
                    }
                    if (KeyPressed(Keys.Return))
                    {
                        value = (byte) (value | 0x10); //Start
                    }
                    if (KeyPressed(Keys.Up))
                    {
                        value = (byte) (value | 0x8);
                    }
                    if (KeyPressed(Keys.Down))
                    {
                        value = (byte) (value | 0x4);
                    }
                    if (KeyPressed(Keys.Left))
                    {
                        value = (byte) (value | 0x2);
                    }
                    if (KeyPressed(Keys.Right))
                    {
                        value = (byte) (value | 0x1);
                    }
                    return value;
                case 0x4214:
                    return (byte) (_divResult & 0xFF);
                case 0x4215:
                    return (byte) ((_divResult & 0xFF00)/0x100);
                case 0x4216:
                    return (byte) (_multResult & 0xFF);
                case 0x4217:
                    return (byte) ((_multResult & 0xFF00)/0x100);
                case 0x4300:
                case 0x4310:
                case 0x4320:
                case 0x4330:
                case 0x4340:
                case 0x4350:
                case 0x4360:
                case 0x4370:
                    return _dmaChannels[address & 0xF0/0x10].Control;
                case 0x4301:
                case 0x4311:
                case 0x4321:
                case 0x4331:
                case 0x4341:
                case 0x4351:
                case 0x4361:
                case 0x4371:
                    return _dmaChannels[(address & 0xF0)/0x10].Dest;
                case 0x4302:
                case 0x4312:
                case 0x4322:
                case 0x4332:
                case 0x4342:
                case 0x4352:
                case 0x4362:
                case 0x4372:
                    return (byte) (_dmaChannels[(address & 0xF0)/0x10].Source & 0xFF);
                case 0x4303:
                case 0x4313:
                case 0x4323:
                case 0x4333:
                case 0x4343:
                case 0x4353:
                case 0x4363:
                case 0x4373:
                    return (byte) ((_dmaChannels[(address & 0xF0)/0x10].Source >> 8) & 0xFF);
                case 0x4304:
                case 0x4314:
                case 0x4324:
                case 0x4334:
                case 0x4344:
                case 0x4354:
                case 0x4364:
                case 0x4374:
                    return _dmaChannels[(address & 0xF0)/0x10].SourceBank;
                case 0x4305:
                case 0x4315:
                case 0x4325:
                case 0x4335:
                case 0x4345:
                case 0x4355:
                case 0x4365:
                case 0x4375:
                    return (byte) (_dmaChannels[(address & 0xF0)/0x10].Size & 0xFF);
                case 0x4306:
                case 0x4316:
                case 0x4326:
                case 0x4336:
                case 0x4346:
                case 0x4356:
                case 0x4366:
                case 0x4376:
                    return (byte) ((_dmaChannels[(address & 0xF0)/0x10].Size >> 8) & 0xFF);
            }
            return 0;
        }

        public void WriteIO(int address, byte value)
        {
            DMAChannel dChannel;
            switch (address)
            {
                case 0x4200:
                    NMIEnable = (value & 0x80) != 0;
                    IRQEnable = (byte) ((value & 0x30)/0x10);
                    if (IRQEnable == 0)
                    {
                        C65816.IRQOcurred = false;
                    }
                    break;
                case 0x4202:
                    _multiplicand = value;
                    break;
                case 0x4203 :
                    _multiplier = value;
                    _multResult = _multiplicand*_multiplier;
                    break;
                case 0x4204:
                    _dividend = value + (_dividend & 0xFF00);
                    break;
                case 0x4205:
                    _dividend = (value*0x100) + (_dividend & 0xFF);
                    break;
                case 0x4206:
                    _divisor = value;
                    if (_dividend == 0 || _divisor == 0)
                    {
                        _divResult = 0xFFFF;
                        _multResult = _dividend;
                    }
                    else
                    {
                        _divResult = _dividend/_divisor;
                        _multResult = _dividend%_divisor;
                    }
                    break;
                case 0x4207:
                    HCount = value + (HCount & 0xFF00);
                    break;
                case 0x4208:
                    HCount = (value*0x100) + (HCount & 0xFF);
                    break;
                case 0x4209:
                    VCount = value + (VCount & 0xFF00);
                    break;
                case 0x420A:
                    VCount = (value*0x100) + (VCount & 0xFF);
                    C65816.IRQOcurred = false;
                    break;
                case 0x420B:
                    for (var channel = 0; channel <= 7; channel++)
                    {
                        if ((value & (1 << channel)) <= 0) continue;
                        var dmaChannel = _dmaChannels[channel];
                        int originalDest = dmaChannel.Dest;

                        if (dmaChannel.Size == 0)
                        {
                            dmaChannel.Size = 0x10000;
                        }
                        while (dmaChannel.Size != 0)
                        {
                            if ((dmaChannel.Control & 0x80) != 0)
                            {
                                C65816.WriteMemory(dmaChannel.SourceBank, dmaChannel.Source, C65816.ReadMemory(0, 0x2100 | dmaChannel.Dest));
                            }
                            else
                            {
                                C65816.WriteMemory(0, 0x2100 | dmaChannel.Dest, C65816.ReadMemory(dmaChannel.SourceBank, dmaChannel.Source));
                            }
                            C65816.Cycles += 1;

                            switch (dmaChannel.Control & 0xF)
                            {
                                case 0:
                                case 2:
                                    if ((dmaChannel.Control & 0x10) != 0)
                                    {
                                        dmaChannel.Source -= 1;
                                    }
                                    else
                                    {
                                        dmaChannel.Source += 1;
                                    }
                                    break;
                                case 1:
                                    if (dmaChannel.Dest == originalDest)
                                    {
                                        dmaChannel.Dest += 1;
                                    }
                                    else
                                    {
                                        dmaChannel.Dest -= 1;
                                    }
                                    if ((dmaChannel.Control & 0x10) != 0)
                                    {
                                        dmaChannel.Source -= 1;
                                    }
                                    else
                                    {
                                        dmaChannel.Source += 1;
                                    }
                                    break;
                                case 9:
                                    if (dmaChannel.Dest == originalDest)
                                    {
                                        dmaChannel.Dest += 1;
                                    }
                                    else
                                    {
                                        dmaChannel.Dest -= 1;
                                    }
                                    break;
                            }
                            dmaChannel.Size -= 1;
                        }
                        _dmaChannels[channel] = dmaChannel;
                    }
                    break;
                case 0x420C:
                    _hdmaEnabled = value;
                    break;
                case 0x420D:
                    FastROM = (value & 0x1) != 0;
                    break;
                case 0x4300:
                case 0x4310:
                case 0x4320:
                case 0x4330:
                case 0x4340:
                case 0x4350:
                case 0x4360:
                case 0x4370:
                    _dmaChannels[(address & 0xF0)/0x10].Control = value;
                    break;
                case 0x4301 :
                case 0x4311 :
                case 0x4321 :
                case 0x4331 :
                case 0x4341 :
                case 0x4351 :
                case 0x4361 :
                case 0x4371 :
                    _dmaChannels[(address & 0xF0)/0x10].Dest = value;
                    break;
                case 0x4302:
                case 0x4312:
                case 0x4322:
                case 0x4332:
                case 0x4342:
                case 0x4352:
                case 0x4362:
                case 0x4372:
                    dChannel = _dmaChannels[(address & 0xF0)/0x10];
                    dChannel.Source = value + (dChannel.Source & 0xFF00);
                    _dmaChannels[(address & 0xF0) / 0x10] = dChannel;
                    break;
                case 0x4303:
                case 0x4313:
                case 0x4323:
                case 0x4333:
                case 0x4343:
                case 0x4353:
                case 0x4363:
                case 0x4373:
                    dChannel = _dmaChannels[(address & 0xF0)/0x10];
                    dChannel.Source = (value*0x100) + (dChannel.Source & 0xFF);
                    _dmaChannels[(address & 0xF0) / 0x10] = dChannel;
                    break;
                case 0x4304:
                case 0x4314:
                case 0x4324:
                case 0x4334:
                case 0x4344:
                case 0x4354:
                case 0x4364:
                case 0x4374:
                    _dmaChannels[(address >> 4) & 7].SourceBank = value;
                    break;
                case 0x4305:
                case 0x4315:
                case 0x4325:
                case 0x4335:
                case 0x4345:
                case 0x4355:
                case 0x4365:
                case 0x4375:
                    dChannel = _dmaChannels[(address & 0xF0) / 0x10];
                    dChannel.Size = value + (dChannel.Size & 0xFF00);
                    _dmaChannels[(address & 0xF0)/0x10] = dChannel;
                    break;
                case 0x4306:
                case 0x4316:
                case 0x4326:
                case 0x4336:
                case 0x4346:
                case 0x4356:
                case 0x4366:
                case 0x4376:
                    dChannel = _dmaChannels[(address & 0xF0) / 0x10];
                    dChannel.Size = (value * 0x100) + (dChannel.Size & 0xFF);
                    _dmaChannels[(address & 0xF0) / 0x10] = dChannel;
                    break;
                case 0x4307:
                case 0x4317:
                case 0x4327:
                case 0x4337:
                case 0x4347:
                case 0x4357:
                case 0x4367:
                case 0x4377:
                    _dmaChannels[(address & 0xF0) / 0x10].HDMABank = value;
                    break;
            }
        }

        public void HBlankDMA(int scanline)
        {
            for (var channel = 0; channel <= 7; channel++)
            {
                var hdmaChannel = _hdmaChannels[channel];
                if (scanline == 0)
                {
                    hdmaChannel.Source = _dmaChannels[channel].Source;
                    hdmaChannel.SourceBank = _dmaChannels[channel].SourceBank;
                    hdmaChannel.Count = 0;
                }
                if ((_hdmaEnabled & (1 << channel)) != 0)
                {
                    if ((hdmaChannel.Count & 0x7F) == 0)
                    {
                        hdmaChannel.Count = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source);
                        hdmaChannel.Source += 1;
                        hdmaChannel.Repeat = hdmaChannel.Count & 0x80;
                        hdmaChannel.Count = hdmaChannel.Count & 0x7F;
                        int address;
                        switch (_dmaChannels[channel].Control & 0x47)
                        {
                            case 0:
                                hdmaChannel.Data[0] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Source += 1;
                                break;
                            case 1:
                            case 2:
                                hdmaChannel.Data[0] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Data[1] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source + 1);
                                hdmaChannel.Source += 2;
                                break;
                            case 3 :
                            case 4 :
                                hdmaChannel.Data[0] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Data[1] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source + 1);
                                hdmaChannel.Data[2] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source + 2);
                                hdmaChannel.Data[3] = C65816.ReadMemory(hdmaChannel.SourceBank, hdmaChannel.Source + 3);
                                hdmaChannel.Source += 4;
                                break;
                            case 0x40 :
                                address = C65816.ReadMemory16(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Data[0] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address);
                                hdmaChannel.Source += 2;
                                break;
                            case 0x41:
                            case 0x42:
                                address = C65816.ReadMemory16(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Data[0] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address);
                                hdmaChannel.Data[1] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address + 1);
                                hdmaChannel.Source += 2;
                                break;
                            case 0x43 :
                            case 0x44 :
                                address = C65816.ReadMemory16(hdmaChannel.SourceBank, hdmaChannel.Source);
                                hdmaChannel.Data[0] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address);
                                hdmaChannel.Data[1] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address + 1);
                                hdmaChannel.Data[2] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address + 2);
                                hdmaChannel.Data[3] = C65816.ReadMemory(_dmaChannels[channel].HDMABank, address + 3);
                                hdmaChannel.Source += 2;
                                break;
                        }
                        hdmaChannel.First = true;
                    }
                    if (hdmaChannel.First || hdmaChannel.Repeat != 0)
                    {
                        hdmaChannel.First = false;
                        switch (_dmaChannels[channel].Control & 0x7)
                        {
                            case 0:
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[0]);
                                break;
                            case 1:
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[0]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 1), hdmaChannel.Data[1]);
                                break;
                            case 2:
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[0]);
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[1]);
                                break;
                            case 3:
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[0]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 1), hdmaChannel.Data[1]);
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[2]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 1), hdmaChannel.Data[3]);
                                break;
                            case 4:
                                C65816.WriteMemory(0, 0x2100 | _dmaChannels[channel].Dest, hdmaChannel.Data[0]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 1), hdmaChannel.Data[1]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 2), hdmaChannel.Data[2]);
                                C65816.WriteMemory(0, 0x2100 | (_dmaChannels[channel].Dest + 3), hdmaChannel.Data[3]);
                                break;
                        }
                    }
                    hdmaChannel.Count -= 1;
                }
                _hdmaChannels[channel] = hdmaChannel;
            }
        }
    }
}