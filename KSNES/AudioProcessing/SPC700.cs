using System;
using Newtonsoft.Json;

namespace KSNES.AudioProcessing
{
    public class SPC700 : ISPC700
    {
        private const int A = 0;
        private const int X = 1;
        private const int Y = 2;
        private const int SP = 3;
        private const int PC = 0;

        private const int IMP = 0;
        private const int REL = 1;
        private const int DP = 2;
        private const int DPR = 3;
        private const int ABS = 4;
        private const int IND = 5;
        private const int IDX = 6;
        private const int IMM = 7;
        private const int DPX = 8;
        private const int ABX = 9;
        private const int ABY = 10;
        private const int IDY = 11;
        private const int DD = 12;
        private const int II = 13;
        private const int DI = 14;
        private const int DPY = 15;
        private const int ABB = 16;
        private const int DXR = 17;
        private const int IAX = 18;
        private const int IPI = 19;

        private IAPU _apu;
        private byte[] _r;
        private readonly ushort[] _br = new ushort[1];
        private int _cyclesLeft;

        private bool _n;
        private bool _v;
        private bool _p;
        private bool _b;
        private bool _h;
        private bool _i;
        private bool _z;
        private bool _c;

        [JsonIgnore] 
        private readonly int[] _modes =
        {
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMP, ABS, IMP,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, ABS, IAX,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMP, DPR, REL,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, DP , ABS,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMP, ABS, DP ,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, ABS, ABS,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMP, DPR, IMP,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, DP , IMP,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMM, IMP, DI ,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, IMP, IMP,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, DD , ABB, DP , ABS, IMM, IMP, IPI,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DI , II , DP , DPX, IMP, IMP, IMP, IPI,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, ABS, ABB, DP , ABS, IMM, IMP, IMP,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DP , DPY, DP , DPX, IMP, IMP, DXR, IMP,
            IMP, IMP, DP , DPR, DP , ABS, IND, IDX, IMM, ABS, ABB, DP , ABS, IMP, IMP, IMP,
            REL, IMP, DP , DPR, DPX, ABX, ABY, IDY, DP , DPY, DD , DPX, IMP, IMP, REL, IMP
        };

        [JsonIgnore]
        private readonly Action<int, int, int>[] _functions;


        [JsonIgnore]
        private readonly int[] _cycles = 
        {
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 5, 4, 5, 4, 6, 8,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 6, 5, 2, 2, 4, 6,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 5, 4, 5, 4, 5, 4,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 6, 5, 2, 2, 3, 8,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 4, 4, 5, 4, 6, 6,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 4, 5, 2, 2, 4, 3,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 4, 4, 5, 4, 5, 5,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 5, 5, 2, 2, 3, 6,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 5, 4, 5, 2, 4, 5,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 5, 5, 2, 2, 12,5,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 6, 4, 4, 5, 2, 4, 4,
            2, 8, 4, 5, 4, 5, 5, 6, 5, 5, 5, 5, 2, 2, 3, 4,
            2, 8, 4, 5, 4, 5, 4, 7, 2, 5, 6, 4, 5, 2, 4, 9,
            2, 8, 4, 5, 5, 6, 6, 7, 4, 5, 5, 5, 2, 2, 6, 3,
            2, 8, 4, 5, 3, 4, 3, 6, 2, 4, 5, 3, 4, 3, 4, 3,
            2, 8, 4, 5, 4, 5, 5, 6, 3, 4, 5, 4, 2, 2, 4, 3
        };

