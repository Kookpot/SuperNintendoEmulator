using System;

namespace SuperNintendo.Core
{
    public static class c65816
    {
        #region Private Members

        //possible shifted PB
        private static UInt32 _intShiftedPB;
        //operand
        private static Int64 _lngOperand;
        //current program counter
        private static Int64 _lngProgramCounter;
        //beginning of program counter
        private static Int64 _lngProgramCounterBase;
        //current word
        private static UInt16 _intWork16;

        //registers??
        private static Boolean _blnZero;
        private static Boolean _blnCarry;
        private static Boolean _blnNegative;
        private static Boolean _blnOverflow;
        private static Boolean _blnDecimal;
        private static Boolean _blnIRQDisable;
        private static Boolean _blnIndex8;
        private static Boolean _blnRegister8;

        private static UInt16 _intA_W;
        private static SByte _objCarry;
        private static SByte _objOverflow;
        private static SByte _objZero;
        private static SByte _objNegative;

        #endregion

        #region Public Methods

        /// <summary>
        /// execute an instruction
        /// </summary>
        /// <param name="objW"></param>
        public static void Execute(Word objW)
        {
            switch(objW.Instruction)
            {
                //adc indexed indirect x
                case 97:
                    break;
                //adc stack relative
                case 99:
                    break;
                //adc direct page
                case 101:
                    break;
                //adc indirect long
                case 103:
                    break;
                //adc immediate
                case 105:
                    Immediate16();
                    ADC16();
                    break;
                //adc absolute
                case 109:
                    break;
                //adc absolute long
                case 111:
                    break;
                //adc indexed indirect y
                case 113:
                    break;
                //adc dp indirect
                case 114:
                    break;
                //adc SR indirect index y
                case 115:
                    break;
                //adc dp indexed x
                case 117:
                    break;
                //adc dp indirect long indexed y
                case 119:
                    break;
                //adc absolute indexed y
                case 121:
                    break;
                //adc absolute indexed x
                case 125:
                    break;
                //adc absolute long indexed x
                case 127:
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// read immediate 16 bit
        /// </summary>
        private static void Immediate16()
        {
            _lngOperand = _intShiftedPB + _lngProgramCounter - _lngProgramCounterBase;
            _lngProgramCounter += 2;
        }

        /// <summary>
        /// checkoverflow
        /// </summary>
        /// <param name="objA1"></param>
        /// <param name="objA2"></param>
        private static void CheckOverFlow(ref SByte objA1, ref SByte objA2)
        {
            if (objA1 > 9)
            {
                objA1 -= 10;
                objA1 &= 0xF;
                objA2++;
            }
        }

        /// <summary>
        /// set Z and N
        /// </summary>
        /// <param name="intWork"></param>
        private static void SetZN16 (UInt16 intWork)
        {
            if (intWork != 0)
            {
                _objZero = 1;
            }
            else
            {
                _objZero = 0;
            }
            _objNegative = (SByte)(intWork >> 8);
        }

        /// <summary>
        /// adc 16 bit
        /// </summary>
        private static void ADC16()
        {
            _intWork16 = Memory.objGet16((UInt32)_lngOperand);
            if (_blnDecimal)
            {
                SByte objA1 = (SByte) ((_intA_W) & 0xF);
                SByte objA2 = (SByte) ((_intA_W >> 4) & 0xF);
                SByte objA3 = (SByte) ((_intA_W >> 8) & 0xF);
                SByte objA4 = (SByte) ((_intA_W >> 12) & 0xF);
                SByte objW1 = (SByte) (_intWork16 & 0xF);
                SByte objW2 = (SByte) ((_intWork16 >> 4) & 0xF);
                SByte objW3 = (SByte) ((_intWork16 >> 8) & 0xF);
                SByte objW4 = (SByte) ((_intWork16 >> 12) & 0xF);
	            objA1 += (SByte) (objW1 + _objCarry);
                CheckOverFlow(ref objA1,ref objA2);
	            objA2 += objW2;
	            CheckOverFlow(ref objA2,ref objA3);
	            objA3 += objW3;
                CheckOverFlow(ref objA3,ref objA4);
	            objA4 += objW4;
                _objCarry = 0;
                CheckOverFlow(ref objA4,ref _objCarry);
                UInt16 intAns16 = (UInt16) ((objA4 << 12) | (objA3 << 8) | (objA2 << 4) | (objA1));
                if ((~(_intA_W ^ _intWork16) & (_intWork16 ^ intAns16) & 0x8000)>0)
                {
                    _objOverflow =1;
                }
                else
                {
                    _objOverflow=0;
                }
	            _intA_W = intAns16;
                SetZN16(_intA_W);
            }
            else
            {
	            UInt32 intAns32 = (UInt32) (_intA_W + _intWork16 + _objCarry);
                if (intAns32 >= 0x10000)
                {
                    _objCarry = 1;
                }
                else
                {
                    _objCarry = 0;
                }
                if ((~(_intA_W ^ _intWork16) & (_intWork16 ^ (UInt16)intAns32) & 0x8000)>0)
                {
                    _objOverflow =1;
                }
                else
                {
                    _objOverflow=0;
                }
	            _intA_W = (UInt16) intAns32;
                SetZN16(_intA_W);
            }
        }

        #endregion
    }
}
