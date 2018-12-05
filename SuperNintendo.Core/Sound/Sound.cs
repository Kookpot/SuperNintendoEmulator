using SlimDX.Multimedia;
using SlimDX.XAudio2;
using System;

namespace SuperNintendo.Core.Sound
{
    public static class Sound
    {
        private static bool initDone;
        private static long bufferCount;              // currently submitted XAudio2 buffers

        private static uint sum_bufferSize;                  // the size of soundBuffer
        private static uint singleBufferSamples;             // samples in one block
        private static uint singleBufferBytes;               // bytes in one block
        private static uint blockCount;                      // soundBuffer is divided into blockCount blocks
        private static uint writeOffset;                     // offset into the buffer for the next block
        private static byte[] soundBuffer;						// the buffer itself

        private static XAudio2 _audio;
        private static MasteringVoice _masterVoice;
        private static SourceVoice _sourceVoice;
        private static WaveFormat _format;

        public static void ReInitSound()
        {
            DeInitSound();
            InitSound(Constants.SoundBufferSize, 0);
        }

        public static void InitSound(int buffer_ms, int lag_ms)
        {
            // buffer_ms : buffer size given in millisecond
            // lag_ms    : allowable time-lag given in millisecond

            var sample_count = buffer_ms * 32040 / 1000;
            var lag_sample_count = lag_ms * 32040 / 1000;

            SPC.lag_master = lag_sample_count << 1;
            SPC.lag = SPC.lag_master;

            SPC.buffer_size = sample_count << 2;
            MSU.buffer_size = (int)((sample_count << 2) * 1.5); // Always 16-bit, Stereo; 1.5 to never overflow before dsp buffer

            SPC.landing_buffer = new byte[SPC.buffer_size * 2];
            MSU.landing_buffer = new byte[MSU.buffer_size * 2];

            /* The resampler and spc unit use samples (16-bit short) as
               arguments. Use 2x in the resampler for buffer leveling with SoundSync */
            if (SPC.resampler == null)
                SPC.resampler = new HermiteResampler(SPC.buffer_size);
            else
                SPC.resampler.Resize(SPC.buffer_size);


            if (MSU.resampler == null)
            {
                MSU.resampler = new HermiteResampler(MSU.buffer_size);
            }
            else
                MSU.resampler.Resize(MSU.buffer_size);

            SPC_DSP.SetOutput(SPC.landing_buffer, SPC.buffer_size);
            UpdatePlaybackRate();

            SPC.sound_enabled = OpenSoundDevice();
        }

        private static void UpdatePlaybackRate()
        {
            //if (Settings.SoundInputRate == 0)
            //    Settings.SoundInputRate = APU_DEFAULT_INPUT_RATE;

            double time_ratio = (double)Constants.SoundInputRate * SPC.timing_hack_numerator / (Constants.SoundPlaybackRate * SPC.timing_hack_denominator);

            //if (Settings.DynamicRateControl)
            //{
            //    time_ratio *= spc::dynamic_rate_multiplier;
            //}

            SPC.resampler.SetTimeRatio(time_ratio);

            //if (Settings.MSU1)
            //{
            //    time_ratio = (44100.0 / Settings.SoundPlaybackRate) * (Settings.SoundInputRate / 32040.0);
            //    msu::resampler->time_ratio(time_ratio);
            //}
        }

        private static bool OpenSoundDevice()
        {
            SetSamplesAvailableCallback(null, null);
            // point the interface to the correct output object

            if (!InitSoundOutput())
                return false;

            if (!SetupSound())
                return false;

            SetSamplesAvailableCallback(SoundCallback, null);
            return true;
        }

        private static void SetSamplesAvailableCallback(Action callback, Action data)
        {
            SPC.sa_callback = callback;
            SPC.extra_data = data;
        }

        private static void SoundCallback()
        {
            //static double last_volume = 1.0;

            //// only try to change volume if we actually need to switch it
            //double current_volume = (Settings.TurboMode ? GUI.VolumeTurbo : GUI.VolumeRegular) / 100.;
            //if (last_volume != current_volume)
            //{
            //    SetVolume(current_volume);
            //    last_volume = current_volume;
            //}

            ProcessSound();
        }

        private static void ProcessSound()
        {
            int freeBytes = (int)((blockCount - bufferCount) * singleBufferBytes);

            //if (Settings.DynamicRateControl)
            //{
            //    UpdateDynamicRate(freeBytes, sum_bufferSize);
            //}

            FinalizeSamples();

            //UINT32 availableSamples;

            //availableSamples = GetSampleCount();

            //if (Settings.DynamicRateControl)
            //{
            //    // Using rate control, we should always keep the emulator's sound buffers empty to
            //    // maintain an accurate measurement.
            //    if (availableSamples > (freeBytes >> (Settings.SixteenBitSound ? 1 : 0)))
            //    {
            //        S9xClearSamples();
            //        return;
            //    }
            //}

            //if (!initDone)
            //    return;

            //BYTE* curBuffer;

            //while (availableSamples > singleBufferSamples && bufferCount < blockCount)
            //{
            //    curBuffer = soundBuffer + writeOffset;
            //    S9xMixSamples(curBuffer, singleBufferSamples);
            //    PushBuffer(singleBufferBytes, curBuffer, NULL);
            //    writeOffset += singleBufferBytes;
            //    writeOffset %= sum_bufferSize;
            //    availableSamples -= singleBufferSamples;
            //}
        }

