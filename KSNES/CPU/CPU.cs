using System;
using Newtonsoft.Json;
using KSNES.SNESSystem;

namespace KSNES.CPU
{
    public class CPU : ICPU
    {
        private const int DBR = 0;
        private const int K = 1;
        private const int A = 0;
        private const int X = 1;
        private const int Y = 2;
        private const int SP = 3;
        private const int PC = 4;
        private const int DPR = 5;
        private const int IMP = 0;
        private const int IMM = 1;
        private const int IMMm = 2;
        private const int IMMx = 3;
        private const int IMMl = 4;
        private const int DP = 5;
        private const int DPX = 6;
        private const int DPY = 7;
        private const int IDP = 8;
        private const int IDX = 9;
        private const int IDY = 10;
        private const int IDYr = 11;
        private const int IDL = 12;
        private const int ILY = 13;
        private const int SR = 14;
        private const int ISY = 15;
        private const int ABS = 16;
        private const int ABX = 17;
        private const int ABXr = 18;
        private const int ABY = 19;
        private const int ABYr = 20;
        private const int ABL = 21;
        private const int ALX = 22;
        private const int IND = 23;
        private const int IAX = 24;
        private const int IAL = 25;
        private const int REL = 26;
        private const int RLL = 27;
        private const int BM = 28;

        private byte[] _r;
        private ushort[] _br;

        [JsonIgnore]
        private readonly int[] _modes =
        {
            IMP, IDX, IMM, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, DP, DPX, DPX, ILY, IMP, ABYr, IMP, IMP, ABS, ABXr, ABX, ALX,
            ABS, IDX, ABL, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, DPX, DPX, DPX, ILY, IMP, ABYr, IMP, IMP, ABXr,ABXr, ABX, ALX,
            IMP, IDX, IMM, SR, BM, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, BM, DPX, DPX, ILY, IMP, ABYr,IMP, IMP, ABL, ABXr, ABX, ALX,
            IMP, IDX, RLL, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, IND, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, DPX, DPX, DPX, ILY, IMP, ABYr,IMP, IMP, IAX, ABXr, ABX, ALX,
            REL, IDX, RLL, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDY, IDP, ISY, DPX, DPX, DPY, ILY, IMP, ABY, IMP, IMP, ABS, ABX, ABX, ALX,
            IMMx, IDX, IMMx, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, DPX, DPX, DPY, ILY, IMP, ABYr, IMP, IMP, ABXr, ABXr, ABYr,ALX,
            IMMx, IDX, IMM, SR, DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, DP, DPX, DPX, ILY, IMP, ABYr, IMP, IMP, IAL, ABXr, ABX, ALX,
            IMMx, IDX, IMM, SR , DP, DP, DP, IDL, IMP, IMMm, IMP, IMP, ABS, ABS, ABS, ABL,
            REL, IDYr, IDP, ISY, IMMl,DPX, DPX, ILY, IMP, ABYr, IMP, IMP, IAX, ABXr, ABX, ALX,
            IMP, IMP, IMP
        };
            
        [JsonIgnore]
        private readonly int[] _cycles = {
            7, 6, 7, 4, 5, 3, 5, 6, 3, 2, 2, 4, 6, 4, 6, 5,
            2, 5, 5, 7, 5, 4, 6, 6, 2, 4, 2, 2, 6, 4, 7, 5,
            6, 6, 8, 4, 3, 3, 5, 6, 4, 2, 2, 5, 4, 4, 6, 5,
            2, 5, 5, 7, 4, 4, 6, 6, 2, 4, 2, 2, 4, 4, 7, 5,
            6, 6, 2, 4, 7, 3, 5, 6, 3, 2, 2, 3, 3, 4, 6, 5,
            2, 5, 5, 7, 7, 4, 6, 6, 2, 4, 3, 2, 4, 4, 7, 5,
            6, 6, 6, 4, 3, 3, 5, 6, 4, 2, 2, 6, 5, 4, 6, 5,
            2, 5, 5, 7, 4, 4, 6, 6, 2, 4, 4, 2, 6, 4, 7, 5,
            3, 6, 4, 4, 3, 3, 3, 6, 2, 2, 2, 3, 4, 4, 4, 5,
            2, 6, 5, 7, 4, 4, 4, 6, 2, 5, 2, 2, 4, 5, 5, 5,
            2, 6, 2, 4, 3, 3, 3, 6, 2, 2, 2, 4, 4, 4, 4, 5,
            2, 5, 5, 7, 4, 4, 4, 6, 2, 4, 2, 2, 4, 4, 4, 5,
            2, 6, 3, 4, 3, 3, 5, 6, 2, 2, 2, 3, 4, 4, 6, 5,
            2, 5, 5, 7, 6, 4, 6, 6, 2, 4, 3, 3, 6, 4, 7, 5,
            2, 6, 3, 4, 3, 3, 5, 6, 2, 2, 2, 3, 4, 4, 6, 5,
            2, 5, 5, 7, 5, 4, 6, 6, 2, 4, 4, 2, 8, 4, 7, 5,
            7, 7, 7
        };

        [JsonIgnore]
        private readonly Action<int, int>[] _functions;
        
        private bool _n;
        private bool _v;
        private bool _m;
        private bool _x;
        private bool _d;
        private bool _i;
        private bool _z;
        private bool _c;
        private bool _e;

        public bool IrqWanted { get; set; }
        public bool NmiWanted { get; set; }
        private bool _aboWanted;

        private bool _stopped;
        private bool _waiting;

        public int CyclesLeft { get; set; }

        private ISNESSystem _snes;

