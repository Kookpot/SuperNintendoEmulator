using System;
using System.IO;
using System.Text;

namespace SuperNintendo.Sound
{
    /// <summary>
    /// http://www.emulatronia.com/doctec/consolas/snes/spc.htm
    /// </summary>
    public class SPC700
    {
        #region Members

        //process counter
        private Int32 PC { get; set; }
        private Byte Y {
            get { return (Byte)(W >> 8); } 
            set { W = (Int16)((value << 8) + A); }
        }
        private Byte A {
            get { return (Byte)W; }
            set { W = (Int16)((Y << 8) + value); }
        }
        private Int16 W { get; set; }
        private Byte X { get; set; }
        //program status word : http://nl.wikipedia.org/wiki/Program_Status_Word
        private Byte PSW { get; set; }
        //stackpointer
        private Byte SP { get; set; }
        private Byte Reserved1 { get; set; }
        private Byte Reserved2 { get; set; }
        private Int32 DirectPage {get;set;}
        public Boolean Executing { get; set; }
        private Byte[] OutPorts { get; set; }
        private Int32[] _arrTimer = new Int32[3];
        private Int32[] _arrTimerTarget = new Int32[3];
        private Boolean[] _arrTimerEnabled = new Boolean[3];
        private Byte[] _arrTimerValueWritten = new Byte[3];
        private Int32 _intTimerErrorCounter;
        private Boolean Carry 
        {
            get { return (PSW & 0x01) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x01); } else { PSW = (Byte) (PSW & ~0x01); }; }
        }
        private Boolean Zero
        {
            get { return (PSW & 0x02) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x02); } else { PSW = (Byte) (PSW & ~0x02); }; }
        }
        private Boolean Interrupts
        {
            get { return (PSW & 0x04) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x04); } else { PSW = (Byte) (PSW & ~0x04); }; }
        }
        private Boolean HalfCarry
        {
            get { return (PSW & 0x08) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x08); } else { PSW = (Byte) (PSW & ~0x08); }; }
        }
        private Boolean SoftwareBreak
        {
            get { return (PSW & 0x10) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x10); } else { PSW = (Byte) (PSW & ~0x10); }; }
        }
        private Boolean DirectPageSelector
        {
            get { return (PSW & 0x20) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x20); } else { PSW = (Byte) (PSW & ~0x20); }; }
        }
        private Boolean Overflow
        {
            get { return (PSW & 0x40) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x40); } else { PSW = (Byte) (PSW & ~0x40); }; }
        }
        private Boolean Negative
        {
            get { return (PSW & 0x80) != 0; }
            set { if (value) { PSW = (Byte) (PSW | 0x80); } else { PSW = (Byte) (PSW & ~0x80); }; }
        }
        //ram memory
        private Byte[] _objRam;
        private Byte[] Ram {
            get { return _objRam; }
            set { _objRam = value; }
        }
        //extra ram
        private Byte[] ExtraRam { get; set; }
        //dsp
        private DSP _objDSP;

        //TEMP VALUES
        private Int32 Relative1
        {
            get { return PC + 2 + (SByte) Ram[PC + 1]; }
        }

        private Int32 Relative2
        {
            get { return PC + 3 + (SByte) Ram[PC + 2]; }
        }

        //clock speed
        private const Int32 _intClk = 24576000;

        private Int32 intCounter;

        #endregion

        #region Public Members

        /// <summary>
        /// run based on a file
        /// </summary>
        /// <param name="strFile">path to file</param>
        public void Prepare(String strFile)
        {
            var objTempRegisters = new Byte[128];
            using (var objReader = new System.IO.FileStream(strFile, FileMode.Open))
            {
                var strHeader = ReadString(objReader, 33);
                var bteS = new Byte[4];
                objReader.Read(bteS, 0, 4);
                ReadRegisters(objReader);
                bteS = new Byte[210];
                objReader.Read(bteS, 0, 210);
                Ram = new Byte[65536];
                objReader.Read(Ram, 0, 65536);
                objReader.Read(objTempRegisters, 0, 128);
                bteS = new Byte[64];
                objReader.Read(bteS, 0, 64);
                ExtraRam = new Byte[64];
                objReader.Read(ExtraRam, 0, 64);
            }
            Executing = true;
            for (Int32 intI = 0; intI < 3; intI++)
            {
                _arrTimerEnabled[intI] = false;
                _arrTimerValueWritten[intI] = 0;
                _arrTimerTarget[intI] = 0;
                _arrTimer[intI] = 0;
            }
            OutPorts = new Byte[4];
            for (Int32 intI = 0; intI < 4; intI++)
            {
                OutPorts[intI] = Ram[0xf4 + intI];
            }
            for (Int32 intI = 0; intI < 3; intI++)
            {
                if (Ram[0xfa + intI] == 0)
                {
                    _arrTimerTarget[intI] = 0x100;
                }
                else
                {
                    _arrTimerTarget[intI] = Ram[0xfa + intI];
                }
            }
            SetAPUControl(Ram[0xf1]);
            if (DirectPageSelector)
            {
                DirectPage = 0x100;
            }
            else
            {
                DirectPage = 0;
            }
            _objDSP = new DSP();
            _objDSP.Prepare(objTempRegisters, ref _objRam);
        }

        /// <summary>
        /// run
        /// </summary>
        /// <param name="str"></param>
        public void Run(String str)
        {
            for (Int32 intC = 0; intC < 2048000 / 32 / 100; intC++)
            {
                for (Int32 intIC = 0; intIC < 32; intIC++)
                {
                    try
                    {
                        intCounter++;
                        if (intCounter == 1900)
                        {
                            intCounter = 1900;
                        }
                        Console.WriteLine(intCounter + ";" + PC + ";" + ToHex(Ram[PC]) + ";" + ToHex(Ram[PC + 1]) + ";" + ToHex(Ram[PC + 2]) + ";" + X);
                        Execute();
                        _intTimerErrorCounter++;
                        DoTimer();
                    }
                    catch (Exception)
                    {
                        Executing = false;
                    }
                }
            }
            Int32[] arrBuffer = _objDSP.MixSamples(ref _objRam);
        }

        #endregion

        #region Private Methods

        /*/// <summary>
        /// filler
        /// </summary>
        /// <param name="objData"></param>
        /// <param name="intSize"></param>
        private void Filler(IntPtr objData, Int32 intSize)
        {
            var objLength = Math.Min(intSize, _queue.Length);
            Marshal.Copy(_queue, 0, objData, objLength);
            _queue = _queue.Skip(intSize).ToArray();
        }*/

        /// <summary>
        /// byte to hex
        /// </summary>
        /// <param name="bteB"></param>
        /// <returns></returns>
        private String ToHex(Byte bteB)
        {
            var str=String.Empty;
            if ((bteB >> 4) < 10)
            {
                str = (bteB >> 4).ToString();
            }
            else
            {
                switch (bteB >> 4)
                {
                    case 10:
                        str = "A";
                        break;
                    case 11:
                        str = "B";
                        break;
                    case 12:
                        str = "C";
                        break;
                    case 13:
                        str = "D";
                        break;
                    case 14:
                        str = "E";
                        break;
                    case 15:
                        str = "F";
                        break;
                }
            }
            if ((bteB % 16) < 10)
            {
                str += (bteB % 16);
            }
            else
            {
                switch (bteB % 16)
                {
                    case 10:
                        str += "A";
                        break;
                    case 11:
                        str += "B";
                        break;
                    case 12:
                        str += "C";
                        break;
                    case 13:
                        str += "D";
                        break;
                    case 14:
                        str += "E";
                        break;
                    case 15:
                        str += "F";
                        break;
                }
            }
            return str;
        }

        /// <summary>
        /// execute
        /// </summary>
        private void Execute()
        {
            switch (Ram[PC] & 0x0f)
            {
                case 0 :
                    switch(Ram[PC] >> 4)
                    {
                        case 0 :
                            //00 : NOP
                            PC++;
                            break;
                        case 1 :
                            //10 : BRPL : Branch if Plus : http://support.atmel.no/knowledgebase/avrstudiohelp/mergedProjects/AVRASM/Html/BRPL.html
                            if(!Negative)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                        case 2 :
                            //20 : clear direct page
                            DirectPageSelector = false;
                            PC++;
                            break;
                        case 3 :
                            //30 : BRMI : branch if minus : http://www.wrightflyer.co.uk/asm/Html/BRMI.html
                            if (Negative)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                        case 4 :
                            //40 : SETP : Set Direct Page
                            DirectPageSelector = true;
                             PC++;
                            break;
                        case 5 :
                            //50 : BRVC : branch if overflow clear : http://www.wrightflyer.co.uk/asm/Html/BRVC.html
                            if (!Overflow)
                            {
                                PC = Relative1;
                            }
                            else
                            {
                                PC += 2;
                            }
                            break;
                        case 6 :
                            //60 : CLRC : clear carry
                            Carry = false;
                            PC++;
                            break;
                        case 7 :
                            //70 : BRVS : branch if overflow is set : http://www.wrightflyer.co.uk/asm/Html/BRVS.html
                            if (Overflow)
                            {
                                PC = Relative1;
                            }
                            else
                            {
                                PC += 2;
                            }
                            break;
                        case 8 :
                            //80 : SETC : Set Carry
                            Carry = true;
                            PC++;
                            break;
                        case 9 :
                            //90 : BRCC : branch if carry cleared : http://support.atmel.no/knowledgebase/avrstudiohelp/mergedProjects/AVRASM/Html/BRCC.html
                            if(!Carry)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                        case 10 :
                            //A0 : SETI : Set Interrupts
                            Interrupts = true;
                            PC++;
                            break;
                        case 11 :
                            //B0 : BRCS : branch if carry is set : http://www.wrightflyer.co.uk/asm/Html/BRCS.html
                            if(Carry)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                        case 12 :
                            //C0 : CLRI : clear Interrupts
                            Interrupts = false;
                            PC++;
                            break;
                        case 13 :
                            //D0 : BRNE : branch on not equals : http://support.atmel.no/knowledgebase/avrstudiohelp/mergedProjects/AVRASM/Html/BRNE.html
                            if(!Zero)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                        case 14 :
                            //E0 : CLRV : clear halfcarry and overflow
                            HalfCarry = false;
                            Overflow = false;
                            PC++;
                            break;
                        case 15 :
                            //F0 : BREQ (BRZ) : branch on equal : http://support.atmel.no/knowledgebase/avrstudiohelp/mergedProjects/AVRASM/Html/BREQ.html
                            if(Zero)
                            {
                                PC = Relative1;
                            }
                            else
                            {
	                            PC += 2;
                            }
                            break;
                    }
                    break;
                case 1:
                    TCALL(Ram[PC] >> 4);
                    break;
                case 2:
                    if ((Ram[PC] >> 4) % 2 == 0)
                    {
                        SET((Byte)(Ram[PC] >> 5));
                    }
                    else
                    {
                        CLR((Byte)(Ram[PC] >> 5));
                    }
                    break;
                case 3:
                    if ((Ram[PC] >> 4) % 2 == 0)
                    {
                        BRBS((Byte)(Ram[PC] >> 5));
                    }
                    else
                    {
                        BRBC((Byte)(Ram[PC] >> 5));
                    }
                    break;
                case 4:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //04 : OR A,dp
                            A |= objGetByte(Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative= (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 1:
                            //14 : OR A,dp+X
                            A |= objGetByte((Byte)(Ram[PC + 1] + X));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 2:
                            //24 : AND A,dp
                            A &= objGetByte(Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 3:
                            //34 : AND A,dp+X
                            A &= objGetByte((Byte)(Ram[PC + 1] + X));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 4:
                            //44 : EXOR A,dp
                            A ^= objGetByte(Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 5:
                            //54 : EXOR A,dp+X
                            A ^= objGetByte((Byte)(Ram[PC + 1] + X));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 6:
                            //64 : CMP A,dp : compare
                            Int32 intTemp = Math.Abs(A - objGetByte(Ram[PC + 1]));
                            Carry = intTemp >= 0;
                            Zero = intTemp == 0 ? true : false;
                            Negative = (intTemp & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 7:
                            //74 : CMP A, dp+X : compare
                            intTemp = Math.Abs(A - objGetByte((Byte)(Ram[PC + 1] + X)));
                            Carry = intTemp >= 0;
                            Zero = intTemp == 0 ? true : false;
                            Negative = (intTemp & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 8:
                            //84 : ADC A,dp : add with carry
                            Byte bteT = A;
                            ADC(ref bteT, objGetByte(Ram[PC + 1]));
                            A = bteT;
                            PC += 2;
                            break;
                        case 9:
                            //94 : ADC A,dp+X : add with carry
                            bteT = A;
                            ADC(ref bteT, objGetByte((Byte)(Ram[PC + 1] + X)));
                            A = bteT;
                            PC += 2;
                            break;
                        case 10:
                            //A4 : SBC A, dp
                            Byte bteTemp = A;
                            SBC(ref bteTemp, objGetByte(Ram[PC + 1]));
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 11:
                            //B4 : SBC A, dp+X
                            bteT = A;
                            SBC(ref bteT, objGetByte((Byte)(Ram[PC + 1] + X)));
                            A = bteT;
                            PC += 2;
                            break;
                        case 12:
                            //C4 : MOV dp,A
                            SetByte(A, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 13:
                            //D4 : MOV dp+X, A
                            SetByte(A, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 14:
                            //E4 : MOV A, dp
                            A = objGetByte(Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 15 :
                            //F4 : MOV A, dp+X
                            A = objGetByte((Byte)(Ram[PC + 1] + X));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        }
                    break;
                case 5:
                    switch (Ram[PC] >> 4)
                    {
                        case 0 :
                            //05 : OR A,abs
                            A |= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 1 :
                            //15 : OR A,abs+X
                            A |= objGetByteA(objGetByte((Byte)((Ram[PC + 2] << 8) + Ram[PC + 1] + X)));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 2 :
                            //25 : AND A,abs
                            A &= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 3 :
                            //35 : AND A,abs+X
                            A &= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 4 :
                            //45 : EOR A,abs
                            A ^= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 5 :
                            //55 : EOR A,abs+X
                            A ^= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 6 :
                            //65 : CMP A,abs
                            Int32 intTemp = Math.Abs(A - objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]));
                            Carry = intTemp >= 0;
                            Zero = intTemp == 0 ? true : false;
                            Negative = (intTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 7 :
                            //75 : CMP A,abs+X
                            var objM = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X);
                            intTemp = Math.Abs(A - objM);
                            Carry = A >= objM;
                            Zero = intTemp == 0 ? true : false;
                            Negative = (intTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 8 :
                            //85 : ADC A, abs
                            Byte bteTemp = A;
                            ADC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 9 :
                            //95 : ADC A, abs+X
                            bteTemp = A;
                            ADC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 10 : 
                            //A5 : SBC A, abs
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 11 :
                            //B5 : SBC A,abs+X
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 12 :
                            //C5 : MOV abs,A
                            SetByteA(A, (Ram[PC + 2] << 8) + Ram[PC + 1]);
                            PC += 3;
                            break;
                        case 13 : 
                            //D5 : MOV abs+X,A
                            SetByteA(A, (Ram[PC + 2] << 8) + Ram[PC + 1] + X);
                            PC += 3;
                            break;
                        case 14 :
                            //E5 : MOV A,abs
                            A = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 15 :
                            //F5 : MOV A, abs+X
                            A = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                    }
                    break;
                case 6:
                    switch (Ram[PC] >> 4)
                    {
                        case 0 :
                           //06 : OR A,(X)
                            A |= objGetByte(X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 1:
                            //16 : OR A,abs+Y
                            A |= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 2:
                            //26 : AND A,(X)
                            A &= objGetByte(X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 3:
                            //36 : AND A,abs+Y
                            A &= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 4:
                            //46 : EOR A,(X)
                            A ^= objGetByte(X);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 5:
                            //56 : EOR A,abs+Y
                            A ^= objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 6:
                            //66 : CMP A,(X)
                            CMP(A, objGetByte(X));
                            PC++;
                            break;
                        case 7:
                            //76 : CMP A, abs+Y
                            CMP(A, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y));
                            PC += 3;
                            break;
                        case 8 :
                            //86 : ADC A,(X)
                            Byte bteTemp = A;
                            ADC(ref bteTemp, objGetByte(X));
                            A = bteTemp;
                            PC++;
                            break;
                        case 9:
                            //96 : ADC A, abs+Y
                            bteTemp = A;
                            ADC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 10 :
                            //A6 : SBC A,(X)
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByte(X));
                            A = bteTemp;
                            PC++;
                            break;
                        case 11 :
                            //B6 : SBC A,abs+Y
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y));
                            A = bteTemp;
                            PC += 3;
                            break;
                        case 12:
                            //C6 : MOV(X), A
                            SetByte(A, X);
                            PC++;
                            break;
                        case 13:
                            //D6 : MOV abs+Y,A
                            SetByteA(A, (Ram[PC + 2] << 8) + Ram[PC + 1] + Y);
                            PC += 3;
                            break;
                        case 14 :
                            //E6 : MOV A,(X)
                            A = objGetByte(X);
                            Zero = A == 0 ? true : false;
                            PC++;
                            break;
                        case 15 :
                            //F6 : MOV A, abs+Y
                            A = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1] + Y);
                            Zero = A == 0 ? true : false;
                            PC += 3;
                            break;
                    }
                    break;
                case 7:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //07 : OR A,(dp+X)
                            A |= objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 1 :
                            //17 : OR A,(dp)+Y
                            A |= objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]]+ Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 2:
                            //27 : AND A,(dp+X)
                            Int32 intTemp = (Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)];
                            A &= objGetByteA(intTemp);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 3:
                            //37 : AND A,(dp)+Y
                            intTemp = (Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y;
                            A &= objGetByteA(intTemp);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 4:
                            //47 : EOR A,(dp+X)
                            A ^= objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 5 :
                            //57 : EOR A,(dp)+Y
                            A ^= objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 6 :
                            //67 : CMP A,(dp+X)
                            CMP(A, objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]));
                            PC += 2;
                            break;
                        case 7:
                            //77 : CMP A,(dp)+Y
                            CMP(A, objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y));
                            PC += 2;
                            break;
                        case 8:
                            //87 : ADC A,(dp+X)
                            Byte bteTemp = A;
                            ADC(ref bteTemp, objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]));
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 9:
                            //97 : ADC A,(dp)+Y
                            bteTemp = A;
                            ADC(ref bteTemp, objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y));
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 10 :
                            //A7 : SBC A,(dp+X)
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]));
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 11:
                            //B7 : SBC A,(dp)+Y
                            bteTemp = A;
                            SBC(ref bteTemp, objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y));
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 12:
                            //C7 : MOV(dp+X),A
                            SetByteA(A, (Ram[DirectPage + ((Ram[PC + 1] + 1 + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]);
                            PC += 2;
                            break;
                        case 13 :
                            //D7 : MOV(dp)+Y,A
                            SetByteA(A, (Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y);
                            PC += 2;
                            break;
                        case 14:
                            //E7 : MOV A,(dp+X)
                            A = objGetByteA((Ram[DirectPage + 1 + ((Ram[PC + 1] + X) & 0xff)] << 8) + Ram[DirectPage + ((Ram[PC + 1] + X) & 0xff)]);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 15:
                            //F7 : MOV A,(dp)+Y
                            A = objGetByteA((Ram[DirectPage + Ram[PC + 1] + 1] << 8) + Ram[DirectPage + Ram[PC + 1]] + Y);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                    }
                    break;
                case 8:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //08 : OR A,#00
                            A |= Ram[PC + 1];
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 1:
                            //18 : OR dp,#00
                            Byte bteTemp = Ram[PC + 1];
                            bteTemp |= objGetByte(Ram[PC + 2]);
                            SetByte(bteTemp, Ram[PC + 2]);
                            Zero = bteTemp == 0 ? true : false;
                            Negative = (bteTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 2:
                            //28 : AND A,#00
                            A &= Ram[PC + 1];
                            Zero = A == 0 ? true : false;
                            PC += 2;
                            break;
                        case 3:
                            //38 : AND dp,#00
                            Byte bteByte = Ram[PC + 1];
                            bteByte &= objGetByte(Ram[PC + 2]);
                            SetByte(bteByte, Ram[PC + 2]);
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 4:
                            //48 : EOR A,#00
                            A ^= Ram[PC + 1];
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 5:
                            //58 : EOR dp,#00
                            bteByte = Ram[PC + 1];
                            bteByte ^= objGetByte(Ram[PC + 2]);
                            SetByte(bteByte, Ram[PC + 2]);
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 6:
                            //68 : CMP A,#00
                            CMP(A, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 7:
                            //78 : CMP dp,#00
                            CMP(objGetByte(Ram[PC + 2]), Ram[PC + 1]);
                            PC += 3;
                            break;
                        case 8:
                            //88 : ADC A,#00
                            bteTemp = A;
                            ADC(ref bteTemp, Ram[PC + 1]);
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 9:
                            //98 : ADC dp,#00
                            bteByte = objGetByte(Ram[PC + 2]);
                            ADC(ref bteByte, Ram[PC + 1]);
                            SetByte(bteByte, Ram[PC + 2]);
                            PC += 3;
                            break;
                        case 10:
                            //A8 : SBC A,#00
                            bteTemp = A;
                            SBC(ref bteTemp, Ram[PC + 1]);
                            A = bteTemp;
                            PC += 2;
                            break;
                        case 11:
                            //B8 : SBC dp,#00
                            bteByte = objGetByte(Ram[PC + 2]);
                            SBC(ref bteByte, Ram[PC + 1]);
                            SetByte(bteByte, Ram[PC + 2]);
                            PC += 3;
                            break;
                        case 12:
                            //C8 : CMP X,#00
                            CMP(X, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 13:
                            //D8 : MOV dp,X
                            SetByte(X, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 14:
                            //E8 : MOV A,#00
                            A = Ram[PC + 1];
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 15:
                            //F8 : MOV X,dp
                            X = objGetByte(Ram[PC + 1]);
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                    }
                    break;
                case 9:
                    switch (Ram[PC] >> 4)
                    {
                        case 0 :
                            //09 : OR dp(dest),dp(src)
                            Byte bteTemp = objGetByte(Ram[PC + 1]);
                            bteTemp |= objGetByte(Ram[PC + 2]);
                            SetByte(bteTemp, Ram[PC + 2]);
                            Zero = bteTemp == 0 ? true : false;
                            Negative = (bteTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 1:
                            //19 : OR(X),(Y)
                            bteTemp = (Byte)(objGetByte(X) | objGetByte(Y));
                            Zero = bteTemp == 0 ? true : false;
                            Negative = (bteTemp & 0x80) > 1 ? true : false;
                            SetByte(bteTemp, X);
                            PC++;
                            break;
                        case 2:
                            //29 : AND dp(dest),dp(src)
                            Byte objByte = objGetByte(Ram[PC + 1]);
                            objByte &= objGetByte(Ram[PC + 2]);
                            SetByte(objByte, Ram[PC + 2]);
                            Zero = objByte == 0 ? true : false;
                            Negative = (objByte & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 3:
                            //39 : AND(X),(Y)
                            objByte = (Byte)(objGetByte(X) & objGetByte(Y));
                            Zero = objByte == 0 ? true : false;
                            Negative = (objByte & 0x80) > 1 ? true : false;
                            SetByte(objByte, X);
                            PC++;
                            break;
                        case 4:
                            //49 : EOR dp(dest),dp(src)
                            objByte = objGetByte(Ram[PC + 1]);
                            objByte ^= objGetByte(Ram[PC + 2]);
                            SetByte(objByte, Ram[PC + 2]);
                            Zero = objByte == 0 ? true : false;
                            Negative = (objByte & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 5:
                            //59 : EOR(X),(Y)
                            objByte = (Byte) (objGetByte(X) ^ objGetByte(Y));
                            Zero = objByte == 0 ? true : false;
                            Negative = (objByte & 0x80) > 1 ? true : false;
                            SetByte(objByte, X);
                            PC++;
                            break;
                        case 6:
                            //69 : CMP dp(dest), dp(src)
                            CMP(objGetByte(Ram[PC + 2]), objGetByte(Ram[PC + 1]));
                            PC += 3;
                            break;
                        case 7:
                            //79 : CMP(X),(Y)
                            CMP(objGetByte(X), objGetByte(Y));
                            PC++;
                            break;
                        case 8:
                            //89 : ADC dp(dest),dp(src)
                            objByte = objGetByte(Ram[PC + 2]);
                            ADC(ref objByte, objGetByte(Ram[PC + 1]));
                            SetByte(objByte, Ram[PC + 2]);
                            PC += 3;
                            break;
                        case 9:
                            //99 : ADC(X),(Y)
                            objByte = objGetByte(X);
                            ADC(ref objByte, objGetByte(Y));
                            SetByte(objByte, X);
                            PC++;
                            break;
                        case 10 :
                            //A9 : SBC dp(dest), dp(src)
                            objByte = objGetByte(Ram[PC + 2]);
                            SBC(ref objByte, objGetByte(Ram[PC + 1]));
                            SetByte(objByte, Ram[PC + 2]);
                            PC += 3;
                            break;
                        case 11:
                            //B9 : SBC(X),(Y)
                            objByte = objGetByte(Y);
                            SBC(ref objByte, objGetByte(X));
                            SetByte(objByte, X);
                            PC++;
                            break;
                        case 12 :
                            //C9 : MOV abs,X
                            SetByteA(X, (Ram[PC + 2] << 8) + Ram[PC + 1]);
                            PC += 3;
                            break;
                        case 13:
                            //D9 : MOV dp+Y,X
                            SetByte(X, (Byte)(Ram[PC + 1] + Y));
                            PC += 2;
                            break;
                        case 14 :
                            //E9 : MOV X, abs
                            X = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 15:
                            //F9 : MOV X,dp+Y
                            X = objGetByte((Byte)(Ram[PC + 1] + Y));
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                    }
                    break;
                case 10:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //0A : OR1 C,membit
                            Int16 shrTemp = (Int16)((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Byte bteTemp = (Byte)(shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if(!Carry)
                            {
                                if ((objGetByteA(shrTemp) & (Byte) (1 << bteTemp)) > 0)
                                {
	                                Carry = true;
                                }
                            }
                            PC += 3;
                            break;
                        case 1 :
                            //1A : DECW dp
                            shrTemp = (Int16)(objGetByte(Ram[PC + 1]) + (objGetByte((Byte)(Ram[PC + 1] + 1)) << 8));
                            shrTemp--;
                            SetByte((Byte)shrTemp, Ram[PC + 1]);
                            SetByte((Byte)(shrTemp >> 8), (Byte) (Ram[PC + 1] + 1));
                            Zero = shrTemp == 0 ? true : false;
                            Negative = (shrTemp & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 2:
                            //2A : OR1 C,not membit
                            shrTemp = (Int16)((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = (Byte)(shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if(!Carry)
                            {
                                if ((objGetByteA(shrTemp) & (1 << bteTemp))==0)
                                {
	                                Carry = true;
                                }
                            }
                            PC += 3;
                            break;
                        case 3 :
                            //3A : INCW dp
                            shrTemp = (Int16)(objGetByte(Ram[PC + 1]) + (objGetByte((Byte) (Ram[PC + 1] + 1)) << 8));
                            shrTemp++;
                            SetByte((Byte)shrTemp, Ram[PC + 1]);
                            SetByte((Byte)(shrTemp >> 8), Ram[PC + 2]);
                            Zero = shrTemp == 0 ? true : false;
                            Negative = (shrTemp & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 4:
                            //4A : AND1 C,membit
                            shrTemp = (Int16)((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = (Byte) (shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if(Carry)
                            {
                                if ((objGetByteA(shrTemp) & (1 << bteTemp))==0)
                                {
                                    Carry = false;
                                }
                            }
                            PC += 3;
                            break;
                        case 5:
                            //5A : CMPW YA,dp
                            shrTemp = (Int16) (objGetByte(Ram[PC + 1]) + (objGetByte((Byte)(Ram[PC + 1] + 1)) << 8));
                            Int32 intI = W - shrTemp;
                            Carry = intI >= 0;
                            Zero = shrTemp == 0 ? true : false;
                            Negative = (shrTemp & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 6:
                            //6A : AND1 C, not membit
                            shrTemp = (Int16) (Ram[PC + 2] + (Ram[PC + 1] << 8));
                            bteTemp = (Byte) (shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if (Carry)
                            {
                                if ((objGetByteA(shrTemp) & (1 << bteTemp))>0)
                                {
                                    Carry = false;
                                }
                            }
                            PC += 3;
                            break;
                        case 7:
                            //7A : ADDW YA,dp
                            shrTemp = (Int16)(objGetByte(Ram[PC + 1]) + (objGetByte((Byte)(Ram[PC + 1] + 1)) << 8));
                            Int32 intTemp2 = W + shrTemp;
                            Carry = (intTemp2 >= 0x10000);
                            if ((~(W ^ shrTemp) & (shrTemp ^ intTemp2) & 0x8000) > 0)
                            {
	                            Overflow = true;
                            }
                            else
                            {
                                Overflow = false;
                            }
                            HalfCarry = false;
                            if (((W ^ shrTemp ^ intTemp2) & 0x10) > 0)
                            {
                                HalfCarry = true;
                            }
                            W = (Int16)intTemp2;
                            Zero = W == 0 ? true : false;
                            Negative = (W & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 8 :
                            //8A : EOR1 C, membit
                            shrTemp = (Int16) (Ram[PC + 2] << 8 + (Ram[PC + 1]));
                            bteTemp = (Byte) (shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if(Carry)
                            {
                                if ((objGetByteA(shrTemp) & (1 << bteTemp))>0)
                                {
	                                Carry = false;
                                }
                            }
                            else
                            {
                                if ((objGetByteA(shrTemp) & (1 << bteTemp)) >0)
                                {
	                                Carry = true;
                                }
                            }
                            PC += 3;
                            break;
                        case 9 :
                            //9A : SUBW YA,dp
                            shrTemp = (Int16)(objGetByte(Ram[PC + 1]) + (objGetByte((Byte) (Ram[PC + 1] + 1)) << 8));
                            intI = (Int16) (W - shrTemp);
                            HalfCarry = false;
                            Carry = intI >= 0;
                            if ((((W ^ shrTemp) & 0x8000) & ((W ^ intI) & 0x8000)) > 0)
                            {
	                            Overflow = true;
                            }
                            else
                            {
                                Overflow = false;
                            }
                            HalfCarry = true;
                            if (((W ^ shrTemp ^ intI) & 0x10) > 0)
                            {
                                HalfCarry = false;
                            }
                            W = (Int16) intI;
                            Zero = W == 0 ? true : false;
                            Negative = (W & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 10 :
                            //AA : MOV1 C,membit
                            shrTemp = (Int16) ((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = (Byte) (shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if ((objGetByteA(shrTemp) & (1 << bteTemp)) > 0)
                            {
	                            Carry = true;
                            }
                            else
                            {
	                            Carry = false;
                            }
                            PC += 3;
                            break;
                        case 11:
                            //BA : MOVW YA,dp
                            A = objGetByte(Ram[PC + 1]);
                            Y = objGetByte((Byte)(Ram[PC + 1] + 1));
                            Zero = W == 0 ? true : false;
                            Negative = (W & 0x8000) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 12 :
                            //CA : MOV1 membit,C
                            shrTemp = (Int16) ((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = (Byte) (shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            if(Carry)
                            {
                                SetByteA((Byte) (objGetByteA(shrTemp) | (1 << bteTemp)), shrTemp);
                            }
                            else
                            {
                                SetByteA((Byte) (objGetByteA(shrTemp) & ~(1 << bteTemp)), shrTemp);
                            }
                            PC += 3;
                            break;
                        case 13 :
                            //DA : MOVW dp,YA
                            SetByte(A, Ram[PC + 1]);
                            SetByte(Y, (Byte)(Ram[PC + 1] + 1));
                            PC += 2;
                            break;
                        case 14:
                            //EA : NOT1 membit
                            shrTemp = (Int16)((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = (Byte)(shrTemp >> 13);
                            shrTemp &= 0x1fff;
                            SetByteA((Byte) (objGetByteA(shrTemp) ^ (1 << bteTemp)), shrTemp);
                            PC += 3;
                            break;
                        case 15:
                            //FA : MOV dp(dest),dp(src)
                            SetByte(objGetByte(Ram[PC + 1]), Ram[PC + 2]);
                            PC += 3;
                            break;
                    }
                    break;
                case 11:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //0B : ASL dp
                            Byte bteTemp = objGetByte(Ram[PC + 1]);
                            ASL(ref bteTemp);
                            SetByte(bteTemp, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 1:
                            //1B : ASL dp+X
                            bteTemp = objGetByte((Byte)(Ram[PC + 1] + X));
                            ASL(ref bteTemp);
                            SetByte(bteTemp, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 2:
                            //2B : ROL dp
                            Byte bteByte = objGetByte(Ram[PC + 1]);
                            ROL(ref bteByte);
                            SetByte(bteByte, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 3:
                            //3B : ROL dp+X
                            bteByte = objGetByte((Byte)(Ram[PC + 1] + X));
                            ROL(ref bteByte);
                            SetByte(bteByte, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 4:
                            //4B : LSR dp
                            bteByte = objGetByte(Ram[PC + 1]);
                            LSR(ref bteByte);
                            SetByte(bteByte, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 5:
                            //5B : LSR dp+X
                            bteByte = objGetByte((Byte)(Ram[PC + 1] + X));
                            LSR(ref bteByte);
                            SetByte(bteByte, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 6:
                            //6B : ROR dp
                            bteByte = objGetByte(Ram[PC + 1]);
                            ROR(ref bteByte);
                            SetByte(bteByte, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 7:
                            //7B : ROR dp+X
                            bteByte = objGetByte((Byte)(Ram[PC + 1] + X));
                            ROR(ref bteByte);
                            SetByte(bteByte, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 8:
                            //8B : DEC dp
                            bteByte = (Byte)(objGetByte(Ram[PC + 1]) - 1);
                            SetByte(bteByte, Ram[PC + 1]);
                            Zero = bteByte == 0 ? true : false;
                            PC += 2;
                            break;
                        case 9:
                            //9B : DEC dp+X
                            bteByte = (Byte)(objGetByte((Byte)(Ram[PC + 1] + X)) - 1);
                            SetByte(bteByte, (Byte)(Ram[PC + 1] + X));
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 10:
                            //AB : INC dp
                            bteByte = (Byte)(objGetByte(Ram[PC + 1]) + 1);
                            SetByte(bteByte, Ram[PC + 1]);
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 11:
                            //BB : INC dp+X
                            bteByte = (Byte)(objGetByte((Byte)(Ram[PC + 1] + X)) + 1);
                            SetByte(bteByte, (Byte)(Ram[PC + 1] + X));
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 12:
                            //CB : MOV dp,Y
                            SetByte(Y, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 13:
                            //DB : MOV dp+X,Y
                            SetByte(Y, (Byte)(Ram[PC + 1] + X));
                            PC += 2;
                            break;
                        case 14:
                            //EB : MOV Y,dp
                            Y = objGetByte(Ram[PC + 1]);
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 15:
                            //FB : MOV Y,dp+X
                            Y = objGetByte((Byte)(Ram[PC + 1] + X));
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                    }
                    break;
                case 12:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //0C : ASL abs
                            Int16 shrAddress = (Int16)((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Byte bteTemp = objGetByteA(shrAddress);
                            ASL(ref bteTemp);
                            SetByteA(bteTemp, shrAddress);
                            PC += 3;
                            break;
                        case 1:
                            //1C : ASL A
                            bteTemp = A;
                            ASL(ref bteTemp);
                            A = bteTemp;
                            PC++;
                            break;
                        case 2:
                            //2C : ROL abs
                            Int32 intTemp = (Ram[PC + 2] << 8) + Ram[PC + 1];
                            Byte bteByte = objGetByteA(intTemp);
                            ROL(ref bteByte);
                            SetByteA(bteByte, intTemp);
                            PC += 3;
                            break;
                        case 3 :
                            //3C : ROL A
                            bteByte = A;
                            ROL(ref bteByte);
                            A = bteByte;
                            PC++;
                            break;
                        case 4 :
                            //4C : LSR abs
                            shrAddress = (Int16) ((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            bteByte = objGetByteA(shrAddress);
                            LSR(ref bteByte);
                            SetByteA(bteByte, shrAddress);
                            PC += 3;
                            break;
                        case 5:
                            //5C : LSR A
                            bteTemp = A;
                            LSR(ref bteTemp);
                            A = bteTemp;
                            PC++;
                            break;
                        case 6 :
                            //6C : ROR abs
                            shrAddress = (Int16) ((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            bteByte = objGetByteA(shrAddress);
                            ROR(ref bteByte);
                            SetByteA(bteByte, shrAddress);
                            PC += 3;
                            break;
                        case 7:
                            //7C : ROR A
                            bteByte = A;
                            ROR(ref bteByte);
                            A = bteByte;
                            PC++;
                            break;
                        case 8 :
                            //8C : DEC abs
                            shrAddress = (Int16) ((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            bteByte = (Byte)(objGetByteA(shrAddress) - 1);
                            SetByteA(bteByte, shrAddress);
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            break;
                        case 9:
                            //9C : DEC A
                            A--;
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 10:
                            //AC : INC abs
                            shrAddress = (Int16) ((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            bteByte = (Byte)(objGetByteA(shrAddress) + 1);
                            SetByteA(bteByte, shrAddress);
                            Zero = bteByte == 0 ? true : false;
                            Negative = (bteByte & 0x80) > 1 ? true : false;
                            break;
                        case 11:
                            //BC : INC A
                            A++;
                            Zero = A == 0 ? true : false;
                            PC++;
                            break;
                        case 12:
                            //CC : MOV abs,Y
                            SetByteA(Y, (Ram[PC + 2] << 8) + Ram[PC + 1]);
                            PC += 3;
                            break;
                        case 13 :
                            //DC : DEC Y
                            Y--;
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 14 :
                            //EC : MOV Y,abs
                            Y = objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]);
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 15:
                            //FC : INC Y
                            Y++;
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                    }
                    break;
                case 13:
                    switch (Ram[PC] >> 4)
                    {
                        case 0 :
                            //0D : PUSH PSW
                            Ram[0x100 + SP] = PSW;
                            SP--;
                            PC++;
                            break;
                        case 1:
                            //1D : DEC X
                            X--;
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 2 :
                            //2D : PUSH A
                            Ram[0x100 + SP] = A;
                            SP--;
                            PC++;
                            break;
                        case 3 :
                            //3D : INC X
                            X++;
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 4:
                            //4D : PUSH X
                            Ram[0x100 + SP] = X;
                            SP--;
                            PC++;
                            break;
                        case 5:
                            //5D : MOV X,A
                            X = A;
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 6:
                            //6D : PUSH Y
                            Ram[0x100 + SP] = Y;
                            SP--;
                            PC++;
                            break;
                        case 7:
                            //7D : MOV A,X
                            A = X;
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 8 :
                            //8D : MOV Y,#00
                            Y = Ram[PC + 1];
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 9:
                            //9D : MOV X,SP
                            X = SP;
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 10 :
                            //AD : CMP Y,#00
                            CMP(Y, Ram[PC + 1]);
                            PC += 2;
                            break;
                        case 11:
                            //BD : MOV SP,X
                            SP = X;
                            PC++;
                            break;
                        case 12:
                            //CD : MOV X,#00
                            X = Ram[PC + 1];
                            Zero = X == 0 ? true : false;
                            Negative = (X & 0x80) > 1 ? true : false;
                            PC += 2;
                            break;
                        case 13 :
                            //DD : MOV A,Y
                            A = Y;
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 14 :
                            //ED : NOTC
                            Carry = Carry ? false : true;
                            PC++;
                            break;
                        case 15:
                            //FD : MOV Y,A
                            Y = A;
                            Zero = Y == 0 ? true : false;
                            Negative = (Y & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                    }
                    break;
                case 14:
                    switch (Ram[PC] >> 4)
                    {
                        case 0 :
                            //0E : TSET1 abs
                            Int16 shrT = (Int16) ((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            Byte bteTemp = objGetByteA(shrT);
                            SetByteA((Byte) (bteTemp | A), shrT);
                            bteTemp &= A;
                            Zero = bteTemp == 0 ? true : false;
                            Negative = (bteTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 1 :
                            //1E : CMP X,abs
                            CMP(X, objGetByteA((Ram[PC + 2] << 8) + Ram[PC + 1]));
                            PC += 3;
                            break;
                        case 2:
                            //2E : CBNE dp,rel
                            if (objGetByte(Ram[PC + 1]) != A)
                            {
                                PC = (Int16) Relative2;
                            }
                            else
                            {
                                PC += 3;
                            }
                            break;
                        case 3:
                            //3E : CMP X,dp
                            CMP(X, objGetByte(Ram[PC + 1]));
                            PC += 2;
                            break;
                        case 4:
                            //4E : TCLR1 abs
                            shrT = (Int16) ((Ram[PC + 2] << 8) + (Ram[PC + 1]));
                            bteTemp = objGetByteA(shrT);
                            SetByteA((Byte) (bteTemp & ~A), shrT);
                            bteTemp &= A;
                            Zero = bteTemp == 0 ? true : false;
                            Negative = (bteTemp & 0x80) > 1 ? true : false;
                            PC += 3;
                            break;
                        case 5:
                            //5E : CMP Y,abs
                            CMP(Y, objGetByteA((objGetByteA(Ram[PC + 2] << 8) + Ram[PC + 1])));
                            PC += 3;
                            break;
                        case 6:
                            //6E : DBNZ dp,rel
                            Byte objByte = (Byte)(objGetByte(Ram[PC + 1]) - 1);
                            SetByte(objByte, Ram[PC + 1]);
                            if (objByte != 0)
                            {
                                PC = (Int16)Relative2;
                            }
                            else
                            {
                                PC += 3;
                            }
                            break;
                        case 7:
                            //7E : CMP Y,dp
                            CMP(Y, objGetByte(Ram[PC + 1]));
                            PC += 2;
                            break;
                        case 8:
                            //POP PSW
                            SP++;
                            PSW = Ram[0x100 + SP];
                            if(DirectPageSelector)
                            {
	                            DirectPage = 0x100;
                            }
                            else
                            {
	                            DirectPage = 0;
                            }
                            PC++;
                            break;
                        case 9:
                            //9E : DIV YA,X
                            if (X == 0)
                            {
                                Overflow = true;
                                Y = 0xff;
                                A = 0xff;
                            }
                            else
                            {
                                Overflow = false;
                                Y = (Byte)(W % X);
                                A = (Byte)(W / X);
                            }
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 10:
                            //AE : POP A
                            SP++;
                            A = Ram[0x100 + SP];
                            PC++;
                            break;
                        case 11:
                            //BE : DAS
                            if ((A & 0x0f) > 9 || !HalfCarry)
                            {
                                A -= 6;
                            }
                            if (A > 0x9f || !Carry)
                            {
                                A -= 0x60;
                                Carry = false;
                            }
                            else
                            {
                                Carry = true;
                            }
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 12:
                            //CE : POP X
                            SP++;
                            X = Ram[0x100 + SP];
                            PC++;
                            break;
                        case 13 :
                            //DE : CBNE dp+X,rel
                            if (objGetByte((Byte)(Ram[PC + 1] + X)) != A)
                            {
                                PC = (Int16)Relative2;
                            }
                            else
                            {
                                PC += 3;
                            }
                            break;
                        case 14 :
                            //EE : POP Y
                            SP++;
                            Y = Ram[0x100 + SP];
                            PC++;
                            break;
                        case 15:
                            //FE : DBNZ Y,rel
                            Y--;
                            if (Y != 0)
                            {
                                PC = (Int16)Relative1;
                            }
                            else
                            {
                                PC += 2;
                            }
                            break;
                    }
                    break;
                case 15:
                    switch (Ram[PC] >> 4)
                    {
                        case 0:
                            //0F : BRK
                            PushWord((Int16) ((Ram[PC + 2] << 8) + Ram[PC + 1]));
                            Ram[0x100 + SP] = PSW;
                            SP--;
                            SoftwareBreak = true;
                            Interrupts = false;
                            PC = Ram[ExtraRam[0x20] +(ExtraRam[0x21] << 8)];
                            break;
                        case 1:
                            //1F : JMP(abs+X)
                            Int32 intTemp = (Ram[PC + 2] << 8) + Ram[PC + 1];
                            PC = objGetByteA(intTemp + X) + (objGetByteA(intTemp + X + 1) << 8);
                            break;
                        case 2:
                            //2F : BRA
                            PC = Relative1;
                            break;
                        case 3:
                            //3F : CALL absolute
                            PushWord((Int16)(PC + 3));
                            PC = (Ram[PC + 2] << 8) + Ram[PC + 1];
                            break;
                        case 4:
                            //4F : PCALL $XX
                            PushWord((Int16)(PC + 2));
                            PC = Ram[0xff00 + Ram[PC + 1]];
                            break;
                        case 5:
                            //5F : JMP abs
                            PC = (Ram[PC + 2] << 8) + Ram[PC + 1];
                            break;
                        case 6:
                            //6F : RET
                            SP += 2;
                            PC = (Int16)((Ram[0x100 + SP] << 8) + Ram[0xff + SP]);
                            break;
                        case 7:
                            //7F : STOP
                            SP++;
                            PSW = Ram[0x100 + SP];
                            SP += 2;
                            PC = (Int16)((Ram[0x100 + SP] << 8) + Ram[0xff + SP]);
                            break;
                        case 8:
                            //8F : MOV dp,#00
                            SetByte(Ram[PC + 1], Ram[PC + 2]);
                            PC += 3;
                            break;
                        case 9 :
                            //9F : XCN A
                            A = (Byte) ((A >> 4) | (A << 4));
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 10:
                            //AF : MOV(X)+, A
                            SetByte(A, X++);
                            PC++;
                            break;
                        case 11:
                            //BF : MOV A,(X)+
                            A = objGetByte(X++);
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 12:
                            //CF : MUL YA
                            W = (Int16) (A * Y);
                            Zero = W == 0 ? true : false;
                            Negative = (W & 0x8000) > 1 ? true : false;
                            PC++;
                            break;
                        case 13:
                            //DF : DAA
                            if ((A & 0x0f) > 9 || HalfCarry)
                            {
                                if (A > 0xf0)
                                {
                                    Carry = true;
                                }
                                A += 6;
                            }
                            if (A > 0x9f || Carry)
                            {
                                A += 0x60;
                                Carry = true;
                            }
                            else
                            {
                                Carry = false;
                            }
                            Zero = A == 0 ? true : false;
                            Negative = (A & 0x80) > 1 ? true : false;
                            PC++;
                            break;
                        case 14:
                            //EF : SLEEP
                            Executing = false;
                            PC++;
                            break;
                        case 15:
                            //FF : STOP
                            Executing = false;
                            PC++;
                            break;
                    }
                break;
            }
        }

        /// <summary>
        /// do timer
        /// </summary>
        private void DoTimer()
        {
            if (_arrTimerEnabled[2])
            {
                _arrTimer[2] += 4;
                if (_arrTimer[2] >= _arrTimerTarget[2]) 
                {
	                Ram[0xff] = (Byte)((Ram[0xff] + 1) & 0xf);
                    _arrTimer[2] -= _arrTimerTarget[2];
	            }
            }
            if (_intTimerErrorCounter >= 8)
            {
                _intTimerErrorCounter = 0;
                if (_arrTimerEnabled[0])
	            {
	                _arrTimer[0]++;
	                if (_arrTimer[0] >= _arrTimerTarget[0])
	                {
	                    Ram[0xfd] = (Byte)((Ram[0xfd] + 1) & 0xf);
	                    _arrTimer [0] = 0;        
	                }
	            }
                if (_arrTimerEnabled[1])
	            {
	                _arrTimer[1]++;
	                if (_arrTimer[1] >= _arrTimerTarget[1])
	                {
	                    Ram[0xfe] = (Byte)((Ram[0xfe] + 1) & 0xf);
	                    _arrTimer[1] = 0;     
	                }
	            }
            }
        }

        #region Reading

        /// <summary>
        /// read a string based on filestream and length
        /// </summary>
        /// <param name="objReader">reader</param>
        /// <param name="intCount">length</param>
        /// <returns>read string</returns>
        private String ReadString(FileStream objReader, Int32 intCount)
        {
            var arrBuffer = new Byte[intCount];
            objReader.Read(arrBuffer, 0, intCount);
            return Encoding.ASCII.GetString(arrBuffer);
        }

        /// <summary>
        /// read registers
        /// </summary>
        /// <param name="objReader">filestream</param>
        public void ReadRegisters(FileStream objReader)
        {
            var arrBuffer = new Byte[9];
            objReader.Read(arrBuffer, 0, 9);
            PC = arrBuffer[1]<<8 | arrBuffer[0];
            X = arrBuffer[3];
            W = (Int16)((arrBuffer[4] << 8) + arrBuffer[2]);
            PSW = arrBuffer[5];
            SP = arrBuffer[6];
            Reserved1 = arrBuffer[7];
            Reserved2 = arrBuffer[8];
        }

        #endregion

        #region Helper

        /// <summary>
        /// push word in stack
        /// </summary>
        /// <param name="shrTemp">the word</param>
        private void PushWord(Int16 shrTemp)
        {
            Ram[0xff + SP] = (Byte)shrTemp;
            Ram[0x100 + SP] = (Byte)(shrTemp >> 8);
            SP -= 2;
        }

        /// <summary>
        /// call a function by setting the PC to a spot stored in the ram indirected by the extra ram
        /// </summary>
        /// <param name="intN">number to call</param>
        private void TCALL(Int32 intN)
        {
            PushWord((Int16) (PC + 1)); 
            PC = Ram[(ExtraRam[((15 - intN) << 1)] +(ExtraRam [((15 - intN) << 1) + 1] << 8))]; 
        }

        /// <summary>
        /// BRBS: branch before shift?
        /// </summary>
        /// <param name="bteShift">shift</param>
        private void BRBS(Byte bteShift)
        {
            if ((objGetByte(Ram[PC + 1]) & (1 << (bteShift))) > 0)
            {
                PC = Relative2;
            }
            else 
            {
                PC += 3;
            }
        }

        /// <summary>
        /// BRBC : branch before carry?
        /// </summary>
        /// <param name="bteShift">shift</param>
        private void BRBC(Byte bteShift)
        {
            if ((objGetByte(Ram[PC + 1]) & (1 << (bteShift))) == 0) 
            {
                PC = Relative2;
            }
            else
            {
                PC += 3;
            }
        }

        /// <summary>
        /// set shift
        /// </summary>
        /// <param name="bteShift">shift</param>
        private void SET(Byte bteShift)
        {
            SetByte((Byte)(objGetByte(Ram[PC + 1]) | (1 << bteShift)), Ram[PC + 1]);
            PC += 2;
        }

        /// <summary>
        /// clear
        /// </summary>
        /// <param name="bteShift">shift</param>
        private void CLR(Byte bteShift)
        {
            SetByte(objGetByte((Byte) (Ram[PC + 1] & ~(1 << bteShift))), Ram[PC + 1]);
            PC += 2;
        }

        /// <summary>
        /// ADC add with carry
        /// </summary>
        /// <param name="bteA">a</param>
        /// <param name="bteB">b</param>
        private void ADC(ref Byte bteA, Byte bteB)
        {
            Int16 shrTemp = (Int16) (bteA + bteB + (Carry ? 1 : 0));
            Carry = shrTemp >= 0x100;
            if ((~(bteA ^ bteB) & (bteB ^ shrTemp) & 0x80)>0)
            {
                Overflow = true;
            }
            else
            {
                Overflow = false;
            }
            HalfCarry = false;
            if (((bteA ^ bteB ^ shrTemp) & 0x10)>0)
            {
                HalfCarry = true;
            }
            bteA = (Byte) shrTemp;
            Zero = shrTemp == 0 ? true : false;
            Negative = (shrTemp & 0x80)> 0 ? true : false;
        }

        /// <summary>
        /// SBC subtract with carry
        /// </summary>
        /// <param name="bteA">a</param>
        /// <param name="bteB">b</param>
        private void SBC(ref Byte bteA, Byte bteB)
        {
            Int16 shrTemp = (Int16) (bteA - bteB + (Carry ? 1 : 0) - 1);
            Carry = shrTemp >= 0;
            if ((((bteA ^ bteB) & 0x80) & ((bteA ^ shrTemp) & 0x80))>0)
            {
                Overflow = true;
            }
            else
            {
                Overflow = false;
            }
            HalfCarry = true;
            if (((bteA ^ bteB ^ shrTemp) & 0x10)>0)
            {
                HalfCarry = false;
            }
            bteA = (Byte) shrTemp;
            Zero = shrTemp == 0 ? true : false;
            Negative = (shrTemp & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// CMP
        /// </summary>
        /// <param name="bteA">a</param>
        /// <param name="bteB">b</param>
        private void CMP(Byte bteA, Byte bteB)
        {
            Int16 shrTemp = (Int16) Math.Abs(bteA - bteB);
            Carry = bteA >= bteB;
            Zero = shrTemp == 0 ? true : false;
            Negative = (shrTemp & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// ASL
        /// </summary>
        /// <param name="bteB">b</param>
        private void ASL(ref Byte bteB)
        {
            Carry = (bteB & 0x80) != 0;
            bteB <<= 1;
            Zero = bteB == 0 ? true : false;
            Negative = (bteB & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// LSR
        /// </summary>
        /// <param name="bteB">b</param>
        private void LSR(ref Byte bteB)
        {
            Carry = (bteB & 1)>0 ? true : false;
            bteB >>= 1;
            Zero = bteB == 0 ? true : false;
            Negative = (bteB & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// ROL
        /// </summary>
        /// <param name="bteB">b</param>
        private void ROL(ref Byte bteB)
        {
            Int16 shrTemp = (Int16) ((bteB << 1) | (Carry ? 1 : 0));
            Carry = shrTemp >= 0x100;
            bteB = (Byte) shrTemp;
            Zero = bteB == 0 ? true : false;
            Negative = (bteB & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// ROR
        /// </summary>
        /// <param name="bteB">b</param>
        private void ROR(ref Byte bteB)
        {
            Int16 shrTemp = (Int16) (bteB | ((Carry ? 1 : 0) << 8));
            Carry = (shrTemp & 1)>0 ? true : false;
            shrTemp >>= 1;
            bteB = (Byte) shrTemp;
            Zero = bteB == 0 ? true : false;
            Negative = (bteB & 0x80) > 0 ? true : false;
        }

        /// <summary>
        /// get byte
        /// </summary>
        /// <param name="bteByte">position to read</param>
        /// <returns>the byte</returns>
        private Byte objGetByte(Byte bteByte)
        {
            if(bteByte >= 0xf0 && DirectPage == 0)
            {
	            if(bteByte >= 0xf4 && bteByte <= 0xf7)
	            {   
	                return Ram[bteByte];
	            }
	            if(bteByte >= 0xfd)
	            {   
	                Byte bteTemp = Ram[bteByte];
	                Ram[bteByte] = 0;
	                return(bteTemp);
	            } 
                else if(bteByte == 0xf3)
                {
                    return _objDSP.objGetByte(Ram[0xf2] & 0x7f);
                }
	            return(Ram[bteByte]);
            }
            else
            {
	            return(Ram[DirectPage+bteByte]);
            }
        }

        /// <summary>
        /// get byte A
        /// </summary>
        /// <param name="intPos">position to read</param>
        /// <returns>the byte</returns>
        private Byte objGetByteA(Int32 intAddress)
        {
            unchecked { intAddress &= (Int16)0xffff; }
            if (intAddress <= 0xff && intAddress >= 0xf0)
            {
                if (intAddress >= 0xf4 && intAddress <= 0xf7)
	            {
                    return (Ram[intAddress]);
	            }
                else if (intAddress == 0xf3)
                {
                    return _objDSP.objGetByte(Ram[0xf2] & 0x7f);
                }
                if (intAddress >= 0xfd)
	            {
                    Byte objT = Ram[intAddress];
                    Ram[intAddress] = 0;
	                return objT;
	            }
                return (Ram[intAddress]);
            }
            else
	        {
                return (Ram[intAddress]);
            }
        }

        /// <summary>
        /// set byte
        /// </summary>
        /// <param name="bteByte">byte to set</param>
        /// <param name="intAddress">address</param>
        private void SetByte(Byte bteByte, Byte btePos)
        {
            if (btePos >= 0xf0 && DirectPage == 0)
            {
                if (btePos == 0xf3)
                {
	                _objDSP.SetByte(bteByte,Ram[0xf2], ref _objRam);
                }
                else if (btePos >= 0xf4 && btePos <= 0xf7)
                {
                    OutPorts[btePos - 0xf4] = bteByte;
                }
                else if (btePos == 0xf1)
                {
	                SetAPUControl(bteByte);
                }
                else if (btePos < 0xfd)
	            {
                    Ram[btePos] = bteByte;
                    if (btePos >= 0xfa)
                    {
                        if (bteByte == 0)
                        {
                            _arrTimerTarget[btePos - 0xfa] = 0x100;
                        }
                        else
                        {
                            _arrTimerTarget[btePos - 0xfa] = bteByte;
                        }
                    }
	            }
            }
            else
            {
                Ram[DirectPage + btePos] = bteByte;
            }
        }

        /// <summary>
        /// set apu control
        /// </summary>
        /// <param name="bteByte">byte</param>
        private void SetAPUControl(Byte bteByte)
        {
            if ((bteByte & 1) != 0 && !_arrTimerEnabled [0])
            {
		        _arrTimer[0] = 0;
		        Ram[0xfd] = 0;
		        if ((_arrTimerTarget [0] = Ram[0xfa]) == 0)
                {
			        _arrTimerTarget [0] = 0x100;
                }
            }
            if ((bteByte & 2) != 0 && !_arrTimerEnabled [1])
            {
		        _arrTimer[1] = 0;
		        Ram[0xfe] = 0;
		        if ((_arrTimerTarget [1] = Ram[0xfb]) == 0)
                {
			        _arrTimerTarget [1] = 0x100;
                }
            }
            if ((bteByte & 4) != 0 && !_arrTimerEnabled [2])
            {
		        _arrTimer[2] = 0;
		        Ram[0xff] = 0;
		        if ((_arrTimerTarget [2] = Ram[0xfc]) == 0)
                {
			        _arrTimerTarget [2] = 0x100;
                }
            }
            _arrTimerEnabled[0] = (bteByte & 1) > 0 ? true : false;
            _arrTimerEnabled[1] = ((bteByte & 2) >> 1) > 0 ? true : false;
            _arrTimerEnabled[2] = ((bteByte & 4) >> 2) > 0 ? true : false;
	
            if ((bteByte & 0x10)>0)
            {
		        Ram[0xf4] = Ram[0xf5] = 0;
            }
            if ((bteByte & 0x20) > 0)
            {
		        Ram[0xf6] = Ram[0xf7] = 0;
            }
            /*TODO if (objByte & 0x80)
            {
		        if (!APU.ShowROM)
		        {
			        memmove (&IAPU.RAM [0xffc0], APUROM, sizeof (APUROM));
			       APU.ShowROM = TRUE;
		        }
            }
            else
            {
		        if (APU.ShowROM)
		        {
			        APU.ShowROM = FALSE;
			        memmove (&IAPU.RAM [0xffc0], APU.ExtraRAM, sizeof (APUROM));
		        }
            }*/
            Ram[0xf1] =bteByte;
        }

        /// <summary>
        /// set byte
        /// </summary>
        /// <param name="bteByte">byte to set</param>
        /// <param name="intAddress">address</param>
        private void SetByteA(Byte bteByte, Int32 intAddress)
        {
            intAddress &= 0xffff;
            if (intAddress <= 0xff && intAddress >= 0xf0)
            {
	            if (intAddress == 0xf3)
                {
                    _objDSP.SetByte(bteByte, Ram[0xf2], ref _objRam);
                }
	            else if (intAddress >= 0xf4 && intAddress <= 0xf7)
                {
	                OutPorts[intAddress - 0xf4] = bteByte;
                }
	            else if (intAddress == 0xf1)
                {
	                SetAPUControl(bteByte);
                }
	            else if (intAddress < 0xfd)
	            {
	                Ram[intAddress] = bteByte;
	                if (intAddress >= 0xfa)
	                {
		                if (bteByte == 0)
                        {
		                    _arrTimerTarget[intAddress - 0xfa] = 0x100;
                        }
		                else
                        {
		                    _arrTimerTarget[intAddress - 0xfa] = bteByte;
	                    }
                    }
	            }
            }
            else
            {
	            if (intAddress < 0xffc0)
                {
	                Ram[intAddress] = bteByte;
                }
	            else
	            {
	                ExtraRam[intAddress - 0xffc0] = bteByte;
	                /*TODO if (!APU.ShowROM)
                    {
		                Ram[intAddress] = objByte;
                    }*/
	            }
            }
        }

        #endregion

        #endregion
    }
}