        public SPC700()
        {
            _functions = new Action<int, int, int>[]
            {
                Nop, TCall, Set1, Bbs, Or, Or, Or, Or, Or, Orm, Or1, Asl, Asl, Pushp, TSet1, Brk,
                Bpl, TCall, Clr1, Bbc, Or, Or, Or, Or, Orm, Orm, Decw, Asl, Asla, Decx, Cmpx, Jmp,
                Clrp, TCall, Set1, Bbs, And, And, And, And, And, Andm, Or1n, Rol, Rol, Pusha, Cbne, Bra,
                Bmi, TCall, Clr1, Bbc, And, And, And, And, Andm, Andm, Incw, Rol, Rola, Incx,  Cmpx, Call,
                Setp, TCall, Set1, Bbs, Eor, Eor, Eor, Eor, Eor, Eorm, And1, Lsr, Lsr, Pushx, TClr1, PCall,
                Bvc, TCall, Clr1, Bbc, Eor, Eor, Eor, Eor, Eorm, Eorm, Cmpw, Lsr, Lsra, Movxa, Cmpy, Jmp,
                Clrc, TCall, Set1, Bbs, Cmp, Cmp, Cmp, Cmp, Cmp, Cmpm, And1n, Ror, Ror, Pushy, Dbnz, Ret,
                Bvs, TCall, Clr1, Bbc, Cmp, Cmp, Cmp, Cmp, Cmpm, Cmpm, Addw, Ror, Rora, Movax, Cmpy, Reti,
                Setc, TCall, Set1, Bbs, Adc, Adc, Adc, Adc, Adc, Adcm, Eor1, Dec, Dec, Movy, Popp, Movm,
                Bcc, TCall, Clr1, Bbc, Adc, Adc, Adc, Adc, Adcm, Adcm, Subw, Dec, Deca, Movxp, Div , Xcn,
                Ei, TCall, Set1, Bbs, Sbc, Sbc, Sbc, Sbc, Sbc , Sbcm, Mov1, Inc, Inc, Cmpy, Popa, Movs,
                Bcs, TCall, Clr1, Bbc, Sbc, Sbc, Sbc, Sbc, Sbcm, Sbcm, Movw, Inc, Inca, Movpx, Das, Mov,
                Di, TCall, Set1, Bbs, Movs, Movs, Movs, Movs, Cmpx, Movsx, Mov1s, Movsy, Movsy, Movx, Popx, Mul,
                Bne, TCall, Clr1, Bbc, Movs, Movs, Movs, Movs, Movsx, Movsx, Movws, Movsy, Decy, Movay, Cbne, Daa,
                Clrv, TCall, Set1, Bbs, Mov, Mov, Mov, Mov, Mov, Movx, Not1, Movy, Movy, Notc, Popy, Sleep,
                Beq, TCall, Clr1, Bbc, Mov, Mov, Mov, Mov, Movx, Movx, Movm, Movy, Incy, Movya, Dbnzy, Stop
            };
        }

        public void Reset()
        {
            _r = new byte[4];
            _br[PC] = (ushort) (_apu.Read(0xfffe) | (_apu.Read(0xffff) << 8));
            _n = false;
            _v = false;
            _p = false;
            _b = false;
            _h = false;
            _i = false;
            _z = false;
            _c = false;
            _cyclesLeft = 7;
        }

        public void SetAPU(IAPU apu)
        {
            _apu = apu;
        }

