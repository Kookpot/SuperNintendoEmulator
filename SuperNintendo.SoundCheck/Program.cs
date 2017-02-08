using System;
using SuperNintendo.Sound;

namespace SuperNintendo.SoundCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var objSPC700 = new SPC700();
            objSPC700.Prepare("smk-02.spc");
            while (objSPC700.Executing)
            {
                objSPC700.Run("");
            }
        }
    }
}