        public CPU()
        {
            _functions = new Action<int, int>[] {
                Brk, Ora, Cop, Ora, Tsb, Ora, Asl, Ora, Php, Ora, Asla, Phd, Tsb, Ora, Asl, Ora,
                Bpl, Ora, Ora, Ora, Trb, Ora, Asl, Ora, Clc, Ora, Inca, Tcs, Trb, Ora, Asl, Ora,
                Jsr, And, Jsl, And, Bit, And, Rol, And, Plp, And, Rola, Pld, Bit, And, Rol, And,
                Bmi, And, And, And, Bit, And, Rol, And, Sec, And, Deca, Tsc, Bit, And, Rol, And,
                Rti, Eor, Wdm, Eor, Mvp, Eor, Lsr, Eor, Pha, Eor, Lsra, Phk, Jmp, Eor, Lsr, Eor,
                Bvc, Eor, Eor, Eor, Mvn, Eor, Lsr, Eor, Cli, Eor, Phy, Tcd, Jml, Eor, Lsr, Eor,
                Rts, Adc, Per, Adc, Stz, Adc, Ror, Adc, Pla, Adc, Rora, Rtl, Jmp, Adc, Ror, Adc,
                Bvs, Adc, Adc, Adc, Stz, Adc, Ror, Adc, Sei, Adc, Ply, Tdc, Jmp, Adc, Ror, Adc,
                Bra, Sta, Brl, Sta, Sty, Sta, Stx, Sta, Dey, Biti, Txa, Phb, Sty, Sta, Stx, Sta,
                Bcc, Sta, Sta, Sta, Sty, Sta, Stx, Sta, Tya, Sta, Txs, Txy, Stz, Sta, Stz, Sta,
                Ldy, Lda, Ldx, Lda, Ldy, Lda, Ldx, Lda, Tay, Lda, Tax, Plb, Ldy, Lda, Ldx, Lda,
                Bcs, Lda, Lda, Lda, Ldy, Lda, Ldx, Lda, Clv, Lda, Tsx, Tyx, Ldy, Lda, Ldx, Lda,
                Cpy, Cmp, Rep, Cmp, Cpy, Cmp, Dec, Cmp, Iny, Cmp, Dex, Wai, Cpy, Cmp, Dec, Cmp,
                Bne, Cmp, Cmp, Cmp, Pei, Cmp, Dec, Cmp, Cld, Cmp, Phx, Stp, Jml, Cmp, Dec, Cmp,
                Cpx, Sbc, Sep, Sbc, Cpx, Sbc, Inc, Sbc, Inx, Sbc, Nop, Xba, Cpx, Sbc, Inc, Sbc,
                Beq, Sbc, Sbc, Sbc, Pea, Sbc, Inc, Sbc, Sed, Sbc, Plx, Xce, Jsr, Sbc, Inc, Sbc,
                Abo, Nmi, Irq
            };
        }

        public void SetSystem(ISNESSystem system)
        {
            _snes = system;
        }

        public void Reset()
        {
            _r = new byte[2];
            _br = new ushort[6];
            _br[PC] = (ushort) (_snes.Read(0xfffc) | (_snes.Read(0xfffd) << 8));
            _n = false;
            _v = false;
            _m = true;
            _x = true;
            _d = false;
            _i = false;
            _z = false;
            _c = false;
            _e = true;
            IrqWanted = false;
            NmiWanted = false;
            _aboWanted = false;
            _stopped = false;
            _waiting = false;
            CyclesLeft = 7;
        }

        public void Cycle() 
        {
            if (CyclesLeft == 0)
            {
                if (_stopped)
                {
                    CyclesLeft = 1;
                }
                else if (!_waiting)
                {
                    int instr = _snes.Read((_r[K] << 16) | _br[PC]++);
                    CyclesLeft = _cycles[instr];
                    int mode = _modes[instr];
                    if (IrqWanted && !_i || NmiWanted || _aboWanted)
                    {
                        _br[PC]--;
                        if (_aboWanted)
                        {
                            _aboWanted = false;
                            instr = 0x100;
                        }
                        else if (NmiWanted)
                        {
                            NmiWanted = false;
                            instr = 0x101;
                        }
                        else
                        {
                            instr = 0x102;
                        }
                        CyclesLeft = _cycles[instr];
                        mode = _modes[instr];
                    }
                    var (item1, item2) = GetAdr(mode);
                    if (_functions[instr] == null)
                    {
                        Uni();
                    }
                    else
                    {
                        _functions[instr](item1, item2);
                    }
                }
                else
                {
                    if (_aboWanted || IrqWanted || NmiWanted)
                    {
                        _waiting = false;
                    }
                    CyclesLeft = 1;
                }
            }
            CyclesLeft--;
        }

        private byte GetP()
        {
            byte val = 0;
            val |= (byte) (_n ? 0x80 : 0);
            val |= (byte) (_v ? 0x40 : 0);
            val |= (byte) (_m ? 0x20 : 0);
            val |= (byte) (_x ? 0x10 : 0);
            val |= (byte) (_d ? 0x08 : 0);
            val |= (byte) (_i ? 0x04 : 0);
            val |= (byte) (_z ? 0x02 : 0);
            val |= (byte) (_c ? 0x01 : 0);
            return val;
        }

        private void SetP(byte value)
        {
            _n = (value & 0x80) > 0;
            _v = (value & 0x40) > 0;
            _m = (value & 0x20) > 0;
            _x = (value & 0x10) > 0;
            _d = (value & 0x08) > 0;
            _i = (value & 0x04) > 0;
            _z = (value & 0x02) > 0;
            _c = (value & 0x01) > 0;
            if (_x)
            {
                _br[X] &= 0xff;
                _br[Y] &= 0xff;
            }
        }

        private void SetZAndN(int value, bool byt) 
        {
            if (byt)
            {
                _z = (value & 0xff) == 0;
                _n = (value & 0x80) > 0;
                return;
            }
            _z = (value & 0xffff) == 0;
            _n = (value & 0x8000) > 0;
        }

        private static int GetSigned(int value, bool byt) 
        {
            if (byt)
            {
                return (value & 0xff) > 127 ? -(256 - (value & 0xff)) : value & 0xff;
            }
            return value > 32767 ? -(65536 - value) : value;
        }

