using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace SuperNintendo.Core.Sound
{
    public static class Sound
    {
        private static bool initDone = false;
        private static XAudio2 _audio;
        private static MasteringVoice _masterVoice;
        private static SourceVoice _sourceVoice;
        private static WaveFormat _format;

        public static void ReInitSound()
        {
            DeInitSound();
            InitSound(Constants.SoundBufferSize, 0);
        }

        private static void InitSound(int buffer_ms, int lag_ms)
        {
            // buffer_ms : buffer size given in millisecond
            // lag_ms    : allowable time-lag given in millisecond

            var sample_count = buffer_ms * 32040 / 1000;
            var lag_sample_count = lag_ms * 32040 / 1000;

            SPC.lag_master = lag_sample_count << 1;
            SPC.lag = SPC.lag_master;

            if (sample_count < Constants.APU_MINIMUM_SAMPLE_COUNT)
                sample_count = Constants.APU_MINIMUM_SAMPLE_COUNT;

            SPC.buffer_size = sample_count << 2;
            MSU.buffer_size = (int)((sample_count << 2) * 1.5); // Always 16-bit, Stereo; 1.5 to never overflow before dsp buffer

            SPC.landing_buffer = new byte[SPC.buffer_size * 2];
            MSU.landing_buffer = new byte[MSU.buffer_size * 2];

            /* The resampler and spc unit use samples (16-bit short) as
               arguments. Use 2x in the resampler for buffer leveling with SoundSync */
            if (!spc::resampler)
            {
                spc::resampler = new HermiteResampler(spc::buffer_size >> (Settings.SoundSync ? 0 : 1));
                if (!spc::resampler)
                {
                    delete[] spc::landing_buffer;
                    return (FALSE);
                }
            }
            else
                spc::resampler->resize(spc::buffer_size >> (Settings.SoundSync ? 0 : 1));


            if (!msu::resampler)
            {
                msu::resampler = new HermiteResampler(msu::buffer_size);
                if (!msu::resampler)
                {
                    delete[] msu::landing_buffer;
                    return (FALSE);
                }
            }
            else
                msu::resampler->resize(msu::buffer_size);


            SNES::dsp.spc_dsp.set_output((SNES::SPC_DSP::sample_t*)spc::landing_buffer, spc::buffer_size);

            UpdatePlaybackRate();

            spc::sound_enabled = S9xOpenSoundDevice();

            return (spc::sound_enabled);
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

        private static void DeInitVoices()
        {
            StopPlayback();
        }

        private static void StopPlayback()
        {
            _sourceVoice.Stop();
        }
    }
}