        private static void FinalizeSamples()
        {
            bool drop_current_msu1_samples = true;

            drop_current_msu1_samples = false;

            //if (!spc::resampler->push((short*)spc::landing_buffer, SNES::dsp.spc_dsp.sample_count()))
            //{
            //    /* We weren't able to process the entire buffer. Potential overrun. */
            //    spc::sound_in_sync = FALSE;

            //    if (Settings.SoundSync && !Settings.TurboMode)
            //        return;

            //    // since we drop the current dsp samples we also want to drop generated msu1 samples
            //    drop_current_msu1_samples = true;
            //}

            //// only generate msu1 if we really consumed the dsp samples (sample_count() resets at end of function),
            //// otherwise we will generate multiple times for the same samples - so this needs to be after all early
            //// function returns
            //if (Settings.MSU1)
            //{
            //    // generate the same number of msu1 samples as dsp samples were generated
            //    S9xMSU1SetOutput((int16*)msu::landing_buffer, msu::buffer_size);
            //    S9xMSU1Generate(SNES::dsp.spc_dsp.sample_count());
            //    if (!drop_current_msu1_samples && !msu::resampler->push((short*)msu::landing_buffer, S9xMSU1Samples()))
            //    {
            //        // should not occur, msu buffer is larger and we drop msu samples if spc buffer overruns
            //        assert(0);
            //    }
            //}


            //if (!Settings.SoundSync || Settings.TurboMode || Settings.Mute)
            //    spc::sound_in_sync = TRUE;
            //else
            //if (spc::resampler->space_empty() >= spc::resampler->space_filled())
            //    spc::sound_in_sync = TRUE;
            //else
            //    spc::sound_in_sync = FALSE;

            //SNES::dsp.spc_dsp.set_output((SNES::SPC_DSP::sample_t*)spc::landing_buffer, spc::buffer_size);

        }
        private static bool InitSoundOutput()
        {
            if (initDone)
                return true;

            _audio = new XAudio2(0, ProcessorSpecifier.DefaultProcessor);
            initDone = true;
            return true;
        }

        private static bool SetupSound()
        {
            if (!initDone)
                return false;

            DeInitVoices();

            blockCount = 8;
            var blockTime = Constants.SoundBufferSize / blockCount;

            singleBufferSamples = 2 * (Constants.SoundPlaybackRate * blockTime / 1000);
            singleBufferBytes = 2 * singleBufferSamples;
            sum_bufferSize = singleBufferBytes * blockCount;

            InitVoices();
            soundBuffer = new byte[sum_bufferSize];
            writeOffset = 0;

            bufferCount = 0;

            BeginPlayback();
            return true;
        }

        //public void SetVolume(float volume)
        //{
        //    _masterVoice.Volume = volume;
        //}

        //public void InitSound()
        //{
        //    _masterVoice = new MasteringVoice(_audio, 2, 44100); //2 because of stereo
        //    var t = _masterVoice.Volume;
        //    _format = new WaveFormat
        //    {
        //        FormatTag = WaveFormatTag.Pcm,
        //        Channels = 2,
        //        SamplesPerSecond = 44100,
        //        BlockAlignment = 4, //16bit sound * stereo
        //        BitsPerSample = 16,
        //        AverageBytesPerSecond = 44100 * 4 //SamplesperSecond * BlockAlignment
        //    };
        //    _sourceVoice = new SourceVoice(_audio, _format, VoiceFlags.NoSampleRateConversion, 2.0f);
        //    _sourceVoice.Start(PlayFlags.None);
        //}

        //public void PushBuffer(Stream stream)
        //{
        //    var buffer = new AudioBuffer
        //    {
        //        AudioBytes = (int)stream.Length,
        //        AudioData = stream,
        //        Flags = BufferFlags.EndOfStream
        //    };
        //    _sourceVoice.SubmitSourceBuffer(buffer);
        //    //_sourceVoice.Start(PlayFlags.None);
        //}

        private static void DeInitSound()
        {
            initDone = false;
            DeInitVoices();
        }

        private static void InitVoices()
        {
            _masterVoice = new MasteringVoice(_audio, 2, Constants.SoundPlaybackRate);
            _format = new WaveFormat
            {
                FormatTag = WaveFormatTag.Pcm,
                Channels = 2,
                SamplesPerSecond = Constants.SoundPlaybackRate,
                BlockAlignment = 4, //16bit sound * stereo
                BitsPerSample = 16,
                AverageBytesPerSecond = Constants.SoundPlaybackRate * 4 //SamplesperSecond * BlockAlignment
            };
            _sourceVoice = new SourceVoice(_audio, _format, VoiceFlags.NoSampleRateConversion, 2.0f);
        }

        private static void DeInitVoices()
        {
            if (_sourceVoice != null)
                StopPlayback();
        }

        private static void BeginPlayback()
        {
            _sourceVoice.Start(PlayFlags.None);
        }

        private static void StopPlayback()
        {
            _sourceVoice.Stop();
        }
    }
}