        public void Cycle() 
        {
            if (_cyclesLeft == 0)
            {
                byte instr = _apu.Read(_br[PC]++);
                int mode = _modes[instr];
                _cyclesLeft = _cycles[instr];
                try
                {
                    (int item1, int item2) = GetAdr(mode);
                    _functions[instr](item1, item2, instr);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            _cyclesLeft--;
        }

        private int GetP() 
        {
            int value = 0;
            value |= _n ? 0x80 : 0;
            value |= _v ? 0x40 : 0;
            value |= _p ? 0x20 : 0;
            value |= _b ? 0x10 : 0;
            value |= _h ? 0x08 : 0;
            value |= _i ? 0x04 : 0;
            value |= _z ? 0x02 : 0;
            value |= _c ? 0x01 : 0;
            return value;
        }

        private void SetP(int value) 
        {
            _n = (value & 0x80) > 0;
            _v = (value & 0x40) > 0;
            _p = (value & 0x20) > 0;
            _b = (value & 0x10) > 0;
            _h = (value & 0x08) > 0;
            _i = (value & 0x04) > 0;
            _z = (value & 0x02) > 0;
            _c = (value & 0x01) > 0;
        }

        private void SetZAndN(byte val) 
        {
            val &= 0xff;
            _n = val > 0x7f;
            _z = val == 0;
        }

        private static int GetSigned(byte val) 
        {
            if (val > 127)
            {
                return -(256 - val);
            }
            return val;
        }

        private void DoBranch(bool check, int rel)
        {
            if (check)
            {
                _br[PC] = (ushort) (_br[PC] + rel);
                _cyclesLeft += 2;
            }
        }

        private void Push(byte value)
        {
            _apu.Write(_r[SP] | 0x100, value);
            _r[SP]--;
        }

        private int Pop()
        {
            _r[SP]++;
            return _apu.Read(_r[SP] | 0x100);
        }

        private (int, int) GetAdr(int mode)
        {
            switch (mode)
            {
                case IMP:
                    return (0, 0);
                case REL:
                    byte rel = _apu.Read(_br[PC]++);
                    return (GetSigned(rel), 0);
                case DP:
                    int adr = _apu.Read(_br[PC]++); 
                    return (adr | (_p ? 0x100 : 0), ((adr + 1) & 0xff) | (_p ? 0x100 : 0));
                case DPR:
                    byte adr2 = _apu.Read(_br[PC]++);
                    byte rel2 = _apu.Read(_br[PC]++);
                    return (adr2 | (_p ? 0x100 : 0), GetSigned(rel2));
                case ABS:
                    int adr3 = _apu.Read(_br[PC]++);
                    adr3 |= _apu.Read(_br[PC]++) << 8;
                    return (adr3, 0);
                case IND:
                    return (_r[X] | (_p ? 0x100 : 0), 0);
                case IDX:
                    int pointer = _apu.Read(_br[PC]++);
                    int adr4 = _apu.Read(((pointer + _r[X]) & 0xff) | (_p ? 0x100 : 0));
                    adr4 |= _apu.Read(((pointer + 1 + _r[X]) & 0xff) | (_p ? 0x100 : 0)) << 8;
                    return (adr4, 0);
                case IMM:
                    return (_br[PC]++, 0);
                case DPX:
                    int adr5 = _apu.Read(_br[PC]++);
                    return (((adr5 + _r[X]) & 0xff) | (_p ? 0x100 : 0), 0);
                case ABX:
                    int adr6 = _apu.Read(_br[PC]++);
                    adr6 |= _apu.Read(_br[PC]++) << 8;
                    return ((adr6 + _r[X]) & 0xffff, 0);
                case ABY:
                    int adr7 = _apu.Read(_br[PC]++);
                    adr7 |= _apu.Read(_br[PC]++) << 8;
                    return ((adr7 + _r[Y]) & 0xffff, 0);
                case IDY:
                    int pointer2 = _apu.Read(_br[PC]++);
                    int adr8 = _apu.Read(pointer2 | (_p ? 0x100 : 0));
                    adr8 |= _apu.Read(((pointer2 + 1) & 0xff) | (_p ? 0x100 : 0)) << 8;
                    return ((adr8 + _r[Y]) & 0xffff, 0);
                case DD:
                    int adr9 = _apu.Read(_br[PC]++);
                    int adr10 = _apu.Read(_br[PC]++);
                    return (adr9 | (_p ? 0x100 : 0), adr10 | (_p ? 0x100 : 0));
                case II:
                    return (_r[Y] | (_p ? 0x100 : 0), _r[X] | (_p ? 0x100 : 0));
                case DI:
                    int imm = _br[PC]++;
                    int adr11 = _apu.Read(_br[PC]++);
                    return (imm, adr11 | (_p ? 0x100 : 0));
                case DPY:
                    int adr12 = _apu.Read(_br[PC]++);
                    return (((adr12 + _r[Y]) & 0xff) | (_p ? 0x100 : 0), 0);
                case ABB:
                    int adr13 = _apu.Read(_br[PC]++);
                    adr13 |= _apu.Read(_br[PC]++) << 8;
                    return (adr13 & 0x1fff, adr13 >> 13);
                case DXR:
                    int adr14 = _apu.Read(_br[PC]++);
                    int rel3 = GetSigned(_apu.Read(_br[PC]++));
                    return (((adr14 + _r[X]) & 0xff) | (_p ? 0x100 : 0), rel3);
                case IAX:
                    int adr15 = _apu.Read(_br[PC]++);
                    adr15 |= _apu.Read(_br[PC]++) << 8;
                    int radr = _apu.Read((adr15 + _r[X]) & 0xffff);
                    radr |= _apu.Read((adr15 + _r[X] + 1) & 0xffff) << 8;
                    return (radr, 0);
                case IPI:
                    return (_r[X]++ | (_p ? 0x100 : 0), 0);
            }
            return (0 , 0);
        }

        private static void Nop(int adr, int adrh, int instr) { }

        private void Clrp(int adr, int adrh, int instr) 
        {
            _p = false;
        }

        private void Setp(int adr, int adrh, int instr)
        {
            _p = true;
        }

        private void Clrc(int adr, int adrh, int instr) 
        {
            _c = false;
        }

        private void Setc(int adr, int adrh, int instr)
        {
            _c = true;
        }

        private void Ei(int adr, int adrh, int instr)
        {
            _i = true;
        }

        private void Di(int adr, int adrh, int instr)
        {
            _i = false;
        }
        
        private void Clrv(int adr, int adrh, int instr)
        {
            _v = false;
            _h = false;
        }

        private void Bpl(int adr, int adrh, int instr)
        {
            DoBranch(!_n, adr);
        }

        private void Bmi(int adr, int adrh, int instr) 
        {
            DoBranch(_n, adr);
        }

        private void Bvc(int adr, int adrh, int instr)
        {
            DoBranch(!_v, adr);
        }

        private void Bvs(int adr, int adrh, int instr)
        {
            DoBranch(_v, adr);
        }

        private void Bcc(int adr, int adrh, int instr)
        {
            DoBranch(!_c, adr);
        }

        private void Bcs(int adr, int adrh, int instr) 
        {
            DoBranch(_c, adr);
        }

        private void Bne(int adr, int adrh, int instr) 
        {
            DoBranch(!_z, adr);
        }

        private void Beq(int adr, int adrh, int instr)
        {
            DoBranch(_z, adr);
        }

        private void TCall(int adr, int adrh, int instr)
        {
            Push((byte) (_br[PC] >> 8));
            Push((byte) (_br[PC] & 0xff));
            int padr = 0xffc0 + ((15 - (instr >> 4)) << 1);
            _br[PC] = (ushort) (_apu.Read(padr) | (_apu.Read(padr + 1) << 8));
        }

        private void Set1(int adr, int adrh, int instr)
        {
            int value = _apu.Read(adr);
            value |= 1 << (instr >> 5);
            _apu.Write(adr, (byte) value);
        }

        private void Clr1(int adr, int adrh, int instr)
        {
            int value = _apu.Read(adr);
            value &= ~(1 << (instr >> 5));
            _apu.Write(adr, (byte) value);
        }

        private void Bbs(int adr, int adrh, int instr)
        {
            byte value = _apu.Read(adr);
            DoBranch((value & (1 << (instr >> 5))) > 0, adrh);
        }

        private void Bbc(int adr, int adrh, int instr)
        {
            byte value = _apu.Read(adr);
            DoBranch((value & (1 << (instr >> 5))) == 0, adrh);
        }

        private void Or(int adr, int adrh, int instr)
        {
            _r[A] |= _apu.Read(adr);
            SetZAndN(_r[A]);
        }

        private void Orm(int adr, int adrh, int instr) 
        {
            byte value = _apu.Read(adrh);
            value |= _apu.Read(adr);
            _apu.Write(adrh, value);
            SetZAndN(value);
        }

        private void And(int adr, int adrh, int instr) 
        {
            _r[A] &= _apu.Read(adr);
            SetZAndN(_r[A]);
        }

        private void Andm(int adr, int adrh, int instr) 
        {
            byte value = _apu.Read(adrh);
            value &= _apu.Read(adr);
            _apu.Write(adrh, value);
            SetZAndN(value);
        }

        private void Eor(int adr, int adrh, int instr) 
        {
            _r[A] ^= _apu.Read(adr);
            SetZAndN(_r[A]);
        }

        private void Eorm(int adr, int adrh, int instr) 
        {
            byte value = _apu.Read(adrh);
            value ^= _apu.Read(adr);
            _apu.Write(adrh, value);
            SetZAndN(value);
        }

        private void Cmp(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr) ^ 0xff;
            int result = _r[A] + value + 1;
            _c = result > 0xff;
            SetZAndN((byte) result);
        }

        private void Cmpm(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adrh);
            int result = value + (_apu.Read(adr) ^ 0xff) + 1;
            _c = result > 0xff;
            SetZAndN((byte) result);
        }

