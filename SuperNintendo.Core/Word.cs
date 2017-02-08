using System;

namespace SuperNintendo.Core
{
    /// <summary>
    /// a word (instruction)
    /// </summary>
    public class Word
    {
        #region Properties

        public Byte Instruction {get;set;}
        public Byte[] Parameters { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public Word()
        {
        }

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="objB1"></param>
        /// <param name="objB2"></param>
        public Word(Byte objB1,Byte objB2)
        {
            Instruction = objB1;
            Parameters = new Byte[] { objB2 };
        }

        #endregion
    }
}
