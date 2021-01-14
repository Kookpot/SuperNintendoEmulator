using SNESFromScratch.SNESSystem;

namespace SNESFromScratch.CentralMemory
{
    public class DMA : IDMA
    {
        private class DMAChannel
        {
            public int Parameters;
            public int PPUAddress;
            public int DMACurrent;
            public int Counter;
            public int HDMAIB;
            public int HDMACurrent;
            public int HDMALine;
            public int Unused;
            public bool Enabled;
        }

        private readonly DMAChannel[] _channels =
        {
            new DMAChannel(), new DMAChannel(), new DMAChannel(), new DMAChannel(), new DMAChannel(), new DMAChannel(),
            new DMAChannel(), new DMAChannel()
        };
        private ISNESSystem _system;

        public void SetSystem(ISNESSystem system)
        {
            _system = system;
        }

        public void SetChannelEnabled(int channel, bool enabled)
        {
            _channels[channel].Enabled = enabled;
        }

        public int Read8(int address)
        {
            if (0x4300 <= address && address <= 0x437F)
            {
                int channel = (address & 0xF0) >> 4;
                DMAChannel currentChannel = _channels[channel];
                switch (address & 0xF)
                {
                    case 0x0:
                        return currentChannel.Parameters;
                    case 0x1:
                        return currentChannel.PPUAddress & 0xFF;
                    case 0x2:
                        return currentChannel.DMACurrent & 0xFF;
                    case 0x3:
                        return (currentChannel.DMACurrent >> 8) & 0xFF;
                    case 0x4:
                        return currentChannel.DMACurrent >> 16;
                    case 0x5:
                        return currentChannel.Counter & 0xFF;
                    case 0x6:
                        return currentChannel.Counter >> 8;
                    case 0x7:
                        return currentChannel.HDMAIB;
                    case 0x8:
                        return currentChannel.HDMACurrent & 0xFF;
                    case 0x9:
                        return currentChannel.HDMACurrent >> 8;
                    case 0xA:
                        return currentChannel.HDMALine;
                    case 0xB:
                        return currentChannel.Unused;
                    case 0xF:
                        return currentChannel.Unused;
                }
            }
            return 0;
        }

        public void Write8(int address, int value)
        {
            if (0x4300 <= address && address <= 0x437F)
            {
                int channel = (address & 0xF0) >> 4;
                DMAChannel currentChannel = _channels[channel];
                switch (address & 0xF)
                {
                    case 0x0:
                        currentChannel.Parameters = value;
                        break;
                    case 0x1:
                        currentChannel.PPUAddress = value | 0x2100;
                        break;
                    case 0x2:
                        currentChannel.DMACurrent = value | (currentChannel.DMACurrent & 0xFFFF00);
                        break;
                    case 0x3:
                        currentChannel.DMACurrent = (value << 8) | (currentChannel.DMACurrent & 0xFF00FF);
                        break;
                    case 0x4:
                        currentChannel.DMACurrent = (value << 16) | (currentChannel.DMACurrent & 0xFFFF);
                        break;
                    case 0x5:
                        currentChannel.Counter = value | (currentChannel.Counter & 0xFF00);
                        break;
                    case 0x6:
                        currentChannel.Counter = (value << 8) | (currentChannel.Counter & 0xFF);
                        break;
                    case 0x7:
                        currentChannel.HDMAIB = value;
                        break;
                    case 0x8:
                        currentChannel.HDMACurrent = value | (currentChannel.HDMACurrent & 0xFF00);
                        break;
                    case 0x9:
                        currentChannel.HDMACurrent = (value << 8) | (currentChannel.HDMACurrent & 0xFF);
                        break;
                    case 0xA:
                        currentChannel.HDMALine = value;
                        break;
                    case 0xB:
                        currentChannel.Unused = value;
                        break;
                    case 0xF:
                        currentChannel.Unused = value;
                        break;
                }
                _channels[channel] = currentChannel;
            }
        }

        public void DMATransfer()
        {
            for (int channelId = 0; channelId <= 7; channelId++)
            {
                if ((_system.IO.MDMAEnabled & (1 << channelId)) != 0)
                {
                    DMAChannel currentChannel = _channels[channelId];
                    if (currentChannel.Counter == 0)
                    {
                        currentChannel.Counter = 0x10000;
                    }
                    int size = 0;
                    switch (currentChannel.Parameters & 7)
                    {
                        case 0:
                            size = 1;
                            break;
                        case 1:
                        case 2:
                        case 6:
                            size = 2;
                            break;
                        case 3:
                        case 4:
                        case 5:
                        case 7:
                            size = 4;
                            break;
                    }
                    for (int i = 0; i <= size - 1; i++)
                    {
                        int ppuInc = 0;
                        switch (currentChannel.Parameters & 7)
                        {
                            case 1:
                            case 4:
                                ppuInc = i;
                                break;
                            case 3:
                            case 7:
                                ppuInc = i >> 1;
                                break;
                            case 5:
                                ppuInc = i & 1;
                                break;
                        }
                        if ((currentChannel.Parameters & 0x80) != 0)
                        {
                            _system.CPU.Write8(currentChannel.DMACurrent, _system.CPU.Read8(currentChannel.PPUAddress + ppuInc, false), false);
                        }
                        else
                        {
                            _system.CPU.Write8(currentChannel.PPUAddress + ppuInc, _system.CPU.Read8(currentChannel.DMACurrent, false), false);
                        }
                        switch ((currentChannel.Parameters & 0x18) >> 3)
                        {
                            case 0:
                                currentChannel.DMACurrent = ((currentChannel.DMACurrent + 1) & 0xFFFF) | (currentChannel.DMACurrent & 0xFF0000);
                                break;
                            case 2:
                                currentChannel.DMACurrent = ((currentChannel.DMACurrent - 1) & 0xFFFF) | (currentChannel.DMACurrent & 0xFF0000);
                                break;
                        }
                        _system.CPU.Cycles += 8;
                        currentChannel.Counter -= 1;
                        if (currentChannel.Counter == 0)
                        {
                            _system.IO.MDMAEnabled &= ~(1 << channelId);
                            _system.CPU.Cycles += 8;
                            break;
                        }
                    }
                    _channels[channelId] = currentChannel;
                    break;
                }
            }
        }