       private void DoBranch(bool check, int rel)
       {
            if (check)
            {
                CyclesLeft++;
                _br[PC] = (ushort) (_br[PC] + rel);
            }
        }

        private void PushByte(int value) 
        {
            _snes.Write(_br[SP], value);
            _br[SP]--;
        }

        private int PullByte() 
        {
            _br[SP]++;
            return _snes.Read(_br[SP]);
        }

        private void PushWord(int value) 
        {
            PushByte((value & 0xff00) >> 8);
            PushByte(value & 0xff);
        }

        private int PullWord() 
        {
            int value = PullByte();
            value |= PullByte() << 8;
            return value;
        }

        private int ReadWord(int adr, int adrh) 
        {
            int value = _snes.Read(adr);
            value |= _snes.Read(adrh) << 8;
            return value;
        }

        private void WriteWord(int adr, int adrh, int result, bool reversed = false) 
        {
            if (reversed)
            {
                _snes.Write(adrh, (result & 0xff00) >> 8);
                _snes.Write(adr, result & 0xff);
            }
            else
            {
                _snes.Write(adr, result & 0xff);
                _snes.Write(adrh, (result & 0xff00) >> 8);
            }
        }

        private (int, int) GetAdr(int mode) 
        {
            switch (mode)
            {
                case IMP:
                    return (0, 0);
                case IMM:
                    return ((_r[K] << 16) | _br[PC]++, 0);
                case IMMm:
                    if (_m)
                    {
                        return ((_r[K] << 16) | _br[PC]++, 0);
                    }
                    else
                    {
                        int low = (_r[K] << 16) | _br[PC]++;
                        return (low, (_r[K] << 16) | _br[PC]++);
                    }
                case IMMx:
                    if (_x)
                    {
                        return ((_r[K] << 16) | _br[PC]++, 0);
                    }
                    else
                    {
                        int low = (_r[K] << 16) | _br[PC]++;
                        return (low, (_r[K] << 16) | _br[PC]++);
                    }
                case IMMl:
                    int low2 = (_r[K] << 16) | _br[PC]++;
                    return (low2, (_r[K] << 16) | _br[PC]++);
                case DP:
                    int adr = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    return ((_br[DPR] + adr) & 0xffff, (_br[DPR] + adr + 1) & 0xffff);
                case DPX:
                    int adr2 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    return ((_br[DPR] + adr2 + _br[X]) & 0xffff, (_br[DPR] + adr2 + _br[X] + 1) & 0xffff);
                case DPY:
                    int adr3 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    return ((_br[DPR] + adr3 + _br[Y]) & 0xffff, (_br[DPR] + adr3 + _br[Y] + 1) & 0xffff);
                case IDP:
                    int adr4 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer = _snes.Read((_br[DPR] + adr4) & 0xffff);
                    pointer |= _snes.Read((_br[DPR] + adr4 + 1) & 0xffff) << 8;
                    return ((_r[DBR] << 16) + pointer, (_r[DBR] << 16) + pointer + 1);
                case IDX:
                    int adr5 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer2 = _snes.Read((_br[DPR] + adr5 + _br[X]) & 0xffff);
                    pointer2 |= _snes.Read((_br[DPR] + adr5 + _br[X] + 1) & 0xffff) << 8;
                    return ((_r[DBR] << 16) + pointer2, (_r[DBR] << 16) + pointer2 + 1);
                case IDY:
                    int adr6 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer3 = _snes.Read((_br[DPR] + adr6) & 0xffff);
                    pointer3 |= _snes.Read((_br[DPR] + adr6 + 1) & 0xffff) << 8;
                    return ((_r[DBR] << 16) + pointer3 + _br[Y], (_r[DBR] << 16) + pointer3 + _br[Y] + 1);
                case IDYr:
                    int adr7 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer4 = _snes.Read((_br[DPR] + adr7) & 0xffff);
                    pointer4 |= _snes.Read((_br[DPR] + adr7 + 1) & 0xffff) << 8;
                    if (pointer4 >> 8 != (pointer4 + _br[Y]) >> 8 || !_x)
                    {
                        CyclesLeft++;
                    }
                    return ((_r[DBR] << 16) + pointer4 + _br[Y], (_r[DBR] << 16) + pointer4 + _br[Y] + 1);
                case IDL:
                    int adr8 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer5 = _snes.Read((_br[DPR] + adr8) & 0xffff);
                    pointer5 |= _snes.Read((_br[DPR] + adr8 + 1) & 0xffff) << 8;
                    pointer5 |= _snes.Read((_br[DPR] + adr8 + 2) & 0xffff) << 16;
                    return (pointer5, pointer5 + 1);
                case ILY:
                    int adr9 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    if ((_br[DPR] & 0xff) != 0)
                    {
                        CyclesLeft++;
                    }
                    int pointer6 = _snes.Read((_br[DPR] + adr9) & 0xffff);
                    pointer6 |= _snes.Read((_br[DPR] + adr9 + 1) & 0xffff) << 8;
                    pointer6 |= _snes.Read((_br[DPR] + adr9 + 2) & 0xffff) << 16;
                    return (pointer6 + _br[Y], pointer6 + _br[Y] + 1);
                case SR:
                    int adr10 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    return ((_br[SP] + adr10) & 0xffff, (_br[SP] + adr10 + 1) & 0xffff);
                case ISY:
                    int adr11 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    int pointer7 = _snes.Read((_br[SP] + adr11) & 0xffff);
                    pointer7 |= _snes.Read((_br[SP] + adr11 + 1) & 0xffff) << 8;
                    return ((_r[DBR] << 16) + pointer7 + _br[Y], (_r[DBR] << 16) + pointer7 + _br[Y] + 1);
                case ABS:
                    int adr12 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr12 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    return ((_r[DBR] << 16) + adr12, (_r[DBR] << 16) + adr12 + 1);
                case ABX:
                    int adr13 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr13 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    return ((_r[DBR] << 16) + adr13 + _br[X], (_r[DBR] << 16) + adr13 + _br[X] + 1);
                case ABXr:
                    int adr14 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr14 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    if (adr14 >> 8 != (adr14 + _br[X]) >> 8 || !_x)
                    {
                        CyclesLeft++;
                    }
                    return ((_r[DBR] << 16) + adr14 + _br[X], (_r[DBR] << 16) + adr14 + _br[X] + 1);
                case ABY:
                    int adr15 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr15 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    return ((_r[DBR] << 16) + adr15 + _br[Y], (_r[DBR] << 16) + adr15 + _br[Y] + 1);
                case ABYr:
                    int adr16 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr16 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    if (adr16 >> 8 != (adr16 + _br[Y]) >> 8 || !_x)
                    {
                        CyclesLeft++;
                    }
                    return ((_r[DBR] << 16) + adr16 + _br[Y], (_r[DBR] << 16) + adr16 + _br[Y] + 1);
                case ABL:
                    int adr17 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr17 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    adr17 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 16;
                    return (adr17, adr17 + 1);
                case ALX:
                    int adr18 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr18 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    adr18 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 16;
                    return (adr18 + _br[X], adr18 + _br[X] + 1);
                case IND:
                    int adr19 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr19 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    int pointer8 = _snes.Read(adr19);
                    pointer8 |= _snes.Read((adr19 + 1) & 0xffff) << 8;
                    return ((_r[K] << 16) + pointer8, 0);
                case IAX:
                    int adr20 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr20 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    int pointer9 = _snes.Read((_r[K] << 16) | ((adr20 + _br[X]) & 0xffff));
                    pointer9 |= _snes.Read((_r[K] << 16) | ((adr20 + _br[X] + 1) & 0xffff)) << 8;
                    return ((_r[K] << 16) + pointer9, 0);
                case IAL:
                    int adr21 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    adr21 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    int pointer10 = _snes.Read(adr21);
                    pointer10 |= _snes.Read((adr21 + 1) & 0xffff) << 8;
                    pointer10 |= _snes.Read((adr21 + 2) & 0xffff) << 16;
                    return (pointer10, 0);
                case REL:
                    int rel = _snes.Read((_r[K] << 16) | _br[PC]++);
                    return (GetSigned(rel, true), 0);
                case RLL:
                    int rel2 = _snes.Read((_r[K] << 16) | _br[PC]++);
                    rel2 |= _snes.Read((_r[K] << 16) | _br[PC]++) << 8;
                    return (GetSigned(rel2, false), 0);
                case BM:
                    int dest = _snes.Read((_r[K] << 16) | _br[PC]++);
                    int src = _snes.Read((_r[K] << 16) | _br[PC]++);
                    return (dest, src);
            }
            return (0, 0);
        }

