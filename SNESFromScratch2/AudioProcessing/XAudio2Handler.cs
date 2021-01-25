using System;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace SNESFromScratch2.AudioProcessing
{
    public class XAudio2Handler : IAudioHandler
    {
        public float[] SampleBufferL { get; set; } = new float[735];
        public float[] SampleBufferR { get; set; } = new float[735];

        private readonly float[] _inputBufferL = new float[4096];
        private readonly float[] _inputBufferR = new float[4096];
        private int _inputBufferPos;
        private int _inputReadPos;
        private DataStream _dataStream;
        private readonly SourceVoice _sourceVoice;
        private readonly float[] _inputBuffer = new float[4096];

        public XAudio2Handler()
        {
            var xaudio2 = new XAudio2();
            new MasteringVoice(xaudio2);
            var waveFormat = new WaveFormat(44100, 32, 2);
            _sourceVoice = new SourceVoice(xaudio2, waveFormat);

            _dataStream = DataStream.Create(_inputBuffer, true, true);
            var audioBuffer = new AudioBuffer { Stream = _dataStream, Flags = BufferFlags.EndOfStream, AudioBytes = (int) _dataStream.Length };
            _sourceVoice.SubmitSourceBuffer(audioBuffer, null);
            _sourceVoice.BufferEnd += Process;
            _sourceVoice.Start();
            Process(new IntPtr(0));
        }

        private void Process(IntPtr e)
        {
            if (_inputReadPos + 2048 > _inputBufferPos)
            {
                _inputReadPos = _inputBufferPos - 2048;
            }
            if (_inputReadPos + 4096 < _inputBufferPos)
            {
                _inputReadPos += 2048;
            }
            for (var i = 0; i < 2048; i++)
            {
                _inputBuffer[i * 2] = _inputBufferL[_inputReadPos & 0xfff];
                _inputBuffer[i * 2 + 1] = _inputBufferR[_inputReadPos & 0xfff];
                _inputReadPos++;
            }
            _dataStream = DataStream.Create(_inputBuffer, true, true);
            var audioBuffer = new AudioBuffer { Stream = _dataStream, Flags = BufferFlags.EndOfStream, AudioBytes = (int)_dataStream.Length };
            _sourceVoice.SubmitSourceBuffer(audioBuffer, null);
        }

        public void NextBuffer()
        {
            for (var i = 0; i < 735; i++)
            {
                var valL = SampleBufferL[i];
                var valR = SampleBufferR[i];
                _inputBufferL[_inputBufferPos & 0xfff] = valL;
                _inputBufferR[_inputBufferPos & 0xfff] = valR;
                _inputBufferPos++;
            }
        }

        public void Pauze()
        {
            _sourceVoice.Stop(PlayFlags.None);
        }

        public void Resume()
        {
            _sourceVoice.Start();
        }
    }
}
