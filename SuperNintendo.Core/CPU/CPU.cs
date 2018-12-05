using SuperNintendo.Core.Memory;
using System;

namespace SuperNintendo.Core.CPU
{
    public static class CPU
    {
        public static Action[] OpcodesE1 = new Action[256]
        {
            Op00, Op01E1, Op02, Op03M1, Op04M1,
            Op05M1, Op06M1, Op07M1, Op08E1, Op09M1,
            Op0AM1, Op0BE1, Op0CM1, Op0DM1, Op0EM1,
            Op0FM1, Op10E1, Op11E1, Op12E1, Op13M1,
            Op14M1, Op15E1, Op16E1, Op17M1, Op18,
            Op19M1X1, Op1AM1, Op1B, Op1CM1, Op1DM1X1,
            Op1EM1X1, Op1FM1, Op20E1, Op21E1, Op22E1,
            Op23M1, Op24M1, Op25M1, Op26M1, Op27M1,
            Op28E1, Op29M1, Op2AM1, Op2BE1, Op2CM1,
            Op2DM1, Op2EM1, Op2FM1, Op30E1, Op31E1,
            Op32E1, Op33M1, Op34E1, Op35E1, Op36E1,
            Op37M1, Op38, Op39M1X1, Op3AM1, Op3B,
            Op3CM1X1, Op3DM1X1, Op3EM1X1, Op3FM1, Op40Slow,
            Op41E1, Op42, Op43M1, Op44X1, Op45M1,
            Op46M1, Op47M1, Op48E1, Op49M1, Op4AM1,
            Op4BE1, Op4C, Op4DM1, Op4EM1, Op4FM1,
            Op50E1, Op51E1, Op52E1, Op53M1, Op54X1,
            Op55E1, Op56E1, Op57M1, Op58, Op59M1X1,
            Op5AE1, Op5B, Op5C, Op5DM1X1, Op5EM1X1,
            Op5FM1, Op60E1, Op61E1, Op62E1, Op63M1,
            Op64M1, Op65M1, Op66M1, Op67M1, Op68E1,
            Op69M1, Op6AM1, Op6BE1, Op6C, Op6DM1,
            Op6EM1, Op6FM1, Op70E1, Op71E1, Op72E1,
            Op73M1, Op74E1, Op75E1, Op76E1, Op77M1,
            Op78, Op79M1X1, Op7AE1, Op7B, Op7C,
            Op7DM1X1, Op7EM1X1, Op7FM1, Op80E1, Op81E1,
            Op82, Op83M1, Op84X1, Op85M1, Op86X1,
            Op87M1, Op88X1, Op89M1, Op8AM1, Op8BE1,
            Op8CX1, Op8DM1, Op8EX1, Op8FM1, Op90E1,
            Op91E1, Op92E1, Op93M1, Op94E1, Op95E1,
            Op96E1, Op97M1, Op98M1, Op99M1X1, Op9A,
            Op9BX1, Op9CM1, Op9DM1X1, Op9EM1X1, Op9FM1,
            OpA0X1, OpA1E1, OpA2X1, OpA3M1, OpA4X1,
            OpA5M1, OpA6X1, OpA7M1, OpA8X1, OpA9M1,
            OpAAX1, OpABE1, OpACX1, OpADM1, OpAEX1,
            OpAFM1, OpB0E1, OpB1E1, OpB2E1, OpB3M1,
            OpB4E1, OpB5E1, OpB6E1, OpB7M1, OpB8,
            OpB9M1X1, OpBAX1, OpBBX1, OpBCX1, OpBDM1X1,
            OpBEX1, OpBFM1, OpC0X1, OpC1E1, OpC2,
            OpC3M1, OpC4X1, OpC5M1, OpC6M1, OpC7M1,
            OpC8X1, OpC9M1, OpCAX1, OpCB, OpCCX1,
            OpCDM1, OpCEM1, OpCFM1, OpD0E1, OpD1E1,
            OpD2E1, OpD3M1, OpD4E1, OpD5E1, OpD6E1,
            OpD7M1, OpD8, OpD9M1X1, OpDAE1, OpDB,
            OpDC, OpDDM1X1, OpDEM1X1, OpDFM1, OpE0X1,
            OpE1E1, OpE2, OpE3M1, OpE4X1, OpE5M1,
            OpE6M1, OpE7M1, OpE8X1, OpE9M1, OpEA,
            OpEB, OpECX1, OpEDM1, OpEEM1, OpEFM1,
            OpF0E1, OpF1E1, OpF2E1, OpF3M1, OpF4E1,
            OpF5E1, OpF6E1, OpF7M1, OpF8, OpF9M1X1,
            OpFAE1, OpFB, OpFCE1, OpFDM1X1, OpFEM1X1,
            OpFFM1
        };

        public static Action[] OpcodesM1X1 = new Action[256]
        {
            Op00, Op01E0M1, Op02, Op03M1, Op04M1,
            Op05M1, Op06M1, Op07M1, Op08E0, Op09M1,
            Op0AM1, Op0BE0, Op0CM1, Op0DM1, Op0EM1,
            Op0FM1, Op10E0, Op11E0M1X1, Op12E0M1, Op13M1,
            Op14M1, Op15E0M1, Op16E0M1, Op17M1, Op18,
            Op19M1X1, Op1AM1, Op1B, Op1CM1, Op1DM1X1,
            Op1EM1X1, Op1FM1, Op20E0, Op21E0M1, Op22E0,
            Op23M1, Op24M1, Op25M1, Op26M1, Op27M1,
            Op28E0, Op29M1, Op2AM1, Op2BE0, Op2CM1,
            Op2DM1, Op2EM1, Op2FM1, Op30E0, Op31E0M1X1,
            Op32E0M1, Op33M1, Op34E0M1, Op35E0M1, Op36E0M1,
            Op37M1, Op38, Op39M1X1, Op3AM1, Op3B,
            Op3CM1X1, Op3DM1X1, Op3EM1X1, Op3FM1, Op40Slow,
            Op41E0M1, Op42, Op43M1, Op44X1, Op45M1,
            Op46M1, Op47M1, Op48E0M1, Op49M1, Op4AM1,
            Op4BE0, Op4C, Op4DM1, Op4EM1, Op4FM1,
            Op50E0, Op51E0M1X1, Op52E0M1, Op53M1, Op54X1,
            Op55E0M1, Op56E0M1, Op57M1, Op58, Op59M1X1,
            Op5AE0X1, Op5B, Op5C, Op5DM1X1, Op5EM1X1,
            Op5FM1, Op60E0, Op61E0M1, Op62E0, Op63M1,
            Op64M1, Op65M1, Op66M1, Op67M1, Op68E0M1,
            Op69M1, Op6AM1, Op6BE0, Op6C, Op6DM1,
            Op6EM1, Op6FM1, Op70E0, Op71E0M1X1, Op72E0M1,
            Op73M1, Op74E0M1, Op75E0M1, Op76E0M1, Op77M1,
            Op78, Op79M1X1, Op7AE0X1, Op7B, Op7C,
            Op7DM1X1, Op7EM1X1, Op7FM1, Op80E0, Op81E0M1,
            Op82, Op83M1, Op84X1, Op85M1, Op86X1,
            Op87M1, Op88X1, Op89M1, Op8AM1, Op8BE0,
            Op8CX1, Op8DM1, Op8EX1, Op8FM1, Op90E0,
            Op91E0M1X1, Op92E0M1, Op93M1, Op94E0X1, Op95E0M1,
            Op96E0X1, Op97M1, Op98M1, Op99M1X1, Op9A,
            Op9BX1, Op9CM1, Op9DM1X1, Op9EM1X1, Op9FM1,
            OpA0X1, OpA1E0M1, OpA2X1, OpA3M1, OpA4X1,
            OpA5M1, OpA6X1, OpA7M1, OpA8X1, OpA9M1,
            OpAAX1, OpABE0, OpACX1, OpADM1, OpAEX1,
            OpAFM1, OpB0E0, OpB1E0M1X1, OpB2E0M1, OpB3M1,
            OpB4E0X1, OpB5E0M1, OpB6E0X1, OpB7M1, OpB8,
            OpB9M1X1, OpBAX1, OpBBX1, OpBCX1, OpBDM1X1,
            OpBEX1, OpBFM1, OpC0X1, OpC1E0M1, OpC2,
            OpC3M1, OpC4X1, OpC5M1, OpC6M1, OpC7M1,
            OpC8X1, OpC9M1, OpCAX1, OpCB, OpCCX1,
            OpCDM1, OpCEM1, OpCFM1, OpD0E0, OpD1E0M1X1,
            OpD2E0M1, OpD3M1, OpD4E0, OpD5E0M1, OpD6E0M1,
            OpD7M1, OpD8, OpD9M1X1, OpDAE0X1, OpDB,
            OpDC, OpDDM1X1, OpDEM1X1, OpDFM1, OpE0X1,
            OpE1E0M1, OpE2, OpE3M1, OpE4X1, OpE5M1,
            OpE6M1, OpE7M1, OpE8X1, OpE9M1, OpEA,
            OpEB, OpECX1, OpEDM1, OpEEM1, OpEFM1,
            OpF0E0, OpF1E0M1X1, OpF2E0M1, OpF3M1, OpF4E0,
            OpF5E0M1, OpF6E0M1, OpF7M1, OpF8, OpF9M1X1,
            OpFAE0X1, OpFB, OpFCE0, OpFDM1X1, OpFEM1X1,
            OpFFM1
        };

        public static Action[] OpcodesM1X0 = new Action[256]
        {
            Op00, Op01E0M1, Op02, Op03M1, Op04M1,
            Op05M1, Op06M1, Op07M1, Op08E0, Op09M1,
            Op0AM1, Op0BE0, Op0CM1, Op0DM1, Op0EM1,
            Op0FM1, Op10E0, Op11E0M1X0, Op12E0M1, Op13M1,
            Op14M1, Op15E0M1, Op16E0M1, Op17M1, Op18,
            Op19M1X0, Op1AM1, Op1B, Op1CM1, Op1DM1X0,
            Op1EM1X0, Op1FM1, Op20E0, Op21E0M1, Op22E0,
            Op23M1, Op24M1, Op25M1, Op26M1, Op27M1,
            Op28E0, Op29M1, Op2AM1, Op2BE0, Op2CM1,
            Op2DM1, Op2EM1, Op2FM1, Op30E0, Op31E0M1X0,
            Op32E0M1, Op33M1, Op34E0M1, Op35E0M1, Op36E0M1,
            Op37M1, Op38, Op39M1X0, Op3AM1, Op3B,
            Op3CM1X0, Op3DM1X0, Op3EM1X0, Op3FM1, Op40Slow,
            Op41E0M1, Op42, Op43M1, Op44X0, Op45M1,
            Op46M1, Op47M1, Op48E0M1, Op49M1, Op4AM1,
            Op4BE0, Op4C, Op4DM1, Op4EM1, Op4FM1,
            Op50E0, Op51E0M1X0, Op52E0M1, Op53M1, Op54X0,
            Op55E0M1, Op56E0M1, Op57M1, Op58, Op59M1X0,
            Op5AE0X0, Op5B, Op5C, Op5DM1X0, Op5EM1X0,
            Op5FM1, Op60E0, Op61E0M1, Op62E0, Op63M1,
            Op64M1, Op65M1, Op66M1, Op67M1, Op68E0M1,
            Op69M1, Op6AM1, Op6BE0, Op6C, Op6DM1,
            Op6EM1, Op6FM1, Op70E0, Op71E0M1X0, Op72E0M1,
            Op73M1, Op74E0M1, Op75E0M1, Op76E0M1, Op77M1,
            Op78, Op79M1X0, Op7AE0X0, Op7B, Op7C,
            Op7DM1X0, Op7EM1X0, Op7FM1, Op80E0, Op81E0M1,
            Op82, Op83M1, Op84X0, Op85M1, Op86X0,
            Op87M1, Op88X0, Op89M1, Op8AM1, Op8BE0,
            Op8CX0, Op8DM1, Op8EX0, Op8FM1, Op90E0,
            Op91E0M1X0, Op92E0M1, Op93M1, Op94E0X0, Op95E0M1,
            Op96E0X0, Op97M1, Op98M1, Op99M1X0, Op9A,
            Op9BX0, Op9CM1, Op9DM1X0, Op9EM1X0, Op9FM1,
            OpA0X0, OpA1E0M1, OpA2X0, OpA3M1, OpA4X0,
            OpA5M1, OpA6X0, OpA7M1, OpA8X0, OpA9M1,
            OpAAX0, OpABE0, OpACX0, OpADM1, OpAEX0,
            OpAFM1, OpB0E0, OpB1E0M1X0, OpB2E0M1, OpB3M1,
            OpB4E0X0, OpB5E0M1, OpB6E0X0, OpB7M1, OpB8,
            OpB9M1X0, OpBAX0, OpBBX0, OpBCX0, OpBDM1X0,
            OpBEX0, OpBFM1, OpC0X0, OpC1E0M1, OpC2,
            OpC3M1, OpC4X0, OpC5M1, OpC6M1, OpC7M1,
            OpC8X0, OpC9M1, OpCAX0, OpCB, OpCCX0,
            OpCDM1, OpCEM1, OpCFM1, OpD0E0, OpD1E0M1X0,
            OpD2E0M1, OpD3M1, OpD4E0, OpD5E0M1, OpD6E0M1,
            OpD7M1, OpD8, OpD9M1X0, OpDAE0X0, OpDB,
            OpDC, OpDDM1X0, OpDEM1X0, OpDFM1, OpE0X0,
            OpE1E0M1, OpE2, OpE3M1, OpE4X0, OpE5M1,
            OpE6M1, OpE7M1, OpE8X0, OpE9M1, OpEA,
            OpEB, OpECX0, OpEDM1, OpEEM1, OpEFM1,
            OpF0E0, OpF1E0M1X0, OpF2E0M1, OpF3M1, OpF4E0,
            OpF5E0M1, OpF6E0M1, OpF7M1, OpF8, OpF9M1X0,
            OpFAE0X0, OpFB, OpFCE0, OpFDM1X0, OpFEM1X0,
            OpFFM1
        };

        public static Action[] OpcodesM0X1 = new Action[256]
        {
            Op00, Op01E0M0, Op02, Op03M0, Op04M0,
            Op05M0, Op06M0, Op07M0, Op08E0, Op09M0,
            Op0AM0, Op0BE0, Op0CM0, Op0DM0, Op0EM0,
            Op0FM0, Op10E0, Op11E0M0X1, Op12E0M0, Op13M0,
            Op14M0, Op15E0M0, Op16E0M0, Op17M0, Op18,
            Op19M0X1, Op1AM0, Op1B, Op1CM0, Op1DM0X1,
            Op1EM0X1, Op1FM0, Op20E0, Op21E0M0, Op22E0,
            Op23M0, Op24M0, Op25M0, Op26M0, Op27M0,
            Op28E0, Op29M0, Op2AM0, Op2BE0, Op2CM0,
            Op2DM0, Op2EM0, Op2FM0, Op30E0, Op31E0M0X1,
            Op32E0M0, Op33M0, Op34E0M0, Op35E0M0, Op36E0M0,
            Op37M0, Op38, Op39M0X1, Op3AM0, Op3B,
            Op3CM0X1, Op3DM0X1, Op3EM0X1, Op3FM0, Op40Slow,
            Op41E0M0, Op42, Op43M0, Op44X1, Op45M0,
            Op46M0, Op47M0, Op48E0M0, Op49M0, Op4AM0,
            Op4BE0, Op4C, Op4DM0, Op4EM0, Op4FM0,
            Op50E0, Op51E0M0X1, Op52E0M0, Op53M0, Op54X1,
            Op55E0M0, Op56E0M0, Op57M0, Op58, Op59M0X1,
            Op5AE0X1, Op5B, Op5C, Op5DM0X1, Op5EM0X1,
            Op5FM0, Op60E0, Op61E0M0, Op62E0, Op63M0,
            Op64M0, Op65M0, Op66M0, Op67M0, Op68E0M0,
            Op69M0, Op6AM0, Op6BE0, Op6C, Op6DM0,
            Op6EM0, Op6FM0, Op70E0, Op71E0M0X1, Op72E0M0,
            Op73M0, Op74E0M0, Op75E0M0, Op76E0M0, Op77M0,
            Op78, Op79M0X1, Op7AE0X1, Op7B, Op7C,
            Op7DM0X1, Op7EM0X1, Op7FM0, Op80E0, Op81E0M0,
            Op82, Op83M0, Op84X1, Op85M0, Op86X1,
            Op87M0, Op88X1, Op89M0, Op8AM0, Op8BE0,
            Op8CX1, Op8DM0, Op8EX1, Op8FM0, Op90E0,
            Op91E0M0X1, Op92E0M0, Op93M0, Op94E0X1, Op95E0M0,
            Op96E0X1, Op97M0, Op98M0, Op99M0X1, Op9A,
            Op9BX1, Op9CM0, Op9DM0X1, Op9EM0X1, Op9FM0,
            OpA0X1, OpA1E0M0, OpA2X1, OpA3M0, OpA4X1,
            OpA5M0, OpA6X1, OpA7M0, OpA8X1, OpA9M0,
            OpAAX1, OpABE0, OpACX1, OpADM0, OpAEX1,
            OpAFM0, OpB0E0, OpB1E0M0X1, OpB2E0M0, OpB3M0,
            OpB4E0X1, OpB5E0M0, OpB6E0X1, OpB7M0, OpB8,
            OpB9M0X1, OpBAX1, OpBBX1, OpBCX1, OpBDM0X1,
            OpBEX1, OpBFM0, OpC0X1, OpC1E0M0, OpC2,
            OpC3M0, OpC4X1, OpC5M0, OpC6M0, OpC7M0,
            OpC8X1, OpC9M0, OpCAX1, OpCB, OpCCX1,
            OpCDM0, OpCEM0, OpCFM0, OpD0E0, OpD1E0M0X1,
            OpD2E0M0, OpD3M0, OpD4E0, OpD5E0M0, OpD6E0M0,
            OpD7M0, OpD8, OpD9M0X1, OpDAE0X1, OpDB,
            OpDC, OpDDM0X1, OpDEM0X1, OpDFM0, OpE0X1,
            OpE1E0M0, OpE2, OpE3M0, OpE4X1, OpE5M0,
            OpE6M0, OpE7M0, OpE8X1, OpE9M0, OpEA,
            OpEB, OpECX1, OpEDM0, OpEEM0, OpEFM0,
            OpF0E0, OpF1E0M0X1, OpF2E0M0, OpF3M0, OpF4E0,
            OpF5E0M0, OpF6E0M0, OpF7M0, OpF8, OpF9M0X1,
            OpFAE0X1, OpFB, OpFCE0, OpFDM0X1, OpFEM0X1,
            OpFFM0
        };