        private static void Uni() { }

        private void Adc(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int result;
                if (_d)
                {
                    result = (_br[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0);
                    result += result > 9 ? 6 : 0;
                    result = (_br[A] & 0xf0) + (value & 0xf0) + (result > 0xf ? 0x10 : 0) + (result & 0xf);
                }
                else
                {
                    result = (_br[A] & 0xff) + value + (_c ? 1 : 0);
                }
                _v = (_br[A] & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
                result += _d && result > 0x9f ? 0x60 : 0;
                _c = result > 0xff;
                SetZAndN(result, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (result & 0xff));
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                int result;
                if (_d)
                {
                    result = (_br[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0);
                    result += result > 9 ? 6 : 0;
                    result = (_br[A] & 0xf0) + (value & 0xf0) + (result > 0xf ? 0x10 : 0) + (result & 0xf);
                    result += result > 0x9f ? 0x60 : 0;
                    result = (_br[A] & 0xf00) + (value & 0xf00) + (result > 0xff ? 0x100 : 0) + (result & 0xff);
                    result += result > 0x9ff ? 0x600 : 0;
                    result = (_br[A] & 0xf000) + (value & 0xf000) + (result > 0xfff ? 0x1000 : 0) + (result & 0xfff);
                }
                else
                {
                    result = _br[A] + value + (_c ? 1 : 0);
                }
                _v = (_br[A] & 0x8000) == (value & 0x8000) && (value & 0x8000) != (result & 0x8000);
                result += _d && result > 0x9fff ? 0x6000 : 0;
                _c = result > 0xffff;
                SetZAndN(result, _m);
                _br[A] = (ushort) result;
            }
        }

        private void Sbc(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr) ^ 0xff;
                int result;
                if (_d)
                {
                    result = (_br[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0);
                    result -= result <= 0xf ? 6 : 0;
                    result = (_br[A] & 0xf0) + (value & 0xf0) + (result > 0xf ? 0x10 : 0) + (result & 0xf);
                }
                else
                {
                    result = (_br[A] & 0xff) + value + (_c ? 1 : 0);
                }
                _v = (_br[A] & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
                result -= _d && result <= 0xff ? 0x60 : 0;
                _c = result > 0xff;
                SetZAndN(result, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (result & 0xff));
            }
            else
            {
                int value = ReadWord(adr, adrh) ^ 0xffff;
                CyclesLeft++;
                int result;
                if (_d)
                {
                    result = (_br[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0);
                    result -= result <= 0x0f ? 6 : 0;
                    result = (_br[A] & 0xf0) + (value & 0xf0) + (result > 0xf ? 0x10 : 0) + (result & 0xf);
                    result -= result <= 0xff ? 0x60 : 0;
                    result = (_br[A] & 0xf00) + (value & 0xf00) + (result > 0xff ? 0x100 : 0) + (result & 0xff);
                    result -= result <= 0xfff ? 0x600 : 0;
                    result = (_br[A] & 0xf000) + (value & 0xf000) + (result > 0xfff ? 0x1000 : 0) + (result & 0xfff);
                }
                else
                {
                    result = _br[A] + value + (_c ? 1 : 0);
                }
                _v = (_br[A] & 0x8000) == (value & 0x8000) && (value & 0x8000) != (result & 0x8000);
                result -= _d && result <= 0xffff ? 0x6000 : 0;
                _c = result > 0xffff;
                SetZAndN(result, _m);
                _br[A] = (ushort) result;
            }
        }

        private void Cmp(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr) ^ 0xff;
                int result = (_br[A] & 0xff) + value + 1;
                _c = result > 0xff;
                SetZAndN(result, _m);
            }
            else
            {
                int value = ReadWord(adr, adrh) ^ 0xffff;
                CyclesLeft++;
                int result = _br[A] + value + 1;
                _c = result > 0xffff;
                SetZAndN(result, _m);
            }
        }

        private void Cpx(int adr, int adrh) 
        {
            if (_x)
            {
                int value = _snes.Read(adr) ^ 0xff;
                int result = (_br[X] & 0xff) + value + 1;
                _c = result > 0xff;
                SetZAndN(result, _x);
            }
            else
            {
                int value = ReadWord(adr, adrh) ^ 0xffff;
                CyclesLeft++;
                int result = _br[X] + value + 1;
                _c = result > 0xffff;
                SetZAndN(result, _x);
            }
        }

        private void Cpy(int adr, int adrh)
        {
            if (_x)
            {
                int value = _snes.Read(adr) ^ 0xff;
                int result = (_br[Y] & 0xff) + value + 1;
                _c = result > 0xff;
                SetZAndN(result, _x);
            }
            else
            {
                int value = ReadWord(adr, adrh) ^ 0xffff;
                CyclesLeft++;
                int result = _br[Y] + value + 1;
                _c = result > 0xffff;
                SetZAndN(result, _x);
            }
        }

        private void Dec(int adr, int adrh)
        {
            if (_m)
            {
                int result = (_snes.Read(adr) - 1) & 0xff;
                SetZAndN(result, _m);
                _snes.Write(adr, (byte) result);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                int result = (value - 1) & 0xffff;
                SetZAndN(result, _m);
                WriteWord(adr, adrh, result, true);
            }
        }

        private void Deca(int adr, int adrh)
        {
            if (_m)
            {
                int result = ((_br[A] & 0xff) - 1) & 0xff;
                SetZAndN(result, _m);
                _br[A] = (ushort) (_br[A] & 0xff00 | result);
            }
            else
            {
                _br[A]--;
                SetZAndN(_br[A], _m);
            }
        }

        private void Dex(int adr, int adrh) 
        {
            if (_x)
            {
                int result = ((_br[X] & 0xff) - 1) & 0xff;
                SetZAndN(result, _x);
                _br[X] = (ushort) result;
            }
            else
            {
                _br[X]--;
                SetZAndN(_br[X], _x);
            }
        }

        private void Dey(int adr, int adrh) 
        {
            if (_x)
            {
                int result = ((_br[Y] & 0xff) - 1) & 0xff;
                SetZAndN(result, _x);
                _br[Y] = (ushort) result;
            }
            else
            {
                _br[Y]--;
                SetZAndN(_br[Y], _x);
            }
        }

        private void Inc(int adr, int adrh) 
        {
            if (_m)
            {
                int result = (_snes.Read(adr) + 1) & 0xff;
                SetZAndN(result, _m);
                _snes.Write(adr, (byte) result);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                int result = (value + 1) & 0xffff;
                SetZAndN(result, _m);
                WriteWord(adr, adrh, result, true);
            }
        }

        private void Inca(int adr, int adrh)
        {
            if (_m)
            {
                int result = ((_br[A] & 0xff) + 1) & 0xff;
                SetZAndN(result, _m);
                _br[A] = (ushort) (_br[A] & 0xff00 | result);
            }
            else
            {
                _br[A]++;
                SetZAndN(_br[A], _m);
            }
        }

        private void Inx(int adr, int adrh)
        {
            if (_x)
            {
                int result = ((_br[X] & 0xff) + 1) & 0xff;
                SetZAndN(result, _x);
                _br[X] = (ushort) result;
            }
            else
            {
                _br[X]++;
                SetZAndN(_br[X], _x);
            }
        }

        private void Iny(int adr, int adrh) 
        {
            if (_x)
            {
                int result = ((_br[Y] & 0xff) + 1) & 0xff;
                SetZAndN(result, _x);
                _br[Y] = (ushort) result;
            }
            else
            {
                _br[Y]++;
                SetZAndN(_br[Y], _x);
            }
        }

        private void And(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (_br[A] & value & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                _br[A] &= (ushort) value;
                SetZAndN(_br[A], _m);
            }
        }

        private void Eor(int adr, int adrh)
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _br[A] = (ushort) ((_br[A] & 0xff00) | ((_br[A] ^ value) & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                _br[A] ^= (ushort) value;
                SetZAndN(_br[A], _m);
            }
        }

        private void Ora(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _br[A] = (ushort) ((_br[A] & 0xff00) | ((_br[A] | value) & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                _br[A] |= (ushort) value;
                SetZAndN(_br[A], _m);
            }
        }

        private void Bit(int adr, int adrh)
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int result = _br[A] & 0xff & value;
                _z = result == 0;
                _n = (value & 0x80) > 0;
                _v = (value & 0x40) > 0;
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                int result = _br[A] & value;
                _z = result == 0;
                _n = (value & 0x8000) > 0;
                _v = (value & 0x4000) > 0;
            }
        }

