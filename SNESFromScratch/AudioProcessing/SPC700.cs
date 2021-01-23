namespace SNESFromScratch.AudioProcessing
{
    public class SPC700 : ISPC700
    {
        public byte[] WRAM { get; private set; } = new byte[65536];

        public int Cycles { get; set; }

        public IDSP DSP;

        private const int CyclesToSample = 32;

        private enum Flags
        {
            Carry = 0x1,
            Zero = 0x2,
            IRQ = 0x4,
            HalfCarry = 0x8,
            Break = 0x10,
            DirectPage = 0x20,
            Overflow = 0x40,
            Negative = 0x80
        }

        // Registers
        private int _a;
        private int _x;
        private int _y;
        private int _s;
        private int _psw;
        private int _pc;
        private int _control;
        private int _dspAddress;
        private int _cpuI0;
        private int _cpuO0;
        private int _cpuI1;
        private int _cpuO1;
        private int _cpuI2;
        private int _cpuO2;
        private int _cpuI3;
        private int _cpuO3;
        private bool Halted;

        private class Tmr
        {
            public int Clock;
            public int Cycles;
            public int Counter;
            public int Target;
            public int Out;
        }

        private readonly Tmr[] Timer = {new Tmr(), new Tmr(), new Tmr()};
        private int _dspCycles;

        public void Write8IO(int address, int value)
        {
            switch (address & 3)
            {
                case 0:
                    _cpuI0 = value;
                    break;
                case 1:
                    _cpuI1 = value;
                    break;
                case 2:
                    _cpuI2 = value;
                    break;
                case 3:
                    _cpuI3 = value;
                    break;
            }
        }

        public int Read8IO(int address)
        {
            int returnVal = 0;
            switch (address & 3)
            {
                case 0:
                    returnVal = _cpuO0;
                    break;
                case 1:
                    returnVal = _cpuO1;
                    break;
                case 2:
                    returnVal = _cpuO2;
                    break;
                case 3:
                    returnVal = _cpuO3;
                    break;
            }
            return returnVal;
        }

        public void Reset()
        {
            DSP = new DSP(this);
            _control = 0xB0;
            Timer[0].Clock = 128;
            Timer[1].Clock = 128;
            Timer[2].Clock = 16;
            _a = 0;
            _x = 0;
            _y = 0;
            _s = 0xFF;
            _psw = 0;
            _pc = Read16(0xFFFE);
            Cycles = 0;
            Halted = false;
        }

        public void Execute(int targetCycles)
        {
            while (Cycles < targetCycles & !Halted)
            {
                ExecuteStep();
            }
        }

        public void ExecuteStep()
        {
            int startCycles = Cycles;
            int opCode = Read8PC();
            switch (opCode)
            {
                case 0x99:
                    Write8DP(_x, ADC(Read8DP(_x), Read8DP(_y))); 
                    Cycles += 5;
                    break;
                case 0x88:
                    _a = ADC(_a, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x86:
                    _a = ADC(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0x97:
                    _a = ADC(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0x87:
                    _a = ADC(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x84:
                    _a = ADC(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x94:
                    _a = ADC(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0x85:
                    _a = ADC(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x95:
                    _a = ADC(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0x96:
                    _a = ADC(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0x89:
                    int src = D();
                    int dst = D();
                    Write8(dst, ADC(Read8(dst), Read8(src)));
                    Cycles += 6;
                    break;
                case 0x98:
                    src = IMM();
                    dst = D();
                    Write8(dst, ADC(Read8(dst), Read8(src)));
                    Cycles += 5;
                    break;
                case 0x7A:
                    SetYA(ADDW(GetYA(), Read16WP(D()))); 
                    Cycles += 5;
                    break;
                case 0x39:
                    Write8DP(_x, _AND(Read8DP(_x), Read8DP(_y))); 
                    Cycles += 5;
                    break;
                case 0x28:
                    _a = _AND(_a, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x26:
                    _a = _AND(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0x37:
                    _a = _AND(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0x27:
                    _a = _AND(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x24:
                    _a = _AND(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x34:
                    _a = _AND(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0x25:
                    _a = _AND(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x35:
                    _a = _AND(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0x36:
                    _a = _AND(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0x29:
                    src = D();
                    dst = D();
                    Write8(dst, _AND(Read8(dst), Read8(src)));
                    Cycles += 6;
                    break;
                case 0x38:
                    src = IMM();
                    dst = D();
                    Write8(dst, _AND(Read8(dst), Read8(src)));
                    Cycles += 5;
                    break;
                case 0x6A:
                    int address = ABS();
                    if ((_psw & (int) Flags.Carry) != 0)
                    {
                        int bit = address >> 13;
                        int value3 = Read8(address & 0x1FFF);
                        if ((value3 & (1 << bit)) != 0)
                        {
                            ClearFlag(Flags.Carry);
                        }
                    }
                    Cycles += 4;
                    break;
                case 0x4A:
                    address = ABS();
                    if ((_psw & (int) Flags.Carry) != 0)
                    {
                        int bit = address >> 13;
                        int value3 = Read8(address & 0x1FFF);
                        if ((value3 & (1 << bit)) == 0)
                        {
                            ClearFlag(Flags.Carry);
                        }
                    }
                    Cycles += 4;
                    break;
                case 0x1C:
                    ASLA(); 
                    Cycles += 2;
                    break;
                case 0xB:
                    ASL(D()); 
                    Cycles += 4;
                    break;
                case 0x1B:
                    ASL(DX()); 
                    Cycles += 5;
                    break;
                case 0xC:
                    ASL(ABS());
                    Cycles += 5;
                    break;
                case 0x13:
                    BBC(0x1); 
                    Cycles += 5;
                    break;
                case 0x33:
                    BBC(0x2); 
                    Cycles += 5;
                    break;
                case 0x53:
                    BBC(0x4); 
                    Cycles += 5;
                    break;
                case 0x73:
                    BBC(0x8); 
                    Cycles += 5;
                    break;
                case 0x93:
                    BBC(0x10); 
                    Cycles += 5;
                    break;
                case 0xB3:
                    BBC(0x20); 
                    Cycles += 5;
                    break;
                case 0xD3:
                    BBC(0x40); 
                    Cycles += 5;
                    break;
                case 0xF3:
                    BBC(0x80); 
                    Cycles += 5;
                    break;
                case 0x3:
                    BBS(0x1); 
                    Cycles += 5;
                    break;
                case 0x23:
                    BBS(0x2); 
                    Cycles += 5;
                    break;
                case 0x43:
                    BBS(0x4); 
                    Cycles += 5;
                    break;
                case 0x63:
                    BBS(0x8); 
                    Cycles += 5;
                    break;
                case 0x83:
                    BBS(0x10); 
                    Cycles += 5;
                    break;
                case 0xA3:
                    BBS(0x20); 
                    Cycles += 5;
                    break;
                case 0xC3:
                    BBS(0x40); 
                    Cycles += 5;
                    break;
                case 0xE3:
                    BBS(0x80); 
                    Cycles += 5;
                    break;
                case 0x90:
                    BCC(REL()); 
                    Cycles += 2;
                    break;
                case 0xB0:
                    BCS(REL()); 
                    Cycles += 2;
                    break;
                case 0xF0:
                    BEQ(REL()); 
                    Cycles += 2;
                    break;
                case 0x30:
                    BMI(REL()); 
                    Cycles += 2;
                    break;
                case 0xD0:
                    BNE(REL()); 
                    Cycles += 2;
                    break;
                case 0x10:
                    BPL(REL()); 
                    Cycles += 2;
                    break;
                case 0x2F:
                    BRA(REL()); 
                    Cycles += 2;
                    break;
                case 0x50:
                    BVC(REL()); 
                    Cycles += 2;
                    break;
                case 0x70:
                    BVS(REL()); 
                    Cycles += 2;
                    break;
                case 0xF:
                    BRK(); 
                    Cycles += 8;
                    break;
                case 0x3F:
                    _CALL(ABS()); 
                    Cycles += 8;
                    break;
                case 0xDE:
                    CBNE(DX()); 
                    Cycles += 6;
                    break;
                case 0x2E:
                    CBNE(D()); 
                    Cycles += 5;
                    break;
                case 0x12:
                    CLR1(D(), 0x1); 
                    Cycles += 4;
                    break;
                case 0x32:
                    CLR1(D(), 0x2); 
                    Cycles += 4;
                    break;
                case 0x52:
                    CLR1(D(), 0x4); 
                    Cycles += 4;
                    break;
                case 0x72:
                    CLR1(D(), 0x8); 
                    Cycles += 4;
                    break;
                case 0x92:
                    CLR1(D(), 0x10); 
                    Cycles += 4;
                    break;
                case 0xB2:
                    CLR1(D(), 0x20); 
                    Cycles += 4;
                    break;
                case 0xD2:
                    CLR1(D(), 0x40); 
                    Cycles += 4;
                    break;
                case 0xF2:
                    CLR1(D(), 0x80); 
                    Cycles += 4;
                    break;
                case 0x60:
                    CLRC(); 
                    Cycles += 2;
                    break;
                case 0x20:
                    CLRP(); 
                    Cycles += 2;
                    break;
                case 0xE0:
                    CLRV(); 
                    Cycles += 2;
                    break;
                case 0x79:
                    CMP(Read8DP(_x), Read8DP(_y)); 
                    Cycles += 5;
                    break;
                case 0x68:
                    CMP(_a, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x66:
                    CMP(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0x77:
                    CMP(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0x67:
                    CMP(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x64:
                    CMP(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x74:
                    CMP(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0x65:
                    CMP(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x75:
                    CMP(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0x76:
                    CMP(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0xC8:
                    CMP(_x, Read8(IMM()));
                    Cycles += 2;
                    break;
                case 0x3E:
                    CMP(_x, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x1E:
                    CMP(_x, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0xAD:
                    CMP(_y, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x7E:
                    CMP(_y, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x5E:
                    CMP(_y, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x69:
                    src = D();
                    dst = D();
                    CMP(Read8(dst), Read8(src));
                    Cycles += 6;
                    break;
                case 0x78:
                    src = IMM();
                    dst = D();
                    CMP(Read8(dst), Read8(src));
                    Cycles += 5;
                    break;
                case 0x5A:
                    CMPW(GetYA(), Read16WP(D())); 
                    Cycles += 4;
                    break;
                case 0xDF:
                    DAA(); 
                    Cycles += 3;
                    break;
                case 0xBE:
                    DAS(); 
                    Cycles += 3;
                    break;
                case 0xFE:
                    DBNZ(); 
                    Cycles += 4;
                    break;
                case 0x6E:
                    DBNZM(); 
                    Cycles += 5;
                    break;
                case 0x9C:
                    _a = DEC(_a); 
                    Cycles += 2;
                    break;
                case 0x1D:
                    _x = DEC(_x); 
                    Cycles += 2;
                    break;
                case 0xDC:
                    _y = DEC(_y); 
                    Cycles += 2;
                    break;
                case 0x8B:
                    DECM(D()); 
                    Cycles += 4;
                    break;
                case 0x9B:
                    DECM(DX());
                    Cycles += 5;
                    break;
                case 0x8C:
                    DECM(ABS()); 
                    Cycles += 5;
                    break;
                case 0x1A:
                    DECW(D()); 
                    Cycles += 6;
                    break;
                case 0xC0:
                    DI(); 
                    Cycles += 3;
                    break;
                case 0x9E:
                    DIV(); 
                    Cycles += 12;
                    break;
                case 0xA0:
                    EI(); 
                    Cycles += 3;
                    break;
                case 0x59:
                    Write8DP(_x, EOR(Read8DP(_x), Read8DP(_y))); 
                    Cycles += 5;
                    break;
                case 0x48:
                    _a = EOR(_a, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x46:
                    _a = EOR(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0x57:
                    _a = EOR(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0x47:
                    _a = EOR(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x44:
                    _a = EOR(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x54:
                    _a = EOR(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0x45:
                    _a = EOR(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x55:
                    _a = EOR(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0x56:
                    _a = EOR(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0x49:
                    src = D();
                    dst = D();
                    Write8(dst, EOR(Read8(dst), Read8(src)));
                    Cycles += 6;
                    break;
                case 0x58:
                    src = IMM();
                    dst = D();
                    Write8(dst, EOR(Read8(dst), Read8(src)));
                    Cycles += 5;
                    break;
                case 0x8A:
                    address = ABS();
                    int bit2 = address >> 13;
                    int value2 = Read8(address & 0x1FFF);
                    if ((_psw & (int) Flags.Carry) != 0)
                    {
                        if ((value2 & (1 << bit2)) != 0)
                        {
                            ClearFlag(Flags.Carry);
                        }
                    }
                    else if ((value2 & (1 << bit2)) != 0)
                    {
                        SetFlag(Flags.Carry);
                    }
                    Cycles += 5;
                    break;
                case 0xBC:
                    _a = INC(_a); 
                    Cycles += 2;
                    break;
                case 0x3D:
                    _x = INC(_x); 
                    Cycles += 2;
                    break;
                case 0xFC:
                    _y = INC(_y); 
                    Cycles += 2;
                    break;
                case 0xAB:
                    INCM(D()); 
                    Cycles += 4;
                    break;
                case 0xBB:
                    INCM(DX()); 
                    Cycles += 5;
                    break;
                case 0xAC:
                    INCM(ABS()); 
                    Cycles += 5;
                    break;
                case 0x3A:
                    INCW(D()); 
                    Cycles += 6;
                    break;
                case 0x1F:
                    JMP(Read16(ABSX())); 
                    Cycles += 6;
                    break;
                case 0x5F:
                    JMP(ABS()); 
                    Cycles += 3;
                    break;
                case 0x5C:
                    LSRA(); 
                    Cycles += 2;
                    break;
                case 0x4B:
                    LSR(D()); 
                    Cycles += 4;
                    break;
                case 0x5B:
                    LSR(DX()); 
                    Cycles += 5;
                    break;
                case 0x4C:
                    LSR(ABS()); 
                    Cycles += 5;
                    break;
                case 0xAF:
                    Write8DP(_x, _a); 
                    _x += 1; 
                    Cycles += 4;
                    break;
                case 0xC6:
                    Write8DP(_x, _a); 
                    Cycles += 4;
                    break;
                case 0xD7:
                    Write8(DINDY(), _a); 
                    Cycles += 7;
                    break;
                case 0xC7:
                    Write8(DINDX(), _a); 
                    Cycles += 7;
                    break;
                case 0xE8:
                    MOVA(Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0xE6:
                    MOVA(Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0xBF:
                    MOVA(Read8DP(_x));
                    _x += 1; 
                    Cycles += 4;
                    break;
                case 0xF7:
                    MOVA(Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0xE7:
                    MOVA(Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x7D:
                    MOVA(_x); 
                    Cycles += 2;
                    break;
                case 0xDD:
                    MOVA(_y); 
                    Cycles += 2;
                    break;
                case 0xE4:
                    MOVA(Read8(D())); 
                    Cycles += 3;
                    break;
                case 0xF4:
                    MOVA(Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0xE5:
                    MOVA(Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0xF5:
                    MOVA(Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0xF6:
                    MOVA(Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0xBD:
                    MOVS(_x); 
                    Cycles += 2;
                    break;
                case 0xCD:
                    MOVX(Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x5D:
                    MOVX(_a); 
                    Cycles += 2;
                    break;
                case 0x9D:
                    MOVX(_s); 
                    Cycles += 2;
                    break;
                case 0xF8:
                    MOVX(Read8(D())); 
                    Cycles += 3;
                    break;
                case 0xF9:
                    MOVX(Read8(DY())); 
                    Cycles += 4;
                    break;
                case 0xE9:
                    MOVX(Read8(ABS()));
                    Cycles += 4;
                    break;
                case 0x8D:
                    MOVY(Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0xFD:
                    MOVY(_a); 
                    Cycles += 2;
                    break;
                case 0xEB:
                    MOVY(Read8(D())); 
                    Cycles += 3;
                    break;
                case 0xFB:
                    MOVY(Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0xEC:
                    MOVY(Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0xFA:
                    src = D();
                    dst = D();
                    Write8(dst, Read8(src));
                    Cycles += 5;
                    break;
                case 0xD4:
                    Write8(DX(), _a); 
                    Cycles += 5;
                    break;
                case 0xDB:
                    Write8(DX(), _y); 
                    Cycles += 5;
                    break;
                case 0xD9:
                    Write8(DY(), _x); 
                    Cycles += 5;
                    break;
                case 0x8F:
                    int value = Read8(IMM());
                    Write8(D(), value);
                    Cycles += 5;
                    break;
                case 0xC4:
                    Write8(D(), _a); 
                    Cycles += 4;
                    break;
                case 0xD8:
                    Write8(D(), _x); 
                    Cycles += 4;
                    break;
                case 0xCB:
                    Write8(D(), _y); 
                    Cycles += 4;
                    break;
                case 0xD5:
                    Write8(ABSX(), _a); 
                    Cycles += 6;
                    break;
                case 0xD6:
                    Write8(ABSY(), _a); 
                    Cycles += 6;
                    break;
                case 0xC5:
                    Write8(ABS(), _a); 
                    Cycles += 5;
                    break;
                case 0xC9:
                    Write8(ABS(), _x); 
                    Cycles += 5;
                    break;
                case 0xCC:
                    Write8(ABS(), _y); 
                    Cycles += 5;
                    break;
                case 0xAA:
                    address = ABS();
                    bit2 = address >> 13;
                    value = Read8(address & 0x1FFF);
                    SetFlag((value & (1 << bit2)) != 0, Flags.Carry);
                    Cycles += 4;
                    break;
                case 0xCA:
                    address = ABS();
                    bit2 = address >> 13;
                    value = Read8(address & 0x1FFF);
                    if ((_psw & (int) Flags.Carry) != 0)
                    {
                        Write8(address & 0x1FFF, value | (1 << bit2));
                    }
                    else
                    {
                        Write8(address & 0x1FFF, value & ~(1 << bit2));
                    }
                    Cycles += 6;
                    break;
                case 0xBA:
                    MOVWYA(Read16WP(D())); 
                    Cycles += 5;
                    break;
                case 0xDA:
                    Write16(D(), GetYA()); 
                    Cycles += 5;
                    break;
                case 0xCF:
                    MUL(); 
                    Cycles += 9;
                    break;
                case 0x0:
                    Cycles += 2;
                    break;
                case 0xEA:
                    NOT1(); 
                    Cycles += 5;
                    break;
                case 0xED:
                    NOTC(); 
                    Cycles += 3;
                    break;
                case 0x19:
                    Write8DP(_x, OR(Read8DP(_x), Read8DP(_y))); 
                    Cycles += 5;
                    break;
                case 0x8:
                    _a = OR(_a, Read8(IMM())); 
                    Cycles += 2;
                    break;
                case 0x6:
                    _a = OR(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0x17:
                    _a = OR(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0x7:
                    _a = OR(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0x4:
                    _a = OR(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0x14:
                    _a = OR(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0x5:
                    _a = OR(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0x15:
                    _a = OR(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0x16:
                    _a = OR(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0x9:
                    src = D();
                    dst = D();
                    Write8(dst, OR(Read8(dst), Read8(src)));
                    Cycles += 6;
                    break;
                case 0x18:
                    src = IMM();
                    dst = D();
                    Write8(dst, OR(Read8(dst), Read8(src)));
                    Cycles += 5;
                    break;
                case 0x2A:
                    address = ABS();
                    if ((_psw & (int) Flags.Carry) == 0)
                    {
                        int bit3 = address >> 13;
                        value = Read8(address & 0x1FFF);
                        if ((value & (1 << bit3)) == 0)
                        {
                            SetFlag(Flags.Carry);
                        }
                    }
                    Cycles += 5;
                    break;
                case 0xA:
                    address = ABS();
                    if ((_psw & (int) Flags.Carry) == 0)
                    {
                        int bit3 = address >> 13;
                        value = Read8(address & 0x1FFF);
                        if ((value & (1 << bit3)) != 0)
                        {
                            SetFlag(Flags.Carry);
                        }
                    }
                    Cycles += 5;
                    break;
                case 0x4F:
                    _CALL(0xFF00 | Read8PC());
                    Cycles += 6;
                    break;
                case 0xAE:
                    _a = Pull8(); 
                    Cycles += 4;
                    break;
                case 0x8E:
                    _psw = Pull8(); 
                    Cycles += 4;
                    break;
                case 0xCE:
                    _x = Pull8(); 
                    Cycles += 4;
                    break;
                case 0xEE:
                    _y = Pull8(); 
                    Cycles += 4;
                    break;
                case 0x2D:
                    Push8(_a); 
                    Cycles += 4;
                    break;
                case 0xD:
                    Push8(_psw); 
                    Cycles += 4;
                    break;
                case 0x4D:
                    Push8(_x); 
                    Cycles += 4;
                    break;
                case 0x6D:
                    Push8(_y); 
                    Cycles += 4;
                    break;
                case 0x6F:
                    RET(); 
                    Cycles += 5;
                    break;
                case 0x7F:
                    RETI(); 
                    Cycles += 6;
                    break;
                case 0x3C:
                    ROLA(); 
                    Cycles += 2;
                    break;
                case 0x2B:
                    ROL(D()); 
                    Cycles += 4;
                    break;
                case 0x3B:
                    ROL(DX()); 
                    Cycles += 5;
                    break;
                case 0x2C:
                    ROL(ABS()); 
                    Cycles += 5;
                    break;
                case 0x7C:
                    RORA(); 
                    Cycles += 2;
                    break;
                case 0x6B:
                    ROR(D()); 
                    Cycles += 4;
                    break;
                case 0x7B:
                    ROR(DX()); 
                    Cycles += 5;
                    break;
                case 0x6C:
                    ROR(ABS()); 
                    Cycles += 5;
                    break;
                case 0xB9:
                    Write8DP(_x, SBC(Read8DP(_x), Read8DP(_y)));
                    Cycles += 5;
                    break; 
                case 0xA8:
                    _a = SBC(_a, Read8(IMM()));
                    Cycles += 2;
                    break;
                case 0xA6:
                    _a = SBC(_a, Read8DP(_x)); 
                    Cycles += 3;
                    break;
                case 0xB7:
                    _a = SBC(_a, Read8(DINDY())); 
                    Cycles += 6;
                    break;
                case 0xA7:
                    _a = SBC(_a, Read8(DINDX())); 
                    Cycles += 6;
                    break;
                case 0xA4:
                    _a = SBC(_a, Read8(D())); 
                    Cycles += 3;
                    break;
                case 0xB4:
                    _a = SBC(_a, Read8(DX())); 
                    Cycles += 4;
                    break;
                case 0xA5:
                    _a = SBC(_a, Read8(ABS())); 
                    Cycles += 4;
                    break;
                case 0xB5:
                    _a = SBC(_a, Read8(ABSX())); 
                    Cycles += 5;
                    break;
                case 0xB6:
                    _a = SBC(_a, Read8(ABSY())); 
                    Cycles += 5;
                    break;
                case 0xA9:
                    src = D();
                    dst = D();
                    Write8(dst, SBC(Read8(dst), Read8(src)));
                    Cycles += 6;
                    break;
                case 0xB8 :
                    src = IMM();
                    dst = D();
                    Write8(dst, SBC(Read8(dst), Read8(src)));
                    Cycles += 5;
                    break;
                case 0x2:
                    SET1(D(), 0x1); 
                    Cycles += 4;
                    break;
                case 0x22:
                    SET1(D(), 0x2); 
                    Cycles += 4;
                    break;
                case 0x42:
                    SET1(D(), 0x4); 
                    Cycles += 4;
                    break;
                case 0x62:
                    SET1(D(), 0x8); 
                    Cycles += 4;
                    break;
                case 0x82:
                    SET1(D(), 0x10); 
                    Cycles += 4;
                    break;
                case 0xA2:
                    SET1(D(), 0x20); 
                    Cycles += 4;
                    break;
                case 0xC2:
                    SET1(D(), 0x40); 
                    Cycles += 4;
                    break;
                case 0xE2:
                    SET1(D(), 0x80); 
                    Cycles += 4;
                    break;
                case 0x80:
                    SETC(); 
                    Cycles += 2;
                    break;
                case 0x40:
                    SETP(); 
                    Cycles += 2;
                    break;
                case 0xEF:
                    Halted = true;
                    break;
                case 0xFF:
                    Halted = true;
                    break;
                case 0x9A:
                    SetYA(SUBW(GetYA(), Read16WP(D()))); 
                    Cycles += 5;
                    break;
                case 0x1:
                    _CALL(Read16(0xFFDE)); 
                    Cycles += 8;
                    break;
                case 0x11:
                    _CALL(Read16(0xFFD));
                    Cycles += 8;
                    break;
                case 0x21:
                    _CALL(Read16(0xFFDA)); 
                    Cycles += 8;
                    break;
                case 0x31:
                    _CALL(Read16(0xFFD8)); 
                    Cycles += 8;
                    break;
                case 0x41:
                    _CALL(Read16(0xFFD6)); 
                    Cycles += 8;
                    break;
                case 0x51:
                    _CALL(Read16(0xFFD4)); 
                    Cycles += 8;
                    break;
                case 0x61:
                    _CALL(Read16(0xFFD2)); 
                    Cycles += 8;
                    break;
                case 0x71:
                    _CALL(Read16(0xFFD0)); 
                    Cycles += 8;
                    break;
                case 0x81:
                    _CALL(Read16(0xFFCE)); 
                    Cycles += 8;
                    break;
                case 0x91:
                    _CALL(Read16(0xFFC)); 
                    Cycles += 8;
                    break;
                case 0xA1:
                    _CALL(Read16(0xFFCA)); 
                    Cycles += 8;
                    break;
                case 0xB1:
                    _CALL(Read16(0xFFC8)); 
                    Cycles += 8;
                    break;
                case 0xC1:
                    _CALL(Read16(0xFFC6)); 
                    Cycles += 8;
                    break;
                case 0xD1:
                    _CALL(Read16(0xFFC4)); 
                    Cycles += 8;
                    break;
                case 0xE1:
                    _CALL(Read16(0xFFC2));
                    Cycles += 8;
                    break;
                case 0xF1:
                    _CALL(Read16(0xFFC0)); 
                    Cycles += 8; 
                    break;
                case 0x4E:
                    TCLR1(); 
                    Cycles += 6;
                    break;
                case 0xE:
                    TSET1(); 
                    Cycles += 6;
                    break;
                case 0x9F:
                    XCN(); 
                    Cycles += 5;
                    break;
            }
            int spentCycles = Cycles - startCycles;
            _dspCycles += spentCycles;
            if (_dspCycles >= CyclesToSample)
            {
                _dspCycles -= CyclesToSample;
                DSP.ProcessSample();
            }
            TickTimers(spentCycles);
        }

        private int GetYA()
        {
            return _a | (_y << 8);
        }
        private void SetYA(int ya)
        {
            _a = ya & 0xFF;
            _y = ya >> 8;
        }

        private void SetFlag(Flags flag)
        {
            _psw |= (int) flag;
        }
        private void ClearFlag(Flags flag)
        {
            _psw &= ~(int) flag;
        }
        private void SetFlag(bool condition, Flags flag)
        {
            if (condition)
            {
                SetFlag(flag);
            }
            else
            {
                ClearFlag(flag);
            }
        }

        private void SetZNFlags8(byte value)
        {
            SetFlag(value == 0, Flags.Zero);
            SetFlag((value & 0x80) != 0, Flags.Negative);
        }
        private void SetZNFlags16(int value)
        {
            SetFlag(value == 0, Flags.Zero);
            SetFlag((value & 0x8000) != 0, Flags.Negative);
        }

        private void Push8(int Value)
        {
            Write8(_s | 0x100, Value);
            _s = (_s - 1) & 0xFF;
        }

        private void Push16(int Value)
        {
            Push8((Value >> 8) & 0xFF);
            Push8(Value & 0xFF);
        }

        private int Pull8()
        {
            _s = (_s + 1) & 0xFF;
            return Read8(_s | 0x100);
        }

        private int Pull16()
        {
            int returnVal = Pull8();
            returnVal |= Pull8() << 8;
            return returnVal;
        }

        private void TickTimers(int ElapsedCycles)
        {
            for (int i = 0; i <= 2; i++)
            {
                if ((_control & (1 << i)) != 0)
                {
                    Tmr timer = Timer[i];
                    timer.Cycles += ElapsedCycles;
                    if (timer.Cycles >= timer.Clock)
                    {
                        timer.Counter = (timer.Counter + 1) & 0xFF;
                        if (timer.Counter == timer.Target)
                        {
                            timer.Out = (timer.Out + 1) & 0xF;
                            timer.Counter = 0;
                        }
                        timer.Cycles -= timer.Clock;
                    }
                }
            }
        }

        private int ABS()
        {
            return Read16PC();
        }

        private int ABSX()
        {
            return (ABS() + _x) & 0xFFFF;
        }

        private int ABSY()
        {
            return (ABS() + _y) & 0xFFFF;
        }

        private int D()
        {
            return Read8PC() | ((_psw & (int) Flags.DirectPage) << 3);
        }

        private int DX()
        {
            int addressBase = D();
            int addressHigh = addressBase & 0xFF00;
            int address = addressBase + _x;
            return addressHigh | (address & 0xFF);
        }

        private int DY()
        {
            int addressBase = D();
            int addressHigh = addressBase & 0xFF00;
            int address = addressBase + _y;
            return addressHigh | (address & 0xFF);
        }

        private int DINDX()
        {
            return Read16(DX());
        }

        private int DINDY()
        {
            return (Read16(D()) + _y) & 0xFFFF;
        }

        private int IMM()
        {
            int returnVal = _pc;
            _pc = (_pc + 1) & 0xFFFF;
            return returnVal;
        }

        private int REL()
        {
            int displacement = Read8PC();
            if ((displacement & 0x80) != 0)
            {
                displacement = (int) (displacement |0xFFFFFF00);
            }
            return (_pc + displacement) & 0xFFFF;
        }

        private int ADC(int l, int r)
        {
            int result = l + r + (_psw & (int) Flags.Carry);
            SetFlag((~(l ^ r) & (l ^ result) & 0x80) != 0, Flags.Overflow);
            SetFlag(((l ^ r ^ result) & 0x10) != 0, Flags.HalfCarry);
            SetFlag(result > 0xFF, Flags.Carry);
            int returnVal = result & 0xFF;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private int ADDW(int l, int r)
        {
            int result = l + r;
            SetFlag((~(l ^ r) & (l ^ result) & 0x8000) != 0, Flags.Overflow);
            SetFlag(((l ^ r ^ result) & 0x1000) != 0, Flags.HalfCarry);
            SetFlag(result > 0xFFFF, Flags.Carry);
            int returnVal = result & 0xFFFF;
            SetZNFlags16(returnVal);
            return returnVal;
        }

        private int _AND(int l, int r)
        {
            int returnVal = l & r;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private void ASL(int ea)
        {
            int value = Read8(ea);
            SetFlag((value & 0x80) != 0, Flags.Carry);
            value = (value << 1) & 0xFF;
            Write8(ea, value);
            SetZNFlags8((byte) value);
        }

        private void ASLA()
        {
            SetFlag((_a & 0x80) != 0, Flags.Carry);
            _a = (_a << 1) & 0xFF;
            SetZNFlags8((byte) _a);
        }

        private void BBC(int mask)
        {
            int value = Read8(D());
            int branch = REL();
            if ((value & mask) == 0)
            {
                BRA(branch);
            }
        }

        private void BBS(int mask)
        {
            int value = Read8(D());
            int branch = REL();
            if ((value & mask) != 0)
            {
                BRA(branch);
            }
        }

        private void BCC(int EA)
        {
            if ((_psw & (int) Flags.Carry) == 0)
                BRA(EA);
        }

        private void BCS(int ea)
        {
            if ((_psw & (int) Flags.Carry) != 0)
            {
                BRA(ea);
            }
        }

        private void BEQ(int ea)
        {
            if ((_psw & (int) Flags.Zero) != 0)
            {
                BRA(ea);
            }
        }

        private void BMI(int ea)
        {
            if ((_psw & (int) Flags.Negative) != 0)
            {
                BRA(ea);
            }
        }

        private void BNE(int ea)
        {
            if ((_psw & (int) Flags.Zero) == 0)
            {
                BRA(ea);
            }
        }

        private void BPL(int ea)
        {
            if ((_psw & (int) Flags.Negative) == 0)
            {
                BRA(ea);
            }
        }

        private void BRA(int ea)
        {
            _pc = ea;
            Cycles += 2;
        }

        private void BVC(int ea)
        {
            if ((_psw & (int) Flags.Overflow) == 0)
            {
                BRA(ea);
            }
        }

        private void BVS(int ea)
        {
            if ((_psw & (int) Flags.Overflow) != 0)
            {
                BRA(ea);
            }
        }

        private void BRK()
        {
            Push16(_pc);
            Push8(_psw);
            _pc = Read16(0xFFDE);
            SetFlag(Flags.Break);
            ClearFlag(Flags.IRQ);
        }

        private void _CALL(int ea)
        {
            Push16(_pc);
            _pc = ea;
        }

        private void CBNE(int ea)
        {
            int branch = REL();
            if (Read8(ea) != _a)
            {
                _pc = branch;
                Cycles += 2;
            }
        }

        private void CLR1(int ea, int mask)
        {
            Write8(ea, Read8(ea) & ~mask);
        }

        private void CLRC()
        {
            ClearFlag(Flags.Carry);
        }

        private void CLRP()
        {
            ClearFlag(Flags.DirectPage);
        }

        private void CLRV()
        {
            ClearFlag(Flags.Overflow);
            ClearFlag(Flags.HalfCarry);
        }

        private void CMP(int l, int r)
        {
            int result = l - r;
            SetFlag(result >= 0, Flags.Carry);
            SetZNFlags8((byte) (result & 0xFF));
        }

        private void CMPW(int l, int r)
        {
            int result = l - r;
            SetFlag(result >= 0, Flags.Carry);
            SetZNFlags16(result & 0xFFFF);
        }

        private void DAA()
        {
            if ((_psw & (int) Flags.Carry) != 0 | _a > 0x99)
            {
                SetFlag(Flags.Carry);
                _a += 0x60;
            }
            if ((_psw & (int) Flags.HalfCarry) != 0 | (_a & 0xF) > 9)
            {
                _a += 6;
            }
            _a &= 0xFF;
            SetZNFlags8((byte) _a);
        }

        private void DAS()
        {
            if ((_psw & (int) Flags.Carry) == 0 | _a > 0x99)
            {
                ClearFlag(Flags.Carry);
                _a -= 0x60;
            }
            if ((_psw & (int) Flags.HalfCarry) == 0 | (_a & 0xF) > 9)
            {
                _a -= 6;
            }
            _a &= 0xFF;
            SetZNFlags8((byte) _a);
        }

        private void DBNZ()
        {
            int address = REL();
            _y = (_y - 1) & 0xFF;
            if (_y != 0)
            {
                _pc = address;
                Cycles += 2;
            }
        }

        private void DBNZM()
        {
            int address = D();
            int branch = REL();
            int value = Read8(address);
            value = (value - 1) & 0xFF;
            Write8(address, value);
            if (value != 0)
            {
                _pc = branch;
                Cycles += 2;
            }
        }

        private int DEC(int value)
        {
            int returnVal = (value - 1) & 0xFF;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private void DECM(int ea)
        {
            Write8(ea, DEC(Read8(ea)));
        }

        private void DECW(int ea)
        {
            int value = Read16WP(ea);
            value = value - 1 & 0xFFFF;
            Write16(ea, value);
            SetZNFlags16(value);
        }

        private void DI()
        {
            ClearFlag(Flags.IRQ);
        }

        private void DIV()
        {
            int yva = GetYA();
            int slx = _x << 9;
            bool hc = (_x & 0xF) <= (_y & 0xF);
            for (int i = 0; i <= 8; i++)
            {
                yva <<= 1;
                if ((yva & 0x20000) != 0)
                {
                    yva = (yva & 0x1FFFF) | 1;
                }
                if (yva >= slx)
                {
                    yva ^= 1;
                }
                if ((yva & 1) != 0)
                {
                    yva = (yva - slx) & 0x1FFFF;
                }
            }
            _a = yva & 0xFF;
            _y = (yva >> 9) & 0xFF;
            SetFlag(hc, Flags.HalfCarry);
            SetFlag((yva & 0x100) != 0, Flags.Overflow);
            SetZNFlags8((byte) _a);
        }

        private void EI()
        {
            SetFlag(Flags.IRQ);
        }

        public int EOR(int l, int r)
        {
            int returnVal = l ^ r;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private int INC(int value)
        {
            int returnVal = (value + 1) & 0xFF;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private void INCM(int ea)
        {
            Write8(ea, INC(Read8(ea)));
        }

        private void INCW(int ea)
        {
            int value = Read16WP(ea);
            value = value + 1 & 0xFFFF;
            Write16(ea, value);
            SetZNFlags16(value);
        }

        private void JMP(int ea)
        {
            _pc = ea;
        }

        private void LSR(int ea)
        {
            int value = Read8(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value >>= 1;
            Write8(ea, value);
            SetZNFlags8((byte) value);
        }

        private void LSRA()
        {
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a >>= 1;
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void MOVA(int value)
        {
            _a = value;
            SetZNFlags8((byte) _a);
        }

        private void MOVX(int value)
        {
            _x = value;
            SetZNFlags8((byte) _x);
        }

        private void MOVY(int value)
        {
            _y = value;
            SetZNFlags8((byte) _y);
        }

        private void MOVS(int value)
        {
            _s = value;
        }

        private void MOVWYA(int value)
        {
            SetYA(value);
            SetZNFlags16(value);
        }

        private void MUL()
        {
            SetYA(_y * _a);
            SetZNFlags8((byte) _y);
        }

        private void NOT1()
        {
            int address = ABS();
            int bit = address >> 13;
            int value = Read8(address & 0x1FFF);
            Write8(address & 0x1FFF, value ^ (1 << bit));
        }

        private void NOTC()
        {
            SetFlag((_psw & (int) Flags.Carry) == 0, Flags.Carry);
        }

        public int OR(int l, int r)
        {
            int returnVal = l | r;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private void RET()
        {
            _pc = Pull16();
        }

        private void RETI()
        {
            _psw = Pull8();
            _pc = Pull16();
        }

        private void ROL(int ea)
        {
            int carry = _psw & (int) Flags.Carry;
            int value = Read8(ea);
            SetFlag((value & 0x80) != 0, Flags.Carry);
            value = (value << 1) & 0xFF;
            value |= carry;
            Write8(ea, value);
            SetZNFlags8((byte) value);
        }

        private void ROLA()
        {
            int carry = _psw & (int) Flags.Carry;
            SetFlag((_a & 0x80) != 0, Flags.Carry);
            _a = (_a << 1) & 0xFF;
            _a |= carry;
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void ROR(int ea)
        {
            int carry = _psw & (int) Flags.Carry;
            int value = Read8(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value = (value >> 1) | (carry << 7);
            Write8(ea, value);
            SetZNFlags8((byte) value);
        }

        private void RORA()
        {
            int carry = _psw & (int) Flags.Carry;
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a = (_a >> 1) | (carry << 7);
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private int SBC(int l, int r)
        {
            int result = l - r - (1 - (_psw & (int) Flags.Carry));
            SetFlag(((l ^ r) & (l ^ result) & 0x80) != 0, Flags.Overflow);
            SetFlag(((l ^ r ^ result) & 0x10) == 0, Flags.HalfCarry);
            SetFlag(result >= 0, Flags.Carry);
            int returnVal = result & 0xFF;
            SetZNFlags8((byte) returnVal);
            return returnVal;
        }

        private void SET1(int ea, int mask)
        {
            Write8(ea, Read8(ea) | mask);
        }

        private void SETC()
        {
            SetFlag(Flags.Carry);
        }

        private void SETP()
        {
            SetFlag(Flags.DirectPage);
        }

        private int SUBW(int l, int r)
        {
            int result = l - r;
            SetFlag(((l ^ r) & (l ^ result) & 0x8000) != 0, Flags.Overflow);
            SetFlag(((l ^ r ^ result) & 0x1000) == 0, Flags.HalfCarry);
            SetFlag(result >= 0, Flags.Carry);
            int returnVal = result & 0xFFFF;
            SetZNFlags16(returnVal);
            return returnVal;
        }

        private void TCLR1()
        {
            int address = ABS();
            int value = Read8(address);
            Write8(address, value & ~_a);
            SetZNFlags8((byte) ((_a - value) & 0xFF));
        }

        private void TSET1()
        {
            int address = ABS();
            int value = Read8(address);
            Write8(address, value | _a);
            SetZNFlags8((byte) ((_a - value) & 0xFF));
        }

        private void XCN()
        {
            int al = _a & 0xF;
            int ah = (_a & 0xF0) >> 4;
            _a = ah | (al << 4);
            SetZNFlags8((byte) _a);
        }

        private int Read8(int address)
        {
            int returnVal = 0;
            switch (address)
            {
                case object _ when 0xF0 <= address && address <= 0xFF:
                    switch (address & 0xF)
                    {
                        case 0x2:
                            returnVal = _dspAddress;
                            break;
                        case 0x3:
                            returnVal = DSP.Read8(_dspAddress);
                            break;
                        case 0x4:
                            returnVal = _cpuI0;
                            break;
                        case 0x5:
                            returnVal = _cpuI1;
                            break;
                        case 0x6:
                            returnVal = _cpuI2;
                            break;
                        case 0x7:
                            returnVal = _cpuI3;
                            break;
                        case 0x8:
                            returnVal = WRAM[address];
                            break;
                        case 0x9:
                            returnVal = WRAM[address];
                            break;
                        case 0xD:
                            returnVal = Timer[0].Out;
                            Timer[0].Out = 0;
                            break;
                        case 0xE:
                            returnVal = Timer[1].Out;
                            Timer[1].Out = 0;
                            break;
                        case 0xF:
                            returnVal = Timer[2].Out;
                            Timer[2].Out = 0;
                            break;
                    }
                    break;
                case object _ when 0xFFC0 <= address && address <= 0xFFFF:
                    returnVal = (_control & 0x80) != 0 ? Properties.Resources.spc700[address & 0x3F] : WRAM[address];
                    break;
                default:
                    returnVal = WRAM[address];
                    break;
            }
            return returnVal;
        }

        private int Read16(int Address)
        {
            int returnVal = Read8(Address);
            returnVal |= Read8(AddWB(Address, 1)) << 8;
            return returnVal;
        }

        private int Read16WP(int address)
        {
            int returnVal = Read8(address);
            returnVal |= Read8(AddWP(address, 1)) << 8;
            return returnVal;
        }

        private int Read8PC()
        {
            int returnVal = Read8(_pc);
            _pc = (_pc + 1) & 0xFFFF;
            return returnVal;
        }

        private int Read16PC()
        {
            int returnVal = Read16(_pc);
            _pc = (_pc + 2) & 0xFFFF;
            return returnVal;
        }

        private int Read8DP(int Address)
        {
            return Read8(Address | ((_psw & (int) Flags.DirectPage) << 3));
        }

        private void Write8(int address, int value)
        {
            switch (address)
            {
                case object _ when 0xF0 <= address && address <= 0xFF:
                    switch (address & 0xF)
                    {
                        case 0x0:
                            break;
                        case 0x1:
                            _control = value;
                            if ((_control & 0x1) != 0)
                            {
                                Timer[0].Counter = 0;
                            }
                            if ((_control & 0x2) != 0)
                            {
                                Timer[1].Counter = 0;
                            }
                            if ((_control & 0x4) != 0)
                            {
                                Timer[2].Counter = 0;
                            }
                            if ((_control & 0x10) != 0)
                            {
                                _cpuI0 = 0;
                                _cpuI1 = 0;
                            }
                            if ((_control & 0x20) != 0)
                            {
                                _cpuI2 = 0;
                                _cpuI3 = 0;
                            }
                            break;
                        case 0x2:
                            _dspAddress = value;
                            break;
                        case 0x3:
                            DSP.Write8(_dspAddress, value);
                            break;
                        case 0x4:
                            _cpuO0 = value;
                            break;
                        case 0x5:
                            _cpuO1 = value;
                            break;
                        case 0x6:
                            _cpuO2 = value;
                            break;
                        case 0x7:
                            _cpuO3 = value;
                            break;
                        case 0x8:
                            WRAM[address] = (byte) value;
                            break;
                        case 0x9:
                            WRAM[address] = (byte) value;
                            break;
                        case 0xA:
                            Timer[0].Target = value;
                            break;
                        case 0xB:
                            Timer[1].Target = value;
                            break;
                        case 0xC:
                            Timer[2].Target = value;
                            break;
                    }
                    break;
                default:
                    WRAM[address] = (byte) value;
                    break;
            }
        }

        private void Write16(int address, int value)
        {
            Write8(address, value & 0xFF);
            Write8(AddWB(address, 1), (value >> 8) & 0xFF);
        }

        private void Write8DP(int address, int value)
        {
            Write8(address | ((_psw & (int) Flags.DirectPage) << 3), value);
        }

        private static int AddWP(int address, int amount)
        {
            return ((address + amount) & 0xFF) | (address & 0xFF00);
        }

        private static int AddWB(int address, int amount)
        {
            return (address + amount) & 0xFFFF;
        }
    }
}