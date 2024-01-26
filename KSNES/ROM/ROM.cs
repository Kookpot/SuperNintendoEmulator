namespace KSNES.ROM;

public class ROM : IROM
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Header Header { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private byte[] _data = [];
    private byte[] _sram = [];
    private bool _hasSram;
    private int _banks;
    private int _sramSize;

    private ISNESSystem? _system;

    private Timer? _sRAMTimer;

    public void LoadROM(byte[] data, Header header)
    {
        _data = data;
        Header = header;
        _sram = new byte[header.RamSize];
        _hasSram = header.Chips > 0;
        _banks = header.RomSize / 0x8000;
        _sramSize = header.RamSize;
    }

    public void LoadSRAM()
    {
        string fileName = GetSRAMFileName();
        if (new FileInfo(fileName).Exists)
        {
            _sram = File.ReadAllBytes(fileName);
        }
    }

    public byte Read(int bank, int adr)
    {
        if (adr < 0x8000)
        {
            if (bank >= 0x70 && bank < 0x7e && _hasSram)
            {
                return _sram[(((bank - 0x70) << 15) | (adr & 0x7fff)) & (_sramSize - 1)];
            }
        }
        return _data[((bank & (_banks - 1)) << 15) | (adr & 0x7fff)];
    }

    public void Write(int bank, int adr, byte value)
    {
        if (adr < 0x8000 && bank >= 0x70 && bank < 0x7e && _hasSram)
        {
            _sram[(((bank - 0x70) << 15) | (adr & 0x7fff)) & (_sramSize - 1)] = value;
            _sRAMTimer ??= new Timer(SaveSRAM, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }
    }

    public void SetSystem(ISNESSystem system)
    {
        _system = system;
    }

    private void SaveSRAM(object? state)
    {
        string fileName = GetSRAMFileName();
        File.WriteAllBytes(fileName, _sram);
        _sRAMTimer?.Dispose();
        _sRAMTimer = null;
    }

    private string GetSRAMFileName()
    {
        return _system!.FileName.Replace(".smc", ".srm").Replace(".sfc", ".srm");;
    }
}