        public void HDMATransfer()
        {
            for (int channelIndex = 0; channelIndex <= 7; channelIndex++)
            {
                if ((_system.IO.HDMAEnabled & (1 << channelIndex)) != 0)
                {
                    DMAChannel currentChannel = _channels[channelIndex];
                    if (currentChannel.Enabled)
                    {
                        if ((currentChannel.HDMALine & 0x7F) == 0 | currentChannel.HDMALine > 0x80)
                        {
                            if ((currentChannel.HDMALine & 0x7F) == 0)
                            {
                                currentChannel.HDMALine = _system.CPU.Read8(currentChannel.HDMACurrent | (currentChannel.DMACurrent & 0xFF0000));
                                currentChannel.HDMACurrent = (currentChannel.HDMACurrent + 1) & 0xFFFF;
                                if (currentChannel.HDMALine == 0)
                                {
                                    currentChannel.Enabled = false;
                                    _channels[channelIndex] = currentChannel;
                                    continue;
                                }
                                if ((currentChannel.Parameters & 0x40) != 0)
                                {
                                    currentChannel.Counter = _system.CPU.Read16(currentChannel.HDMACurrent | (currentChannel.DMACurrent & 0xFF0000));
                                    currentChannel.HDMACurrent = (currentChannel.HDMACurrent + 2) & 0xFFFF;
                                    _system.CPU.Cycles += 16;
                                }
                            }
                            int aAddress;
                            int size = 0;
                            switch (currentChannel.Parameters & 7)
                            {
                                case 0:
                                    size = 1;
                                    break;
                                case 1:
                                case 2:
                                case 6:
                                    size = 2;
                                    break;
                                case 3:
                                case 4:
                                case 5:
                                case 7:
                                    size = 4;
                                    break;
                            }
                            if ((currentChannel.Parameters & 0x40) != 0)
                            {
                                aAddress = currentChannel.Counter | (currentChannel.HDMAIB << 16);
                                currentChannel.Counter = (currentChannel.Counter + size) & 0xFFFF;
                            }
                            else
                            {
                                aAddress = currentChannel.HDMACurrent | (currentChannel.DMACurrent & 0xFF0000);
                                currentChannel.HDMACurrent = (currentChannel.HDMACurrent + size) & 0xFFFF;
                            }
                            for (int i = 0; i <= size - 1; i++)
                            {
                                int ppuInc = 0;
                                switch (currentChannel.Parameters & 7)
                                {
                                    case 1:
                                    case 4:
                                        ppuInc = i;
                                        break;
                                    case 3:
                                    case 7:
                                        ppuInc = i >> 1;
                                        break;
                                    case 5:
                                        ppuInc = i & 1;
                                        break;
                                }
                                if ((currentChannel.Parameters & 0x80) != 0)
                                {
                                    _system.CPU.Write8(aAddress, _system.CPU.Read8(currentChannel.PPUAddress + ppuInc, false), false);
                                }
                                else
                                {
                                    _system.CPU.Write8(currentChannel.PPUAddress + ppuInc, _system.CPU.Read8(aAddress, false), false);
                                }
                                _system.CPU.Cycles += 8;
                                aAddress = ((aAddress + 1) & 0xFFFF) | (aAddress & 0xFF0000);
                            }
                        }
                        if ((currentChannel.HDMALine & 0x7F) != 0)
                        {
                            currentChannel.HDMALine -= 1;
                        }
                        _system.CPU.Cycles += 8;
                    }
                    _channels[channelIndex] = currentChannel;
                }
            }
            if (_system.IO.HDMAEnabled != 0)
            {
                _system.CPU.Cycles += 18;
            }
        }

        public void HDMAReset()
        {
            for (int channelIndex = 0; channelIndex <= 7; channelIndex++)
            {
                if ((_system.IO.HDMAEnabled & (1 << channelIndex)) != 0)
                {
                    DMAChannel currentChannel = _channels[channelIndex];
                    currentChannel.HDMACurrent = currentChannel.DMACurrent & 0xFFFF;
                    currentChannel.HDMALine = 0;
                    currentChannel.Enabled = true;
                    _channels[channelIndex] = currentChannel;
                }
            }
        }
    }
}