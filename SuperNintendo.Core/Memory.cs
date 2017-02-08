using System;

namespace SuperNintendo.Core
{
    /// <summary>
    /// memory
    /// </summary>
    public static class Memory
    {
        #region Public Members

        private static SByte _objOpenBus;
        // bank     //offset
        //$00-$3F	$0000-$1FFF	LowRAM
        //$00-$3F   $2000-$2FFF	PPU1, APU
        //$00-$3F   $3000-$3FFF	DSP, SuperFX	hardware registers
        //$00-$3F   $4000-$41FF	controller	hardware registers
        //$00-$3F   $4200-$4FFF	DMA, PPU2?	hardware registers
        //$00-$3F   $6000-$7FFF	Chips	enhancement chips memory
        //$00-$3F   $8000-$FFFF	ROM	Data that is mapped here depends on the cartridge.
        //$40-$7D	$0000-$FFFF	ROM	Data that is mapped here depends on the cartridge.
        //$7E	    $0000-$1FFF	LowRAM
        //$7E       $2000-$7FFF	HighRAM
        //$7E       $8000-$FFFF	Extended RAM
        //$7F	    $0000-$FFFF	Extended RAM

        private static Byte[] _objMemory = new Byte[16777216]; 

        #endregion

        #region Public Methods

        /// <summary>
        /// get word from address
        /// </summary>
        public static UInt16 objGet16(UInt32 intAddress)
        {
            return (UInt16) (objGetByte(intAddress) | (objGetByte(intAddress+1) << 8));
        }

        /// <summary>
        /// get byte
        /// </summary>
        /// <param name="intAddress">addres to get</param>
        /// <returns>byte at place</returns>
        public static Byte objGetByte(UInt32 intAddress)
        {
            return _objMemory[intAddress];
        }

        /// <summary>
        /// memory move
        /// </summary>
        /// <param name="arr1"></param>
        /// <param name="?"></param>
        /// <param name="arr2"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        public static void memmove(ref SByte[] arr1, Int32 intI, ref SByte[] arr2, Int32 intJ, Int32 intLength)
        {
            for(Int32 intK=0;intK<intLength;intK++)
            {
                arr1[intI+intK] = arr2[intJ+intK];
            }
        }

        #endregion
    }
}
