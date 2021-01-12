namespace SNESFromScratch.CPU
{
    public partial class C65816
    {
        private void ADC(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ADC8(ea);
            }
            else
            {
                ADC16(ea);
            }
        }

        private void ADCIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ADC8(IMM8());
            }
            else
            {
                ADC16(IMM16());
            }
        }

        private void _AND(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                AND8(ea);
            }
            else
            {
                AND16(ea);
            }
        }

        private void ANDIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                AND8(IMM8());
            }
            else
            {
                AND16(IMM16());
            }
        }

        private void ASL(int EA)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ASL8(EA);
            }
            else
            {
                ASL16(EA);
            }
        }

        private void ASLA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ASLA8();
            }
            else
            {
                ASLA16();
            }
        }

        private void BIT(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                BIT8(ea);
            }
            else
            {
                BIT16(ea);
            }
        }

        private void BITIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                BITIMM8();
            }
            else
            {
                BITIMM16();
            }
        }

        private void CMP(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                CMP8(ea);
            }
            else
            {
                CMP16(ea);
            }
        }

        private void CMPIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                CMP8(IMM8());
            }
            else
            {
                CMP16(IMM16());
            }
        }

        private void CPX(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                CPX8(ea);
            }
            else
            {
                CPX16(ea);
            }
        }

        private void CPXIMM()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                CPX8(IMM8());
            }
            else
            {
                CPX16(IMM16());
            }
        }

        private void CPY(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                CPY8(ea);
            }
            else
            {
                CPY16(ea);
            }
        }

        private void CPYIMM()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                CPY8(IMM8());
            }
            else
            {
                CPY16(IMM16());
            }
        }

        private void DEC(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                DEC8(ea);
            }
            else
            {
                DEC16(ea);
            }
        }

        private void DECA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                DECA8();
            }
            else
            {
                DECA16();
            }
        }

        private void DEX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                DEX8();
            }
            else
            {
                DEX16();
            }
        }

        private void DEY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                DEY8();
            }
            else
            {
                DEY16();
            }
        }

        private void EOR(int ea)
        {
            if ((_p & (int) Flags.M) != 0 | _m6502)
            {
                EOR8(ea);
            }
            else
            {
                EOR16(ea);
            }
        }

        private void EORIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                EOR8(IMM8());
            }
            else
            {
                EOR16(IMM16());
            }
        }

        private void INC(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                INC8(ea);
            }
            else
            {
                INC16(ea);
            }
        }

        private void INCA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                INCA8();
            }
            else
            {
                INCA16();
            }
        }

        private void INX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                INX8();
            }
            else
            {
                INX16();
            }
        }

        private void INY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                INY8();
            }
            else
            {
                INY16();
            }
        }

        private void LDA(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                LDA8(ea);
            }
            else
            {
                LDA16(ea);
            }
        }

        private void LDAIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                LDA8(IMM8());
            }
            else
            {
                LDA16(IMM16());
            }
        }

        private void LDX(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                LDX8(ea);
            }
            else
            {
                LDX16(ea);
            }
        }

        private void LDXIMM()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                LDX8(IMM8());
            }
            else
            {
                LDX16(IMM16());
            }
        }

        private void LDY(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                LDY8(ea);
            }
            else
            {
                LDY16(ea);
            }
        }

        private void LDYIMM()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                LDY8(IMM8());
            }
            else
            {
                LDY16(IMM16());
            }
        }

        private void LSR(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                LSR8(ea);
            }
            else
            {
                LSR16(ea);
            }
        }

        private void LSRA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                LSRA8();
            }
            else
            {
                LSRA16();
            }
        }

        private void MVN()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                MVN8();
            }
            else
            {
                MVN16();
            }
        }

        private void MVP()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                MVP8();
            }
            else
            {
                MVP16();
            }
        }

        private void ORA(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ORA8(ea);
            }
            else
            {
                ORA16(ea);
            }
        }

        private void ORAIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ORA8(IMM8());
            }
            else
            {
                ORA16(IMM16());
            }
        }

        private void PHA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                PHA8();
            }
            else
            {
                PHA16();
            }
        }

        private void PHX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                PHX8();
            }
            else
            {
                PHX16();
            }
        }

        private void PHY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                PHY8();
            }
            else
            {
                PHY16();
            }
        }

        private void PLA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                PLA8();
            }
            else
            {
                PLA16();
            }
        }

        private void PLX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                PLX8();
            }
            else
            {
                PLX16();
            }
        }

        private void PLY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                PLY8();
            }
            else
            {
                PLY16();
            }
        }

        private void ROL(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ROL8(ea);
            }
            else
            {
                ROL16(ea);
            }
        }

        private void ROLA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ROLA8();
            }
            else
            {
                ROLA16();
            }
        }

        private void ROR(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                ROR8(ea);
            }
            else
            {
                ROR16(ea);
            }
        }

        private void RORA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                RORA8();
            }
            else
            {
                RORA16();
            }
        }

        private void SBC(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                SBC8(ea);
            }
            else
            {
                SBC16(ea);
            }
        }

        private void SBCIMM()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                SBC8(IMM8());
            }
            else
            {
                SBC16(IMM16());
            }
        }

        private void STA(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                STA8(ea);
            }
            else
            {
                STA16(ea);
            }
        }

        private void STX(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                STX8(ea);
            }
            else
            {
                STX16(ea);
            }
        }

        private void STY(int ea)
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                STY8(ea);
            }
            else
            {
                STY16(ea);
            }
        }

        private void STZ(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                STZ8(ea);
            }
            else
            {
                STZ16(ea);
            }
        }

        private void TAX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                TAX8();
            }
            else
            {
                TAX16();
            }
        }

        private void TAY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                TAY8();
            }
            else
            {
                TAY16();
            }
        }

        private void TRB(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                TRB8(ea);
            }
            else
            {
                TRB16(ea);
            }
        }

        private void TSB(int ea)
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                TSB8(ea);
            }
            else
            {
                TSB16(ea);
            }
        }

        private void TXA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                TXA8();
            }
            else
            {
                TXA16();
            }
        }

        private void TXY()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                TXY8();
            }
            else
            {
                TXY16();
            }
        }

        private void TYA()
        {
            if ((_p & (int) Flags.M) != 0 || _m6502)
            {
                TYA8();
            }
            else
            {
                TYA16();
            }
        }

        private void TYX()
        {
            if ((_p & (int) Flags.X) != 0 || _m6502)
            {
                TYX8();
            }
            else
            {
                TYX16();
            }
        }

        private void ADC8(int ea)
        {
            int al = _a & 0xFF;
            int ah = _a & 0xFF00;
            int value = Read8(ea);
            if ((_p & (int) Flags.BCD) != 0)
            {
                int an0 = _a & 0xF;
                int an1 = _a & 0xF0;
                int vn0 = value & 0xF;
                int vn1 = value & 0xF0;
                an0 += _p & (int) Flags.Carry;
                an0 += vn0;
                an1 += vn1;
                if (an0 > 0x9)
                {
                    an0 += 0x6;
                }
                if (an0 > 0xF)
                {
                    an1 += 0x10;
                }
                an0 &= 0xF;
                int result = an0 + an1;
                SetFlag((~(al ^ value) & (al ^ result) & 0x80) != 0, Flags.Overflow);
                if (result > 0x9F)
                {
                    result += 0x60;
                }
                SetFlag(result > 0xFF, Flags.Carry);
                _a = result & 0xFF;
            }
            else
            {
                int result = al + value + (_p & (int) Flags.Carry);
                SetFlag((~(al ^ value) & (al ^ result) & 0x80) != 0, Flags.Overflow);
                SetFlag(result > 0xFF, Flags.Carry);
                _a = result & 0xFF;
            }
            SetZNFlags8((byte) _a);
            _a |= ah;
        }

        private void ADC16(int ea)
        {
            int value = Read16(ea);
            if ((_p & (int) Flags.BCD) != 0)
            {
                int an0 = _a & 0xF;
                int an1 = _a & 0xF0;
                int an2 = _a & 0xF00;
                int an3 = _a & 0xF000;
                int vn0 = value & 0xF;
                int vn1 = value & 0xF0;
                int vn2 = value & 0xF00;
                int vn3 = value & 0xF000;
                an0 += _p & (int) Flags.Carry;
                an0 += vn0;
                an1 += vn1;
                an2 += vn2;
                an3 += vn3;
                if (an0 > 0x9)
                {
                    an0 += 0x6;
                }
                if (an0 > 0xF)
                {
                    an1 += 0x10;
                }
                an0 &= 0xF;
                if (an1 > 0x9F)
                {
                    an1 += 0x60;
                }
                if (an1 > 0xFF)
                {
                    an2 += 0x100;
                }
                an1 &= 0xF0;
                if (an2 > 0x9FF)
                {
                    an2 += 0x600;
                }
                if (an2 > 0xFFF)
                {
                    an3 += 0x1000;
                }
                an2 &= 0xF00;
                int result = an0 + an1 + an2 + an3;
                SetFlag((~(_a ^ value) & (_a ^ result) & 0x8000) != 0, Flags.Overflow);
                if (result > 0x9FFF)
                {
                    result += 0x6000;
                }
                SetFlag(result > 0xFFFF, Flags.Carry);
                _a = result & 0xFFFF;
            }
            else
            {
                int result = _a + value + (_p & (int) Flags.Carry);
                SetFlag((~(_a ^ value) & (_a ^ result) & 0x8000) != 0, Flags.Overflow);
                SetFlag(result > 0xFFFF, Flags.Carry);
                _a = result & 0xFFFF;
            }
            SetZNFlags16(_a);
        }

        private void AND8(int ea)
        {
            _a &= Read8(ea) | 0xFF00;
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void AND16(int ea)
        {
            _a &= Read16(ea);
            SetZNFlags16(_a);
        }

        private void ASL8(int ea)
        {
            int value = Read8(ea);
            SetFlag((value & 0x80) != 0, Flags.Carry);
            value = (value << 1) & 0xFF;
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void ASL16(int ea)
        {
            int value = Read16(ea);
            SetFlag((value & 0x8000) != 0, Flags.Carry);
            value = (value << 1) & 0xFFFF;
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void ASLA8()
        {
            SetFlag((_a & 0x80) != 0, Flags.Carry);
            _a = ((_a << 1) & 0xFF) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void ASLA16()
        {
            SetFlag((_a & 0x8000) != 0, Flags.Carry);
            _a = (_a << 1) & 0xFFFF;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void BCC(int ea)
        {
            if ((_p & (int) Flags.Carry) == 0)
                BRA(ea);
        }

        private void BCS(int ea)
        {
            if ((_p & (int) Flags.Carry) != 0)
            {
                BRA(ea);
            }
        }

        private void BEQ(int ea)
        {
            if ((_p & (int) Flags.Zero) != 0)
            {
                BRA(ea);
            }
        }

        private void BIT8(int ea)
        {
            int value = Read8(ea);
            SetFlag((value & _a & 0xFF) == 0, Flags.Zero);
            SetFlag((value & 0x80) != 0, Flags.Negative);
            SetFlag((value & 0x40) != 0, Flags.Overflow);
        }

        private void BIT16(int ea)
        {
            int value = Read16(ea);
            SetFlag((value & _a) == 0, Flags.Zero);
            SetFlag((value & 0x8000) != 0, Flags.Negative);
            SetFlag((value & 0x4000) != 0, Flags.Overflow);
        }

        private void BITIMM8()
        {
            SetFlag((Read8(IMM8()) & _a & 0xFF) == 0, Flags.Zero);
        }

        private void BITIMM16()
        {
            SetFlag((Read16(IMM16()) & _a) == 0, Flags.Zero);
        }

        private void BMI(int ea)
        {
            if ((_p & (int) Flags.Negative) != 0)
            {
                BRA(ea);
            }
        }

        private void BNE(int ea)
        {
            if ((_p & (int) Flags.Zero) == 0)
            {
                BRA(ea);
            }
        }

        private void BPL(int ea)
        {
            if ((_p & (int) Flags.Negative) == 0)
            {
                BRA(ea);
            }
        }

        private void BRA(int EA)
        {
            if (_m6502 & (_pc & 0xFF00) != (EA & 0xFF00))
            {
                Cycles += TwoCycles;
            }
            else
            {
                Cycles += OneCycle;
            }
            _pc = EA;
        }

        private void BRK()
        {
            Read8PC();
            if (_m6502)
            {
                Push16(_pc);
                Push8(_p | 0x10);
                _pc = Read16(0xFFFE);
            }
            else
            {
                Push8(_pb);
                Push16(_pc);
                Push8(_p);
                _pc = Read16(0xFFE6);
            }
            _pb = 0;
            ClearFlag(Flags.BCD);
            SetFlag(Flags.IRQ);
        }

        private void BVC(int ea)
        {
            if ((_p & (int) Flags.Overflow) == 0)
            {
                BRA(ea);
            }
        }

        private void BVS(int ea)
        {
            if ((_p & (int) Flags.Overflow) != 0)
            {
                BRA(ea);
            }
        }

        private void CLC()
        {
            ClearFlag(Flags.Carry);
            Cycles += OneCycle;
        }

        private void CLD()
        {
            ClearFlag(Flags.BCD);
            Cycles += OneCycle;
        }

        private void CLI()
        {
            ClearFlag(Flags.IRQ);
            Cycles += OneCycle;
        }

        private void CLV()
        {
            ClearFlag(Flags.Overflow);
            Cycles += OneCycle;
        }

        private void CMP8(int ea)
        {
            int al = _a & 0xFF;
            int value = Read8(ea);
            int result = al - value;
            SetFlag(al >= value, Flags.Carry);
            SetZNFlags8((byte) (result & 0xFF));
        }

        private void CMP16(int ea)
        {
            int value = Read16(ea);
            int result = _a - value;
            SetFlag(_a >= value, Flags.Carry);
            SetZNFlags16(result);
        }

        private void COP()
        {
            Read8PC();
            if (_m6502)
            {
                Push16(_pc);
                Push8(_p);
                _pc = Read16(0xFFF4);
            }
            else
            {
                Push8(_pb);
                Push16(_pc);
                Push8(_p);
                _pc = Read16(0xFFE4);
            }
            _pb = 0;
            ClearFlag(Flags.BCD);
            SetFlag(Flags.IRQ);
        }

        private void CPX8(int ea)
        {
            int value = Read8(ea);
            int result = _x - value;
            SetFlag(_x >= value, Flags.Carry);
            SetZNFlags8((byte) (result & 0xFF));
        }

        private void CPX16(int ea)
        {
            int value = Read16(ea);
            int result = _x - value;
            SetFlag(_x >= value, Flags.Carry);
            SetZNFlags16(result);
        }

        private void CPY8(int ea)
        {
            int value = Read8(ea);
            int result = _y - value;
            SetFlag(_y >= value, Flags.Carry);
            SetZNFlags8((byte) (result & 0xFF));
        }

        private void CPY16(int ea)
        {
            int value = Read16(ea);
            int result = _y - value;
            SetFlag(_y >= value, Flags.Carry);
            SetZNFlags16(result);
        }

        private void DEC8(int ea)
        {
            int value = Read8(ea);
            value = (value - 1) & 0xFF;
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void DEC16(int ea)
        {
            int value = Read16(ea);
            value = (value - 1) & 0xFFFF;
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void DECA8()
        {
            _a = ((_a - 1) & 0xFF) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void DECA16()
        {
            _a = (_a - 1) & 0xFFFF;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void DEX8()
        {
            _x = (_x - 1) & 0xFF;
            SetZNFlags8((byte) _x);
            Cycles += OneCycle;
        }

        private void DEX16()
        {
            _x = (_x - 1) & 0xFFFF;
            SetZNFlags16(_x);
            Cycles += OneCycle;
        }

        private void DEY8()
        {
            _y = (_y - 1) & 0xFF;
            SetZNFlags8((byte) _y);
            Cycles += OneCycle;
        }

        private void DEY16()
        {
            _y = (_y - 1) & 0xFFFF;
            SetZNFlags16(_y);
            Cycles += OneCycle;
        }

        private void EOR8(int ea)
        {
            _a ^= Read8(ea);
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void EOR16(int ea)
        {
            _a ^= Read16(ea);
            SetZNFlags16(_a);
        }

        private void INC8(int ea)
        {
            int value = Read8(ea);
            value = (value + 1) & 0xFF;
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void INC16(int ea)
        {
            int value = Read16(ea);
            value = (value + 1) & 0xFFFF;
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void INCA8()
        {
            _a = ((_a + 1) & 0xFF) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void INCA16()
        {
            _a = (_a + 1) & 0xFFFF;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void INX8()
        {
            _x = (_x + 1) & 0xFF;
            SetZNFlags8((byte) _x);
            Cycles += OneCycle;
        }

        private void INX16()
        {
            _x = (_x + 1) & 0xFFFF;
            SetZNFlags16(_x);
            Cycles += OneCycle;
        }

        private void INY8()
        {
            _y = (_y + 1) & 0xFF;
            SetZNFlags8((byte) _y);
            Cycles += OneCycle;
        }

        private void INY16()
        {
            _y = (_y + 1) & 0xFFFF;
            SetZNFlags16(_y);
            Cycles += OneCycle;
        }

        private void JMP(int ea)
        {
            _pc = ea & 0xFFFF;
        }

        private void JMPABSINDX()
        {
            _pc = ABSINDX() & 0xFFFF;
            Cycles += OneCycle;
        }

        private void JML(int ea)
        {
            _pb = ea >> 16;
            JMP(ea);
        }

        private void JSR(int ea)
        {
            Push16((_pc - 1) & 0xFFFF);
            _pc = ea & 0xFFFF;
            Cycles += OneCycle;
        }

        private void JSL(int ea)
        {
            Push8(_pb);
            _pb = ea >> 16;
            JSR(ea);
        }

        private void LDA8(int ea)
        {
            _a = Read8(ea) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void LDA16(int ea)
        {
            _a = Read16(ea);
            SetZNFlags16(_a);
        }

        private void LDX8(int ea)
        {
            _x = Read8(ea);
            SetZNFlags8((byte) _x);
        }

        private void LDX16(int ea)
        {
            _x = Read16(ea);
            SetZNFlags16(_x);
        }

        private void LDY8(int ea)
        {
            _y = Read8(ea);
            SetZNFlags8((byte) _y);
        }

        private void LDY16(int ea)
        {
            _y = Read16(ea);
            SetZNFlags16(_y);
        }

        private void LSR8(int ea)
        {
            int value = Read8(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value >>= 1;
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void LSR16(int ea)
        {
            int value = Read16(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value >>= 1;
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void LSRA8()
        {
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a = ((_a & 0xFF) >> 1) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void LSRA16()
        {
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a >>= 1;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void MVN8()
        {
            _db = Read8PC();
            int bank = Read8PC();
            int value = Read8(_x | (bank << 16));
            Write8(_y | (_db << 16), value);
            _x = (_x + 1) & 0xFF;
            _y = (_y + 1) & 0xFF;
            _a = (_a - 1) & 0xFFFF;
            if (_a != 0xFFFF)
            {
                _pc -= 3;
            }
            Cycles += TwoCycles;
        }

        private void MVN16()
        {
            _db = Read8PC();
            int bank = Read8PC();
            int value = Read8(_x | (bank << 16));
            Write8(_y | (_db << 16), value);
            _x = (_x + 1) & 0xFFFF;
            _y = (_y + 1) & 0xFFFF;
            _a = (_a - 1) & 0xFFFF;
            if (_a != 0xFFFF)
            {
                _pc -= 3;
            }
            Cycles += TwoCycles;
        }

        private void MVP8()
        {
            _db = Read8PC();
            int bank = Read8PC();
            int value = Read8(_x | (bank << 16));
            Write8(_y | (_db << 16), value);
            _x = (_x - 1) & 0xFF;
            _y = (_y - 1) & 0xFF;
            _a = (_a - 1) & 0xFFFF;
            if (_a != 0xFFFF)
            {
                _pc -= 3;
            }
            Cycles += TwoCycles;
        }

        private void MVP16()
        {
            _db = Read8PC();
            int bank = Read8PC();
            int value = Read8(_x | (bank << 16));
            Write8(_y | (_db << 16), value);
            _x = (_x - 1) & 0xFFFF;
            _y = (_y - 1) & 0xFFFF;
            _a = (_a - 1) & 0xFFFF;
            if (_a != 0xFFFF)
            {
                _pc -= 3;
            }
            Cycles += TwoCycles;
        }

        private void NOP()
        {
            Cycles += OneCycle;
        }

        private void ORA8(int ea)
        {
            _a |= Read8(ea);
            SetZNFlags8((byte) (_a & 0xFF));
        }

        private void ORA16(int ea)
        {
            _a |= Read16(ea);
            SetZNFlags16(_a);
        }

        private void PEA(int ea)
        {
            Push16(ea);
        }

        private void PER()
        {
            Push16(RELL());
            Cycles += OneCycle;
        }

        private void PHA8()
        {
            Push8(_a & 0xFF);
            Cycles += OneCycle;
        }

        private void PHA16()
        {
            Push16(_a);
            Cycles += OneCycle;
        }

        private void PHB()
        {
            Push8(_db);
            Cycles += OneCycle;
        }

        private void PHD()
        {
            Push16(_dp);
            Cycles += OneCycle;
        }

        private void PHK()
        {
            Push8(_pb);
            Cycles += OneCycle;
        }

        private void PHP()
        {
            Push8(_p);
            Cycles += OneCycle;
        }

        private void PHX8()
        {
            Push8(_x);
            Cycles += OneCycle;
        }

        private void PHX16()
        {
            Push16(_x);
            Cycles += OneCycle;
        }

        private void PHY8()
        {
            Push8(_y);
            Cycles += OneCycle;
        }

        private void PHY16()
        {
            Push16(_y);
            Cycles += OneCycle;
        }

        private void PLA8()
        {
            _a = Pull8() | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += TwoCycles;
        }

        private void PLA16()
        {
            _a = Pull16();
            SetZNFlags16(_a);
            Cycles += TwoCycles;
        }

        private void PLB()
        {
            _db = Pull8();
            SetZNFlags8((byte) _db);
            Cycles += TwoCycles;
        }

        private void PLD()
        {
            _dp = Pull16();
            SetZNFlags16(_dp);
            Cycles += TwoCycles;
        }

        private void PLP()
        {
            _p = Pull8();
            ClearIndex8();
            Cycles += TwoCycles;
        }

        private void PLX8()
        {
            _x = Pull8();
            SetZNFlags8((byte) _x);
            Cycles += TwoCycles;
        }

        private void PLX16()
        {
            _x = Pull16();
            SetZNFlags16(_x);
            Cycles += TwoCycles;
        }

        private void PLY8()
        {
            _y = Pull8();
            SetZNFlags8((byte) _y);
            Cycles += TwoCycles;
        }

        private void PLY16()
        {
            _y = Pull16();
            SetZNFlags16(_y);
            Cycles += TwoCycles;
        }

        private void REP(int ea)
        {
            int mask = Read8(ea);
            if (_m6502)
            {
                mask &= ~0x30;
            }
            _p &= ~mask;
            ClearIndex8();
            Cycles += OneCycle;
        }

        private void ROL8(int ea)
        {
            int carry = _p & (int) Flags.Carry;
            int value = Read8(ea);
            SetFlag((value & 0x80) != 0, Flags.Carry);
            value = (value << 1) & 0xFF;
            value |= carry;
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void ROL16(int ea)
        {
            int carry = _p & (int) Flags.Carry;
            int value = Read16(ea);
            SetFlag((value & 0x8000) != 0, Flags.Carry);
            value = (value << 1) & 0xFFFF;
            value |= carry;
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void ROLA8()
        {
            int carry = _p & (int) Flags.Carry;
            SetFlag((_a & 0x80) != 0, Flags.Carry);
            _a = ((_a << 1) & 0xFF) | (_a & 0xFF00);
            _a |= carry;
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void ROLA16()
        {
            int carry = _p & (int) Flags.Carry;
            SetFlag((_a & 0x8000) != 0, Flags.Carry);
            _a = (_a << 1) & 0xFFFF;
            _a |= carry;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void ROR8(int ea)
        {
            int carry = _p & (int) Flags.Carry;
            int value = Read8(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value = (value >> 1) | (carry << 7);
            Write8(ea, value);
            SetZNFlags8((byte) value);
            Cycles += OneCycle;
        }

        private void ROR16(int ea)
        {
            int carry = _p & (int) Flags.Carry;
            int value = Read16(ea);
            SetFlag((value & 1) != 0, Flags.Carry);
            value = (value >> 1) | (carry << 15);
            Write16(ea, value);
            SetZNFlags16(value);
            Cycles += OneCycle;
        }

        private void RORA8()
        {
            int carry = _p & (int) Flags.Carry;
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a = ((_a & 0xFF) >> 1) | (_a & 0xFF00);
            _a |= carry << 7;
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void RORA16()
        {
            int carry = _p & (int) Flags.Carry;
            SetFlag((_a & 1) != 0, Flags.Carry);
            _a = (_a >> 1) | (carry << 15);
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void RTI()
        {
            _p = Pull8();
            _pc = Pull16();
            if (!_m6502)
            {
                _pb = Pull8();
            }
            ClearIndex8();
            Cycles += TwoCycles;
        }

        private void RTL()
        {
            _pc = (Pull16() + 1) & 0xFFFF;
            _pb = Pull8();
            Cycles += TwoCycles;
        }

        private void RTS()
        {
            _pc = (Pull16() + 1) & 0xFFFF;
            Cycles += ThreeCycles;
        }

        private void SBC8(int ea)
        {
            int al = _a & 0xFF;
            int ah = _a & 0xFF00;
            byte value = (byte) (Read8(ea) ^ 0xFF);
            if ((_p & (int) Flags.BCD) != 0)
            {
                int an0 = _a & 0xF;
                int an1 = _a & 0xF0;
                int vn0 = value & 0xF;
                int vn1 = value & 0xF0;
                an0 += _p & (int) Flags.Carry;
                an0 += vn0;
                an1 += vn1;
                if (an0 < 0x10)
                {
                    an0 -= 6;
                }
                if (an0 > 0xF)
                {
                    an1 += 0x10;
                }
                an0 &= 0xF;
                int result = an0 + an1;
                SetFlag((~(al ^ value) & (al ^ result) & 0x80) != 0, Flags.Overflow);
                if (result < 0x100)
                {
                    result -= 0x60;
                }
                SetFlag(result > 0xFF, Flags.Carry);
                _a = result & 0xFF;
            }
            else
            {
                int result = al + value + (_p & (int) Flags.Carry);
                SetFlag((~(al ^ value) & (al ^ result) & 0x80) != 0, Flags.Overflow);
                SetFlag(result > 0xFF, Flags.Carry);
                _a = result & 0xFF;
            }
            SetZNFlags8((byte) _a);
            _a |= ah;
        }

        private void SBC16(int ea)
        {
            int value = Read16(ea) ^ 0xFFFF;
            if ((_p & (int) Flags.BCD) != 0)
            {
                int an0 = _a & 0xF;
                int an1 = _a & 0xF0;
                int an2 = _a & 0xF00;
                int an3 = _a & 0xF000;
                int vn0 = value & 0xF;
                int vn1 = value & 0xF0;
                int vn2 = value & 0xF00;
                int vn3 = value & 0xF000;
                an0 += _p & (int) Flags.Carry;
                an0 += vn0;
                an1 += vn1;
                an2 += vn2;
                an3 += vn3;
                if (an0 < 0x10)
                {
                    an0 -= 0x6;
                }
                if (an0 > 0xF)
                {
                    an1 += 0x10;
                }
                an0 &= 0xF;
                if (an1 < 0x100)
                {
                    an1 -= 0x60;
                }
                if (an1 > 0xFF)
                {
                    an2 += 0x100;
                }
                an1 &= 0xF0;
                if (an2 < 0x1000)
                {
                    an2 -= 0x600;
                }
                if (an2 > 0xFFF)
                {
                    an3 += 0x1000;
                }
                an2 &= 0xF00;
                int Result = an0 + an1 + an2 + an3;
                SetFlag((~(_a ^ value) & (_a ^ Result) & 0x8000) != 0, Flags.Overflow);
                if (Result < 0x10000)
                {
                    Result -= 0x6000;
                }
                SetFlag(Result > 0xFFFF, Flags.Carry);
                _a = Result & 0xFFFF;
            }
            else
            {
                int result = _a + value + (_p & (int) Flags.Carry);
                SetFlag((~(_a ^ value) & (_a ^ result) & 0x8000) != 0, Flags.Overflow);
                SetFlag(result > 0xFFFF, Flags.Carry);
                _a = result & 0xFFFF;
            }
            SetZNFlags16(_a);
        }

        private void SEC()
        {
            SetFlag(Flags.Carry);
            Cycles += OneCycle;
        }

        private void SED()
        {
            SetFlag(Flags.BCD);
            Cycles += OneCycle;
        }

        private void SEI()
        {
            SetFlag(Flags.IRQ);
            Cycles += OneCycle;
        }

        private void SEP(int ea)
        {
            int mask = Read8(ea);
            if (_m6502)
            {
                mask &= ~0x30;
            }
            _p |= mask;
            ClearIndex8();
            Cycles += OneCycle;
        }

        private void STA8(int ea)
        {
            Write8(ea, _a & 0xFF);
        }

        private void STA16(int ea)
        {
            Write16(ea, _a);
        }

        private void STP()
        {
            _stpState = true;
            _pc = (_pc - 1) & 0xFFFF;
            Cycles += TwoCycles;
        }

        private void STX8(int ea)
        {
            Write8(ea, _x);
        }

        private void STX16(int ea)
        {
            Write16(ea, _x);
        }

        private void STY8(int ea)
        {
            Write8(ea, _y);
        }

        private void STY16(int ea)
        {
            Write16(ea, _y);
        }

        private void STZ8(int ea)
        {
            Write8(ea, 0);
        }

        private void STZ16(int ea)
        {
            Write16(ea, 0);
        }

        private void TAX8()
        {
            _x = _a & 0xFF;
            SetZNFlags8((byte) _x);
            Cycles += OneCycle;
        }

        private void TAX16()
        {
            _x = _a; 
            SetZNFlags16(_x);
            Cycles += OneCycle;
        }

        private void TAY8()
        {
            _y = _a & 0xFF;
            SetZNFlags8((byte) _y);
            Cycles += OneCycle;
        }

        private void TAY16()
        {
            _y = _a;
            SetZNFlags16(_y);
            Cycles += OneCycle;
        }

        private void TCD()
        {
            _dp = _a;
            SetZNFlags16(_dp);
            Cycles += OneCycle;
        }

        private void TCS()
        {
            _s = _a;
            if (_m6502)
            {
                _s = (_s & 0xFF) | 0x100;
            }
            Cycles += OneCycle;
        }

        private void TDC()
        {
            _a = _dp;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void TRB8(int ea)
        {
            int value = Read8(ea);
            SetFlag((_a & value & 0xFF) == 0, Flags.Zero);
            value &= ~(_a & 0xFF);
            Write8(ea, value);
            Cycles += OneCycle;
        }

        private void TRB16(int ea)
        {
            int value = Read16(ea);
            SetFlag((_a & value) == 0, Flags.Zero);
            value &= ~_a;
            Write16(ea, value);
            Cycles += OneCycle;
        }

        private void TSB8(int ea)
        {
            int value = Read8(ea);
            SetFlag((_a & value & 0xFF) == 0, Flags.Zero);
            value |= _a & 0xFF;
            Write8(ea, value);
            Cycles += OneCycle;
        }

        private void TSB16(int ea)
        {
            int value = Read16(ea);
            SetFlag((_a & value) == 0, Flags.Zero);
            value |= _a;
            Write16(ea, value);
            Cycles += OneCycle;
        }

        private void TSC()
        {
            _a = _s;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void TSX()
        {
            _x = _s;
            SetZNFlags16(_x);
            Cycles += OneCycle;
        }

        private void TXA8()
        {
            _a = (_x & 0xFF) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void TXA16()
        {
            _a = _x;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void TXS()
        {
            _s = _x;
            if (_m6502)
            {
                _s |= 0x100;
            }
            Cycles += OneCycle;
        }

        private void TXY8()
        {
            _y = _x;
            SetZNFlags8((byte) _y);
            Cycles += OneCycle;
        }

        private void TXY16()
        {
            _y = _x;
            SetZNFlags16(_y);
            Cycles += OneCycle;
        }

        private void TYA8()
        {
            _a = (_y & 0xFF) | (_a & 0xFF00);
            SetZNFlags8((byte) (_a & 0xFF));
            Cycles += OneCycle;
        }

        private void TYA16()
        {
            _a = _y;
            SetZNFlags16(_a);
            Cycles += OneCycle;
        }

        private void TYX8()
        {
            _x = _y;
            SetZNFlags8((byte) _x);
            Cycles += OneCycle;
        }

        private void TYX16()
        {
            _x = _y;
            SetZNFlags16(_x);
            Cycles += OneCycle;
        }

        private void WAI()
        {
            _waiState = true;
            _pc = (_pc - 1) & 0xFFFF;
            Cycles += TwoCycles;
        }

        private void WDM()
        {
            NOP();
        }

        private void XBA()
        {
            int al = _a & 0xFF;
            int ah = (_a & 0xFF00) >> 8;
            _a = ah | (al << 8);
            SetZNFlags8((byte) ah);
            Cycles += TwoCycles;
        }

        private void XCE()
        {
            int carry = _p & (int) Flags.Carry;
            SetFlag(_m6502, Flags.Carry);
            _m6502 = carry != 0;
            if (_m6502)
            {
                _s = (_s & 0xFF) | 0x100;
                SetFlag(Flags.X);
                SetFlag(Flags.M);
            }
            ClearIndex8();
            Cycles += OneCycle;
        }
    }
}