        public static Action[] OpcodesM0X0 = new Action[256]
        {
            Op00, Op01E0M0, Op02, Op03M0, Op04M0,
            Op05M0, Op06M0, Op07M0, Op08E0, Op09M0,
            Op0AM0, Op0BE0, Op0CM0, Op0DM0, Op0EM0,
            Op0FM0, Op10E0, Op11E0M0X0, Op12E0M0, Op13M0,
            Op14M0, Op15E0M0, Op16E0M0, Op17M0, Op18,
            Op19M0X0, Op1AM0, Op1B, Op1CM0, Op1DM0X0,
            Op1EM0X0, Op1FM0, Op20E0, Op21E0M0, Op22E0,
            Op23M0, Op24M0, Op25M0, Op26M0, Op27M0,
            Op28E0, Op29M0, Op2AM0, Op2BE0, Op2CM0,
            Op2DM0, Op2EM0, Op2FM0, Op30E0, Op31E0M0X0,
            Op32E0M0, Op33M0, Op34E0M0, Op35E0M0, Op36E0M0,
            Op37M0, Op38, Op39M0X0, Op3AM0, Op3B,
            Op3CM0X0, Op3DM0X0, Op3EM0X0, Op3FM0, Op40Slow,
            Op41E0M0, Op42, Op43M0, Op44X0, Op45M0,
            Op46M0, Op47M0, Op48E0M0, Op49M0, Op4AM0,
            Op4BE0, Op4C, Op4DM0, Op4EM0, Op4FM0,
            Op50E0, Op51E0M0X0, Op52E0M0, Op53M0, Op54X0,
            Op55E0M0, Op56E0M0, Op57M0, Op58, Op59M0X0,
            Op5AE0X0, Op5B, Op5C, Op5DM0X0, Op5EM0X0,
            Op5FM0, Op60E0, Op61E0M0, Op62E0, Op63M0,
            Op64M0, Op65M0, Op66M0, Op67M0, Op68E0M0,
            Op69M0, Op6AM0, Op6BE0, Op6C, Op6DM0,
            Op6EM0, Op6FM0, Op70E0, Op71E0M0X0, Op72E0M0,
            Op73M0, Op74E0M0, Op75E0M0, Op76E0M0, Op77M0,
            Op78, Op79M0X0, Op7AE0X0, Op7B, Op7C,
            Op7DM0X0, Op7EM0X0, Op7FM0, Op80E0, Op81E0M0,
            Op82, Op83M0, Op84X0, Op85M0, Op86X0,
            Op87M0, Op88X0, Op89M0, Op8AM0, Op8BE0,
            Op8CX0, Op8DM0, Op8EX0, Op8FM0, Op90E0,
            Op91E0M0X0, Op92E0M0, Op93M0, Op94E0X0, Op95E0M0,
            Op96E0X0, Op97M0, Op98M0, Op99M0X0, Op9A,
            Op9BX0, Op9CM0, Op9DM0X0, Op9EM0X0, Op9FM0,
            OpA0X0, OpA1E0M0, OpA2X0, OpA3M0, OpA4X0,
            OpA5M0, OpA6X0, OpA7M0, OpA8X0, OpA9M0,
            OpAAX0, OpABE0, OpACX0, OpADM0, OpAEX0,
            OpAFM0, OpB0E0, OpB1E0M0X0, OpB2E0M0, OpB3M0,
            OpB4E0X0, OpB5E0M0, OpB6E0X0, OpB7M0, OpB8,
            OpB9M0X0, OpBAX0, OpBBX0, OpBCX0, OpBDM0X0,
            OpBEX0, OpBFM0, OpC0X0, OpC1E0M0, OpC2,
            OpC3M0, OpC4X0, OpC5M0, OpC6M0, OpC7M0,
            OpC8X0, OpC9M0, OpCAX0, OpCB, OpCCX0,
            OpCDM0, OpCEM0, OpCFM0, OpD0E0, OpD1E0M0X0,
            OpD2E0M0, OpD3M0, OpD4E0, OpD5E0M0, OpD6E0M0,
            OpD7M0, OpD8, OpD9M0X0, OpDAE0X0, OpDB,
            OpDC, OpDDM0X0, OpDEM0X0, OpDFM0, OpE0X0,
            OpE1E0M0, OpE2, OpE3M0, OpE4X0, OpE5M0,
            OpE6M0, OpE7M0, OpE8X0, OpE9M0, OpEA,
            OpEB, OpECX0, OpEDM0, OpEEM0, OpEFM0,
            OpF0E0, OpF1E0M0X0, OpF2E0M0, OpF3M0, OpF4E0,
            OpF5E0M0, OpF6E0M0, OpF7M0, OpF8, OpF9M0X0,
            OpFAE0X0, OpFB, OpFCE0, OpFDM0X0, OpFEM0X0,
            OpFFM0
        };

        public static Action[] OpcodesSlow = new Action[256]
        {
            Op00, Op01Slow, Op02, Op03Slow, Op04Slow, Op05Slow, Op06Slow, Op07Slow, Op08Slow, Op09Slow,
            Op0ASlow, Op0BSlow, Op0CSlow, Op0DSlow, Op0ESlow, Op0FSlow, Op10Slow, Op11Slow, Op12Slow, Op13Slow,
            Op14Slow, Op15Slow, Op16Slow, Op17Slow, Op18,
            Op19Slow, Op1ASlow, Op1B, Op1CSlow, Op1DSlow,
            Op1ESlow, Op1FSlow, Op20Slow, Op21Slow, Op22Slow,
            Op23Slow, Op24Slow, Op25Slow, Op26Slow, Op27Slow,
            Op28Slow, Op29Slow, Op2ASlow, Op2BSlow, Op2CSlow,
            Op2DSlow, Op2ESlow, Op2FSlow, Op30Slow, Op31Slow,
            Op32Slow, Op33Slow, Op34Slow, Op35Slow, Op36Slow,
            Op37Slow, Op38, Op39Slow, Op3ASlow, Op3B,
            Op3CSlow, Op3DSlow, Op3ESlow, Op3FSlow, Op40Slow,
            Op41Slow, Op42, Op43Slow, Op44Slow, Op45Slow,
            Op46Slow, Op47Slow, Op48Slow, Op49Slow, Op4ASlow,
            Op4BSlow, Op4CSlow, Op4DSlow, Op4ESlow, Op4FSlow,
            Op50Slow, Op51Slow, Op52Slow, Op53Slow, Op54Slow,
            Op55Slow, Op56Slow, Op57Slow, Op58, Op59Slow,
            Op5ASlow, Op5B, Op5CSlow, Op5DSlow, Op5ESlow,
            Op5FSlow, Op60Slow, Op61Slow, Op62Slow, Op63Slow,
            Op64Slow, Op65Slow, Op66Slow, Op67Slow, Op68Slow,
            Op69Slow, Op6ASlow, Op6BSlow, Op6CSlow, Op6DSlow,
            Op6ESlow, Op6FSlow, Op70Slow, Op71Slow, Op72Slow,
            Op73Slow, Op74Slow, Op75Slow, Op76Slow, Op77Slow,
            Op78, Op79Slow, Op7ASlow, Op7B, Op7CSlow,
            Op7DSlow, Op7ESlow, Op7FSlow, Op80Slow, Op81Slow,
            Op82Slow, Op83Slow, Op84Slow, Op85Slow, Op86Slow,
            Op87Slow, Op88Slow, Op89Slow, Op8ASlow, Op8BSlow,
            Op8CSlow, Op8DSlow, Op8ESlow, Op8FSlow, Op90Slow,
            Op91Slow, Op92Slow, Op93Slow, Op94Slow, Op95Slow,
            Op96Slow, Op97Slow, Op98Slow, Op99Slow, Op9A,
            Op9BSlow, Op9CSlow, Op9DSlow, Op9ESlow, Op9FSlow,
            OpA0Slow, OpA1Slow, OpA2Slow, OpA3Slow, OpA4Slow,
            OpA5Slow, OpA6Slow, OpA7Slow, OpA8Slow, OpA9Slow,
            OpAASlow, OpABSlow, OpACSlow, OpADSlow, OpAESlow,
            OpAFSlow, OpB0Slow, OpB1Slow, OpB2Slow, OpB3Slow,
            OpB4Slow, OpB5Slow, OpB6Slow, OpB7Slow, OpB8,
            OpB9Slow, OpBASlow, OpBBSlow, OpBCSlow, OpBDSlow,
            OpBESlow, OpBFSlow, OpC0Slow, OpC1Slow, OpC2Slow,
            OpC3Slow, OpC4Slow, OpC5Slow, OpC6Slow, OpC7Slow,
            OpC8Slow, OpC9Slow, OpCASlow, OpCB, OpCCSlow,
            OpCDSlow, OpCESlow, OpCFSlow, OpD0Slow, OpD1Slow,
            OpD2Slow, OpD3Slow, OpD4Slow, OpD5Slow, OpD6Slow,
            OpD7Slow, OpD8, OpD9Slow, OpDASlow, OpDB,
            OpDCSlow, OpDDSlow, OpDESlow, OpDFSlow, OpE0Slow,
            OpE1Slow, OpE2Slow, OpE3Slow, OpE4Slow, OpE5Slow,
            OpE6Slow, OpE7Slow, OpE8Slow, OpE9Slow, OpEA,
            OpEB, OpECSlow, OpEDSlow, OpEESlow, OpEFSlow,
            OpF0Slow, OpF1Slow, OpF2Slow, OpF3Slow, OpF4Slow,
            OpF5Slow, OpF6Slow, OpF7Slow, OpF8, OpF9Slow,
            OpFASlow, OpFB, OpFCSlow, OpFDSlow, OpFESlow,
            OpFFSlow
        };

        public static byte[] OpLengthsM1X1 = new byte[256];
        public static byte[] OpLengthsM1X0 = new byte[256];
        public static byte[] OpLengthsM0X1 = new byte[256];
        public static byte[] OpLengthsM0X0 = new byte[256];

        public static void Reset()
        {
            for (var i = 0; i < 0x20000; i++)
                Memory.Memory.RAM[i] = 0x55;

            for (var i = 0; i < 0x10000; i++)
                Memory.Memory.VRAM[i] = 0x00;

            for (var i = 0; i < 0x8000; i++)
                Memory.Memory.FillRAM[i] = 0x00;

            //IGNORE BSX
            ResetCPU();
            PPU.SPPU.ResetPPU();
            DMA.DMA.ResetDMA();
            //APU.APU.ResetAPU();
            //MSU.MSU.ResetMSU();
        }

