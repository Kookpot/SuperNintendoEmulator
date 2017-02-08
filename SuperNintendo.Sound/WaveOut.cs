using System;
using System.Threading;
using System.Runtime.InteropServices;
namespace SuperNintendo.Sound
{
    internal class WaveOutHelper
    {
        public static void Try(Int32 intErr)
        {
            if (intErr != WaveNative.MMSYSERR_NOERROR) throw new Exception(intErr.ToString());
        }
    }

    public delegate void BufferFillEventHandler(IntPtr objData, Int32 intSize);

    internal class WaveOutBuffer : IDisposable
    {
        public WaveOutBuffer objNextBuffer;

        private AutoResetEvent objPlayEvent = new AutoResetEvent(false);
        private IntPtr objWaveOut;

        private WaveNative.WaveHdr objHeader;
        private Byte[] objHeaderData;
        private GCHandle objHeaderHandle;
        private GCHandle objHeaderDataHandle;

        private Boolean objPlaying;

        internal static void WaveOutProc(IntPtr objHdrvr, Int32 intMsg, Int32 intDwUser, ref WaveNative.WaveHdr objWavhdr, Int32 intDwParam2)
        {
            if (intMsg == WaveNative.MM_WOM_DONE)
            {
                try
                {
                    GCHandle objH = (GCHandle)objWavhdr.objDwUser;
                    WaveOutBuffer objBuf = (WaveOutBuffer)objH.Target;
                    objBuf.OnCompleted();
                }
                catch
                {
                }
            }
        }

        public WaveOutBuffer(IntPtr objWaveOutHandle, Int32 intSize)
        {
            objWaveOut = objWaveOutHandle;
            objHeaderHandle = GCHandle.Alloc(objHeader, GCHandleType.Pinned);
            objHeader.objDwUser = (IntPtr)GCHandle.Alloc(this);
            objHeaderData = new Byte[intSize];
            objHeaderDataHandle = GCHandle.Alloc(objHeaderData, GCHandleType.Pinned);
            objHeader.objLpData = objHeaderDataHandle.AddrOfPinnedObject();
            objHeader.intDwBufferLength = intSize;
            WaveOutHelper.Try(WaveNative.waveOutPrepareHeader(objWaveOut, ref objHeader, Marshal.SizeOf(objHeader)));
        }

        ~WaveOutBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (objHeader.objLpData != IntPtr.Zero)
            {
                WaveNative.waveOutUnprepareHeader(objWaveOut, ref objHeader, Marshal.SizeOf(objHeader));
                objHeaderHandle.Free();
                objHeader.objLpData = IntPtr.Zero;
            }
            objPlayEvent.Close();
            if (objHeaderDataHandle.IsAllocated)
            {
                objHeaderDataHandle.Free();
            }
            GC.SuppressFinalize(this);
        }

        public int Size
        {
            get { return objHeader.intDwBufferLength; }
        }

        public IntPtr Data
        {
            get { return objHeader.objLpData; }
        }

        public Boolean Play()
        {
            lock(this)
            {
                objPlayEvent.Reset();
                objPlaying = WaveNative.waveOutWrite(objWaveOut, ref objHeader, Marshal.SizeOf(objHeader)) == WaveNative.MMSYSERR_NOERROR;
                return objPlaying;
            }
        }
        public void WaitFor()
        {
            if (objPlaying)
            {
                objPlaying = objPlayEvent.WaitOne();
            }
            else
            {
                Thread.Sleep(0);
            }
        }
        public void OnCompleted()
        {
            objPlayEvent.Set();
            objPlaying = false;
        }
    }

    public class WaveOutPlayer : IDisposable
    {
        private IntPtr objWaveOut;
        private WaveOutBuffer objBuffers; // linked list
        private WaveOutBuffer objCurrentBuffer;
        private Thread objThread;
        private BufferFillEventHandler objFillProc;
        private Boolean blnFinished;
        private Byte objZero;

        private WaveNative.WaveDelegate objBufferProc = new WaveNative.WaveDelegate(WaveOutBuffer.WaveOutProc);

        public static Int32 DeviceCount
        {
            get { return WaveNative.waveOutGetNumDevs(); }
        }

        public WaveOutPlayer(Int32 intDevice, WaveFormat objFormat, Int32 intBufferSize, Int32 intBufferCount, BufferFillEventHandler objFillProc)
        {
            objZero = objFormat.shrWBitsPerSample == 8 ? (Byte)128 : (Byte)0;
            objFillProc = objFillProc;
            WaveOutHelper.Try(WaveNative.waveOutOpen(out objWaveOut, intDevice, objFormat, objBufferProc, 0, WaveNative.CALLBACK_FUNCTION));
            AllocateBuffers(intBufferSize, intBufferCount);
            objThread = new Thread(new ThreadStart(ThreadProc));
            objThread.Start();
        }

        ~WaveOutPlayer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (objThread != null)
            {
                try
                {
                    blnFinished = true;
                    if (objWaveOut != IntPtr.Zero)
                    {
                        WaveNative.waveOutReset(objWaveOut);
                    }
                    objThread.Join();
                    objFillProc = null;
                    FreeBuffers();
                    if (objWaveOut != IntPtr.Zero)
                    {
                        WaveNative.waveOutClose(objWaveOut);
                    }
                }
                finally
                {
                    objThread = null;
                    objWaveOut = IntPtr.Zero;
                }
            }
        }

        private void ThreadProc()
        {
            while (!blnFinished)
            {
                Advance();
                if (objFillProc != null && !blnFinished)
                {
                    objFillProc(objCurrentBuffer.Data, objCurrentBuffer.Size);
                }
                else
                {
                    // zero out buffer
                    Byte bteV = objZero;
                    Byte[] arrB = new Byte[objCurrentBuffer.Size];
                    for (Int32 intI = 0; intI < arrB.Length; intI++)
                    {
                        arrB[intI] = bteV;
                    }
                    Marshal.Copy(arrB, 0, objCurrentBuffer.Data, arrB.Length);
                }
                objCurrentBuffer.Play();
            }
            WaitForAllBuffers();
        }

        private void AllocateBuffers(Int32 intBufferSize, Int32 intBufferCount)
        {
            FreeBuffers();
            if (intBufferCount > 0)
            {
                objBuffers = new WaveOutBuffer(objWaveOut, intBufferSize);
                WaveOutBuffer Prev = objBuffers;
                try
                {
                    for (Int32 intI = 1; intI < intBufferCount; intI++)
                    {
                        var objBuf = new WaveOutBuffer(objWaveOut, intBufferSize);
                        Prev.objNextBuffer = objBuf;
                        Prev = objBuf;
                    }
                }
                finally
                {
                    Prev.objNextBuffer = objBuffers;
                }
            }
        }

        private void FreeBuffers()
        {
            objCurrentBuffer = null;
            if (objBuffers != null)
            {
                WaveOutBuffer First = objBuffers;
                objBuffers = null;
                WaveOutBuffer Current = First;
                do
                {
                    WaveOutBuffer objNext = Current.objNextBuffer;
                    Current.Dispose();
                    Current = objNext;
                } while(Current != First);
            }
        }

        private void Advance()
        {
            objCurrentBuffer = objCurrentBuffer == null ? objBuffers : objCurrentBuffer.objNextBuffer;
            objCurrentBuffer.WaitFor();
        }

        private void WaitForAllBuffers()
        {
            WaveOutBuffer objBuf = objBuffers;
            while (objBuf.objNextBuffer != objBuffers)
            {
                objBuf.WaitFor();
                objBuf = objBuf.objNextBuffer;
            }
        }
    }
}