        private void Cmpx(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr) ^ 0xff;
            int result = _r[X] + value + 1;
            _c = result > 0xff;
            SetZAndN((byte) result);
        }

        private void Cmpy(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr) ^ 0xff;
            int result = _r[Y] + value + 1;
            _c = result > 0xff;
            SetZAndN((byte) result);
        }

        private void Adc(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            int result = _r[A] + value + (_c ? 1 : 0);
            _v = (_r[A] & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
            _h = (_r[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0) > 0xf;
            _c = result > 0xff;
            SetZAndN((byte) result);
            _r[A] = (byte) result;
        }

        private void Adcm(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            int addedTo = _apu.Read(adrh);
            int result = addedTo + value + (_c ? 1 : 0);
            _v = (addedTo & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
            _h = (addedTo & 0xf) + (value & 0xf) + (_c ? 1 : 0) > 0xf;
            _c = result > 0xff;
            SetZAndN((byte) result);
            _apu.Write(adrh, (byte) (result & 0xff));
        }

        private void Sbc(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr) ^ 0xff;
            int result = _r[A] + value + (_c ? 1 : 0);
            _v = (_r[A] & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
            _h = (_r[A] & 0xf) + (value & 0xf) + (_c ? 1 : 0) > 0xf;
            _c = result > 0xff;
            SetZAndN((byte) result);
            _r[A] = (byte) result;
        }

        private void Sbcm(int adr, int adrh, int instr)
        {
            int value = _apu.Read(adr) ^ 0xff;
            int addedTo = _apu.Read(adrh);
            int result = addedTo + value + (_c ? 1 : 0);
            _v = (addedTo & 0x80) == (value & 0x80) && (value & 0x80) != (result & 0x80);
            _h = (addedTo & 0xf) + (value & 0xf) + (_c ? 1 : 0) > 0xf;
            _c = result > 0xff;
            SetZAndN((byte) result);
            _apu.Write(adrh, (byte) (result & 0xff));
        }

        private void Movs(int adr, int adrh, int instr) 
        {
            if (instr != 0xaf)
            {
                _apu.Read(adr);
            }
            _apu.Write(adr, _r[A]);
        }

        private void Movsx(int adr, int adrh, int instr) 
        {
            _apu.Read(adr);
            _apu.Write(adr, _r[X]);
        }

        private void Movsy(int adr, int adrh, int instr) 
        {
            _apu.Read(adr);
            _apu.Write(adr, _r[Y]);
        }

        private void Mov(int adr, int adrh, int instr) 
        {
            _r[A] = _apu.Read(adr);
            SetZAndN(_r[A]);
        }

        private void Movx(int adr, int adrh, int instr) 
        {
            _r[X] = _apu.Read(adr);
            SetZAndN(_r[X]);
        }

        private void Movy(int adr, int adrh, int instr)
        {
            _r[Y] = _apu.Read(adr);
            SetZAndN(_r[Y]);
        }

        private void Or1(int adr, int adrh, int instr) 
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            int result = (_c ? 1 : 0) | bit;
            _c = result > 0;
        }

        private void Or1n(int adr, int adrh, int instr) 
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            int result = (_c ? 1 : 0) | (bit > 0 ? 0 : 1);
            _c = result > 0;
        }

        private void And1(int adr, int adrh, int instr) 
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            int result = (_c ? 1 : 0) & bit;
            _c = result > 0;
        }

        private void And1n(int adr, int adrh, int instr) 
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            int result = (_c ? 1 : 0) & (bit > 0 ? 0 : 1);
            _c = result > 0;
        }

        private void Eor1(int adr, int adrh, int instr) 
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            int result = (_c ? 1 : 0) ^ bit;
            _c = result > 0;
        }

        private void Mov1(int adr, int adrh, int instr)
        {
            int bit = (_apu.Read(adr) >> adrh) & 0x1;
            _c = bit > 0;
        }

        private void Mov1s(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            int bit = 1 << adrh;
            value = _c ? value | bit : value & ~bit;
            _apu.Write(adr, (byte) value);
        }

        private void Not1(int adr, int adrh, int instr) 
        {
            int bit = 1 << adrh;
            int value = _apu.Read(adr) ^ bit;
            _apu.Write(adr, (byte) value);
        }

        private void Decw(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            value |= _apu.Read(adrh) << 8;
            value = (value - 1) & 0xffff;
            _z = value == 0;
            _n = (value & 0x8000) > 0;
            _apu.Write(adr, (byte) (value & 0xff));
            _apu.Write(adrh, (byte) (value >> 8));
        }

        private void Incw(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            value |= _apu.Read(adrh) << 8;
            value = (value + 1) & 0xffff;
            _z = value == 0;
            _n = (value & 0x8000) > 0;
            _apu.Write(adr, (byte) (value & 0xff));
            _apu.Write(adrh, (byte) (value >> 8));
        }

        private void Cmpw(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            value |= _apu.Read(adrh) << 8;
            int addTo = (_r[Y] << 8) | _r[A];
            int result = addTo + (value ^ 0xffff) + 1;
            _z = (result & 0xffff) == 0;
            _n = (result & 0x8000) > 0;
            _c = result > 0xffff;
        }

        private void Addw(int adr, int adrh, int instr)
        {
            int value = _apu.Read(adr);
            value |= _apu.Read(adrh) << 8;
            int addTo = (_r[Y] << 8) | _r[A];
            int result = addTo + value;
            _z = (result & 0xffff) == 0;
            _n = (result & 0x8000) > 0;
            _c = result > 0xffff;
            _v = (addTo & 0x8000) == (value & 0x8000) && (value & 0x8000) != (result & 0x8000);
            _h = (addTo & 0xfff) + (value & 0xfff) > 0x0fff;
            _r[A] = (byte) (result & 0xff);
            _r[Y] = (byte) ((result & 0xff00) >> 8);
        }

        private void Subw(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            value |= _apu.Read(adrh) << 8;
            value ^= 0xffff;
            int addTo = (_r[Y] << 8) | _r[A];
            int result = addTo + value + 1;
            _z = (result & 0xffff) == 0;
            _n = (result & 0x8000) > 0;
            _c = result > 0xffff;
            _v = (addTo & 0x8000) == (value & 0x8000) && (value & 0x8000) != (result & 0x8000);
            _h = (addTo & 0xfff) + (value & 0xfff) + 1 > 0xfff;
            _r[A] = (byte) (result & 0xff);
            _r[Y] = (byte) ((result & 0xff00) >> 8);
        }

        private void Movw(int adr, int adrh, int instr)
        {
            _r[A] = _apu.Read(adr);
            _r[Y] = _apu.Read(adrh);
            _z = _r[A] == 0 && _r[Y] == 0;
            _n = (_r[Y] & 0x80) > 0;
        }

        private void Movws(int adr, int adrh, int instr) 
        {
            _apu.Read(adr);
            _apu.Write(adr, _r[A]);
            _apu.Write(adrh, _r[Y]);
        }

        private void Movm(int adr, int adrh, int instr) 
        {
            if (instr == 0x8f)
            {
                _apu.Read(adrh);
            }
            _apu.Write(adrh, _apu.Read(adr));
        }

        private void Asl(int adr, int adrh, int instr) 
        {
            byte value = _apu.Read(adr);
            _c = (value & 0x80) > 0;
            value <<= 1;
            SetZAndN(value);
            _apu.Write(adr, (byte) (value & 0xff));
        }

        private void Asla(int adr, int adrh, int instr) 
        {
            _c = (_r[A] & 0x80) > 0;
            _r[A] <<= 1;
            SetZAndN(_r[A]);
        }

        private void Rol(int adr, int adrh, int instr)
        {
            int value = _apu.Read(adr);
            bool carry = (value & 0x80) > 0;
            value = (value << 1) | (_c ? 1 : 0);
            _c = carry;
            SetZAndN((byte) value);
            _apu.Write(adr, (byte) (value & 0xff));
        }

        private void Rola(int adr, int adrh, int instr)
        {
            bool carry = (_r[A] & 0x80) > 0;
            _r[A] = (byte) ((_r[A] << 1) | (_c ? 1 : 0));
            _c = carry;
            SetZAndN(_r[A]);
        }

        private void Lsr(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            _c = (value & 0x1) > 0;
            value >>= 1;
            SetZAndN((byte) value);
            _apu.Write(adr, (byte) (value & 0xff));
        }

        private void Lsra(int adr, int adrh, int instr) 
        {
            _c = (_r[A] & 0x1) > 0;
            _r[A] >>= 1;
            SetZAndN(_r[A]);
        }

        private void Ror(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            bool carry = (value & 0x1) > 0;
            value = (value >> 1) | (_c ? 0x80 : 0);
            _c = carry;
            SetZAndN((byte) value);
            _apu.Write(adr, (byte) (value & 0xff));
        }

        private void Rora(int adr, int adrh, int instr) 
        {
            bool carry = (_r[A] & 0x1) > 0;
            _r[A] = (byte) ((_r[A] >> 1) | (_c ? 0x80 : 0));
            _c = carry;
            SetZAndN(_r[A]);
        }

        private void Inc(int adr, int adrh, int instr) 
        {
            int value = (_apu.Read(adr) + 1) & 0xff;
            SetZAndN((byte) value);
            _apu.Write(adr, (byte) value);
        }

        private void Inca(int adr, int adrh, int instr) 
        {
            _r[A]++;
            SetZAndN(_r[A]);
        }

        private void Incx(int adr, int adrh, int instr) 
        {
            _r[X]++;
            SetZAndN(_r[X]);
        }

        private void Incy(int adr, int adrh, int instr) 
        {
            _r[Y]++;
            SetZAndN(_r[Y]);
        }

        private void Dec(int adr, int adrh, int instr)
        {
            int value = (_apu.Read(adr) - 1) & 0xff;
            SetZAndN((byte) value);
            _apu.Write(adr, (byte) value);
        }

        private void Deca(int adr, int adrh, int instr)
        {
            _r[A]--;
            SetZAndN(_r[A]);
        }

        private void Decx(int adr, int adrh, int instr) 
        {
            _r[X]--;
            SetZAndN(_r[X]);
        }

        private void Decy(int adr, int adrh, int instr) 
        {
            _r[Y]--;
            SetZAndN(_r[Y]);
        }

        private void Pushp(int adr, int adrh, int instr) {
            Push((byte) GetP());
        }

        private void Pusha(int adr, int adrh, int instr)
        {
            Push(_r[A]);
        }

        private void Pushx(int adr, int adrh, int instr) 
        {
            Push(_r[X]);
        }

        private void Pushy(int adr, int adrh, int instr) 
        {
            Push(_r[Y]);
        }

        private void Movxa(int adr, int adrh, int instr)
        {
            _r[X] = _r[A];
            SetZAndN(_r[X]);
        }

        private void Movax(int adr, int adrh, int instr) 
        {
            _r[A] = _r[X];
            SetZAndN(_r[A]);
        }

        private void Movxp(int adr, int adrh, int instr) 
        {
            _r[X] = _r[SP];
            SetZAndN(_r[X]);
        }

        private void Movpx(int adr, int adrh, int instr)
        {
            _r[SP] = _r[X];
        }

        private void Movay(int adr, int adrh, int instr) 
        {
            _r[A] = _r[Y];
            SetZAndN(_r[A]);
        }

        private void Movya(int adr, int adrh, int instr) 
        {
            _r[Y] = _r[A];
            SetZAndN(_r[Y]);
        }

        private void Notc(int adr, int adrh, int instr)
        {
            _c = !_c;
        }

        private void TSet1(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            int result = _r[A] + (value ^ 0xff) + 1;
            SetZAndN((byte) result);
            value |= _r[A];
            _apu.Write(adr, (byte) value);
        }

        private void TClr1(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr);
            int result = _r[A] + (value ^ 0xff) + 1;
            SetZAndN((byte) result);
            value &= ~_r[A];
            _apu.Write(adr, (byte) value);
        }

        private void Cbne(int adr, int adrh, int instr) 
        {
            int value = _apu.Read(adr) ^ 0xff;
            int result = _r[A] + value + 1;
            DoBranch((result & 0xff) != 0, (ushort) adrh);
        }

        private void Dbnz(int adr, int adrh, int instr)
        {
            int value = (_apu.Read(adr) - 1) & 0xff;
            _apu.Write(adr, (byte) value);
            DoBranch(value != 0, adrh);
        }

        private void Dbnzy(int adr, int adrh, int instr)
        {
            _r[Y]--;
            DoBranch(_r[Y] != 0, adr);
        }

        private void Popp(int adr, int adrh, int instr)
        {
            SetP(Pop());
        }

        private void Popa(int adr, int adrh, int instr)
        {
            _r[A] = (byte) Pop();
        }

        private void Popx(int adr, int adrh, int instr)
        {
            _r[X] = (byte) Pop();
        }

        private void Popy(int adr, int adrh, int instr)
        {
            _r[Y] = (byte) Pop();
        }

        private void Brk(int adr, int adrh, int instr)
        {
            Push((byte) (_br[PC] >> 8));
            Push((byte) (_br[PC] & 0xff));
            Push((byte) GetP());
            _i = false;
            _b = true;
            _br[PC] = (ushort) (_apu.Read(0xffde) | (_apu.Read(0xffdf) << 8));
        }

        private void Jmp(int adr, int adrh, int instr) {
            _br[PC] = (ushort) adr;
        }

        private void Bra(int adr, int adrh, int instr)
        {
            _br[PC] = (ushort) (_br[PC] + adr);
        }

        private void Call(int adr, int adrh, int instr)
        {
            Push((byte) (_br[PC] >> 8));
            Push((byte) (_br[PC] & 0xff));
            _br[PC] = (ushort) adr;
        }

        private void PCall(int adr, int adrh, int instr)
        {
            Push((byte) (_br[PC] >> 8));
            Push((byte) (_br[PC] & 0xff));
            _br[PC] = (ushort) (0xff00 + (adr & 0xff));
        }

        private void Ret(int adr, int adrh, int instr) 
        {
            _br[PC] = (ushort) Pop();
            _br[PC] |= (ushort) (Pop() << 8);
        }

        private void Reti(int adr, int adrh, int instr) 
        {
            SetP(Pop());
            _br[PC] = (ushort) Pop();
            _br[PC] |= (ushort) (Pop() << 8);
        }

        private void Xcn(int adr, int adrh, int instr) 
        {
            _r[A] = (byte) ((_r[A] >> 4) | (_r[A] << 4));
            SetZAndN(_r[A]);
        }

        private void Sleep(int adr, int adrh, int instr) 
        {
            _br[PC]--;
        }

        private void Stop(int adr, int adrh, int instr) 
        {
            _br[PC]--;
        }

        private void Mul(int adr, int adrh, int instr) 
        {
            int result = _r[Y] * _r[A];
            _r[A] = (byte) (result & 0xff);
            _r[Y] = (byte) ((result & 0xff00) >> 8);
            SetZAndN(_r[Y]);
        }

        private void Div(int adr, int adrh, int instr) 
        {
            int value = _r[A] | (_r[Y] << 8);
            int result = 0xffff;
            int mod = value & 0xff;
            if (_r[X] != 0)
            {
                result = (value / _r[X]) & 0xffff;
                mod = value % _r[X];
            }
            _v = result > 0xff;
            _h = (_r[X] & 0xf) <= (_r[Y] & 0xf);
            _r[A] = (byte) result;
            _r[Y] = (byte) mod;
            SetZAndN(_r[A]);
        }

        private void Daa(int adr, int adrh, int instr)
        {
            if (_r[A] > 0x99 || _c)
            {
                _r[A] += 0x60;
                _c = true;
            }
            if ((_r[A] & 0xf) > 9 || _h)
            {
                _r[A] += 6;
            }
            SetZAndN(_r[A]);
        }

        private void Das(int adr, int adrh, int instr)
        {
            if (_r[A] > 0x99 || !_c)
            {
                _r[A] -= 0x60;
                _c = false;
            }
            if ((_r[A] & 0xf) > 9 || !_h)
            {
                _r[A] -= 6;
            }
            SetZAndN(_r[A]);
        }
    }
}