        public static void ResetCPU()
        {
            SoftResetCPU();
            Registers.SL = 0xff;
            Registers.P.W = 0;
            Registers.A.W = 0;
            Registers.X.W = 0;
            Registers.Y.W = 0;
            SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag | Constants.IRQ | Constants.Emulation));
            ClearFlags(Constants.Decimal);
        }

        public static void SoftResetCPU()
        {
            CPUState.Cycles = 182; // Or 188. This is the cycle count just after the jump to the Reset Vector.
            CPUState.PrevCycles = CPUState.Cycles;
            CPUState.V_Counter = 0;
            CPUState.Flags = CPUState.Flags & (Constants.DEBUG_MODE_FLAG | Constants.TRACE_FLAG);
            CPUState.PCBase = null;
            CPUState.NMIPending = false;
            CPUState.IRQLine = false;
            CPUState.IRQTransition = false;
            CPUState.IRQExternal = false;
            CPUState.MemSpeed = Constants.SLOW_ONE_CYCLE;
            CPUState.MemSpeedx2 = Constants.SLOW_ONE_CYCLE * 2;
            CPUState.FastROMSpeed = Constants.SLOW_ONE_CYCLE;
            CPUState.InDMA = false;
            CPUState.InHDMA = false;
            CPUState.InDMAorHDMA = false;
            CPUState.InWRAMDMAorHDMA = false;
            CPUState.HDMARanInDMA = 0;
            CPUState.CurrentDMAorHDMAChannel = -1;
            CPUState.WhichEvent = HCEvents.HC_RENDER_EVENT;
            CPUState.NextEvent = Timings.Timings.RenderPos;
            CPUState.WaitingForInterrupt = false;
            CPUState.AutoSaveTimer = 0;
            CPUState.SRAMModified = false;

            Registers.PBPC = 0;
            Registers.PB = 0;
            Registers.PCw = Memory.Memory.GetWord(0xfffc);
            Memory.Memory.OpenBus = Registers.PCh;
            Registers.D.W = 0;
            Registers.DB = 0;
            Registers.SH = 1;
            Registers.SL -= 3;
            Registers.XH = 0;
            Registers.YH = 0;

            ICPU.ShiftedPB = 0;
            ICPU.ShiftedDB = 0;
            SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag | Constants.IRQ | Constants.Emulation));
            ClearFlags(Constants.Decimal);

            Timings.Timings.InterlaceField = false;
            Timings.Timings.H_Max = Timings.Timings.H_Max_Master;
            Timings.Timings.V_Max = Timings.Timings.V_Max_Master;
            Timings.Timings.NMITriggerPos = 0xffff;
            Timings.Timings.NextIRQTimer = 0x0fffffff;
            Timings.Timings.IRQFlagChanging = IRQ.IRQ_NONE;

            if (PPU.SnesModel._5A22 == 2)
                Timings.Timings.WRAMRefreshPos = Constants.SNES_WRAM_REFRESH_HC_v2;
            else
                Timings.Timings.WRAMRefreshPos = Constants.SNES_WRAM_REFRESH_HC_v1;

            Memory.Memory.SetPCBase(Registers.PBPC);

            ICPU.Opcodes = OpcodesE1;
            ICPU.OpLengths = OpLengthsM1X1;

            UnpackStatus();
        }

        private static void SetFlags(ushort f)
        {
            Registers.P.W |= f;
        }

        private static void ClearFlags(ushort f)
        {
            Registers.P.W &= (ushort)~f;
        }

        public static bool CheckFlag(ushort f) {
            return (Registers.PL & f) > 0;
        }

        private static void SetCarry()
        {
            ICPU._Carry = 1;
        }

        private static void ClearCarry()
        {
            ICPU._Carry = 0;
        }

        private static void SetZero()
        {
            ICPU._Zero = 0;
        }

        private static void ClearZero()
        {
            ICPU._Zero = 1;
        }

        public static void SetIRQ()
        {
            Registers.PL |= Constants.IRQ;
        }

        public static void ClearIRQ()
        {
            Registers.PL &= (byte)~Constants.IRQ;
        }

        private static void SetDecimal()
        {
            Registers.PL |= Constants.Decimal;
        }

        private static void ClearDecimal()
        {
            Registers.PL &= (byte)~Constants.Decimal;
        }

        private static void SetIndex()
        {
            Registers.PL |= Constants.IndexFlag;
        }

        private static void ClearIndex()
        {
            Registers.PL &= (byte)~Constants.IndexFlag;
        }

        private static void SetMemory()
        {
            Registers.PL |= Constants.MemoryFlag;
        }

        private static void ClearMemory()
        {
            Registers.PL &= (byte)~Constants.MemoryFlag;
        }

        private static void SetOverflow()
        {
            ICPU._Overflow = 1;
        }

        private static void ClearOverflow()
        {
            ICPU._Overflow = 0;
        }

        private static void SetNegative()
        {
            ICPU._Negative = 0x80;
        }

        private static void ClearNegative()
        {
            ICPU._Negative = 0;
        }

        private static byte CheckCarry()
        {
            return ICPU._Carry;
        }

        private static byte CheckZero()
        {
            return (byte)ICPU._Zero;
        }

        private static byte CheckIRQ()
        {
            return (byte)(Registers.PL & Constants.IRQ);
        }

        private static byte CheckDecimal()
        {
            return (byte)(Registers.PL & Constants.Decimal);
        }

        private static byte CheckIndex()
        {
            return (byte)(Registers.PL & Constants.IndexFlag);
        }

        private static byte CheckMemory()
        {
            return (byte)(Registers.PL & Constants.MemoryFlag);
        }

        private static byte CheckOverflow()
        {
            return (byte)ICPU._Overflow;
        }

        private static byte CheckNegative()
        {
            return (byte)(ICPU._Negative & 0x80);
        }

        private static byte CheckEmulation()
        {
            return (byte)(Registers.P.W & Constants.Emulation);
        }

        private static void UnpackStatus()
        {
            ICPU._Zero = (byte)((Registers.PL & Constants.Zero) == 0 ? 1 : 0);
            ICPU._Negative = (byte)(Registers.PL & Constants.Negative);
            ICPU._Carry = (byte)(Registers.PL & Constants.Carry);
            ICPU._Overflow = (byte)((Registers.PL & Constants.Overflow) >> 6);
        }

        private static void PackStatus()
        {
            Registers.PL &= (byte)~(Constants.Zero | Constants.Negative | Constants.Carry | Constants.Overflow);
            Registers.PL |= (byte)(ICPU._Carry | (ICPU._Zero == 0 ? 2 : 0) | (ICPU._Negative & 0x80) | (ICPU._Overflow << 6));
        }

        private static void FixCycles()
        {
            if (CheckEmulation() > 0)
            {
                ICPU.Opcodes = OpcodesE1;
                ICPU.OpLengths = OpLengthsM1X1;
            }
            else if (CheckMemory() > 0)
            {
                if (CheckIndex() > 0)
                {
                    ICPU.Opcodes = OpcodesM1X1;
                    ICPU.OpLengths = OpLengthsM1X1;
                }
                else
                {
                    ICPU.Opcodes = OpcodesM1X0;
                    ICPU.OpLengths = OpLengthsM1X0;
                }
            }
            else
            {
                if (CheckIndex() > 0)
                {
                    ICPU.Opcodes = OpcodesM0X1;
                    ICPU.OpLengths = OpLengthsM0X1;
                }
                else
                {
                    ICPU.Opcodes = OpcodesM0X0;
                    ICPU.OpLengths = OpLengthsM0X0;
                }
            }
        }

        private static byte Immediate8Slow(AccessMode a)
        {
            var val = Memory.Memory.GetByte(Registers.PBPC);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = val;

            Registers.PCw++;

            return val;
        }

        private static byte Immediate8(AccessMode a)
        {
            var val = CPUState.PCBase[Registers.PCw];
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = val;

            AddCycles(CPUState.MemSpeed);
            Registers.PCw++;

            return val;
        }

        private static ushort Immediate16Slow(AccessMode a)
        {
            var val = Memory.Memory.GetWord(Registers.PBPC, WrapType.WRAP_BANK);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(val >> 8);

            Registers.PCw += 2;
            return val;
        }

        private static ushort Immediate16(AccessMode a)
        {
            var val = (ushort)((CPUState.PCBase[Registers.PCw] << 8) | CPUState.PCBase[Registers.PCw + 1]);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(val >> 8);

            AddCycles(CPUState.MemSpeedx2);
            Registers.PCw += 2;
            return val;
        }

        private static uint RelativeSlow(AccessMode a)                     // branch $xx
        {
            var offset = Immediate8Slow(a);
            return (uint)(((short)Registers.PCw + offset) & 0xffff);
        }

        private static uint Relative(AccessMode a)                         // branch $xx
        {
            var offset = Immediate8(a);
            return (uint)(((short)Registers.PCw + offset) & 0xffff);
        }

        private static uint RelativeLongSlow(AccessMode a)                 // BRL $xxxx
        {
            var offset = Immediate16Slow(a);
            return (uint)((Registers.PCw + offset) & 0xffff);
        }

        private static uint RelativeLong(AccessMode a)                     // BRL $xxxx
        {
            var offset = Immediate16(a);
            return (uint)((Registers.PCw + offset) & 0xffff);
        }

        private static uint AbsoluteIndexedIndirectSlow(AccessMode a)          // (a,X)
        {
            ushort addr;
            if ((a & AccessMode.JSR) > 0)
            {
                // JSR (a,X) pushes the old address in the middle of loading the new.
                // OpenBus needs to be set to account for this.
                addr = Immediate8Slow(AccessMode.READ);
                if (a == AccessMode.JSR)
                    Memory.Memory.OpenBus = Registers.PCl;

                addr |= (ushort)(Immediate8Slow(AccessMode.READ) << 8);
            }
            else
                addr = Immediate16Slow(AccessMode.READ);

            AddCycles(Constants.ONE_CYCLE);
            addr += Registers.X.W;

            // Address load wraps within the bank
            var addr2 = Memory.Memory.GetWord(ICPU.ShiftedPB | addr, WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            return addr2;
        }

        private static uint AbsoluteIndexedIndirect(AccessMode a)              // (a,X)
        {
            var addr = Immediate16Slow(AccessMode.READ);
            AddCycles(Constants.ONE_CYCLE);
            addr += Registers.X.W;
            // Address load wraps within the bank
            var addr2 = Memory.Memory.GetWord(ICPU.ShiftedPB | addr, WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            return addr2;
        }

        private static uint AbsoluteIndirectLongSlow(AccessMode a)         // [a]
        {
            var addr = Immediate16Slow(AccessMode.READ);
            // No info on wrapping, but it doesn't matter anyway due to mirroring
            uint addr2 = Memory.Memory.GetWord(addr);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            addr2 |= (uint)(Memory.Memory.OpenBus = Memory.Memory.GetByte((uint)(addr + 2))) << 16;
            return addr2;
        }

        private static uint AbsoluteIndirectLong(AccessMode a)             // [a]
        {
            var addr = Immediate16(AccessMode.READ);
            // No info on wrapping, but it doesn't matter anyway due to mirroring
            uint addr2 = Memory.Memory.GetWord(addr);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            addr2 |= (uint)(Memory.Memory.OpenBus = Memory.Memory.GetByte((uint)(addr + 2))) << 16;
            return addr2;
        }

        private static uint AbsoluteIndirectSlow(AccessMode a)             // (a)
        {
            // No info on wrapping, but it doesn't matter anyway due to mirroring
            var addr2 = Memory.Memory.GetWord(Immediate16Slow(AccessMode.READ));
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            return addr2;
        }

        private static uint AbsoluteIndirect(AccessMode a)                 // (a)
        {
            // No info on wrapping, but it doesn't matter anyway due to mirroring
            var addr2 = Memory.Memory.GetWord(Immediate16(AccessMode.READ));
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            return addr2;
        }

        private static uint AbsoluteSlow(AccessMode a)                     // a
        {
            return ICPU.ShiftedDB | Immediate16Slow(a);
        }

        private static uint Absolute(AccessMode a)                         // a
        {
            return ICPU.ShiftedDB | Immediate16(a);
        }

        private static uint AbsoluteLongSlow(AccessMode a)                 // l
        {
            uint addr = Immediate16Slow(AccessMode.READ);

            // JSR l pushes the old bank in the middle of loading the new.
            // OpenBus needs to be set to account for this.
            if (a == AccessMode.JSR)
                Memory.Memory.OpenBus = Registers.PB;

            addr |= (uint)(Immediate8Slow(a) << 16);
            return addr;
        }

        private static uint AbsoluteLong(AccessMode a)                     // l
        {
            uint addr = (uint)(CPUState.PCBase[Registers.PCw] << 16 | CPUState.PCBase[Registers.PCw + 1] << 8 | CPUState.PCBase[Registers.PCw + 2]);
            AddCycles(CPUState.MemSpeedx2 + CPUState.MemSpeed);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 16);

            Registers.PCw += 3;
            return addr;
        }

        private static uint DirectSlow(AccessMode a)                           // d
        {
            ushort addr = (ushort)(Immediate8Slow(a) + Registers.D.W);
            if (Registers.DL != 0)
                AddCycles(Constants.ONE_CYCLE);

            return addr;
        }

        private static uint Direct(AccessMode a)                               // d
        {
            ushort addr = (ushort)(Immediate8(a) + Registers.D.W);
            if (Registers.DL != 0)
                AddCycles(Constants.ONE_CYCLE);

            return addr;
        }

        private static uint DirectIndirectSlow(AccessMode a)                   // (d)
        {
            uint addr = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), (CheckEmulation() == 0 || Registers.DL > 0) ? WrapType.WRAP_BANK : WrapType.WRAP_PAGE);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            addr |= ICPU.ShiftedDB;
            return addr;
        }

        private static uint DirectIndirectE0(AccessMode a)                 // (d)
        {
            uint addr = Memory.Memory.GetWord(Direct(AccessMode.READ));
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            addr |= ICPU.ShiftedDB;

            return addr;
        }

        private static uint DirectIndirectE1(AccessMode a)                 // (d)
        {
            uint addr = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), (Registers.DL > 0) ? WrapType.WRAP_BANK : WrapType.WRAP_PAGE);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            addr |= ICPU.ShiftedDB;
            return addr;
        }

        private static uint DirectIndirectIndexedSlow(AccessMode a)            // (d),Y
        {
            uint addr = DirectIndirectSlow(a);
            if ((a & AccessMode.WRITE) > 0 || CheckIndex() == 0 || (addr & 0xff) + Registers.YL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.Y.W;
        }

        private static uint DirectIndirectIndexedE0X0(AccessMode a)            // (d),Y
        {
            var addr = DirectIndirectE0(a);
            AddCycles(Constants.ONE_CYCLE);
            return addr + Registers.Y.W;
        }

        private static uint DirectIndirectIndexedE0X1(AccessMode a)            // (d),Y
        {
            var addr = DirectIndirectE0(a);
            if (((a & AccessMode.WRITE) > 0) || (addr & 0xff) + Registers.YL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.Y.W;
        }

        private static uint DirectIndirectIndexedE1(AccessMode a)              // (d),Y
        {
            uint addr = DirectIndirectE1(a);
            if (((a & AccessMode.WRITE) > 0) || (addr & 0xff) + Registers.YL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.Y.W;
        }

        private static uint DirectIndirectLongSlow(AccessMode a)               // [d]
        {
            ushort addr = (ushort)DirectSlow(AccessMode.READ);
            uint addr2 = Memory.Memory.GetWord(addr);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            addr2 |= (uint)((Memory.Memory.OpenBus = Memory.Memory.GetByte((byte)(addr + 2))) << 16);
            return addr2;
        }

        private static uint DirectIndirectLong(AccessMode a)                   // [d]
        {
            ushort addr = (ushort)Direct(AccessMode.READ);
            uint addr2 = Memory.Memory.GetWord(addr);
            Memory.Memory.OpenBus = (byte)(addr2 >> 8);
            addr2 |= (uint)(Memory.Memory.OpenBus = Memory.Memory.GetByte((byte)(addr + 2))) << 16;
            return addr2;
        }

        private static uint DirectIndirectIndexedLongSlow(AccessMode a)        // [d],Y
        {
            return DirectIndirectLongSlow(a) + Registers.Y.W;
        }

        private static uint DirectIndirectIndexedLong(AccessMode a)            // [d],Y
        {
            return DirectIndirectLong(a) + Registers.Y.W;
        }

        private static uint DirectIndexedXSlow(AccessMode a)                   // d,X
        {
            var addr = new Pair
            {
                W = (ushort)DirectSlow(a)
            };
            if (CheckEmulation() == 0 || Registers.DL > 0)
                addr.W += Registers.X.W;
            else
                addr.Low += Registers.XL;

            AddCycles(Constants.ONE_CYCLE);
            return addr.W;
        }

        private static uint DirectIndexedXE0(AccessMode a)                 // d,X
        {
            ushort addr = (ushort)(Direct(a) + Registers.X.W);
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static uint DirectIndexedXE1(AccessMode a)                 // d,X
        {
            if (Registers.DL > 0)
                return DirectIndexedXE0(a);
            else
            {
                Pair addr = new Pair
                {
                    W = (ushort)Direct(a)
                };
                addr.Low += Registers.XL;
                AddCycles(Constants.ONE_CYCLE);

                return addr.W;
            }
        }

        private static uint DirectIndexedYSlow(AccessMode a)                   // d,Y
        {
            var addr = new Pair
            {
                W = (ushort)DirectSlow(a)
            };
            if (CheckEmulation() == 0 || Registers.DL > 0)
                addr.W += Registers.Y.W;
            else
                addr.Low += Registers.YL;

            AddCycles(Constants.ONE_CYCLE);

            return addr.W;
        }

        private static uint DirectIndexedYE0(AccessMode a)                 // d,Y
        {
            var addr = Direct(a) + Registers.Y.W;
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static uint DirectIndexedYE1(AccessMode a)                 // d,Y
        {
            if (Registers.DL > 0)
                return DirectIndexedYE0(a);
            else
            {
                var addr = new Pair
                {
                    W = (ushort)Direct(a)
                };
                addr.Low += Registers.YL;
                AddCycles(Constants.ONE_CYCLE);
                return addr.W;
            }
        }

        private static uint DirectIndexedIndirectSlow(AccessMode a)            // (d,X)
        {
            uint addr = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), (CheckEmulation() == 0 || Registers.DL > 0) ? WrapType.WRAP_BANK : WrapType.WRAP_PAGE);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            return ICPU.ShiftedDB | addr;
        }

        private static uint DirectIndexedIndirectE0(AccessMode a)              // (d,X)
        {
            uint addr = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ));
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            return ICPU.ShiftedDB | addr;
        }

        private static uint DirectIndexedIndirectE1(AccessMode a)              // (d,X)
        {
            uint addr = Memory.Memory.GetWord(DirectIndexedXE1(AccessMode.READ), Registers.DL > 0 ? WrapType.WRAP_BANK : WrapType.WRAP_PAGE);
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            return ICPU.ShiftedDB | addr;
        }

        private static uint AbsoluteIndexedXSlow(AccessMode a)             // a,X
        {
            var addr = AbsoluteSlow(a);
            if ((a & AccessMode.WRITE) > 0 || CheckIndex() == 0 || (addr & 0xff) + Registers.XL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.X.W;
        }

        private static uint AbsoluteIndexedXX0(AccessMode a)                   // a,X
        {
            var addr = Absolute(a);
            AddCycles(Constants.ONE_CYCLE);
            return addr + Registers.X.W;
        }

        private static uint AbsoluteIndexedXX1(AccessMode a)                   // a,X
        {
            var addr = Absolute(a);
            if ((a & AccessMode.WRITE) > 0 || (addr & 0xff) + Registers.XL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.X.W;
        }

        private static uint AbsoluteIndexedYSlow(AccessMode a)             // a,Y
        {
            uint addr = AbsoluteSlow(a);
            if ((a & AccessMode.WRITE) > 0 || CheckIndex() == 0 || (addr & 0xff) + Registers.YL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.Y.W;
        }

        private static uint AbsoluteIndexedYX0(AccessMode a)                   // a,Y
        {
            uint addr = Absolute(a);
            AddCycles(Constants.ONE_CYCLE);
            return addr + Registers.Y.W;
        }

        private static uint AbsoluteIndexedYX1(AccessMode a)                   // a,Y
        {
            uint addr = Absolute(a);
            if ((a & AccessMode.WRITE) > 0 || (addr & 0xff) + Registers.YL >= 0x100)
                AddCycles(Constants.ONE_CYCLE);

            return addr + Registers.Y.W;
        }

        private static uint AbsoluteLongIndexedXSlow(AccessMode a)         // l,X
        {
            return AbsoluteLongSlow(a) + Registers.X.W;
        }

        private static uint AbsoluteLongIndexedX(AccessMode a)             // l,X
        {
            return AbsoluteLong(a) + Registers.X.W;
        }

        private static uint StackRelativeSlow(AccessMode a)                    // d,S
        {
            ushort addr = (ushort)(Immediate8Slow(a) + Registers.S.W);
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static uint StackRelative(AccessMode a)                        // d,S
        {
            ushort addr = (ushort)(Immediate8(a) + Registers.S.W);
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static uint StackRelativeIndirectIndexedSlow(AccessMode a) // (d,S),Y
        {
            uint addr = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ));
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            addr = (addr + Registers.Y.W + ICPU.ShiftedDB) & 0xffffff;
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static uint StackRelativeIndirectIndexed(AccessMode a)     // (d,S),Y
        {
            uint addr = Memory.Memory.GetWord(StackRelative(AccessMode.READ));
            if ((a & AccessMode.READ) > 0)
                Memory.Memory.OpenBus = (byte)(addr >> 8);

            addr = (addr + Registers.Y.W + ICPU.ShiftedDB) & 0xffffff;
            AddCycles(Constants.ONE_CYCLE);
            return addr;
        }

        private static void AddCycles(int n)
        {
            CPUState.Cycles += n;
            //while (CPUState.Cycles >= CPUState.NextEvent)
            //    DoHEventProcessing();
        }

        #region ADC

        private static void Op65M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            ADC(val);
        }

        private static void Op75E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            ADC(val);
        }

        private static void Op75E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            ADC(val);
        }

        private static void Op72E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            ADC(val);
        }

        private static void Op72E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            ADC(val);
        }

        private static void Op61E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            ADC(val);
        }

        private static void Op61E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            ADC(val);
        }

        private static void Op71E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            ADC(val);
        }

        private static void Op71E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            ADC(val);
        }

        private static void Op71E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            ADC(val);
        }

        private static void Op67M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            ADC(val);
        }

        private static void Op77M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            ADC(val);
        }

        private static void Op6DM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            ADC(val);
        }

        private static void Op7DM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            ADC(val);
        }

        private static void Op7DM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            ADC(val);
        }

        private static void Op79M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            ADC(val);
        }

        private static void Op79M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            ADC(val);
        }

        private static void Op6FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            ADC(val);
        }

        private static void Op7FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            ADC(val);
        }

        private static void Op63M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            ADC(val);
        }

        private static void Op73M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            ADC(val);
        }

        private static void Op69M1()
        {
            ADC(Immediate8(AccessMode.READ));
        }

        private static void Op69M0()
        {
            ADC(Immediate16(AccessMode.READ));
        }

        private static void Op69Slow()
        {
            if (CheckMemory() > 0)
                ADC(Immediate8Slow(AccessMode.READ));
            else
                ADC(Immediate16Slow(AccessMode.READ));
        }

        private static void SetZN(byte work8)
        {
            ICPU._Zero = work8;
            ICPU._Negative = work8;
        }

        private static void SetZN(ushort work16)
        {
            ICPU._Zero = (byte)(work16 != 0 ? 1 : 0);
            ICPU._Negative = (byte)(work16 >> 8);
        }

        private static void ADC(ushort work16)
        {
            if (CheckDecimal() > 0)
            {
                uint result;
                uint carry = CheckCarry();

                result = (uint)((Registers.A.W & 0x000F) + (work16 & 0x000F) + carry);
                if (result > 0x0009)
                    result += 0x0006;

                carry = (uint)(result > 0x000F ? 1 : 0);

                result = (uint)((Registers.A.W & 0x00F0) + (work16 & 0x00F0) + (result & 0x000F) + carry * 0x10);
                if (result > 0x009F)
                    result += 0x0060;

                carry = (uint)(result > 0x00FF ? 1 : 0);

                result = (uint)((Registers.A.W & 0x0F00) + (work16 & 0x0F00) + (result & 0x00FF) + carry * 0x100);
                if (result > 0x09FF)
                    result += 0x0600;

                carry = (uint)(result > 0x00FF ? 1 : 0);

                result = (uint)((Registers.A.W & 0xF000) + (work16 & 0xF000) + (result & 0x0FFF) + carry * 0x1000);

                if ((Registers.A.W & 0x8000) == (work16 & 0x8000) && (Registers.A.W & 0x8000) != (result & 0x8000))
                    SetOverflow();
                else
                    ClearOverflow();

                if (result > 0x9FFF)
                    result += 0x6000;

                if (result > 0xFFFF)
                    SetCarry();
                else
                    ClearCarry();

                Registers.A.W = (ushort)(result & 0xFFFF);
                SetZN(Registers.A.W);
            }
            else
            {
                uint ans32 = (uint)(Registers.A.W + work16 + CheckCarry());

                ICPU._Carry = (byte)(ans32 >= 0x10000 ? 1 : 0);

                if ((~(Registers.A.W ^ work16) & (work16 ^ (ushort)ans32) & 0x8000) > 0)
                    SetOverflow();
                else
                    ClearOverflow();

                Registers.A.W = (ushort)ans32;
                SetZN(Registers.A.W);
            }
        }

        private static void ADC(byte work8)
        {
            if (CheckDecimal() > 0)
            {
                uint result;
                uint carry = CheckCarry();

                result = (uint)((Registers.AL & 0x0F) + (work8 & 0x0F) + carry);
                if (result > 0x09)
                    result += 0x06;

                carry = (uint)(result > 0x0F ? 1 : 0);
                result = (uint)((Registers.AL & 0xF0) + (work8 & 0xF0) + (result & 0x0F) + (carry * 0x10));

                if ((Registers.AL & 0x80) == (work8 & 0x80) && (Registers.AL & 0x80) != (result & 0x80))
                    SetOverflow();
                else
                    ClearOverflow();

                if (result > 0x9F)
                    result += 0x60;

                if (result > 0xFF)
                    SetCarry();
                else
                    ClearCarry();

                Registers.AL = (byte)(result & 0xFF);
                SetZN(Registers.AL);
            }
            else
            {
                ushort ans16 = (ushort)(Registers.AL + work8 + CheckCarry());

                ICPU._Carry = (byte)(ans16 >= 0x100 ? 1 : 0);

                if ((~(Registers.AL ^ work8) & (work8 ^ (byte)ans16) & 0x80) > 0)
                    SetOverflow();
                else
                    ClearOverflow();

                Registers.AL = (byte)ans16;
                SetZN(Registers.AL);
            }
        }

        private static void Op65M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op75E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op72E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op61E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op71E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op71E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op67M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op77M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op6DM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op7DM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op7DM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op79M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op79M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op6FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op7FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op63M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op73M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ADC(val);
        }

        private static void Op65Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op75Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op72Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op61Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op71Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op67Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op77Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op6DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op7DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op79Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op6FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op7FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op63Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        private static void Op73Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                ADC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ADC(val);
            }
        }

        #endregion

        #region AND

        private static void Op29M1()
        {
            Registers.AL &= Immediate8(AccessMode.READ);
            SetZN(Registers.AL);
        }

        private static void Op29M0()
        {
            Registers.A.W &= Immediate16(AccessMode.READ);
            SetZN(Registers.A.W);
        }

        private static void Op29Slow()
        {
            if (CheckMemory() > 0)
            {
                Registers.AL &= Immediate8Slow(AccessMode.READ);
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W &= Immediate16Slow(AccessMode.READ);
                SetZN(Registers.A.W);
            }
        }

        private static void AND(ushort work16)
        {
            Registers.A.W &= work16;
            SetZN(Registers.A.W);
        }

        private static void AND(byte work8)
        {
            Registers.AL &= work8;
            SetZN(Registers.AL);
        }

        private static void Op25M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            AND(val);
        }

        private static void Op35E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            AND(val);
        }

        private static void Op35E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            AND(val);
        }

        private static void Op32E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            AND(val);
        }

        private static void Op32E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            AND(val);
        }

        private static void Op21E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            AND(val);
        }

        private static void Op21E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            AND(val);
        }

        private static void Op31E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            AND(val);
        }

        private static void Op31E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            AND(val);
        }

        private static void Op31E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            AND(val);
        }

        private static void Op27M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            AND(val);
        }

        private static void Op37M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            AND(val);
        }

        private static void Op2DM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            AND(val);
        }

        private static void Op3DM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            AND(val);
        }

        private static void Op3DM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            AND(val);
        }

        private static void Op39M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            AND(val);
        }

        private static void Op39M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            AND(val);
        }

        private static void Op2FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            AND(val);
        }

        private static void Op3FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            AND(val);
        }

        private static void Op23M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            AND(val);
        }

        private static void Op33M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            AND(val);
        }

        private static void Op25M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op35E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op32E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op21E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op31E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op31E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op27M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op37M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op2DM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op3DM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op3DM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op39M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op39M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op2FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op3FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op23M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op33M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            AND(val);
        }

        private static void Op25Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op35Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op32Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op21Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op31Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op27Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op37Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op2DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op3DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op39Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op2FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op3FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op23Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        private static void Op33Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                AND(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                AND(val);
            }
        }

        #endregion

        #region ASL

        private static void Op0AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            ICPU._Carry = (byte)((Registers.AL & 0x80) != 0 ? 1 : 0);
            Registers.AL <<= 1;
            SetZN(Registers.AL);
        }

        private static void Op0AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            ICPU._Carry = (byte)((Registers.AH & 0x80) != 0 ? 1 : 0);
            Registers.A.W <<= 1;
            SetZN(Registers.A.W);
        }

        private static void Op0ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                ICPU._Carry = (byte)((Registers.AL & 0x80) != 0 ? 1 : 0);
                Registers.AL <<= 1;
                SetZN(Registers.AL);
            }
            else
            {
                ICPU._Carry = (byte)((Registers.AH & 0x80) != 0 ? 1 : 0);
                Registers.A.W <<= 1;
                SetZN(Registers.A.W);
            }
        }

        private static void ASL16(uint opAddress, WrapType w)
        {
            var work16 = Memory.Memory.GetWord(opAddress, w);
            ICPU._Carry = (byte)((work16 & 0x8000) != 0 ? 1 : 0);
            work16 <<= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN(work16);
        }

        private static void ASL8(uint opAddress)
        {
            var work8 = Memory.Memory.GetByte(opAddress);
            ICPU._Carry = (byte)((work8 & 0x80) != 0 ? 1 : 0);
            work8 <<= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, opAddress);
            Memory.Memory.OpenBus = work8;
            SetZN(work8);
        }

        private static void Op06M1()
        {
            ASL8(Direct(AccessMode.MODIFY));
        }

        private static void Op16E1()
        {
            ASL8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void Op16E0M1()
        {
            ASL8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void Op0EM1()
        {
            ASL8(Absolute(AccessMode.MODIFY));
        }

        private static void Op1EM1X1()
        {
            ASL8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void Op1EM1X0()
        {
            ASL8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void Op06M0()
        {
            ASL16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op16E0M0()
        {
            ASL16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op0EM0()
        {
            ASL16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op1EM0X1()
        {
            ASL16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op1EM0X0()
        {
            ASL16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op06Slow()
        {
            if (CheckMemory() > 0)
                ASL8(DirectSlow(AccessMode.MODIFY));
            else
                ASL16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op16Slow()
        {
            if (CheckMemory() > 0)
                ASL8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                ASL16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op0ESlow()
        {
            if (CheckMemory() > 0)
                ASL8(AbsoluteSlow(AccessMode.MODIFY));
            else
                ASL16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op1ESlow()
        {
            if (CheckMemory() > 0)
                ASL8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                ASL16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region BIT

        private static void Op89M1()
        {
            ICPU._Zero = (byte)((Registers.AL & Immediate8(AccessMode.READ)) > 0 ? 1 : 0);
        }

        private static void Op89M0()
        {
            ICPU._Zero = (byte)((Registers.A.W & Immediate16(AccessMode.READ)) != 0 ? 1 : 0);
        }

        private static void Op89Slow()
        {
            if (CheckMemory() > 0)
                ICPU._Zero = (byte)(Registers.AL & Immediate8Slow(AccessMode.READ));
            else
                ICPU._Zero = (byte)((Registers.A.W & Immediate16Slow(AccessMode.READ)) != 0 ? 1 : 0);
        }

        private static void BIT(ushort work16)
        {
            ICPU._Overflow = (byte)((work16 & 0x4000) != 0 ? 1 : 0);
            ICPU._Negative = (byte)(work16 >> 8);
            ICPU._Zero = (byte)((work16 & Registers.A.W) != 0 ? 1 : 0);
        }

        private static void BIT(byte work8)
        {
            ICPU._Overflow = (byte)((work8 & 0x40) != 0 ? 1 : 0);
            ICPU._Negative = work8;
            ICPU._Zero = (byte)(work8 & Registers.AL);
        }

        private static void Op24M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            BIT(val);
        }

        private static void Op34E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            BIT(val);
        }

        private static void Op34E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            BIT(val);
        }

        private static void Op2CM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            BIT(val);
        }

        private static void Op3CM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            BIT(val);
        }

        private static void Op3CM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            BIT(val);
        }

        private static void Op24M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            BIT(val);
        }

        private static void Op34E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            BIT(val);
        }

        private static void Op2CM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            BIT(val);
        }

        private static void Op3CM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            BIT(val);
        }

        private static void Op3CM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            BIT(val);
        }

        private static void Op24Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                BIT(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                BIT(val);
            }
        }

        private static void Op34Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                BIT(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                BIT(val);
            }
        }

        private static void Op2CSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                BIT(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                BIT(val);
            }
        }

        private static void Op3CSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                BIT(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                BIT(val);
            }
        }

        #endregion

        #region CMP

        private static void OpC9M1()
        {
            var val = (short)(Registers.AL - Immediate8(AccessMode.READ));
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((byte)val);
        }

        private static void OpC9M0()
        {
            var val = (int)(Registers.A.W - Immediate16(AccessMode.READ));
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)val);
        }

        private static void OpC9Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = (short)(Registers.AL - Immediate8Slow(AccessMode.READ));
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((byte)val);
            }
            else
            {
                var val = (Registers.A.W - Immediate16Slow(AccessMode.READ));
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((byte)val);
            }
        }

        private static void CMP(ushort val)
        {
            var value = (Registers.A.W - val);
            ICPU._Carry = (byte)(value >= 0 ? 1 : 0);
            SetZN((ushort)value);
        }

        private static void CMP(byte val)
        {
            var value = (short)(Registers.AL - val);
            ICPU._Carry = (byte)(value >= 0 ? 1 : 0);
            SetZN((byte)value);
        }

        private static void OpC5M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            CMP(val);
        }

        private static void OpD5E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            CMP(val);
        }

        private static void OpD5E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            CMP(val);
        }

        private static void OpD2E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            CMP(val);
        }

        private static void OpD2E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            CMP(val);
        }

        private static void OpC1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            CMP(val);
        }

        private static void OpC1E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            CMP(val);
        }

        private static void OpD1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            CMP(val);
        }

        private static void OpD1E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            CMP(val);
        }

        private static void OpD1E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            CMP(val);
        }

        private static void OpC7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            CMP(val);
        }

        private static void OpD7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            CMP(val);
        }

        private static void OpCDM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            CMP(val);
        }

        private static void OpDDM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            CMP(val);
        }

        private static void OpDDM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            CMP(val);
        }

        private static void OpD9M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            CMP(val);
        }

        private static void OpD9M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            CMP(val);
        }

        private static void OpCFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            CMP(val);
        }

        private static void OpDFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            CMP(val);
        }

        private static void OpC3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            CMP(val);
        }

        private static void OpD3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            CMP(val);
        }

        private static void OpC5M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD5E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD2E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpC1E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD1E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD1E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpC7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpCDM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpDDM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpDDM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD9M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD9M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpCFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpDFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpC3M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpD3M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CMP(val);
        }

        private static void OpC5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpD5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpD2Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpC1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpD1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpC7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpD7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpCDSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpDDSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpD9Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpCFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpDFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        private static void OpC3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }


        private static void OpD3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                CMP(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CMP(val);
            }
        }

        #endregion

        #region CPX

        private static void OpE0X1()
        {
            var val = (short)(Registers.XL - Immediate8(AccessMode.READ));
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)val);
        }

        private static void OpE0X0()
        {
            var val = Registers.X.W - Immediate16(AccessMode.READ);
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)val);
        }

        private static void OpE0Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = (short)(Registers.XL - Immediate8Slow(AccessMode.READ));
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((ushort)val);
            }
            else
            {
                var val = Registers.X.W - Immediate16Slow(AccessMode.READ);
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((ushort)val);
            }
        }

        private static void CPX(ushort val)
        {
            var value = Registers.X.W - val;
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)value);
        }

        private static void CPX(byte val)
        {
            var value = (short)(Registers.XL - val);
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((byte)value);
        }

        private static void OpE4X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            CPX(val);
        }

        private static void OpECX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            CPX(val);
        }

        private static void OpE4X0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CPX(val);
        }

        private static void OpECX0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CPX(val);
        }

        private static void OpE4Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                CPX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CPX(val);
            }
        }

        private static void OpECSlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                CPX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CPX(val);
            }
        }

        #endregion

        #region CPY

        private static void OpC0X1()
        {
            var val = (short)(Registers.YL - Immediate8(AccessMode.READ));
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((byte)val);
        }

        private static void OpC0X0()
        {
            var val = Registers.Y.W - Immediate16(AccessMode.READ);
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)val);
        }

        private static void OpC0Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = (short)(Registers.YL - Immediate8Slow(AccessMode.READ));
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((byte)val);
            }
            else
            {
                var val = Registers.Y.W - Immediate16Slow(AccessMode.READ);
                ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
                SetZN((ushort)val);
            }
        }

        private static void CPY(ushort val)
        {
            var value = Registers.Y.W - val;
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((ushort)value);
        }

        private static void CPY(byte val)
        {
            var value = (short)(Registers.YL - val);
            ICPU._Carry = (byte)(val >= 0 ? 1 : 0);
            SetZN((byte)value);
        }

        private static void OpC4X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            CPY(val);
        }

        private static void OpCCX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            CPY(val);
        }

        private static void OpC4X0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CPY(val);
        }

        private static void OpCCX0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            CPY(val);
        }

        private static void OpC4Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                CPY(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CPY(val);
            }
        }

        private static void OpCCSlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                CPY(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                CPY(val);
            }
        }

        #endregion

        #region DEC

        private static void Op3AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.AL--;
            SetZN(Registers.AL);
        }

        private static void Op3AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W--;
            SetZN(Registers.A.W);
        }

        private static void Op3ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                Registers.AL--;
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W--;
                SetZN(Registers.A.W);
            }
        }

        private static void DEC16(uint opAddress, WrapType w)
        {
            var work16 = (ushort)(Memory.Memory.GetWord(opAddress, w) - 1);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN(work16);
        }

        private static void DEC8(uint opAddress)
        {
            var work8 = (byte)(Memory.Memory.GetByte(opAddress) - 1);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, opAddress);
            Memory.Memory.OpenBus = work8;
            SetZN(work8);
        }

        private static void OpC6M1()
        {
            DEC8(Direct(AccessMode.MODIFY));
        }

        private static void OpD6E1()
        {
            DEC8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void OpD6E0M1()
        {
            DEC8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void OpCEM1()
        {
            DEC8(Absolute(AccessMode.MODIFY));
        }

        private static void OpDEM1X1()
        {
            DEC8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void OpDEM1X0()
        {
            DEC8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void OpC6M0()
        {
            DEC16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpD6E0M0()
        {
            DEC16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpCEM0()
        {
            DEC16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpDEM0X1()
        {
            DEC16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpDEM0X0()
        {
            DEC16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpC6Slow()
        {
            if (CheckMemory() > 0)
                DEC8(DirectSlow(AccessMode.MODIFY));
            else
                DEC16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpD6Slow()
        {
            if (CheckMemory() > 0)
                DEC8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                DEC16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpCESlow()
        {
            if (CheckMemory() > 0)
                DEC8(AbsoluteSlow(AccessMode.MODIFY));
            else
                DEC16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpDESlow()
        {
            if (CheckMemory() > 0)
                DEC8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                DEC16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region EOR

        private static void Op49M1()
        {
            Registers.AL ^= Immediate8(AccessMode.READ);
            SetZN(Registers.AL);
        }

        private static void Op49M0()
        {
            Registers.A.W ^= Immediate16(AccessMode.READ);
            SetZN(Registers.A.W);
        }

        private static void Op49Slow()
        {
            if (CheckMemory() > 0)
            {
                Registers.AL ^= Immediate8Slow(AccessMode.READ);
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W ^= Immediate16Slow(AccessMode.READ);
                SetZN(Registers.A.W);
            }
        }

        private static void EOR(ushort val)
        {
            Registers.A.W ^= val;
            SetZN(Registers.A.W);
        }

        private static void EOR(byte val)
        {
            Registers.AL ^= val;
            SetZN(Registers.AL);
        }

        private static void Op45M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            EOR(val);
        }

        private static void Op55E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            EOR(val);
        }

        private static void Op55E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            EOR(val);
        }

        private static void Op52E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            EOR(val);
        }

        private static void Op52E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            EOR(val);
        }

        private static void Op41E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            EOR(val);
        }

        private static void Op41E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            EOR(val);
        }

        private static void Op51E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            EOR(val);
        }

        private static void Op51E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            EOR(val);
        }

        private static void Op51E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            EOR(val);
        }

        private static void Op47M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            EOR(val);
        }

        private static void Op57M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            EOR(val);
        }

        private static void Op4DM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            EOR(val);
        }

        private static void Op5DM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            EOR(val);
        }

        private static void Op5DM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            EOR(val);
        }

        private static void Op59M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            EOR(val);
        }

        private static void Op59M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            EOR(val);
        }

        private static void Op4FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            EOR(val);
        }

        private static void Op5FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            EOR(val);
        }

        private static void Op43M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            EOR(val);
        }

        private static void Op53M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            EOR(val);
        }

        private static void Op45M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op55E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op52E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op41E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op51E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op51E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op47M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op57M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op4DM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op5DM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op5DM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op59M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op59M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op4FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op5FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op43M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op53M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            EOR(val);
        }

        private static void Op45Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op55Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op52Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op41Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op51Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op47Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op57Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op4DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op5DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op59Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op4FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op5FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op43Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        private static void Op53Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                EOR(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                EOR(val);
            }
        }

        #endregion

        #region INC

        private static void Op1AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.AL++;
            SetZN(Registers.AL);
        }

        private static void Op1AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W++;
            SetZN(Registers.A.W);
        }

        private static void Op1ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                Registers.AL++;
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W++;
                SetZN(Registers.A.W);
            }
        }

        private static void INC16(uint opAddress, WrapType w)
        {
            var work16 = (ushort)(Memory.Memory.GetWord(opAddress, w) + 1);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN(work16);
        }

        private static void INC8(uint opAddress)
        {
            var work8 = (byte)(Memory.Memory.GetByte(opAddress) + 1);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, opAddress);
            Memory.Memory.OpenBus = work8;
            SetZN(work8);
        }

        private static void OpE6M1()
        {
            INC8(Direct(AccessMode.MODIFY));
        }

        private static void OpF6E1()
        {
            INC8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void OpF6E0M1()
        {
            INC8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void OpEEM1()
        {
            INC8(Absolute(AccessMode.MODIFY));
        }

        private static void OpFEM1X1()
        {
            INC8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void OpFEM1X0()
        {
            INC8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void OpE6M0()
        {
            INC16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpF6E0M0()
        {
            INC16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpEEM0()
        {
            INC16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpFEM0X1()
        {
            INC16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpFEM0X0()
        {
            INC16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpE6Slow()
        {
            if (CheckMemory() > 0)
                INC8(DirectSlow(AccessMode.MODIFY));
            else
                INC16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpF6Slow()
        {
            if (CheckMemory() > 0)
                INC8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                INC16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void OpEESlow()
        {
            if (CheckMemory() > 0)
                INC8(AbsoluteSlow(AccessMode.MODIFY));
            else
                INC16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void OpFESlow()
        {
            if (CheckMemory() > 0)
                INC8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                INC16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region LDA

        private static void OpA9M1()
        {
            Registers.AL = Immediate8(AccessMode.READ);
            SetZN(Registers.AL);
        }

        private static void OpA9M0()
        {
            Registers.A.W = Immediate16(AccessMode.READ);
            SetZN(Registers.A.W);
        }

        private static void OpA9Slow()
        {
            if (CheckMemory() > 0)
            {
                Registers.AL = Immediate8Slow(AccessMode.READ);
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W = Immediate16Slow(AccessMode.READ);
                SetZN(Registers.A.W);
            }
        }

        private static void LDA(ushort val)
        {
            Registers.A.W = val;
            SetZN(Registers.A.W);
        }

        private static void LDA(byte val)
        {
            Registers.AL = val;
            SetZN(Registers.AL);
        }

        private static void OpA5M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            LDA(val);
        }

        private static void OpB5E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            LDA(val);
        }

        private static void OpB5E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            LDA(val);
        }

        private static void OpB2E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            LDA(val);
        }

        private static void OpB2E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            LDA(val);
        }

        private static void OpA1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            LDA(val);
        }

        private static void OpA1E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            LDA(val);
        }

        private static void OpB1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            LDA(val);
        }

        private static void OpB1E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            LDA(val);
        }

        private static void OpB1E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            LDA(val);
        }

        private static void OpA7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            LDA(val);
        }

        private static void OpB7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            LDA(val);
        }

        private static void OpADM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            LDA(val);
        }

        private static void OpBDM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            LDA(val);
        }

        private static void OpBDM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            LDA(val);
        }
        private static void OpB9M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            LDA(val);
        }

        private static void OpB9M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            LDA(val);
        }

        private static void OpAFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            LDA(val);
        }

        private static void OpBFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            LDA(val);
        }

        private static void OpA3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            LDA(val);
        }

        private static void OpB3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            LDA(val);
        }

        private static void OpA5M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB5E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB2E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpA1E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB1E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB1E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpA7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpADM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpBDM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpBDM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB9M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB9M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpAFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpBFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpA3M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpB3M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDA(val);
        }

        private static void OpA5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB2Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpA1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpA7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpADSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpBDSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB9Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpAFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpBFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpA3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        private static void OpB3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                LDA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDA(val);
            }
        }

        #endregion

        #region LDX

        private static void OpA2X1()
        {
            Registers.XL = Immediate8(AccessMode.READ);
            SetZN(Registers.XL);
        }

        private static void OpA2X0()
        {
            Registers.X.W = Immediate16(AccessMode.READ);
            SetZN(Registers.X.W);
        }

        private static void OpA2Slow()
        {
            if (CheckIndex() > 0)
            {
                Registers.XL = Immediate8Slow(AccessMode.READ);
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W = Immediate16Slow(AccessMode.READ);
                SetZN(Registers.X.W);
            }
        }

        private static void LDX(ushort val)
        {
            Registers.X.W = val;
            SetZN(Registers.X.W);
        }

        private static void LDX(byte val)
        {
            Registers.XL = val;
            SetZN(Registers.XL);
        }

        private static void OpA6X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            LDX(val);
        }

        private static void OpB6E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedYE1(AccessMode.READ));
            LDX(val);
        }

        private static void OpB6E0X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedYE0(AccessMode.READ));
            LDX(val);
        }

        private static void OpAEX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            LDX(val);
        }

        private static void OpBEX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            LDX(val);
        }

        private static void OpA6X0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDX(val);
        }

        private static void OpB6E0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedYE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDX(val);
        }

        private static void OpAEX0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDX(val);
        }

        private static void OpBEX0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDX(val);
        }

        private static void OpA6Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                LDX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDX(val);
            }
        }

        private static void OpB6Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedYSlow(AccessMode.READ));
                LDX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedYSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDX(val);
            }
        }

        private static void OpAESlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                LDX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDX(val);
            }
        }

        private static void OpBESlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                LDX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDX(val);
            }
        }

        #endregion

        #region LDY

        private static void OpA0X1()
        {
            Registers.YL = Immediate8(AccessMode.READ);
            SetZN(Registers.YL);
        }

        private static void OpA0X0()
        {
            Registers.Y.W = Immediate16(AccessMode.READ);
            SetZN(Registers.Y.W);
        }

        private static void OpA0Slow()
        {
            if (CheckIndex() > 0)
            {
                Registers.YL = Immediate8Slow(AccessMode.READ);
                SetZN(Registers.YL);
            }
            else
            {
                Registers.Y.W = Immediate16Slow(AccessMode.READ);
                SetZN(Registers.Y.W);
            }
        }

        private static void LDY(ushort val)
        {
            Registers.Y.W = val;
            SetZN(Registers.Y.W);
        }

        private static void LDY(byte val)
        {
            Registers.YL = val;
            SetZN(Registers.YL);
        }

        private static void OpA4X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            LDY(val);
        }

        private static void OpB4E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            LDX(val);
        }

        private static void OpB4E0X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            LDY(val);
        }

        private static void OpACX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            LDY(val);
        }

        private static void OpBCX1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            LDY(val);
        }

        private static void OpA4X0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDY(val);
        }

        private static void OpB4E0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDY(val);
        }

        private static void OpACX0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDY(val);
        }

        private static void OpBCX0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            LDY(val);
        }

        private static void OpA4Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                LDY(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDY(val);
            }
        }

        private static void OpB4Slow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                LDY(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedYSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDY(val);
            }
        }

        private static void OpACSlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                LDY(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDY(val);
            }
        }

        private static void OpBCSlow()
        {
            if (CheckIndex() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                LDX(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                LDX(val);
            }
        }

        #endregion

        #region LSR

        private static void Op4AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            ICPU._Carry = (byte)(Registers.AL & 1);
            Registers.AL >>= 1;
            SetZN(Registers.AL);
        }

        private static void Op4AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            ICPU._Carry = (byte)(Registers.A.W & 1);
            Registers.A.W >>= 1;
            SetZN(Registers.A.W);
        }

        private static void Op4ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                ICPU._Carry = (byte)(Registers.AL & 1);
                Registers.AL >>= 1;
                SetZN(Registers.AL);
            }
            else
            {
                ICPU._Carry = (byte)(Registers.A.W & 1);
                Registers.A.W >>= 1;
                SetZN(Registers.A.W);
            }
        }

        private static void LSR16(uint opAddress, WrapType w)
        {
            var work16 = Memory.Memory.GetWord(opAddress, w);
            ICPU._Carry = (byte)(work16 & 1);
            work16 >>= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN(work16);
        }

        private static void LSR8(uint OpAddress)
        {
            var work8 = Memory.Memory.GetByte(OpAddress);
            ICPU._Carry = (byte)(work8 & 1);
            work8 >>= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, OpAddress);
            Memory.Memory.OpenBus = work8;
            SetZN(work8);
        }

        private static void Op46M1()
        {
            LSR8(Direct(AccessMode.MODIFY));
        }

        private static void Op56E1()
        {
            LSR8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void Op56E0M1()
        {
            LSR8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void Op4EM1()
        {
            LSR8(Absolute(AccessMode.MODIFY));
        }

        private static void Op5EM1X0()
        {
            LSR8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void Op5EM1X1()
        {
            LSR8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void Op46M0()
        {
            LSR16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op56E0M0()
        {
            LSR16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op4EM0()
        {
            LSR16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op5EM0X1()
        {
            LSR16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op5EM0X0()
        {
            LSR16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op46Slow()
        {
            if (CheckMemory() > 0)
                LSR8(DirectSlow(AccessMode.MODIFY));
            else
                LSR16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op56Slow()
        {
            if (CheckMemory() > 0)
                LSR8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                LSR16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op4ESlow()
        {
            if (CheckMemory() > 0)
                LSR8(AbsoluteSlow(AccessMode.MODIFY));
            else
                LSR16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op5ESlow()
        {
            if (CheckMemory() > 0)
                LSR8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                LSR16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region ORA

        private static void Op09M1()
        {
            Registers.AL |= Immediate8(AccessMode.READ);
            SetZN(Registers.AL);
        }

        private static void Op09M0()
        {
            Registers.A.W |= Immediate16(AccessMode.READ);
            SetZN(Registers.A.W);
        }

        private static void Op09Slow()
        {
            if (CheckMemory() > 0)
            {
                Registers.AL |= Immediate8Slow(AccessMode.READ);
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W |= Immediate16Slow(AccessMode.READ);
                SetZN(Registers.A.W);
            }
        }

        private static void ORA(ushort val)
        {
            Registers.A.W |= val;
            SetZN(Registers.A.W);
        }

        private static void ORA(byte val)
        {
            Registers.AL |= val;
            SetZN(Registers.AL);
        }

        private static void Op05M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            ORA(val);
        }

        private static void Op15E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            ORA(val);
        }

        private static void Op15E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            ORA(val);
        }

        private static void Op12E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            ORA(val);
        }

        private static void Op12E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            ORA(val);
        }

        private static void Op01E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            ORA(val);
        }

        private static void Op01E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            ORA(val);
        }

        private static void Op11E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            ORA(val);
        }

        private static void Op11E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            ORA(val);
        }

        private static void Op11E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            ORA(val);
        }

        private static void Op07M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            ORA(val);
        }

        private static void Op17M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            ORA(val);
        }

        private static void Op0DM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            ORA(val);
        }

        private static void Op1DM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            ORA(val);
        }

        private static void Op1DM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            ORA(val);
        }

        private static void Op19M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            ORA(val);
        }

        private static void Op19M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            ORA(val);
        }

        private static void Op0FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            ORA(val);
        }

        private static void Op1FM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            ORA(val);
        }

        private static void Op03M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            ORA(val);
        }

        private static void Op13M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            ORA(val);
        }

        private static void Op05M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op15E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op12E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op01E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op11E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op11E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op07M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op17M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op0DM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op1DM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op1DM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op19M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op19M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op0FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op1FM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op03M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op13M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            ORA(val);
        }

        private static void Op05Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op15Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op12Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op01Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op11Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op07Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op17Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op0DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op1DSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op19Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op0FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op1FSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op03Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        private static void Op13Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                ORA(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                ORA(val);
            }
        }

        #endregion

        #region ROL

        private static void Op2AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            var w = (ushort)((Registers.AL) << 1) | CheckCarry();
            ICPU._Carry = (byte)(w >= 0x100 ? 1 : 0);
            Registers.AL = (byte)w;
            SetZN(Registers.AL);
        }

        private static void Op2AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            var w = (uint)((Registers.A.W) << 1) | CheckCarry();
            ICPU._Carry = (byte)(w >= 0x10000 ? 1 : 0);
            Registers.A.W = (ushort)w;
            SetZN(Registers.A.W);
        }

        private static void Op2ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                var w = (ushort)(((Registers.AL) << 1) | CheckCarry());
                ICPU._Carry = (byte)(w >= 0x100 ? 1 : 0);
                Registers.AL = (byte)w;
                SetZN(Registers.AL);
            }
            else
            {
                var w = (uint)((Registers.A.W) << 1) | CheckCarry();
                ICPU._Carry = (byte)(w >= 0x10000 ? 1 : 0);
                Registers.A.W = (ushort)w;
                SetZN(Registers.A.W);
            }
        }

        private static void ROL16(uint opAddress, WrapType w)
        {
            uint work32 = (uint)(((Memory.Memory.GetWord(opAddress, w)) << 1) | CheckCarry());
            ICPU._Carry = (byte)(work32 >= 0x10000 ? 1 : 0);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord((ushort)work32, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work32 & 0xff);
            SetZN((ushort)work32);
        }

        private static void ROL8(uint opAddress)
        {
            ushort work16 = (ushort)(((Memory.Memory.GetByte(opAddress)) << 1) | CheckCarry());
            ICPU._Carry = (byte)(work16 >= 0x100 ? 1 : 0);
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte((byte)work16, opAddress);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN((byte)work16);
        }

        private static void Op26M1()
        {
            ROL8(Direct(AccessMode.MODIFY));
        }

        private static void Op36E1()
        {
            ROL8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void Op36E0M1()
        {
            ROL8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void Op2EM1()
        {
            ROL8(Absolute(AccessMode.MODIFY));
        }

        private static void Op3EM1X1()
        {
            ROL8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void Op3EM1X0()
        {
            ROL8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void Op26M0()
        {
            ROL16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op36E0M0()
        {
            ROL16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op2EM0()
        {
            ROL16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op3EM0X1()
        {
            ROL16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op3EM0X0()
        {
            ROL16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op26Slow()
        {
            if (CheckMemory() > 0)
                ROL8(DirectSlow(AccessMode.MODIFY));
            else
                ROL16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op36Slow()
        {
            if (CheckMemory() > 0)
                ROL8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                ROL16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op2ESlow()
        {
            if (CheckMemory() > 0)
                ROL8(AbsoluteSlow(AccessMode.MODIFY));
            else
                ROL16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op3ESlow()
        {
            if (CheckMemory() > 0)
                ROL8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                ROL16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region ROR

        private static void Op6AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            var w = (ushort)((Registers.AL) | ((CheckCarry()) << 8));
            ICPU._Carry = (byte)(w & 1);
            w >>= 1;
            Registers.AL = (byte)w;
            SetZN(Registers.AL);
        }

        private static void Op6AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            var w = (uint)((Registers.A.W) | ((CheckCarry()) << 16));
            ICPU._Carry = (byte)(w & 1);
            w >>= 1;
            Registers.A.W = (ushort)w;
            SetZN(Registers.A.W);
        }

        private static void Op6ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                var w = (ushort)((Registers.AL) | ((CheckCarry()) << 8));
                ICPU._Carry = (byte)(w & 1);
                w >>= 1;
                Registers.AL = (byte)w;
                SetZN(Registers.AL);
            }
            else
            {
                var w = (uint)((Registers.A.W) | ((CheckCarry()) << 16));
                ICPU._Carry = (byte)(w & 1);
                w >>= 1;
                Registers.A.W = (ushort)w;
                SetZN(Registers.A.W);
            }
        }

        private static void ROR16(uint opAddress, WrapType w)
        {
            var work32 = (uint)((Memory.Memory.GetWord(opAddress, w)) | ((CheckCarry()) << 16));
            ICPU._Carry = (byte)(work32 & 1);
            work32 >>= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord((ushort)work32, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work32 & 0xff);
            SetZN((ushort)work32);
        }

        private static void ROR8(uint opAddress)
        {
            var work16 = (ushort)((Memory.Memory.GetByte(opAddress)) | ((CheckCarry()) << 8));
            ICPU._Carry = (byte)(work16 & 1);
            work16 >>= 1;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte((byte)work16, opAddress);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
            SetZN((byte)work16);
        }

        private static void Op66M1()
        {
            ROR8(Direct(AccessMode.MODIFY));
        }

        private static void Op76E1()
        {
            ROR8(DirectIndexedXE1(AccessMode.MODIFY));
        }

        private static void Op76E0M1()
        {
            ROR8(DirectIndexedXE0(AccessMode.MODIFY));
        }

        private static void Op6EM1()
        {
            ROR8(Absolute(AccessMode.MODIFY));
        }

        private static void Op7EM1X1()
        {
            ROR8(AbsoluteIndexedXX1(AccessMode.MODIFY));
        }

        private static void Op7EM1X0()
        {
            ROR8(AbsoluteIndexedXX0(AccessMode.MODIFY));
        }

        private static void Op66M0()
        {
            ROR16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op76E0M0()
        {
            ROR16(DirectIndexedXE0(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op6EM0()
        {
            ROR16(Absolute(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op7EM0X1()
        {
            ROR16(AbsoluteIndexedXX1(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op7EM0X0()
        {
            ROR16(AbsoluteIndexedXX0(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op66Slow()
        {
            if (CheckMemory() > 0)
                ROR8(DirectSlow(AccessMode.MODIFY));
            else
                ROR16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op76Slow()
        {
            if (CheckMemory() > 0)
                ROR8(DirectIndexedXSlow(AccessMode.MODIFY));
            else
                ROR16(DirectIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op6ESlow()
        {
            if (CheckMemory() > 0)
                ROR8(AbsoluteSlow(AccessMode.MODIFY));
            else
                ROR16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        private static void Op7ESlow()
        {
            if (CheckMemory() > 0)
                ROR8(AbsoluteIndexedXSlow(AccessMode.MODIFY));
            else
                ROR16(AbsoluteIndexedXSlow(AccessMode.MODIFY), WrapType.WRAP_NONE);
        }

        #endregion

        #region SBC

        private static void SBC(ushort work16)
        {
            if (CheckDecimal() > 0)
            {
                int result;
                int carry = CheckCarry();

                work16 ^= 0xFFFF;

                result = (Registers.A.W & 0x000F) + (work16 & 0x000F) + carry;
                if (result < 0x0010)
                    result -= 0x0006;
                carry = result > 0x000F ? 1 : 0;

                result = (Registers.A.W & 0x00F0) + (work16 & 0x00F0) + (result & 0x000F) + carry * 0x10;
                if (result < 0x0100)
                    result -= 0x0060;
                carry = result > 0x00FF ? 1 : 0;

                result = (Registers.A.W & 0x0F00) + (work16 & 0x0F00) + (result & 0x00FF) + carry * 0x100;
                if (result < 0x1000)
                    result -= 0x0600;
                carry = result > 0x0FFF ? 1 : 0;

                result = (Registers.A.W & 0xF000) + (work16 & 0xF000) + (result & 0x0FFF) + carry * 0x1000;

                if (((Registers.A.W ^ work16) & 0x8000) == 0 && ((Registers.A.W ^ result) & 0x8000) > 0)
                    SetOverflow();
                else
                    ClearOverflow();

                if (result < 0x10000)
                    result -= 0x6000;

                if (result > 0xFFFF)
                    SetCarry();
                else
                    ClearCarry();

                Registers.A.W = (ushort)(result & 0xFFFF);
                SetZN(Registers.A.W);
            }
            else
            {
                var value = (int)(Registers.A.W - work16 + CheckCarry() - 1);

                ICPU._Carry = (byte)(value >= 0 ? 1 : 0);

                if (((Registers.A.W ^ work16) & (Registers.A.W ^ (ushort)value) & 0x8000) > 0)
                    SetOverflow();
                else
                    ClearOverflow();

                Registers.A.W = (ushort)value;
                SetZN(Registers.A.W);
            }
        }

        private static void SBC(byte work8)
        {
            if (CheckDecimal() > 0)
            {
                int result;
                int carry = CheckCarry();

                work8 ^= 0xFF;

                result = (Registers.AL & 0x0F) + (work8 & 0x0F) + carry;
                if (result < 0x10)
                    result -= 0x06;
                carry = (result > 0x0F) ? 1 : 0;

                result = (Registers.AL & 0xF0) + (work8 & 0xF0) + (result & 0x0F) + carry * 0x10;

                if ((Registers.AL & 0x80) == (work8 & 0x80) && (Registers.AL & 0x80) != (result & 0x80))
                    SetOverflow();
                else
                    ClearOverflow();

                if (result < 0x100)
                    result -= 0x60;

                if (result > 0xFF)
                    SetCarry();
                else
                    ClearCarry();

                Registers.AL = (byte)(result & 0xFF);
                SetZN(Registers.AL);
            }
            else
            {
                var value = (short)Registers.AL - (short)work8 + (short)CheckCarry() - 1;

                ICPU._Carry = (byte)(value >= 0 ? 1 : 0);

                if (((Registers.AL ^ work8) & (Registers.AL ^ (byte)value) & 0x80) > 0)
                    SetOverflow();
                else
                    ClearOverflow();

                Registers.AL = (byte)value;
                SetZN(Registers.AL);
            }
        }

        private static void OpE9M1()
        {
            SBC(Immediate8(AccessMode.READ));
        }

        private static void OpE9M0()
        {
            SBC(Immediate16(AccessMode.READ));
        }

        private static void OpE9Slow()
        {
            if (CheckMemory() > 0)
                SBC(Immediate8Slow(AccessMode.READ));
            else
                SBC(Immediate16Slow(AccessMode.READ));
        }

        private static void OpE5M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Direct(AccessMode.READ));
            SBC(val);
        }

        private static void OpF5E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE1(AccessMode.READ));
            SBC(val);
        }

        private static void OpF5E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXE0(AccessMode.READ));
            SBC(val);
        }

        private static void OpF2E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE1(AccessMode.READ));
            SBC(val);
        }

        private static void OpF2E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectE0(AccessMode.READ));
            SBC(val);
        }

        private static void OpE1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE1(AccessMode.READ));
            SBC(val);
        }

        private static void OpE1E0M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectE0(AccessMode.READ));
            SBC(val);
        }

        private static void OpF1E1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE1(AccessMode.READ));
            SBC(val);
        }

        private static void OpF1E0M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X1(AccessMode.READ));
            SBC(val);
        }

        private static void OpF1E0M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedE0X0(AccessMode.READ));
            SBC(val);
        }

        private static void OpE7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLong(AccessMode.READ));
            SBC(val);
        }

        private static void OpF7M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLong(AccessMode.READ));
            SBC(val);
        }

        private static void OpEDM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(Absolute(AccessMode.READ));
            SBC(val);
        }

        private static void OpFDM1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX1(AccessMode.READ));
            SBC(val);
        }

        private static void OpFDM1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXX0(AccessMode.READ));
            SBC(val);
        }

        private static void OpF9M1X1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX1(AccessMode.READ));
            SBC(val);
        }

        private static void OpF9M1X0()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYX0(AccessMode.READ));
            SBC(val);
        }

        private static void OpEFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLong(AccessMode.READ));
            SBC(val);
        }

        private static void OpFFM1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedX(AccessMode.READ));
            SBC(val);
        }

        private static void OpE3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelative(AccessMode.READ));
            SBC(val);
        }

        private static void OpF3M1()
        {
            var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexed(AccessMode.READ));
            SBC(val);
        }

        private static void OpE5M0()
        {
            var val = Memory.Memory.GetWord(Direct(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF5E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedXE0(AccessMode.READ), WrapType.WRAP_BANK);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF2E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpE1E0M0()
        {
            var val = Memory.Memory.GetWord(DirectIndexedIndirectE0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF1E0M0X1()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF1E0M0X0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedE0X0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpE7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF7M0()
        {
            var val = Memory.Memory.GetWord(DirectIndirectIndexedLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpEDM0()
        {
            var val = Memory.Memory.GetWord(Absolute(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpFDM0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpFDM0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedXX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF9M0X1()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX1(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF9M0X0()
        {
            var val = Memory.Memory.GetWord(AbsoluteIndexedYX0(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpEFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLong(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpFFM0()
        {
            var val = Memory.Memory.GetWord(AbsoluteLongIndexedX(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpE3M0()
        {
            var val = Memory.Memory.GetWord(StackRelative(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpF3M0()
        {
            var val = Memory.Memory.GetWord(StackRelativeIndirectIndexed(AccessMode.READ), WrapType.WRAP_NONE);
            Memory.Memory.OpenBus = (byte)(val >> 8);
            SBC(val);
        }

        private static void OpE5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF5Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedXSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedXSlow(AccessMode.READ), WrapType.WRAP_BANK);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF2Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpE1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndexedIndirectSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndexedIndirectSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF1Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpE7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectLongSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF7Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(DirectIndirectIndexedLongSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(DirectIndirectIndexedLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpEDSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpFDSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedXSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF9Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteIndexedYSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteIndexedYSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpEFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpFFSlow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(AbsoluteLongIndexedXSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(AbsoluteLongIndexedXSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpE3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        private static void OpF3Slow()
        {
            if (CheckMemory() > 0)
            {
                var val = Memory.Memory.OpenBus = Memory.Memory.GetByte(StackRelativeIndirectIndexedSlow(AccessMode.READ));
                SBC(val);
            }
            else
            {
                var val = Memory.Memory.GetWord(StackRelativeIndirectIndexedSlow(AccessMode.READ), WrapType.WRAP_NONE);
                Memory.Memory.OpenBus = (byte)(val >> 8);
                SBC(val);
            }
        }

        #endregion

        #region STA

        private static void STA16(uint opAddress, WrapType w)
        {
            Memory.Memory.SetWord(Registers.A.W, opAddress, w);
            Memory.Memory.OpenBus = Registers.AH;
        }

        private static void STA8(uint opAddress)
        {
            Memory.Memory.SetByte(Registers.AL, opAddress);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op85M1()
        {
            STA8(Direct(AccessMode.WRITE));
        }

        private static void Op95E1()
        {
            STA8(DirectIndexedXE1(AccessMode.WRITE));
        }

        private static void Op95E0M1()
        {
            STA8(DirectIndexedXE0(AccessMode.WRITE));
        }

        private static void Op92E1()
        {
            STA8(DirectIndirectE1(AccessMode.WRITE));
        }

        private static void Op92E0M1()
        {
            STA8(DirectIndirectE0(AccessMode.WRITE));
        }

        private static void Op81E1()
        {
            STA8(DirectIndexedIndirectE1(AccessMode.WRITE));
        }

        private static void Op81E0M1()
        {
            STA8(DirectIndexedIndirectE0(AccessMode.WRITE));
        }

        private static void Op91E1()
        {
            STA8(DirectIndirectIndexedE1(AccessMode.WRITE));
        }

        private static void Op91E0M1X1()
        {
            STA8(DirectIndirectIndexedE0X1(AccessMode.WRITE));
        }

        private static void Op91E0M1X0()
        {
            STA8(DirectIndirectIndexedE0X0(AccessMode.WRITE));
        }

        private static void Op87M1()
        {
            STA8(DirectIndirectLong(AccessMode.WRITE));
        }

        private static void Op97M1()
        {
            STA8(DirectIndirectIndexedLong(AccessMode.WRITE));
        }

        private static void Op8DM1()
        {
            STA8(Absolute(AccessMode.WRITE));
        }

        private static void Op9DM1X1()
        {
            STA8(AbsoluteIndexedXX1(AccessMode.WRITE));
        }

        private static void Op9DM1X0()
        {
            STA8(AbsoluteIndexedXX0(AccessMode.WRITE));
        }

        private static void Op99M1X1()
        {
            STA8(AbsoluteIndexedYX1(AccessMode.WRITE));
        }

        private static void Op99M1X0()
        {
            STA8(AbsoluteIndexedYX0(AccessMode.WRITE));
        }

        private static void Op8FM1()
        {
            STA8(AbsoluteLong(AccessMode.WRITE));
        }

        private static void Op9FM1()
        {
            STA8(AbsoluteLongIndexedX(AccessMode.WRITE));
        }

        private static void Op83M1()
        {
            STA8(StackRelative(AccessMode.WRITE));
        }

        private static void Op93M1()
        {
            STA8(StackRelativeIndirectIndexed(AccessMode.WRITE));
        }

        private static void Op85M0()
        {
            STA16(Direct(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op95E0M0()
        {
            STA16(DirectIndexedXE0(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op92E0M0()
        {
            STA16(DirectIndirectE0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op81E0M0()
        {
            STA16(DirectIndexedIndirectE0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op91E0M0X1()
        {
            STA16(DirectIndirectIndexedE0X1(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op91E0M0X0()
        {
            STA16(DirectIndirectIndexedE0X0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op87M0()
        {
            STA16(DirectIndirectLong(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op97M0()
        {
            STA16(DirectIndirectIndexedLong(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op8DM0()
        {
            STA16(Absolute(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9DM0X1()
        {
            STA16(AbsoluteIndexedXX1(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9DM0X0()
        {
            STA16(AbsoluteIndexedXX0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op99M0X1()
        {
            STA16(AbsoluteIndexedYX1(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op99M0X0()
        {
            STA16(AbsoluteIndexedYX0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op8FM0()
        {
            STA16(AbsoluteLong(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9FM0()
        {
            STA16(AbsoluteLongIndexedX(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op83M0()
        {
            STA16(StackRelative(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op93M0()
        {
            STA16(StackRelativeIndirectIndexed(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op85Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectSlow(AccessMode.WRITE));
            else
                STA16(DirectSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op95Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndexedXSlow(AccessMode.WRITE));
            else
                STA16(DirectIndexedXSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op92Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndirectSlow(AccessMode.WRITE));
            else
                STA16(DirectIndirectSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op81Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndexedIndirectSlow(AccessMode.WRITE));
            else
                STA16(DirectIndexedIndirectSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op91Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndirectIndexedSlow(AccessMode.WRITE));
            else
                STA16(DirectIndirectIndexedSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op87Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndirectLongSlow(AccessMode.WRITE));
            else
                STA16(DirectIndirectLongSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op97Slow()
        {
            if (CheckMemory() > 0)
                STA8(DirectIndirectIndexedLongSlow(AccessMode.WRITE));
            else
                STA16(DirectIndirectIndexedLongSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op8DSlow()
        {
            if (CheckMemory() > 0)
                STA8(AbsoluteSlow(AccessMode.WRITE));
            else
                STA16(AbsoluteSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9DSlow()
        {
            if (CheckMemory() > 0)
                STA8(AbsoluteIndexedXSlow(AccessMode.WRITE));
            else
                STA16(AbsoluteIndexedXSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op99Slow()
        {
            if (CheckMemory() > 0)
                STA8(AbsoluteIndexedYSlow(AccessMode.WRITE));
            else
                STA16(AbsoluteIndexedYSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op8FSlow()
        {
            if (CheckMemory() > 0)
                STA8(AbsoluteLongSlow(AccessMode.WRITE));
            else
                STA16(AbsoluteLongSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9FSlow()
        {
            if (CheckMemory() > 0)
                STA8(AbsoluteLongIndexedXSlow(AccessMode.WRITE));
            else
                STA16(AbsoluteLongIndexedXSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op83Slow()
        {
            if (CheckMemory() > 0)
                STA8(StackRelativeSlow(AccessMode.WRITE));
            else
                STA16(StackRelativeSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op93Slow()
        {
            if (CheckMemory() > 0)
                STA8(StackRelativeIndirectIndexedSlow(AccessMode.WRITE));
            else
                STA16(StackRelativeIndirectIndexedSlow(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        #endregion

        #region STX

        private static void STX16(uint opAddress, WrapType w)
        {
            Memory.Memory.SetWord(Registers.X.W, opAddress, w);
            Memory.Memory.OpenBus = Registers.XH;
        }

        private static void STX8(uint OpAddress)
        {
            Memory.Memory.SetByte(Registers.XL, OpAddress);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void Op86X1()
        {
            STX8(Direct(AccessMode.WRITE));
        }

        private static void Op96E1()
        {
            STX8(DirectIndexedYE1(AccessMode.WRITE));
        }

        private static void Op96E0X1()
        {
            STX8(DirectIndexedYE0(AccessMode.WRITE));
        }

        private static void Op8EX1()
        {
            STX8(Absolute(AccessMode.WRITE));
        }

        private static void Op86X0()
        {
            STX16(Direct(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op96E0X0()
        {
            STX16(DirectIndexedYE0(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op8EX0()
        {
            STX16(Absolute(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op86Slow()
        {
            if (CheckIndex() > 0)
                STX8(DirectSlow(AccessMode.WRITE));
            else
                STX16(DirectSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op96Slow()
        {
            if (CheckIndex() > 0)
                STX8(DirectIndexedYSlow(AccessMode.WRITE));
            else
                STX16(DirectIndexedYSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op8ESlow()
        {
            if (CheckIndex() > 0)
                STX8(AbsoluteSlow(AccessMode.WRITE));
            else
                STX16(AbsoluteSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        #endregion

        #region STY

        private static void STY16(uint opAddress, WrapType w)
        {
            Memory.Memory.SetWord(Registers.Y.W, opAddress, w);
            Memory.Memory.OpenBus = Registers.YH;
        }

        private static void STY8(uint OpAddress)
        {
            Memory.Memory.SetByte(Registers.YL, OpAddress);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op84X1()
        {
            STY8(Direct(AccessMode.WRITE));
        }

        private static void Op94E1()
        {
            STY8(DirectIndexedXE1(AccessMode.WRITE));
        }

        private static void Op94E0X1()
        {
            STY8(DirectIndexedXE0(AccessMode.WRITE));
        }

        private static void Op8CX1()
        {
            STY8(Absolute(AccessMode.WRITE));
        }

        private static void Op84X0()
        {
            STY16(Direct(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op94E0X0()
        {
            STY16(DirectIndexedXE0(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op8CX0()
        {
            STY16(Absolute(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op84Slow()
        {
            if (CheckIndex() > 0)
                STY8(DirectSlow(AccessMode.WRITE));
            else
                STY16(DirectSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op94Slow()
        {
            if (CheckIndex() > 0)
                STY8(DirectIndexedYSlow(AccessMode.WRITE));
            else
                STY16(DirectIndexedYSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op8CSlow()
        {
            if (CheckIndex() > 0)
                STY8(AbsoluteSlow(AccessMode.WRITE));
            else
                STY16(AbsoluteSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        #endregion

        #region STZ

        private static void STZ16(uint opAddress, WrapType w)
        {
            Memory.Memory.SetWord(0, opAddress, w);
            Memory.Memory.OpenBus = 0;
        }

        private static void STZ8(uint opAddress)
        {
            Memory.Memory.SetByte(0, opAddress);
            Memory.Memory.OpenBus = 0;
        }

        private static void Op64M1()
        {
	        STZ8(Direct(AccessMode.WRITE));
        }

        private static void Op74E1()
        {
            STZ8(DirectIndexedXE1(AccessMode.WRITE));
        }

        private static void Op74E0M1()
        {
            STZ8(DirectIndexedXE0(AccessMode.WRITE));
        }

        private static void Op9CM1()
        {
            STZ8(Absolute(AccessMode.WRITE));
        }

        private static void Op9EM1X1()
        {
            STZ8(AbsoluteIndexedXX1(AccessMode.WRITE));
        }

        private static void Op9EM1X0()
        {
            STZ8(AbsoluteIndexedXX0(AccessMode.WRITE));
        }

        private static void Op64M0()
        {
	        STZ16(Direct(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op74E0M0()
        {
            STZ16(DirectIndexedXE0(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op9CM0()
        {
            STZ16(Absolute(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9EM0X1()
        {
            STZ16(AbsoluteIndexedXX1(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op9EM0X0()
        {
            STZ16(AbsoluteIndexedXX0(AccessMode.WRITE), WrapType.WRAP_NONE);
        }

        private static void Op64Slow()
        {
	        if (CheckMemory() > 0)
                STZ8(DirectSlow(AccessMode.WRITE));
	        else
                STZ16(DirectSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op74Slow()
        {
            if (CheckMemory() > 0)
                STZ8(DirectIndexedXSlow(AccessMode.WRITE));
            else
                STZ16(DirectIndexedXSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op9CSlow()
        {
            if (CheckMemory() > 0)
                STZ8(AbsoluteSlow(AccessMode.WRITE));
            else
                STZ16(AbsoluteSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        private static void Op9ESlow()
        {
            if (CheckMemory() > 0)
                STZ8(AbsoluteIndexedXSlow(AccessMode.WRITE));
            else
                STZ16(AbsoluteIndexedXSlow(AccessMode.WRITE), WrapType.WRAP_BANK);
        }

        #endregion

        #region TRB

        private static void TRB16(uint opAddress, WrapType w)
        {
	        var work16 = Memory.Memory.GetWord(opAddress, w);
            ICPU._Zero = (byte)((work16 & Registers.A.W) != 0 ? 1 : 0);
	        work16 &= (ushort)~Registers.A.W;
	        AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
        }

        private static void TRB8(uint opAddress)
        {
            var work8 = Memory.Memory.GetByte(opAddress);
            ICPU._Zero = (byte)(work8 & Registers.AL);
            work8 &= (byte)~Registers.AL;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, opAddress);
            Memory.Memory.OpenBus = work8;
        }

        private static void Op14M1()
        {
            TRB8(Direct(AccessMode.MODIFY));
        }

        private static void Op1CM1()
        {
            TRB8(Absolute(AccessMode.MODIFY));
        }

        private static void Op14M0()
        {
            TRB16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op1CM0()
        {
            TRB16(Absolute(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op14Slow()
        {
	        if (CheckMemory() > 0)
		        TRB8(DirectSlow(AccessMode.MODIFY));
	        else
		        TRB16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op1CSlow()
        {
            if (CheckMemory() > 0)
                TRB8(AbsoluteSlow(AccessMode.MODIFY));
            else
                TRB16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        #endregion

        #region TSB

        private static void TSB16(uint opAddress, WrapType w)
        {
            var work16 = Memory.Memory.GetWord(opAddress, w);
            ICPU._Zero = (byte)((work16 & Registers.A.W) != 0 ? 1 : 0);
            work16 |= Registers.A.W;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetWord(work16, opAddress, w, WriteOrder.WRITE_10);
            Memory.Memory.OpenBus = (byte)(work16 & 0xff);
        }

        private static void TSB8(uint opAddress)
        {
            var work8 = Memory.Memory.GetByte(opAddress);
            ICPU._Zero = (byte)(work8 & Registers.AL);
            work8 |= Registers.AL;
            AddCycles(Constants.ONE_CYCLE);
            Memory.Memory.SetByte(work8, opAddress);
            Memory.Memory.OpenBus = work8;
        }

        private static void Op04M1()
        {
            TSB8(Direct(AccessMode.MODIFY));
        }

        private static void Op0CM1()
        {
            TSB8(Absolute(AccessMode.MODIFY));
        }

        private static void Op04M0()
        {
            TSB16(Direct(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op0CM0()
        {
            TSB16(Absolute(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op04Slow()
        {
            if (CheckMemory() > 0)
                TSB8(DirectSlow(AccessMode.MODIFY));
            else
                TSB16(DirectSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        private static void Op0CSlow()
        {
            if (CheckMemory() > 0)
                TSB8(AbsoluteSlow(AccessMode.MODIFY));
            else
                TSB16(AbsoluteSlow(AccessMode.MODIFY), WrapType.WRAP_BANK);
        }

        #endregion

        #region Branch

        private static void Op90E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckCarry() == 0)
	        {
		        AddCycles(Constants.ONE_CYCLE);
		        if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
			        Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
		        else
			        Registers.PCw = newPC.W;
	        }
        }

        private static void Op90E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckCarry() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
			        AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op90Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckCarry() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpB0E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckCarry() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpB0E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckCarry() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpB0Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckCarry() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpF0E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckZero() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpF0E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckZero()  != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpF0Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckZero() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op30E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckNegative() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op30E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckNegative() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op30Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckNegative() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpD0E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckZero() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpD0E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckZero() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void OpD0Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckZero() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op10E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckNegative() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op10E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckNegative() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op10Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckNegative() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op80E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            AddCycles(Constants.ONE_CYCLE);
            if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
            else
                Registers.PCw = newPC.W;
        }

        private static void Op80E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            AddCycles(Constants.ONE_CYCLE);
            if (Registers.PCh != newPC.High)
                AddCycles(Constants.ONE_CYCLE);
            if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
            else
                Registers.PCw = newPC.W;
        }

        private static void Op80Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            AddCycles(Constants.ONE_CYCLE);
            if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                AddCycles(Constants.ONE_CYCLE);
            if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
            else
                Registers.PCw = newPC.W;
        }

        private static void Op50E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckOverflow() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op50E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckOverflow() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op50Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckOverflow() == 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op70E0()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckOverflow() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op70E1()
        {
            var newPC = new Pair
            {
                W = (ushort)Relative(AccessMode.JUMP)
            };
            if (CheckOverflow() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op70Slow()
        {
            var newPC = new Pair
            {
                W = (ushort)RelativeSlow(AccessMode.JUMP)
            };
            if (CheckOverflow() != 0)
            {
                AddCycles(Constants.ONE_CYCLE);
                if (CheckEmulation() > 0 && Registers.PCh != newPC.High)
                    AddCycles(Constants.ONE_CYCLE);
                if ((Registers.PCw & ~Memory.Constants.MASK) != (newPC.W & ~Memory.Constants.MASK))
                    Memory.Memory.SetPCBase(ICPU.ShiftedPB + newPC.W);
                else
                    Registers.PCw = newPC.W;
            }
        }

        private static void Op82()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + RelativeLong(AccessMode.JUMP));
        }

        private static void Op82Slow()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + RelativeLongSlow(AccessMode.JUMP));
        }

        #endregion

        #region Flag

        // CLC
        private static void Op18()
        {
            ClearCarry();
            AddCycles(Constants.ONE_CYCLE);
        }

        // SEC
        private static void Op38()
        {
            SetCarry();
            AddCycles(Constants.ONE_CYCLE);
        }

        // CLD
        private static void OpD8()
        {
            ClearDecimal();
            AddCycles(Constants.ONE_CYCLE);
        }

        // SED
        private static void OpF8()
        {
            SetDecimal();
            AddCycles(Constants.ONE_CYCLE);
        }

        // CLI
        private static void Op58()
        {
            AddCycles(Constants.ONE_CYCLE);
            Timings.Timings.IRQFlagChanging = IRQ.IRQ_CLEAR_FLAG;
        }

        // SEI
        private static void Op78()
        {
            AddCycles(Constants.ONE_CYCLE);
            SetIRQ();
        }

        // CLV
        private static void OpB8()
        {
            ClearOverflow();
            AddCycles(Constants.ONE_CYCLE);
        }

        #endregion

        #region DEX/DEY

        private static void OpCAX1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.XL--;
            SetZN(Registers.XL);
        }

        private static void OpCAX0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.X.W--;
            SetZN(Registers.X.W);
        }

        private static void OpCASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckIndex() > 0)
            {
                Registers.XL--;
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W--;
                SetZN(Registers.X.W);
            }
        }

        private static void Op88X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.YL--;
            SetZN(Registers.YL);
        }

        private static void Op88X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.Y.W--;
            SetZN(Registers.Y.W);
        }

        private static void Op88Slow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckIndex() > 0)
            {
                Registers.YL--;
                SetZN(Registers.YL);
            }
            else
            {
                Registers.Y.W--;
                SetZN(Registers.Y.W);
            }
        }

        #endregion

        #region INX/INY

        private static void OpE8X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.XL++;
            SetZN(Registers.XL);
        }

        private static void OpE8X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.X.W++;
            SetZN(Registers.X.W);
        }

        private static void OpE8Slow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckIndex() > 0)
            {
                Registers.XL++;
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W++;
                SetZN(Registers.X.W);
            }
        }

        private static void OpC8X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.YL++;
            SetZN(Registers.YL);
        }

        private static void OpC8X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.Y.W++;
            SetZN(Registers.Y.W);
        }

        private static void OpC8Slow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckIndex() > 0)
            {
                Registers.YL++;
                SetZN(Registers.YL);
            }
            else
            {
                Registers.Y.W++;
                SetZN(Registers.Y.W);
            }
        }

        #endregion

        #region NOP

        private static void OpEA()
        {
            AddCycles(Constants.ONE_CYCLE);
        }

        #endregion

        #region PUSH

        private static void PushW(ushort w)
        {
            Memory.Memory.SetWord(w, (uint)(Registers.S.W - 1), WrapType.WRAP_BANK, WriteOrder.WRITE_10);
        	Registers.S.W -= 2;
        }

        private static void PushWE(ushort w)
        {
            Registers.SL--;
        	Memory.Memory.SetWord(w, Registers.S.W, WrapType.WRAP_PAGE, WriteOrder.WRITE_10);
        	Registers.SL--;
        }

        private static void PushB(byte b)
        {
            Memory.Memory.SetByte(b, Registers.S.W--);
        }

        private static void PushBE(byte b)
        {
            Memory.Memory.SetByte(b, Registers.S.W);
        	Registers.SL--;
        }

        // PEA
       private static void OpF4E0()
        {
            var val = (ushort)Absolute(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
        }

        private static void OpF4E1()
        {
            // Note: PEA is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            var val = (ushort)Absolute(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            Registers.SH = 1;
        }

        private static void OpF4Slow()
        {
            var val = (ushort)AbsoluteSlow(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // PEI
        private static void OpD4E0()
        {
            var val = (ushort)DirectIndirectE0(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
        }

        private static void OpD4E1()
        {
            // Note: PEI is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            var val = (ushort)DirectIndirectE1(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            Registers.SH = 1;
        }

        private static void OpD4Slow()
        {
            var val = (ushort)DirectIndirectSlow(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // PER
        private static void Op62E0()
        {
            var val = (ushort)RelativeLong(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
        }

        private static void Op62E1()
        {
            // Note: PER is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            var val = (ushort)RelativeLong(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            Registers.SH = 1;
        }

        private static void Op62Slow()
        {
            var val = (ushort)RelativeLongSlow(AccessMode.NONE);
            PushW(val);
            Memory.Memory.OpenBus = (byte)(val & 0xff);
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // PHA
        private static void Op48E1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.AL);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op48E0M1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.AL);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op48E0M0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.A.W);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op48Slow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckEmulation() > 0)
                PushBE(Registers.AL);
            else if (CheckMemory() > 0)
                PushB(Registers.AL);
            else
                PushW(Registers.A.W);

            Memory.Memory.OpenBus = Registers.AL;
        }

        // PHB
        private static void Op8BE1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.DB);
            Memory.Memory.OpenBus = Registers.DB;
        }

        private static void Op8BE0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.DB);
            Memory.Memory.OpenBus = Registers.DB;
        }

        private static void Op8BSlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                PushBE(Registers.DB);
            else
                PushB(Registers.DB);

            Memory.Memory.OpenBus = Registers.DB;
        }

        // PHD
        private static void Op0BE0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DL;
        }

        private static void Op0BE1()
        {
            // Note: PHD is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DL;
            Registers.SH = 1;
        }

        private static void Op0BSlow()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DL;
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // PHK
        private static void Op4BE1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.PB);
            Memory.Memory.OpenBus = Registers.PB;
        }

        private static void Op4BE0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.PB);
            Memory.Memory.OpenBus = Registers.PB;
        }

        private static void Op4BSlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                PushBE(Registers.PB);
            else
                PushB(Registers.PB);

            Memory.Memory.OpenBus = Registers.PB;
        }

        // PHP
        private static void Op08E0()
        {
            PackStatus();
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.PL);
            Memory.Memory.OpenBus = Registers.PL;
        }

        private static void Op08E1()
        {
            PackStatus();
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.PL);
            Memory.Memory.OpenBus = Registers.PL;
        }

        private static void Op08Slow()
        {
            PackStatus();
            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                PushBE(Registers.PL);
            else
                PushB(Registers.PL);

            Memory.Memory.OpenBus = Registers.PL;
        }

        // PHX
        private static void OpDAE1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.XL);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void OpDAE0X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.XL);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void OpDAE0X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.X.W);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void OpDASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                PushBE(Registers.XL);
            else if (CheckIndex() > 0)
                PushB(Registers.XL);
            else
                PushW(Registers.X.W);

            Memory.Memory.OpenBus = Registers.XL;
        }

        // PHY
        private static void Op5AE1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushBE(Registers.YL);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op5AE0X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushB(Registers.YL);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op5AE0X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            PushW(Registers.Y.W);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op5ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                PushBE(Registers.YL);
            else if (CheckIndex() > 0)
                PushB(Registers.YL);
            else
                PushW(Registers.Y.W);

            Memory.Memory.OpenBus = Registers.YL;
        }

        #endregion

        #region PULL

        private static ushort PullW()
        {
            var w = Memory.Memory.GetWord((uint)(Registers.S.W + 1), WrapType.WRAP_BANK);
            Registers.S.W += 2;
            return w;
        }

        private static ushort PullWE()
        {
            Registers.SL++;
            var w = Memory.Memory.GetWord(Registers.S.W, WrapType.WRAP_PAGE);
            Registers.SL++;
            return w;
        }

        private static byte PullB()
        {
            return Memory.Memory.GetByte(++Registers.S.W);
        }

        private static byte PullBE()
        {
            Registers.SL++;
            return Memory.Memory.GetByte(Registers.S.W);
        }

        // PLA
        private static void Op68E1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.AL = PullBE();
            SetZN(Registers.AL);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op68E0M1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.AL = PullB();
            SetZN(Registers.AL);
            Memory.Memory.OpenBus = Registers.AL;
        }

        private static void Op68E0M0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.A.W = PullW();
            SetZN(Registers.A.W);
            Memory.Memory.OpenBus = Registers.AH;
        }

        private static void Op68Slow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
            {
                Registers.AL = PullBE();
                SetZN(Registers.AL);
                Memory.Memory.OpenBus = Registers.AL;
            }
            else if (CheckMemory() > 0)
            {
                Registers.AL = PullB();
                SetZN(Registers.AL);
                Memory.Memory.OpenBus = Registers.AL;
            }
            else
            {
                Registers.A.W = PullW();
                SetZN(Registers.A.W);
                Memory.Memory.OpenBus = Registers.AH;
            }
        }

        // PLB
        private static void OpABE1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.DB = PullBE();
            SetZN(Registers.DB);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            Memory.Memory.OpenBus = Registers.DB;
        }

        private static void OpABE0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.DB = PullB();
            SetZN(Registers.DB);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            Memory.Memory.OpenBus = Registers.DB;
        }

        private static void OpABSlow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
                Registers.DB = PullBE();
            else
                Registers.DB = PullB();

            SetZN(Registers.DB);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            Memory.Memory.OpenBus = Registers.DB;
        }

        // PLD
        private static void Op2BE0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.D.W = PullW();
            SetZN(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DH;
        }

        private static void Op2BE1()
        {
            // Note: PLD is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            AddCycles(Constants.TWO_CYCLES);
            Registers.D.W = PullW();
            SetZN(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DH;
            Registers.SH = 1;
        }

        private static void Op2BSlow()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.D.W = PullW();
            SetZN(Registers.D.W);
            Memory.Memory.OpenBus = Registers.DH;
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // PLP
        private static void Op28E1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PL = PullBE();
            Memory.Memory.OpenBus = Registers.PL;
            SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));
            UnpackStatus();
            FixCycles();
        }

        private static void Op28E0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PL = PullB();
            Memory.Memory.OpenBus = Registers.PL;
            UnpackStatus();
            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        private static void Op28Slow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
            {
                Registers.PL = PullBE();
                Memory.Memory.OpenBus = Registers.PL;
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));
            }
            else
            {
                Registers.PL = PullB();
                Memory.Memory.OpenBus = Registers.PL;
            }
            UnpackStatus();
            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        // PLX
        private static void OpFAE1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.XL = PullBE();
            SetZN(Registers.XL);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void OpFAE0X1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.XL = PullB();
            SetZN(Registers.XL);
            Memory.Memory.OpenBus = Registers.XL;
        }

        private static void OpFAE0X0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.X.W = PullW();
            SetZN(Registers.X.W);
            Memory.Memory.OpenBus = Registers.XH;
        }

        private static void OpFASlow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
            {
                Registers.XL = PullBE();
                SetZN(Registers.XL);
                Memory.Memory.OpenBus = Registers.XL;
            }
            else if (CheckIndex() > 0)
            {
                Registers.XL = PullB();
                SetZN(Registers.XL);
                Memory.Memory.OpenBus = Registers.XL;
            }
            else
            {
                Registers.X.W = PullW();
                SetZN(Registers.X.W);
                Memory.Memory.OpenBus = Registers.XH;
            }
        }

        // PLY
        private static void Op7AE1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.YL = PullBE();
            SetZN(Registers.YL);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op7AE0X1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.YL = PullB();
            SetZN(Registers.YL);
            Memory.Memory.OpenBus = Registers.YL;
        }

        private static void Op7AE0X0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.Y.W = PullW();
            SetZN(Registers.Y.W);
            Memory.Memory.OpenBus = Registers.YH;
        }

        private static void Op7ASlow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
            {
                Registers.YL = PullBE();
                SetZN(Registers.YL);
                Memory.Memory.OpenBus = Registers.YL;
            }
            else if (CheckIndex() > 0)
            {
                Registers.YL = PullB();
                SetZN(Registers.YL);
                Memory.Memory.OpenBus = Registers.YL;
            }
            else
            {
                Registers.Y.W = PullW();
                SetZN(Registers.Y.W);
                Memory.Memory.OpenBus = Registers.YH;
            }
        }

        #endregion

        #region Transfer

        // TAX
        private static void OpAAX1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.XL = Registers.AL;
            SetZN(Registers.XL);
        }

        private static void OpAAX0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.X.W = Registers.A.W;
            SetZN(Registers.X.W);
        }

        private static void OpAASlow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckIndex() > 0)
            {
                Registers.XL = Registers.AL;
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W = Registers.A.W;
                SetZN(Registers.X.W);
            }
        }

        // TAY
        private static void OpA8X1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.YL = Registers.AL;
            SetZN(Registers.YL);
        }

        private static void OpA8X0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.Y.W = Registers.A.W;
            SetZN(Registers.Y.W);
        }

        private static void OpA8Slow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckIndex() > 0)
            {
                Registers.YL = Registers.AL;
                SetZN(Registers.YL);
            }
            else
            {
                Registers.Y.W = Registers.A.W;
                SetZN(Registers.Y.W);
            }
        }

        // TCD
        private static void Op5B()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.D.W = Registers.A.W;
            SetZN(Registers.D.W);
        }

        // TCS
        private static void Op1B()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.S.W = Registers.A.W;
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // TDC
        private static void Op7B()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W = Registers.D.W;
            SetZN(Registers.A.W);
        }

        // TSC
        private static void Op3B()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W = Registers.S.W;
            SetZN(Registers.A.W);
        }

        // TSX
        private static void OpBAX1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.XL = Registers.SL;
            SetZN(Registers.XL);
        }

        private static void OpBAX0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.X.W = Registers.S.W;
            SetZN(Registers.X.W);
        }

        private static void OpBASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckIndex() > 0)
            {
                Registers.XL = Registers.SL;
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W = Registers.S.W;
                SetZN(Registers.X.W);
            }
        }

        // TXA
        private static void Op8AM1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.AL = Registers.XL;
            SetZN(Registers.AL);
        }

        private static void Op8AM0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W = Registers.X.W;
            SetZN(Registers.A.W);
        }

        private static void Op8ASlow()
        {
            AddCycles(Constants.ONE_CYCLE);

            if (CheckMemory() > 0)
            {
                Registers.AL = Registers.XL;
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W = Registers.X.W;
                SetZN(Registers.A.W);
            }
        }

        // TXS
        private static void Op9A()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.S.W = Registers.X.W;
            if (CheckEmulation() > 0)
                Registers.SH = 1;
        }

        // TXY
        private static void Op9BX1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.YL = Registers.XL;
            SetZN(Registers.YL);
        }

        private static void Op9BX0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.Y.W = Registers.X.W;
            SetZN(Registers.Y.W);
        }

        private static void Op9BSlow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckIndex() > 0)
            {
                Registers.YL = Registers.XL;
                SetZN(Registers.YL);
            }
            else
            {
                Registers.Y.W = Registers.X.W;
                SetZN(Registers.Y.W);
            }
        }

        // TYA
        private static void Op98M1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.AL = Registers.YL;
            SetZN(Registers.AL);
        }

        private static void Op98M0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.A.W = Registers.Y.W;
            SetZN(Registers.A.W);
        }

        private static void Op98Slow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckMemory() > 0)
            {
                Registers.AL = Registers.YL;
                SetZN(Registers.AL);
            }
            else
            {
                Registers.A.W = Registers.Y.W;
                SetZN(Registers.A.W);
            }
        }

        // TYX
        private static void OpBBX1()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.XL = Registers.YL;
            SetZN(Registers.XL);
        }

        private static void OpBBX0()
        {
            AddCycles(Constants.ONE_CYCLE);
            Registers.X.W = Registers.Y.W;
            SetZN(Registers.X.W);
        }

        private static void OpBBSlow()
        {
            AddCycles(Constants.ONE_CYCLE);
            if (CheckIndex() > 0)
            {
                Registers.XL = Registers.YL;
                SetZN(Registers.XL);
            }
            else
            {
                Registers.X.W = Registers.Y.W;
                SetZN(Registers.X.W);
            }
        }

        #endregion

        #region XCE

        private static void OpFB()
        {
            AddCycles(Constants.ONE_CYCLE);

            var A1 = ICPU._Carry;
            var A2 = Registers.PH;

            ICPU._Carry = (byte)(A2 & 1);
            Registers.PH = A1;

            if (CheckEmulation() > 0)
            {
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));
                Registers.SH = 1;
            }

            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }

            FixCycles();
        }

        #endregion

        #region BRK

        private static void Op00()
        {
            AddCycles(CPUState.MemSpeed);

            ushort addr;

            if (CheckEmulation() == 0)
            {
                PushB(Registers.PB);
                PushW((ushort)(Registers.PCw + 1));
                PackStatus();
                PushB(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();
                addr = Memory.Memory.GetWord(0xFFE6);
            }
            else
            {
                PushWE((ushort)(Registers.PCw + 1));
                PackStatus();
                PushBE(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();
                addr = Memory.Memory.GetWord(0xFFFE);
            }

            Memory.Memory.SetPCBase(addr);
            Memory.Memory.OpenBus = (byte)(addr >> 8);
        }

        #endregion

        #region IRQ

        public static void OpcodeIRQ()
        {
            // IRQ and NMI do an opcode fetch as their first "IO" cycle.
            AddCycles(CPUState.MemSpeed + Constants.ONE_CYCLE);

            if (CheckEmulation() == 0)
            {
                PushB(Registers.PB);
                PushW(Registers.PCw);
                PackStatus();
                PushB(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();

                if (Settings.SA1 && (Memory.Memory.FillRAM[0x2209] & 0x40) > 0)
                {
                    Memory.Memory.OpenBus = Memory.Memory.FillRAM[0x220f];
                    AddCycles(2 * Constants.SLOW_ONE_CYCLE);
                    Memory.Memory.SetPCBase((uint)(Memory.Memory.FillRAM[0x220e] | (Memory.Memory.FillRAM[0x220f] << 8)));
                }
                else
                {
                    var addr = Memory.Memory.GetWord(0xFFEE);
                    Memory.Memory.OpenBus = (byte)(addr >> 8);
                    Memory.Memory.SetPCBase(addr);
                }
            }
            else
            {
                PushWE(Registers.PCw);
                PackStatus();
                PushBE(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();

                if (Settings.SA1 && (Memory.Memory.FillRAM[0x2209] & 0x40) > 0)
                {
                    Memory.Memory.OpenBus = Memory.Memory.FillRAM[0x220f];
                    AddCycles(2 * Constants.SLOW_ONE_CYCLE);
                    Memory.Memory.SetPCBase((uint)(Memory.Memory.FillRAM[0x220e] | (Memory.Memory.FillRAM[0x220f] << 8)));
                }
                else
                {
                    var addr = Memory.Memory.GetWord(0xFFFE);
                    Memory.Memory.OpenBus = (byte)(addr >> 8);
                    Memory.Memory.SetPCBase(addr);
                }
            }
        }

        #endregion

        #region NMI

        public static void OpcodeNMI()
        {
            // IRQ and NMI do an opcode fetch as their first "IO" cycle.
            AddCycles(CPUState.MemSpeed + Constants.ONE_CYCLE);
            if (CheckEmulation() == 0)
            {
                PushB(Registers.PB);
                PushW(Registers.PCw);
                PackStatus();
                PushB(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();

                if (Settings.SA1 && (Memory.Memory.ROM.bytes[0x2209] & 0x10) > 0)
                {
                    Memory.Memory.OpenBus = Memory.Memory.ROM.bytes[0x220d];
                    AddCycles(2 * Constants.SLOW_ONE_CYCLE);
                    Memory.Memory.SetPCBase((uint)(Memory.Memory.FillRAM[0x220c] | (Memory.Memory.FillRAM[0x220d] << 8)));
                }
                else
                {
                    var addr = Memory.Memory.GetWord(0xFFEA);
                    Memory.Memory.OpenBus = (byte)(addr >> 8);
                    Memory.Memory.SetPCBase(addr);
                }
            }
            else
            {
                PushWE(Registers.PCw);
                PackStatus();
                PushBE(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();

                if (Settings.SA1 && (Memory.Memory.FillRAM[0x2209] & 0x10) > 0)
                {
                    Memory.Memory.OpenBus = Memory.Memory.FillRAM[0x220d];
                    AddCycles(2 * Constants.SLOW_ONE_CYCLE);
                    Memory.Memory.SetPCBase((uint)(Memory.Memory.FillRAM[0x220c] | (Memory.Memory.FillRAM[0x220d] << 8)));
                }
                else
                {
                    var addr = Memory.Memory.GetWord(0xFFFA);
                    Memory.Memory.OpenBus = (byte)(addr >> 8);
                    Memory.Memory.SetPCBase(addr);
                }
            }
        }

        #endregion

        #region COP

        private static void Op02()
        {
            AddCycles(CPUState.MemSpeed);
            ushort addr;

            if (CheckEmulation() == 0)
            {
                PushB(Registers.PB);
                PushW((ushort)(Registers.PCw + 1));
                PackStatus();
                PushB(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();
                addr = Memory.Memory.GetWord(0xFFE4);
            }
            else
            {
                PushWE((ushort)(Registers.PCw + 1));
                PackStatus();
                PushBE(Registers.PL);
                Memory.Memory.OpenBus = Registers.PL;
                ClearDecimal();
                SetIRQ();
                addr = Memory.Memory.GetWord(0xFFF4);
            }

            Memory.Memory.SetPCBase(addr);
            Memory.Memory.OpenBus = (byte)(addr >> 8);
        }

        #endregion

        #region JML

        private static void OpDC()
        {
            Memory.Memory.SetPCBase(AbsoluteIndirectLong(AccessMode.JUMP));
        }

        static void OpDCSlow()
        {
            Memory.Memory.SetPCBase(AbsoluteIndirectLongSlow(AccessMode.JUMP));
        }

        static void Op5C()
        {
            Memory.Memory.SetPCBase(AbsoluteLong(AccessMode.JUMP));
        }

        static void Op5CSlow()
        {
            Memory.Memory.SetPCBase(AbsoluteLongSlow(AccessMode.JUMP));
        }

        #endregion

        #region JMP

        private static void Op4C()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)Absolute(AccessMode.JUMP)));
        }

        private static void Op4CSlow()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)AbsoluteSlow(AccessMode.JUMP)));
        }

        private static void Op6C()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)AbsoluteIndirect(AccessMode.JUMP)));
        }

        private static void Op6CSlow()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)AbsoluteIndirectSlow(AccessMode.JUMP)));
        }

        private static void Op7C()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)AbsoluteIndexedIndirect(AccessMode.JUMP)));
        }

        private static void Op7CSlow()
        {
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + ((ushort)AbsoluteIndexedIndirectSlow(AccessMode.JUMP)));
        }

        #endregion

        #region JSL/RTL

        private static void Op22E1()
        {
            // Note: JSL is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            var addr = AbsoluteLong(AccessMode.JSR);
            PushB(Registers.PB);
            PushW((ushort)(Registers.PCw - 1));
            Registers.SH = 1;
            Memory.Memory.SetPCBase(addr);
        }

        private static void Op22E0()
        {
            var addr = AbsoluteLong(AccessMode.JSR);
            PushB(Registers.PB);
            PushW((ushort)(Registers.PCw - 1));
            Memory.Memory.SetPCBase(addr);
        }

        private static void Op22Slow()
        {
            var addr = AbsoluteLongSlow(AccessMode.JSR);
            PushB(Registers.PB);
            PushW((ushort)(Registers.PCw - 1));
            if (CheckEmulation() > 0)
                Registers.SH = 1;

            Memory.Memory.SetPCBase(addr);
        }

        private static void Op6BE1()
        {
            // Note: RTL is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            AddCycles(Constants.TWO_CYCLES);
            Registers.PCw = PullW();
            Registers.PB = PullB();
            Registers.SH = 1;
            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        private static void Op6BE0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PCw = PullW();
            Registers.PB = PullB();
            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        private static void Op6BSlow()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PCw = PullW();
            Registers.PB = PullB();
            if (CheckEmulation() > 0)
                Registers.SH = 1;

            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        #endregion

        #region JSR/RTS

        private static void Op20E1()
        {
            var addr = Absolute(AccessMode.JSR);
            AddCycles(Constants.ONE_CYCLE);
            PushWE((ushort)(Registers.PCw - 1));
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void Op20E0()
        {
            var addr = Absolute(AccessMode.JSR);
            AddCycles(Constants.ONE_CYCLE);
            PushW((ushort)(Registers.PCw - 1));
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void Op20Slow()
        {
            var addr = AbsoluteSlow(AccessMode.JSR);
            AddCycles(Constants.ONE_CYCLE);
            if (CheckEmulation() > 0)
                PushWE((ushort)(Registers.PCw - 1));
            else
                PushW((ushort)(Registers.PCw - 1));

            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void OpFCE1()
        {
            // Note: JSR (a,X) is a new instruction,
            // and so doesn't respect the emu-mode stack bounds.
            var addr = AbsoluteIndexedIndirect(AccessMode.JSR);
            PushW((ushort)(Registers.PCw - 1));
            Registers.SH = 1;
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void OpFCE0()
        {
            var addr = AbsoluteIndexedIndirect(AccessMode.JSR);
            PushW((ushort)(Registers.PCw - 1));
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void OpFCSlow()
        {
            var addr = AbsoluteIndexedIndirectSlow(AccessMode.JSR);
            PushW((ushort)(Registers.PCw - 1));
            if (CheckEmulation() > 0)
                Registers.SH = 1;
            Memory.Memory.SetPCBase(ICPU.ShiftedPB + addr);
        }

        private static void Op60E1()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PCw = PullWE();
            AddCycles(Constants.ONE_CYCLE);
            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        private static void Op60E0()
        {
            AddCycles(Constants.TWO_CYCLES);
            Registers.PCw = PullW();
            AddCycles(Constants.ONE_CYCLE);
            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        private static void Op60Slow()
        {
            AddCycles(Constants.TWO_CYCLES);
            if (CheckEmulation() > 0)
                Registers.PCw = PullWE();
            else
                Registers.PCw = PullW();

            AddCycles(Constants.ONE_CYCLE);
            Registers.PCw++;
            Memory.Memory.SetPCBase(Registers.PBPC);
        }

        #endregion

        #region MVN/MVP

        private static void Op54X1()
        {
            uint srcBank;

            Registers.DB = Immediate8(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            Registers.XL++;
            Registers.YL++;
            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        private static void Op54X0()
        {
            uint srcBank;

            Registers.DB = Immediate8(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            Registers.X.W++;
            Registers.Y.W++;
            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        private static void Op54Slow()
        {
            uint srcBank;

            Memory.Memory.OpenBus = Registers.DB = Immediate8Slow(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8Slow(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            if (CheckIndex() > 0)
            {
                Registers.XL++;
                Registers.YL++;
            }
            else
            {
                Registers.X.W++;
                Registers.Y.W++;
            }

            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        private static void Op44X1()
        {
            uint srcBank;

            Registers.DB = Immediate8(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            Registers.XL--;
            Registers.YL--;
            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        private static void Op44X0()
        {
            uint srcBank;

            Registers.DB = Immediate8(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            Registers.X.W--;
            Registers.Y.W--;
            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        private static void Op44Slow()
        {
            uint srcBank;

            Memory.Memory.OpenBus = Registers.DB = Immediate8Slow(AccessMode.NONE);
            ICPU.ShiftedDB = (uint)(Registers.DB << 16);
            srcBank = Memory.Memory.OpenBus = Immediate8Slow(AccessMode.NONE);

            Memory.Memory.SetByte(Memory.Memory.OpenBus = Memory.Memory.GetByte((srcBank << 16) + Registers.X.W), ICPU.ShiftedDB + Registers.Y.W);

            if (CheckIndex() > 0)
            {
                Registers.XL--;
                Registers.YL--;
            }
            else
            {
                Registers.X.W--;
                Registers.Y.W--;
            }

            Registers.A.W--;
            if (Registers.A.W != 0xffff)
                Registers.PCw -= 3;

            AddCycles(Constants.TWO_CYCLES);
        }

        #endregion

        #region REP/SEP

        private static void OpC2()
        {
            var work8 = (byte)(~Immediate8(AccessMode.READ));
            Registers.PL &= work8;
            ICPU._Carry &= work8;
            ICPU._Overflow &= (byte)(work8 >> 6);
            ICPU._Negative &= work8;
            ICPU._Zero |= (byte)(~work8 & Constants.Zero);

            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));

            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        private static void OpC2Slow()
        {
            var work8 = (byte)~Immediate8Slow(AccessMode.READ);
            Registers.PL &= work8;
            ICPU._Carry &= work8;
            ICPU._Overflow &= (byte)(work8 >> 6);
            ICPU._Negative &= work8;
            ICPU._Zero |= (byte)(~work8 & Constants.Zero);

            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));

            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        private static void OpE2()
        {
            var work8 = Immediate8(AccessMode.READ);
            Registers.PL |= work8;
            ICPU._Carry |= (byte)(work8 & 1);
            ICPU._Overflow |= (byte)((work8 >> 6) & 1);
            ICPU._Negative |= work8;
            if ((work8 & Constants.Zero) > 0)
                ICPU._Zero = 0;

            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));

            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        private static void OpE2Slow()
        {
            var work8 = Immediate8Slow(AccessMode.READ);
            Registers.PL |= work8;
            ICPU._Carry |= (byte)(work8 & 1);
            ICPU._Overflow |= (byte)((work8 >> 6) & 1);
            ICPU._Negative |= work8;
            if ((work8 & Constants.Zero) > 0)
                ICPU._Zero = 0;

            AddCycles(Constants.ONE_CYCLE);

            if (CheckEmulation() > 0)
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));

            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        #endregion

        #region XBA

        private static void OpEB()
        {
            var work8 = Registers.AL;
            Registers.AL = Registers.AH;
            Registers.AH = work8;
            SetZN(Registers.AL);
            AddCycles(Constants.TWO_CYCLES);
        }

        #endregion

        #region RTI

        private static void Op40Slow()
        {
            AddCycles(Constants.TWO_CYCLES);

            if (CheckEmulation() == 0)
            {
                Registers.PL = PullB();
                UnpackStatus();
                Registers.PCw = PullW();
                Registers.PB = PullB();
                Memory.Memory.OpenBus = Registers.PB;
                ICPU.ShiftedPB = (uint)(Registers.PB << 16);
            }
            else
            {
                Registers.PL = PullBE();
                UnpackStatus();
                Registers.PCw = PullWE();
                Memory.Memory.OpenBus = Registers.PCh;
                SetFlags((ushort)(Constants.MemoryFlag | Constants.IndexFlag));
            }

            Memory.Memory.SetPCBase(Registers.PBPC);
            if (CheckIndex() > 0)
            {
                Registers.XH = 0;
                Registers.YH = 0;
            }
            FixCycles();
        }

        #endregion

        #region STP/WAI

        // WAI
        static void OpCB()
        {
            CPUState.WaitingForInterrupt = true;

            Registers.PCw--;
            AddCycles(Constants.ONE_CYCLE);
        }

        // STP
        private static void OpDB()
        {
            Registers.PCw--;
            CPUState.Flags |= Constants.DEBUG_MODE_FLAG | Constants.HALTED_FLAG;
        }


        private static void Op42()
        {
            Memory.Memory.GetWord(Registers.PBPC);
            Registers.PCw++;
        }

        #endregion

        private static void Reschedule()
        {
            switch (CPUState.WhichEvent)
            {
                case HCEvents.HC_HBLANK_START_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_HDMA_START_EVENT;
                    CPUState.NextEvent = Timings.Timings.HDMAStart;
                    break;

                case HCEvents.HC_HDMA_START_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_HCOUNTER_MAX_EVENT;
                    CPUState.NextEvent = Timings.Timings.H_Max;
                    break;

                case HCEvents.HC_HCOUNTER_MAX_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_HDMA_INIT_EVENT;
                    CPUState.NextEvent = Timings.Timings.HDMAInit;
                    break;

                case HCEvents.HC_HDMA_INIT_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_RENDER_EVENT;
                    CPUState.NextEvent = Timings.Timings.RenderPos;
                    break;

                case HCEvents.HC_RENDER_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_WRAM_REFRESH_EVENT;
                    CPUState.NextEvent = Timings.Timings.WRAMRefreshPos;
                    break;

                case HCEvents.HC_WRAM_REFRESH_EVENT:
                    CPUState.WhichEvent = HCEvents.HC_HBLANK_START_EVENT;
                    CPUState.NextEvent = Timings.Timings.HBlankStart;
                    break;
            }
        }

     //   public static void DoHEventProcessing()
     //   {
	    //    switch (CPUState.WhichEvent)
	    //    {
		   //     case HCEvents.HC_HBLANK_START_EVENT:
			  //      Reschedule();
			  //      break;
		   //     case HCEvents.HC_HDMA_START_EVENT:
			  //      Reschedule();
			  //      if (PPU.SPPU.HDMA > 0 && CPUState.V_Counter <= PPU.SPPU.ScreenHeight)
			  //      {
     //                   PPU.SPPU.HDMA = DMA.DMA.DoHDMA(PPU.SPPU.HDMA);
     //               }
			  //      break;
		   //     case HCEvents.HC_HCOUNTER_MAX_EVENT:
     //               APUEndScanline();
     //               CPUState.Cycles -= Timings.Timings.H_Max;
			  //      if (Timings.Timings.NMITriggerPos != 0xffff)
				 //       Timings.Timings.NMITriggerPos -= Timings.Timings.H_Max;
			  //      if (Timings.Timings.NextIRQTimer != 0x0fffffff)
				 //       Timings.Timings.NextIRQTimer -= Timings.Timings.H_Max;
			  //      APUSetReferenceTime(CPUState.Cycles);

     //               CPUState.V_Counter++;
			  //      if (CPUState.V_Counter >= Timings.Timings.V_Max)	// V ranges from 0 to Timings.V_Max - 1
			  //      {
     //                   CPUState.V_Counter = 0;
     //                   Timings.Timings.InterlaceField = !Timings.Timings.InterlaceField;

				 //       // From byuu:
				 //       // [NTSC]
				 //       // interlace mode has 525 scanlines: 263 on the even frame, and 262 on the odd.
				 //       // non-interlace mode has 524 scanlines: 262 scanlines on both even and odd frames.
				 //       // [PAL] <PAL info is unverified on hardware>
				 //       // interlace mode has 625 scanlines: 313 on the even frame, and 312 on the odd.
				 //       // non-interlace mode has 624 scanlines: 312 scanlines on both even and odd frames.
				 //       if (IPPU.Interlace && !Timings.Timings.InterlaceField)
     //                       Timings.Timings.V_Max = Timings.Timings.V_Max_Master + 1;	// 263 (NTSC), 313?(PAL)
				 //       else
     //                       Timings.Timings.V_Max = Timings.Timings.V_Max_Master;		// 262 (NTSC), 312?(PAL)

     //                   Memory.Memory.ROM[Memory.Memory.FillRAMPosition + 0x213F] ^= 0x80;
				 //       PPU.RangeTimeOver = 0;

     //                   // FIXME: reading $4210 will wait 2 cycles, then perform reading, then wait 4 more cycles.
     //                   Memory.Memory.ROM[Memory.Memory.FillRAMPosition + 0x4210] = Model->_5A22;

				 //       ICPU.Frame++;
				 //       PPU.HVBeamCounterLatched = 0;
			  //      }

			  //      // From byuu:
			  //      // In non-interlace mode, there are 341 dots per scanline, and 262 scanlines per frame.
			  //      // On odd frames, scanline 240 is one dot short.
			  //      // In interlace mode, there are always 341 dots per scanline. Even frames have 263 scanlines,
			  //      // and odd frames have 262 scanlines.
			  //      // Interlace mode scanline 240 on odd frames is not missing a dot.
			  //      if (CPU.V_Counter == 240 && !IPPU.Interlace && Timings.InterlaceField)	// V=240
				 //       Timings.H_Max = Timings.H_Max_Master - ONE_DOT_CYCLE;	// HC=1360
			  //      else
				 //       Timings.H_Max = Timings.H_Max_Master;					// HC=1364

			  //      if (Model->_5A22 == 2)
			  //      {
				 //       if (CPU.V_Counter != 240 || IPPU.Interlace || !Timings.InterlaceField)	// V=240
				 //       {
					//        if (Timings.WRAMRefreshPos == SNES_WRAM_REFRESH_HC_v2 - ONE_DOT_CYCLE)	// HC=534
					//	        Timings.WRAMRefreshPos = SNES_WRAM_REFRESH_HC_v2;					// HC=538
					//        else
					//	        Timings.WRAMRefreshPos = SNES_WRAM_REFRESH_HC_v2 - ONE_DOT_CYCLE;	// HC=534
				 //       }
			  //      }
			  //      else
				 //       Timings.WRAMRefreshPos = SNES_WRAM_REFRESH_HC_v1;

			  //      if (CPU.V_Counter == PPU.ScreenHeight + FIRST_VISIBLE_LINE)	// VBlank starts from V=225(240).
			  //      {
				 //       S9xEndScreenRefresh();

     //   CPU.Flags |= SCAN_KEYS_FLAG;

				 //       PPU.HDMA = 0;
				 //       // Bits 7 and 6 of $4212 are computed when read in S9xGetPPU.
				 //       IPPU.MaxBrightness = PPU.Brightness;
				 //       PPU.ForcedBlanking = (Memory.FillRAM[0x2100] >> 7) & 1;

				 //       if (!PPU.ForcedBlanking)
				 //       {
					//        PPU.OAMAddr = PPU.SavedOAMAddr;

					//        uint8 tmp = 0;

					//        if (PPU.OAMPriorityRotation)
     //                           tmp = (PPU.OAMAddr & 0xFE) >> 1;
					//        if ((PPU.OAMFlip & 1) || PPU.FirstSprite != tmp)
					//        {
					//	        PPU.FirstSprite = tmp;
					//	        IPPU.OBJChanged = TRUE;
					//        }

					//        PPU.OAMFlip = 0;
				 //       }

				 //       // FIXME: writing to $4210 will wait 6 cycles.
				 //       Memory.Memory.RAM[.FillRAM[0x4210] = 0x80 | Model->_5A22;
				 //       if (Memory.FillRAM[0x4200] & 0x80)
				 //       {

     //                       // FIXME: triggered at HC=6, checked just before the final CPU cycle,
     //                       // then, when to call S9xOpcode_NMI()?
     //                       CPUState.NMIPending = true;
					//        Timings.NMITriggerPos = 6 + 6;
				 //       }
			  //      }

			  //      if (CPU.V_Counter == FIRST_VISIBLE_LINE)	// V=1
				 //       StartScreenRefresh();

     //               Reschedule();
			  //      break;
		   // case (int)HCEvents.HC_HDMA_INIT_EVENT:
			  //  Reschedule();
			  //  if (CPUState.V_Counter == 0)
     //               StartHDMA();

			  //  break;
		   // case (int)HCEvents.HC_RENDER_EVENT:
			  //  if (CPUState.V_Counter >= FIRST_VISIBLE_LINE && CPUState.V_Counter <= PPU.ScreenHeight)
				 //   RenderLine((byte) (CPUState.V_Counter - FIRST_VISIBLE_LINE));

			  //  Reschedule();
			  //  break;

		   // case (int) HCEvents.HC_WRAM_REFRESH_EVENT:
     //           CPUState.Cycles += SNES_WRAM_REFRESH_CYCLES;
			  //  Reschedule();
			  //  break;
	    //}
    }
}