using System;
using System.Runtime.InteropServices;

namespace SuperNintendo.Sound
{
    public enum WaveFormats
    {
        Pcm = 1,
        Float = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat
    {
        public Int16 shrWFormatTag;
        public Int16 shrNChannels;
        public Int32 intNSamplesPerSec;
        public Int32 intNAvgBytesPerSec;
        public Int16 shrNBlockAlign;
        public Int16 shrWBitsPerSample;
        public Int16 shrCbSize;

        public WaveFormat(Int32 intRate, Int32 intBits, Int32 intChannels)
        {
            shrWFormatTag = (Int16)WaveFormats.Pcm;
            shrNChannels = (Int16)intChannels;
            intNSamplesPerSec = intRate;
            shrWBitsPerSample = (Int16)intBits;
            shrCbSize = 0;
            shrNBlockAlign = (Int16)(intChannels * (intBits / 8));
            intNAvgBytesPerSec = intNSamplesPerSec * shrNBlockAlign;
        }
    }

    internal class WaveNative
    {
        // consts
        public const Int32 MMSYSERR_NOERROR = 0; // no error
        public const Int32 MM_WOM_OPEN = 0x3BB;
        public const Int32 MM_WOM_CLOSE = 0x3BC;
        public const Int32 MM_WOM_DONE = 0x3BD;
        public const Int32 CALLBACK_FUNCTION = 0x00030000;    // dwCallback is a FARPROC 
        public const Int32 TIME_MS = 0x0001;  // time in milliseconds 
        public const Int32 TIME_SAMPLES = 0x0002;  // number of wave samples 
        public const Int32 TIME_BYTES = 0x0004;  // current byte offset 

        // callbacks
        public delegate void WaveDelegate(IntPtr hdrvr, Int32 uMsg, Int32 dwUser, ref WaveHdr wavhdr, Int32 dwParam2);

        // structs 

        [StructLayout(LayoutKind.Sequential)]
        public struct WaveHdr
        {
            public IntPtr objLpData; // pointer to locked data buffer
            public Int32 intDwBufferLength; // length of data buffer
            public Int32 intDwBytesRecorded; // used for input only
            public IntPtr objDwUser; // for client's use
            public Int32 intDwFlags; // assorted flags (see defines)
            public Int32 intDwLoops; // loop control counter
            public IntPtr objLpNext; // PWaveHdr, reserved for driver
            public Int32 intReserved; // reserved for driver
        }

        private const String mmdll = "winmm.dll";

        // native calls
        [DllImport(mmdll)]
        public static extern Int32 waveOutGetNumDevs();
        [DllImport(mmdll)]
        public static extern Int32 waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, Int32 uSize);
        [DllImport(mmdll)]
        public static extern Int32 waveOutUnprepareHeader(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, Int32 uSize);
        [DllImport(mmdll)]
        public static extern Int32 waveOutWrite(IntPtr hWaveOut, ref WaveHdr lpWaveOutHdr, Int32 uSize);
        [DllImport(mmdll)]
        public static extern Int32 waveOutOpen(out IntPtr hWaveOut, Int32 uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, Int32 dwInstance, Int32 dwFlags);
        [DllImport(mmdll)]
        public static extern Int32 waveOutReset(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern Int32 waveOutClose(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern Int32 waveOutPause(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern Int32 waveOutRestart(IntPtr hWaveOut);
        [DllImport(mmdll)]
        public static extern Int32 waveOutGetPosition(IntPtr hWaveOut, out Int32 lpInfo, Int32 uSize);
        [DllImport(mmdll)]
        public static extern Int32 waveOutSetVolume(IntPtr hWaveOut, Int32 dwVolume);
        [DllImport(mmdll)]
        public static extern Int32 waveOutGetVolume(IntPtr hWaveOut, out Int32 dwVolume);
    }
}