        private void Biti(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int result = _br[A] & 0xff & value;
                _z = result == 0;
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft++;
                int result = _br[A] & value;
                _z = result == 0;
            }
        }

        private void Trb(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int result = _br[A] & 0xff & value;
                value = value & ~(_br[A] & 0xff) & 0xff;
                _z = result == 0;
                _snes.Write(adr, (byte) value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                int result = _br[A] & value;
                value = value & ~_br[A] & 0xffff;
                _z = result == 0;
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Tsb(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int result = _br[A] & 0xff & value;
                value = (value | (_br[A] & 0xff)) & 0xff;
                _z = result == 0;
                _snes.Write(adr, (byte) value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                int result = _br[A] & value;
                value = (value | _br[A]) & 0xffff;
                _z = result == 0;
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Asl(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _c = (value & 0x80) > 0;
                value <<= 1;
                SetZAndN(value, _m);
                _snes.Write(adr, value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                _c = (value & 0x8000) > 0;
                value <<= 1;
                SetZAndN(value, _m);
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Asla(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _br[A] & 0xff;
                _c = (value & 0x80) > 0;
                value <<= 1;
                SetZAndN(value, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (value & 0xff));
            }
            else
            {
                _c = (_br[A] & 0x8000) > 0;
                CyclesLeft += 2;
                _br[A] <<= 1;
                SetZAndN(_br[A], _m);
            }
        }

        private void Lsr(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _c = (value & 0x1) > 0;
                value >>= 1;
                SetZAndN(value, _m);
                _snes.Write(adr, value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                _c = (value & 0x1) > 0;
                value >>= 1;
                SetZAndN(value, _m);
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Lsra(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _br[A] & 0xff;
                _c = (value & 0x1) > 0;
                value >>= 1;
                SetZAndN(value, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (value & 0xff));
            }
            else
            {
                _c = (_br[A] & 0x1) > 0;
                CyclesLeft += 2;
                _br[A] >>= 1;
                SetZAndN(_br[A], _m);
            }
        }

        private void Rol(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                value = (value << 1) | (_c ? 1 : 0);
                _c = (value & 0x100) > 0;
                SetZAndN(value, _m);
                _snes.Write(adr, (byte) value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                value = (value << 1) | (_c ? 1 : 0);
                _c = (value & 0x10000) > 0;
                SetZAndN(value, _m);
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Rola(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _br[A] & 0xff;
                value = (value << 1) | (_c ? 1 : 0);
                _c = (value & 0x100) > 0;
                SetZAndN(value, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (value & 0xff));
            }
            else
            {
                CyclesLeft += 2;
                int value = (_br[A] << 1) | (_c ? 1 : 0);
                _c = (value & 0x10000) > 0;
                SetZAndN(value, _m);
                _br[A] = (ushort) value;
            }
        }

        private void Ror(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                int carry = value & 0x1;
                value = (value >> 1) | (_c ? 0x80 : 0);
                _c = carry > 0;
                SetZAndN(value, _m);
                _snes.Write(adr, (byte) value);
            }
            else
            {
                int value = ReadWord(adr, adrh);
                CyclesLeft += 2;
                int carry = value & 0x1;
                value = (value >> 1) | (_c ? 0x8000 : 0);
                _c = carry > 0;
                SetZAndN(value, _m);
                WriteWord(adr, adrh, value, true);
            }
        }

        private void Rora(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _br[A] & 0xff;
                int carry = value & 0x1;
                value = (value >> 1) | (_c ? 0x80 : 0);
                _c = carry > 0;
                SetZAndN(value, _m);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (value & 0xff));
            }
            else
            {
                CyclesLeft += 2;
                int carry = _br[A] & 0x1;
                int value = (_br[A] >> 1) | (_c ? 0x8000 : 0);
                _c = carry > 0;
                SetZAndN(value, _m);
                _br[A] = (ushort) value;
            }
        }

        private void Bcc(int adr, int adrh) 
        {
            DoBranch(!_c, adr);
        }

        private void Bcs(int adr, int adrh)
        {
            DoBranch(_c, adr);
        }

        private void Beq(int adr, int adrh) 
        {
            DoBranch(_z, adr);
        }

        private void Bmi(int adr, int adrh) 
        {
            DoBranch(_n, adr);
        }

        private void Bne(int adr, int adrh) 
        {
            DoBranch(!_z, adr);
        }

        private void Bpl(int adr, int adrh) 
        {
            DoBranch(!_n, adr);
        }

        private void Bra(int adr, int adrh) 
        {
            _br[PC] = (ushort) (_br[PC] + adr);
        }

        private void Bvc(int adr, int adrh) 
        {
            DoBranch(!_v, adr);
        }

        private void Bvs(int adr, int adrh)
        {
            DoBranch(_v, adr);
        }

        private void Brl(int adr, int adrh)
        {
            _br[PC] = (ushort) (_br[PC] + adr);
        }

        private void Jmp(int adr, int adrh)
        {
            _br[PC] = (ushort) (adr & 0xffff);
        }

        private void Jml(int adr, int adrh) 
        {
            _r[K] = (byte) ((adr & 0xff0000) >> 16);
            _br[PC] = (ushort) (adr & 0xffff);
        }

        private void Jsl(int adr, int adrh)
        {
            int pushPc = (_br[PC] - 1) & 0xffff;
            PushByte(_r[K]);
            PushWord(pushPc);
            _r[K] = (byte) ((adr & 0xff0000) >> 16);
            _br[PC] = (ushort) (adr & 0xffff);
        }

        private void Jsr(int adr, int adrh)
        {
            int pushPc = (_br[PC] - 1) & 0xffff;
            PushWord(pushPc);
            _br[PC] = (ushort) (adr & 0xffff);
        }

        private void Rtl(int adr, int adrh) 
        {
            int pullPc = PullWord();
            _r[K] = (byte) PullByte();
            _br[PC] = (ushort) (pullPc + 1);
        }

        private void Rts(int adr, int adrh)
        {
            int pullPc = PullWord();
            _br[PC] = (ushort) (pullPc + 1);
        }

        private void Brk(int adr, int adrh)
        {
            int pushPc = (_br[PC] + 1) & 0xffff;
            PushByte(_r[K]);
            PushWord(pushPc);
            PushByte(GetP());
            CyclesLeft++;
            _i = true;
            _d = false;
            _r[K] = 0;
            _br[PC] = (ushort) (_snes.Read(0xffe6) | (_snes.Read(0xffe7) << 8));
        }

        private void Cop(int adr, int adrh) 
        {
            PushByte(_r[K]);
            PushWord(_br[PC]);
            PushByte(GetP());
            CyclesLeft++;
            _i = true;
            _d = false;
            _r[K] = 0;
            _br[PC] = (ushort) (_snes.Read(0xffe4) | (_snes.Read(0xffe5) << 8));
        }

        private void Abo(int adr, int adrh)
        {
            PushByte(_r[K]);
            PushWord(_br[PC]);
            PushByte(GetP());
            CyclesLeft++;
            _i = true;
            _d = false;
            _r[K] = 0;
            _br[PC] = (ushort) (_snes.Read(0xffe8) | (_snes.Read(0xffe9) << 8));
        }

        private void Nmi(int adr, int adrh)
        {
            PushByte(_r[K]);
            PushWord(_br[PC]);
            PushByte(GetP());
            CyclesLeft++;
            _i = true;
            _d = false;
            _r[K] = 0;
            _br[PC] = (ushort) (_snes.Read(0xffea) | (_snes.Read(0xffeb) << 8));
        }

        private void Irq(int adr, int adrh) 
        {
            PushByte(_r[K]);
            PushWord(_br[PC]);
            PushByte(GetP());
            CyclesLeft++;
            _i = true;
            _d = false;
            _r[K] = 0;
            _br[PC] = (ushort) (_snes.Read(0xffee) | (_snes.Read(0xffef) << 8));
        }

        private void Rti(int adr, int adrh) 
        {
            SetP((byte) PullByte());
            CyclesLeft++;
            int pullPc = PullWord();
            _r[K] = (byte) PullByte();
            _br[PC] = (ushort) pullPc;
        }

        private void Clc(int adr, int adrh) 
        {
            _c = false;
        }

        private void Cld(int adr, int adrh) 
        {
            _d = false;
        }

        private void Cli(int adr, int adrh) 
        {
            _i = false;
        }

        private void Clv(int adr, int adrh)
        {
            _v = false;
        }

        private void Sec(int adr, int adrh) 
        {
            _c = true;
        }

        private void Sed(int adr, int adrh) 
        {
            _d = true;
        }

        private void Sei(int adr, int adrh)
        {
            _i = true;
        }

        private void Rep(int adr, int adrh)
        {
            int value = _snes.Read(adr);
            SetP((byte) (GetP() & ~value));
        }

        private void Sep(int adr, int adrh) 
        {
            int value = _snes.Read(adr);
            SetP((byte) (GetP() | value));
        }

        private void Lda(int adr, int adrh) 
        {
            if (_m)
            {
                int value = _snes.Read(adr);
                _br[A] = (ushort) ((_br[A] & 0xff00) | (value & 0xff));
                SetZAndN(value, _m);
            }
            else
            {
                CyclesLeft++;
                _br[A] = (ushort) ReadWord(adr, adrh);
                SetZAndN(_br[A], _m);
            }
        }

        private void Ldx(int adr, int adrh) 
        {
            if (_x)
            {
                _br[X] = (ushort) _snes.Read(adr);
                SetZAndN(_br[X], _x);
            }
            else
            {
                CyclesLeft++;
                _br[X] = (ushort) ReadWord(adr, adrh);
                SetZAndN(_br[X], _x);
            }
        }

        private void Ldy(int adr, int adrh) 
        {
            if (_x)
            {
                _br[Y] = (ushort) _snes.Read(adr);
                SetZAndN(_br[Y], _x);
            }
            else
            {
                CyclesLeft++;
                _br[Y] = (ushort) ReadWord(adr, adrh);
                SetZAndN(_br[Y], _x);
            }
        }

        private void Sta(int adr, int adrh)
        {
            if (_m)
            {
                _snes.Write(adr, (byte) (_br[A] & 0xff));
            }
            else
            {
                CyclesLeft++;
                WriteWord(adr, adrh, _br[A]);
            }
        }

        private void Stx(int adr, int adrh)
        {
            if (_x)
            {
                _snes.Write(adr, (byte) (_br[X] & 0xff));
            }
            else
            {
                CyclesLeft++;
                WriteWord(adr, adrh, _br[X]);
            }
        }

        private void Sty(int adr, int adrh) 
        {
            if (_x)
            {
                _snes.Write(adr, (byte) (_br[Y] & 0xff));
            }
            else
            {
                CyclesLeft++;
                WriteWord(adr, adrh, _br[Y]);
            }
        }

        private void Stz(int adr, int adrh) 
        {
            if (_m)
            {
                _snes.Write(adr, 0);
            }
            else
            {
                CyclesLeft++;
                WriteWord(adr, adrh, 0);
            }
        }

        private void Mvn(int adr, int adrh) 
        {
            _r[DBR] = (byte) adr;
            _snes.Write((adr << 16) | _br[Y], _snes.Read((adrh << 16) | _br[X]));
            _br[A]--;
            _br[X]++;
            _br[Y]++;
            if (_br[A] != 0xffff)
            {
                _br[PC] -= 3;
            }
            if (_x)
            {
                _br[X] &= 0xff;
                _br[Y] &= 0xff;
            }
        }

        private void Mvp(int adr, int adrh) 
        {
            _r[DBR] = (byte) adr;
            _snes.Write((adr << 16) | _br[Y], _snes.Read((adrh << 16) | _br[X]));
            _br[A]--;
            _br[X]--;
            _br[Y]--;
            if (_br[A] != 0xffff)
            {
                _br[PC] -= 3;
            }
            if (_x)
            {
                _br[X] &= 0xff;
                _br[Y] &= 0xff;
            }
        }

        private static void Nop(int adr, int adrh) { }

        private static void Wdm(int adr, int adrh) { }

        private void Pea(int adr, int adrh) 
        {
            PushWord(ReadWord(adr, adrh));
        }

        private void Pei(int adr, int adrh) 
        {
            PushWord(ReadWord(adr, adrh));
        }

        private void Per(int adr, int adrh) 
        {
            PushWord((_br[PC] + adr) & 0xffff);
        }

        private void Pha(int adr, int adrh)
        {
            if (_m)
            {
                PushByte((byte) (_br[A] & 0xff));
            }
            else
            {
                CyclesLeft++;
                PushWord(_br[A]);
            }
        }

        private void Phx(int adr, int adrh)
        {
            if (_x)
            {
                PushByte((byte) (_br[X] & 0xff));
            }
            else
            {
                CyclesLeft++;
                PushWord(_br[X]);
            }
        }

        private void Phy(int adr, int adrh) 
        {
            if (_x)
            {
                PushByte((byte) (_br[Y] & 0xff));
            }
            else
            {
                CyclesLeft++;
                PushWord(_br[Y]);
            }
        }

        private void Pla(int adr, int adrh) 
        {
            if (_m)
            {
                _br[A] = (ushort) ((_br[A] & 0xff00) | (PullByte() & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                CyclesLeft++;
                _br[A] = (ushort) PullWord();
                SetZAndN(_br[A], _m);
            }
        }

        private void Plx(int adr, int adrh)
        {
            if (_x)
            {
                _br[X] = (ushort) PullByte();
                SetZAndN(_br[X], _m);
            }
            else
            {
                CyclesLeft++;
                _br[X] = (ushort) PullWord();
                SetZAndN(_br[X], _m);
            }
        }

        private void Ply(int adr, int adrh)
        {
            if (_x)
            {
                _br[Y] = (ushort) PullByte();
                SetZAndN(_br[Y], _m);
            }
            else
            {
                CyclesLeft++;
                _br[Y] = (ushort) PullWord();
                SetZAndN(_br[Y], _m);
            }
        }

        private void Phb(int adr, int adrh) 
        {
            PushByte(_r[DBR]);
        }

        private void Phd(int adr, int adrh) 
        {
            PushWord(_br[DPR]);
        }

        private void Phk(int adr, int adrh) 
        {
            PushByte(_r[K]);
        }

        private void Php(int adr, int adrh)
        {
            PushByte(GetP());
        }

        private void Plb(int adr, int adrh) 
        {
            _r[DBR] = (byte) PullByte();
            SetZAndN(_r[DBR], true);
        }

        private void Pld(int adr, int adrh) 
        {
            _br[DPR] = (ushort) PullWord();
            SetZAndN(_br[DPR], false);
        }

        private void Plp(int adr, int adrh)
        {
            SetP((byte) PullByte());
        }

        private void Stp(int adr, int adrh) 
        {
            _stopped = true;
        }

        private void Wai(int adr, int adrh) 
        {
            _waiting = true;
        }

        private void Tax(int adr, int adrh) 
        {
            if (_x)
            {
                _br[X] = (ushort) (_br[A] & 0xff);
                SetZAndN(_br[X], _x);
            }
            else
            {
                _br[X] = _br[A];
                SetZAndN(_br[X], _x);
            }
        }

        private void Tay(int adr, int adrh) 
        {
            if (_x)
            {
                _br[Y] = (ushort) (_br[A] & 0xff);
                SetZAndN(_br[Y], _x);
            }
            else
            {
                _br[Y] = _br[A];
                SetZAndN(_br[Y], _x);
            }
        }

        private void Tsx(int adr, int adrh) 
        {
            if (_x)
            {
                _br[X] = (ushort) (_br[SP] & 0xff);
                SetZAndN(_br[X], _x);
            }
            else
            {
                _br[X] = _br[SP];
                SetZAndN(_br[X], _x);
            }
        }

        private void Txa(int adr, int adrh) 
        {
            if (_m)
            {
                _br[A] = (ushort) ((_br[A] & 0xff00) | (_br[X] & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                _br[A] = _br[X];
                SetZAndN(_br[A], _m);
            }
        }

        private void Txs(int adr, int adrh) 
        {
            _br[SP] = _br[X];
        }

        private void Txy(int adr, int adrh)
        {
            if (_x)
            {
                _br[Y] = (ushort) (_br[X] & 0xff);
                SetZAndN(_br[Y], _x);
            }
            else
            {
                _br[Y] = _br[X];
                SetZAndN(_br[Y], _x);
            }
        }

        private void Tya(int adr, int adrh) 
        {
            if (_m)
            {
                _br[A] = (ushort) ((_br[A] & 0xff00) | (_br[Y] & 0xff));
                SetZAndN(_br[A], _m);
            }
            else
            {
                _br[A] = _br[Y];
                SetZAndN(_br[A], _m);
            }
        }

        private void Tyx(int adr, int adrh) 
        {
            if (_x)
            {
                _br[X] = (ushort) (_br[Y] & 0xff);
                SetZAndN(_br[X], _x);
            }
            else
            {
                _br[X] = _br[Y];
                SetZAndN(_br[X], _x);
            }
        }

        private void Tcd(int adr, int adrh) 
        {
            _br[DPR] = _br[A];
            SetZAndN(_br[DPR], false);
        }

        private void Tcs(int adr, int adrh) 
        {
            _br[SP] = _br[A];
        }

        private void Tdc(int adr, int adrh)
        {
            _br[A] = _br[DPR];
            SetZAndN(_br[A], false);
        }

        private void Tsc(int adr, int adrh)
        {
            _br[A] = _br[SP];
            SetZAndN(_br[A], false);
        }

        private void Xba(int adr, int adrh)
        {
            int low = _br[A] & 0xff;
            int high = (_br[A] & 0xff00) >> 8;
            _br[A] = (ushort) ((low << 8) | high);
            SetZAndN(_br[A], true);
        }

        private void Xce(int adr, int adrh) 
        {
            bool temp = _c;
            _c = _e;
            _e = temp;
            if (_e)
            {
                _m = true;
                _x = true;
            }
            if (_x)
            {
                _br[X] &= 0xff;
                _br[Y] &= 0xff;
            }
        }
    }
}