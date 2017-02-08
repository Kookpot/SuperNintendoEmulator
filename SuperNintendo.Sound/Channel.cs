using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperNintendo.Sound
{
    /// <summary>
    /// channel
    /// </summary>
    public class Channel
    {
        #region Members

        public Int32 State {get;set;}
        public Int32 EnvX { get; set; }
        public Int32 Type { get; set; }
        public Int16 VolumeLeft { get; set; }
        public Int16 VolumeRight { get; set; }
        public Int32 Hertz { get; set; }
        public Int32 Frequency { get; set; }
        public Int32 Count { get; set; }
        public Boolean Loop { get; set; }
        public Int16 LeftVolLevel { get; set; }
        public Int16 RightVolLevel { get; set; }
        public Int16 EnvxTarget { get; set; }
        public Int64 EnvError { get; set; }
        public Int64 ERate { get; set; }
        public Int32 Direction { get; set; } 
        public Int64 AttackRate { get; set; }
        public Int64 DecayRate { get; set; }
        public Int64 SustainRate { get; set; }
        public Int64 ReleaseRate { get; set; }
        public Int64 SustainLevel { get; set; }
        public Int16 Sample { get; set; }
        public Int16[] Decoded { get; set; }
        public Int16[] Previous16 { get; set; }
        public Int16[] Block { get; set; } 
        public Int16 SampleNumber { get; set; }
        public Boolean LastBlock { get; set; }
        public Boolean NeedsDecode { get; set; }
        public Int32 BlockPointer { get; set; }
        public Int32 SamplePointer { get; set; }
        public Int32[] EchoBufPtr { get; set; }
        public Int32 Mode { get; set; }
        public Int32 Envxx { get; set; }
        public Int16 NextSample { get; set; }
        public Int32 Interpolate { get; set; }
        public Int32[] Previous { get; set; }
        // Just incase they are needed in the future, for snapshot compatibility.
        public Int32[] Dummy { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// constructor
        /// </summary>
        public Channel()
        {
            Decoded = new Int16[16];
            Previous16 = new Int16[2];
            Previous = new Int32[2];
            Dummy = new Int32[8];
        }

        #endregion
    }
}
