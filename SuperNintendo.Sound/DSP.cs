using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperNintendo.Sound
{
    /// <summary>
    /// DSP
    /// </summary>
    public class DSP
    {
        #region Members

        private const Int32 _intMaxSize = 2048;
        private Double[] _arrCos = new Double[_intMaxSize];
        private Double[] _arrSin = new Double[_intMaxSize];
        private Byte[] _arrRegisters = new Byte[128];
        private Byte _bteKeyedChannels;
        private Channel[] _arrChannels = new Channel[8];
        private Int16 _shrMasterVolumeLeft;
        private Int16 _shrMasterVolumeRight;
        private Int16 _shrEchoVolumeLeft;
        private Int16 _shrEchoVolumeRight;
        private Int32 _intEchoEnable;
        private Int32 _intEchoFeedback;
        private Int32 _intEchoPtr;
        private Int32 _intEchoBufferSize;
        private Int32 _intEchoWriteEnabled;
        private Int32 _intEchoChannelEnable;
        private Int32 _intPitchMod;
        private Int32[] _arrDummy = new Int32[3];
        private Boolean _blnNoFilter;
        private Int32[] _arrMasterVolume = new Int32[2];
        private Int32[] _arrEchoVolume = new Int32[2];
        private Int32 _intNoiseHertz;

        private Int32 _intSoundFD;
        private Int32 _intSoundSwitch;
        private Int32 _intPlaybackRate;
        private Int32 _intBufferSize;
        private Int32 _intNoiseGen;
        private Boolean _blnMuteSound;
        private Int32 _intStereo;
        private Boolean _blnSixteenBit;
        private Boolean _blnEncoded;
        private Int32 _intSamplesPerMix;
        private Int32 _intSamplesMixedSoFar;
        private Int32 _intPlayPosition;
        private Int32 _intErrCounter;
        private Int32 _intErrRate;
        private Int32 _intScanline;

        private Int32[] _arrEcho = new Int32[24000];
        private Int32[] _arrDummyEchoBuffer = new Int32[16384];
        private Int32[] _arrMixBuffer = new Int32[16384];
        private Int32[] _arrEchoBuffer = new Int32[16384];
        private Int32[] _arrFilterTaps = new Int32[8];
        private Int64 _lngZ;
        private Int32[] _arrLoop = new Int32[16];
        private Int64[,] _arrFilterValues  = new Int64[4,2];
        private Int32[] _arrNoiseFreq = new Int32[32];

        private Int32[] _arrAttackRate = new Int32[] { 4100, 2600, 1500, 1000, 640, 380, 260, 160, 96, 64, 40, 24, 16, 10, 6, 1 };
        private Int32[] _arrDecayRate = new Int32[] { 1200, 740, 440, 290, 180, 110, 74, 37 };
        private Int32[] _arrSustainRate = new Int32[] { 0, 38000, 28000, 24000, 19000, 14000, 12000, 9400, 7100, 5900, 4700, 3500, 2900, 2400, 1800, 1500, 1200, 880, 740, 590, 440, 370, 290, 220, 180, 150, 110, 92, 74, 55, 37, 18 };
        private Int32[] _arrIncreaseRate = new Int32[] { 0, 4100, 3100, 2600, 2000, 1500, 1300, 1000, 770, 640, 510, 380, 320, 260, 190, 160, 130, 96, 80, 64, 48, 40, 32, 24, 20, 16, 12, 10, 8, 6, 4, 2 };
        private Int32[] _arrDecreaseRateExp = new Int32[] { 0, 38000, 28000, 24000, 19000, 14000, 12000, 9400, 7100, 5900, 4700, 3500, 2900, 2400, 1800, 1500, 1200, 880, 740, 590, 440, 370, 290, 220, 180, 150, 110, 92, 74, 55, 37, 18 };

        #endregion

        #region Constructors

        /// <summary>
        /// constructor
        /// </summary>
        public DSP()
        {
            _arrChannels = new Channel[8];
            _arrRegisters = new Byte[128];
            _arrRegisters[0x7c] = 0;
            _arrRegisters[0x5c] = 0;
            _arrRegisters[0x4c] = 0;
            _arrRegisters[0x6c] = 0x40 | 0x20;
            _bteKeyedChannels = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// prepare
        /// </summary>
        public void Prepare(Byte[] arrTempRegisters, ref Byte[] arrRam)
        {
            _intSamplesPerMix = 2 * 44100 / 100;
            _intPlaybackRate = 44100;
            _intErrRate = (Int32)((63.49e-6) * 0x10000UL / (1.0 / _intPlaybackRate));
            _intBufferSize = _intSamplesPerMix*2;
            _intStereo = 0;
            _blnSixteenBit = true;
            _blnEncoded = false;
            _blnMuteSound = false;

            Byte objMask = 1;
            for (Int32 intI = 0; intI < 8; intI++, objMask <<= 1)
            {
                _arrChannels[intI] = new Channel();
            }
            SetEchoDelay(_arrRegisters[0x7d] & 0xf);
            _arrRegisters = arrTempRegisters;
            blnSetSoundMute(false);
            FixSoundAfterSnapshotLoad();
            SetFrequencyModulationEnable(_arrRegisters[0x2d]);
            SetMasterVolume(_arrRegisters[0x0c], _arrRegisters[0x1c]);
            SetEchoVolume(_arrRegisters[0x2c], _arrRegisters[0x3c]);

            Int32 intType;
            for (Int32 intI = 0; intI < 8; intI++, objMask <<= 1) 
            {
                Channel objCh = _arrChannels[intI];
                FixEnvelope(intI, _arrRegisters[0x07 + (intI << 4)], _arrRegisters[0x05 + (intI << 4)], _arrRegisters[0x06 + (intI << 4)]);
                SetSoundVolume(intI, _arrRegisters[0x00 + (intI << 4)], _arrRegisters[0x01 + (intI << 4)]);
                SetSoundFrequency(intI, ((_arrRegisters[0x02 + (intI << 4)] + (_arrRegisters[0x03 + (intI << 4)] << 8)) & 0x3fff) * 8);
                if ((_arrRegisters[0x3d] & objMask) > 0)
                {
                    intType = 1;
                }
                else
                {
                    intType = 0;
                }
                SetSoundType(intI, intType);
                if ((_arrRegisters[0x4c] & objMask) != 0)
	            {
                    _bteKeyedChannels |= objMask;
                    PlaySample(intI,ref arrRam);
	            }
            }
            var bits = 8;
            var channels = 1;
            var bufferSize = 256;
            var waveFormat = new WaveFormat(_intPlaybackRate, bits, channels);
            //TODOvar waveOutPlayer = new WaveOutPlayer(-1, waveFormat, bufferSize, 1, Filler);
        }

        /// <summary>
        /// set dsp
        /// </summary>
        /// <param name="bteByte">byte to set</param>
        public void SetByte(Byte bteByte, Byte btePos, ref Byte[] arrRam)
        {
            Byte bteKeyOn = 0;
            Byte bteKeyOnPrev = 0;

            switch (btePos & 0x0f)
            {
                case 0:
                    SetSoundVolume(btePos >> 4, bteByte, _arrRegisters[btePos + 1]);
                    break;
                case 1:
                    SetSoundVolume(btePos >> 4, _arrRegisters[btePos - 1], bteByte);
                    break;
                case 2:
                    SetSoundHertz(btePos >> 4, ((bteByte + (_arrRegisters[btePos + 1] << 8)) & 0x3fff) * 8);
                    break;
                case 3:
                    SetSoundHertz(btePos >> 4, (((bteByte << 8) + _arrRegisters[btePos - 1]) & 0x3fff) * 8);
                    break;
                case 4:
                    break;
                case 5:
                    if (bteByte != _arrRegisters[btePos])
                    {
                        {
                            FixEnvelope(btePos >> 4, _arrRegisters[btePos + 2], bteByte, _arrRegisters[btePos + 1]);
                        }
                    }
                    break;
                case 6:
                    if (bteByte != _arrRegisters[btePos])
                    {
                        {
                            FixEnvelope(btePos >> 4, _arrRegisters[btePos + 1], _arrRegisters[btePos - 1], bteByte);
                        }
                    }
                    break;
                case 7:
                    if (bteByte != _arrRegisters[btePos])
                    {
                        {
                            FixEnvelope(btePos >> 4, bteByte, _arrRegisters[btePos - 2], _arrRegisters[btePos - 1]);
                        }
                    }
                    break;
                case 12:
                    switch (btePos >> 4)
                    {
                        case 0:
                            //0x0c
                            if (bteByte != _arrRegisters[0x0c])
                            {
                                SetMasterVolume(bteByte, _arrRegisters[0x1c]);
                            }
                            break;
                        case 1:
                            //0x1c
                            if (bteByte != _arrRegisters[0x1c])
                            {
                                SetMasterVolume(_arrRegisters[0x0c], bteByte);
                            }
                            break;
                        case 2:
                            //0x2c
                            if (bteByte != _arrRegisters[0x2c])
                            {
                                SetEchoVolume(bteByte, _arrRegisters[0x3c]);
                            }
                            break;
                        case 3:
                            //0x3c
                            if (bteByte != _arrRegisters[0x3c])
                            {
                                SetEchoVolume(_arrRegisters[0x2c], bteByte);
                            }
                            break;
                        case 4:
                            Byte bteMask = 1;
                            for (Int32 intC = 0; intC < 8; intC++, bteMask <<= 1)
                            {
                                if ((bteByte & bteMask) != 0)
                                {
                                    if ((_arrRegisters[0x5c] & bteMask) == 0)
                                    {
                                        bteKeyOnPrev &= (Byte)(~bteMask);
                                        _bteKeyedChannels |= bteMask;
                                        _arrRegisters[0x7c] &= (Byte)(~bteMask);
                                        PlaySample(intC, ref arrRam);
                                    }
                                    else 
                                    {
                                        bteKeyOn |= bteMask;
                                    }
                                }
                            }
                            return;
                        case 5:
                            Byte bteMask2 = 1;
                            for (Int32 intC = 0; intC < 8; intC++, bteMask2 <<= 1)
                            {
                                if ((bteByte & bteMask2) != 0)
                                {
                                    if ((_bteKeyedChannels & bteMask2) > 0)
                                    {
                                        {
                                            bteKeyOnPrev &= (Byte)(~bteMask2);
                                            _bteKeyedChannels &= (Byte)(~bteMask2);
                                            _arrRegisters[0x4c] &= (Byte)(~bteMask2);
                                            SetSoundKeyOff(intC);
                                        }
                                    }
                                }
                                else if ((bteKeyOnPrev & bteMask2) != 0)
                                {
                                    bteKeyOnPrev &= (Byte)(~bteMask2);
                                    _bteKeyedChannels |= bteMask2;
                                    _arrRegisters[0x5c] &= (Byte)(~bteMask2);
                                    _arrRegisters[0x7c] &= (Byte)(~bteMask2);
                                    PlaySample(intC, ref arrRam);
                                }
                            }
                            _arrRegisters[0x5c] = bteByte;
                            return;
                        case 6:
                            //0x6c
                            if ((bteByte & 0x80) > 0)
                            {
                                _arrRegisters[btePos] = (Byte)(0x40 | 0x20 | (bteByte & 0x1f));
                                _arrRegisters[0x7c] = 0;
                                _arrRegisters[0x5c] = 0;
                                _arrRegisters[0x4c] = 0;
                                SetEchoWriteEnable(false);
                                ResetSound(false);
                            }
                            else
                            {
                                SetEchoWriteEnable(!((bteByte & 0x20) > 0));
                                if ((bteByte & 0x40) > 0)
                                {
                                    blnSetSoundMute(true);
                                }
                                else
                                {
                                    blnSetSoundMute(false);
                                }
                                _intNoiseHertz = _arrNoiseFreq[bteByte & 0x1f];
                                for (Int32 intI2 = 0; intI2 < 8; intI2++)
                                {
                                    if (_arrChannels[intI2].Type == 1)
                                    {
                                        SetSoundFrequency(intI2, _intNoiseHertz);
                                    }
                                }
                            }
                            break;
                        case 7:
                            //0x7c
                            bteByte = 0;
                            break;
                        default:
                            break;
                    }
                    break;
                case 13:
                    switch (btePos >> 4)
                    {
                        case 0:
                            SetEchoFeedback(bteByte);
                            break;
                        case 2:
                            if (bteByte != _arrRegisters[0x2d])
                            {
                                SetFrequencyModulationEnable(bteByte);
                            }
                            break;
                        case 3:
                            //0x3d
                            if (bteByte != _arrRegisters[0x3d])
                            {
                                Byte bteMask = 1;
                                for (Int32 intC = 0; intC < 8; intC++, bteMask <<= 1)
                                {
                                    Int32 intType;
                                    if ((bteByte & bteMask) > 0)
                                    {
                                        intType = 1;
                                    }
                                    else
                                    {
                                        intType = 0;
                                    }
                                    SetSoundType(intC, intType);
                                }
                            }
                            break;
                        case 4:
                            if (bteByte != _arrRegisters[0x4d])
                            {
                                SetEchoEnable(bteByte);
                            }
                            break;
                        case 7:
                            SetEchoDelay(bteByte & 0xf);
                            break;
                        default:
                            break;
                    }
                    break;
                case 15:
                    SetFilterCoefficient(btePos >> 4, bteByte);
                    break;
                default:
                    break;
            }
            bteKeyOnPrev |= bteKeyOn;
            bteKeyOn = 0;
            if (btePos < 0x80)
            {
                _arrRegisters[btePos] = bteByte;
            }
        }

        /// <summary>
        /// mix samples
        /// </summary>
        /// <param name="arrRam"></param>
        public Int32[] MixSamples(ref Byte[] arrRam)
        {
            Int32[] arrBuffer = new Int32[_intSamplesPerMix];
            Int32 intJ;
            Int32 intI;
	
            if (!_blnMuteSound)
            {
                _arrMixBuffer= new Int32[_intSamplesPerMix];
		        if (_intEchoEnable>0)
                {
			        _arrEchoBuffer = new Int32[_intSamplesPerMix];
                }
                if (_intStereo > 0)
                {
                    MixStereo(_intSamplesPerMix, ref arrRam);
                }
                else
                {
                    MixMono(_intSamplesPerMix, ref arrRam);
                }
            }
	
            /* Mix and convert waveforms */
            if (_blnSixteenBit)
            {
                Int32 intByteCount = _intSamplesPerMix << 1;
                // 16-bit sound
                if (_blnMuteSound)
                {
                    //TODOmemset (buffer, 0, intByteCount);
                }
                else
                {
                    if (_intEchoEnable > 0 && _intEchoBufferSize > 0)
                    {
                        if (_intStereo > 0)
                        {
                            // 16-bit stereo sound with echo enabled ...
                            if (_blnNoFilter)
                            {
                                // ... but no filter defined.
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];
                                    _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1] + intE * _arrEchoVolume[intJ & 1]) / 0x0080;
                                    intI = intCLIP16(intI);
                                    arrBuffer[intJ] = intI;
                                }
                            }
                            else
                            {
                                // ... with filter defined.
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];
                                    _arrLoop[(_lngZ - 0) & 15] = intE;
                                    intE = intE * _arrFilterTaps[0];
                                    intE += _arrLoop[(_lngZ - 2) & 15] * _arrFilterTaps[1];
                                    intE += _arrLoop[(_lngZ - 4) & 15] * _arrFilterTaps[2];
                                    intE += _arrLoop[(_lngZ - 6) & 15] * _arrFilterTaps[3];
                                    intE += _arrLoop[(_lngZ - 8) & 15] * _arrFilterTaps[4];
                                    intE += _arrLoop[(_lngZ - 10) & 15] * _arrFilterTaps[5];
                                    intE += _arrLoop[(_lngZ - 12) & 15] * _arrFilterTaps[6];
                                    intE += _arrLoop[(_lngZ - 14) & 15] * _arrFilterTaps[7];
                                    intE /= 128;
                                    _lngZ++;
                                    _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1] + intE * _arrEchoVolume[intJ & 1]) / 0x0080;
                                    intI = intCLIP16(intI);
                                    arrBuffer[intJ] = intI;
                                }
                            }
                        }
                        else
				        {
					        // 16-bit mono sound with echo enabled...
					        if (_blnNoFilter)
					        {
						        // ... no filter defined
						        for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
						        {
							        Int32 intE = _arrEcho[_intEchoPtr];
							        _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
							        if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
								        _intEchoPtr = 0;
                                    }
							        intI = (_arrMixBuffer[intJ] * _arrMasterVolume[0] + intE * _arrEchoVolume[0]) / 0x0080;
							        intI = intCLIP16(intI);
							        arrBuffer[intJ] = intI;
						        }
					        }
					        else
					        {
						        // ... with filter defined
						        for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
						        {
							        Int32 intE = _arrEcho[_intEchoPtr];
							        _arrLoop [(_lngZ - 0) & 7] = intE;
							        intE =  intE * _arrFilterTaps [0];
							        intE += _arrLoop[(_lngZ - 1) & 7] * _arrFilterTaps[1];
							        intE += _arrLoop[(_lngZ - 2) & 7] * _arrFilterTaps[2];
							        intE += _arrLoop[(_lngZ - 3) & 7] * _arrFilterTaps[3];
							        intE += _arrLoop[(_lngZ - 4) & 7] * _arrFilterTaps[4];
							        intE += _arrLoop[(_lngZ - 5) & 7] * _arrFilterTaps[5];
							        intE += _arrLoop[(_lngZ - 6) & 7] * _arrFilterTaps[6];
							        intE += _arrLoop[(_lngZ - 7) & 7] * _arrFilterTaps[7];
							        intE /= 128;
							        _lngZ++;
							        _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
							
							        if (_intEchoPtr >= _intEchoBufferSize)
                                    {
								        _intEchoPtr = 0;
                                    }
							        intI = (_arrMixBuffer[intJ] * _arrMasterVolume[0] + intE * _arrEchoVolume[0]) / 0x0080;
							        intI = intCLIP16(intI);
							        arrBuffer[intJ] = intI;
						        }
					        }
				        }
                    }
                    else
                    {
                        // 16-bit mono or stereo sound, no echo
                        for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                        {
                            intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1]) / 0x0080;
                            intI = intCLIP16(intI);
                            arrBuffer[intJ] = intI;
                        }
                    }
                }
            }
            else
            {
                // 8-bit sound
                if (_blnMuteSound)
                {
                    //TODOmemset (buffer, 128, intByteCount);
                }
                else
                {
                    if (_intEchoEnable > 0 && _intEchoBufferSize > 0)
                    {
                        if (_intStereo > 0)
                        {
                            // 16-bit stereo sound with echo enabled ...
                            if (_blnNoFilter)
                            {
                                // ... but no filter
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];
                                    _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1] + intE * _arrEchoVolume[intJ & 1]) / 0x80000;
                                    intI = intCLIP8(intI);
                                    arrBuffer[intJ] = intI + 128;
                                }
                            }
                            else
                            {
                                // ... with filter
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];

                                    _arrLoop[(_lngZ - 0) & 15] = intE;
                                    intE = intE * _arrFilterTaps[0];
                                    intE += _arrLoop[(_lngZ - 2) & 15] * _arrFilterTaps[1];
                                    intE += _arrLoop[(_lngZ - 4) & 15] * _arrFilterTaps[2];
                                    intE += _arrLoop[(_lngZ - 6) & 15] * _arrFilterTaps[3];
                                    intE += _arrLoop[(_lngZ - 8) & 15] * _arrFilterTaps[4];
                                    intE += _arrLoop[(_lngZ - 10) & 15] * _arrFilterTaps[5];
                                    intE += _arrLoop[(_lngZ - 12) & 15] * _arrFilterTaps[6];
                                    intE += _arrLoop[(_lngZ - 14) & 15] * _arrFilterTaps[7];
                                    intE /= 128;
                                    _lngZ++;
                                    _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];

                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1] + intE * _arrEchoVolume[intJ & 1]) / 0x8000;
                                    intI = intCLIP8(intI);
                                    arrBuffer[intJ] = intI + 128;
                                }
                            }
                        }
                        else
                        {
                            // 8-bit mono sound with echo enabled...
                            if (_blnNoFilter)
                            {
                                // ... but no filter.
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];
                                    _arrEcho[_intEchoPtr] = (intE * this._intEchoFeedback) / 128 + _arrEchoBuffer[intJ];

                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[0] + intE * _arrEchoVolume[0]) / 0x80000;
                                    intI = intCLIP8(intI);
                                    arrBuffer[intJ] = intI + 128;
                                }
                            }
                            else
                            {
                                // ... with filter.
                                for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                                {
                                    Int32 intE = _arrEcho[_intEchoPtr];
                                    _arrLoop[(_lngZ - 0) & 7] = intE;
                                    intE = intE * _arrFilterTaps[0];
                                    intE += _arrLoop[(_lngZ - 1) & 7] * _arrFilterTaps[1];
                                    intE += _arrLoop[(_lngZ - 2) & 7] * _arrFilterTaps[2];
                                    intE += _arrLoop[(_lngZ - 3) & 7] * _arrFilterTaps[3];
                                    intE += _arrLoop[(_lngZ - 4) & 7] * _arrFilterTaps[4];
                                    intE += _arrLoop[(_lngZ - 5) & 7] * _arrFilterTaps[5];
                                    intE += _arrLoop[(_lngZ - 6) & 7] * _arrFilterTaps[6];
                                    intE += _arrLoop[(_lngZ - 7) & 7] * _arrFilterTaps[7];
                                    intE /= 128;
                                    _lngZ++;
                                    _arrEcho[_intEchoPtr] = (intE * _intEchoFeedback) / 128 + _arrEchoBuffer[intJ];
                                    if ((_intEchoPtr += 1) >= _intEchoBufferSize)
                                    {
                                        _intEchoPtr = 0;
                                    }
                                    intI = (_arrMixBuffer[intJ] * _arrMasterVolume[0] + intE * _arrEchoVolume[0]) / 0x8000;
                                    intI = intCLIP8(intI);
                                    arrBuffer[intJ] = intI + 128;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 8-bit mono or stereo sound, no echo
                        for (intJ = 0; intJ < _intSamplesPerMix; intJ++)
                        {
                            intI = (_arrMixBuffer[intJ] * _arrMasterVolume[intJ & 1]) / 0x8000;
                            intI = intCLIP8(intI);
                            arrBuffer[intJ] = intI + 128;
                        }
                    }
                }
            }
            return arrBuffer;
        }

        /// <summary>
        /// get byte from dsp
        /// </summary>
        /// <returns>the byte from dsp</returns>
        public Byte objGetByte(Int32 intPos)
        {
            Byte bteByte = _arrRegisters[intPos];
            switch (intPos)
            {
                //key on
                case 0x4c:
                    break;
                //key off
                case 0x5c:
                    break;
                case 0x09:
                case 0x19:
                case 0x29:
                case 0x39:
                case 0x49:
                case 0x59:
                case 0x69:
                case 0x79:
                    if (_arrChannels[intPos >> 4].State == 0)
                    {
                        return 0;
                    }
                    return (Byte)((_arrChannels[intPos >> 4].Sample >> 8) | (_arrChannels[intPos >> 4].Sample & 0xff));
                case 0x08:
                case 0x18:
                case 0x28:
                case 0x38:
                case 0x48:
                case 0x58:
                case 0x68:
                case 0x78:
                    return (Byte)intGetEnvelopeHeight(intPos >> 4);
                case 0x7c:
                    break;
                default:
                    break;
            }
            return (bteByte);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// clip 8
        /// </summary>
        /// <param name="intI"></param>
        /// <returns></returns>
        private Int32 intCLIP8(Int32 intI)
        {
	        if (intI < -128)
            {
                return -128;
            }
	        if (intI > 127) 
            {
                return 127;
            }
            return intI;
        }
    
        /// <summary>
        /// mix mono
        /// </summary>
        /// <param name="intSampleCount"></param>
        /// <param name="arrRam"></param>
        private void MixMono(Int32 intSampleCount, ref Byte[] arrRam)
        {
            Int32[] arrWave = new Int32[16384];
            Int32 intPitchMod = _intPitchMod & ~_arrRegisters[0x3d];
	
            for (Int32 intJ = 0; intJ < 8; intJ++) 
            {
		        Channel objCH = _arrChannels[intJ];
		        Int64 lngFreq0 = objCH.Frequency;
		
		        if (objCH.State == 0 || (_intSoundSwitch & (1 << intJ))<=0)
                {
			        continue;
                }
		        Boolean blnMod = (intPitchMod & (1 << intJ))>0;
		        if (objCH.NeedsDecode) 
		        {
                    DecodeBlock(objCH, ref arrRam);
			        objCH.NeedsDecode = false;
			        objCH.Sample = objCH.Block[0];
			        objCH.SamplePointer = (Int32) (lngFreq0 >> 16);
			        if (objCH.SamplePointer == 0)
                    {
				        objCH.SamplePointer = 1;
                    }
			        if (objCH.SamplePointer > 16)
                    {
				        objCH.SamplePointer = 15;
                    }
			        objCH.NextSample = objCH.Block[objCH.SamplePointer];
			        objCH.Interpolate = 0;
		        }
		        Int32 intV = (objCH.Sample * objCH.LeftVolLevel) / 128;
		
		        for (Int32 intI = 0; intI < intSampleCount; intI++)
		        {
			        Int64 lngFreq = lngFreq0;

                    if (blnMod)
                    {
                        lngFreq = (lngFreq * (((arrWave[intI] + 0x800000) >> 16) >> 7));
                    }
                    objCH.EnvError += objCH.ERate;
                    if (objCH.EnvError >= 0x10000) 
			        {
                        Int32 intStep = (Int32)(objCH.EnvError >> 16);
				        switch (objCH.State)
				        {
				            case 1:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX += intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;

                                if (objCH.EnvX >= 126)
					            {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 2;
                                    if (objCH.SustainLevel != 8) 
						            {
                                        SetEnvelopeRate(intJ, objCH.DecayRate, -1, (Int32)((127 * objCH.SustainLevel) >> 3));
							            break;
						            }
                                    objCH.State = 3;
                                    SetEnvelopeRate(intJ, objCH.SustainRate, -1, 0);
					            }
					            break;
				            case 2:
                                while (objCH.EnvError >= 0x10000)
					            {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
					            }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= objCH.EnvxTarget)
					            {
                                    if (objCH.EnvX <= 0)
						            {
                                        SetEndOfSample(intJ, objCH);
                                        return;
						            }
						            objCH.State = 3;
                                    SetEnvelopeRate(intJ, objCH.SustainRate, -1, 0);
					            }
					            break;
				            case 3:
                                while (objCH.EnvError >= 0x10000)
					            {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
					            }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
					            {
                                    SetEndOfSample(intJ, objCH);
                                    return;
					            }
					            break;
				            case 4:
                                while (objCH.EnvError >= 0x10000)
					            {
                                    objCH.Envxx -= (127 << 24) / 256;
                                    objCH.EnvError -= 0x10000;
					            }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
					            {
						            SetEndOfSample(intJ, objCH);
                                    return;
					            }
					            break;
				            case 6:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX += intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;

                                if (objCH.EnvX >= 126)
					            {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 5;
                                    objCH.Mode = 5;
                                    SetEnvelopeRate(intJ, 0, -1, 0);
					            }
					            break;
				            case 7:
                                if (objCH.EnvX >= (127 * 3) / 4)
					            {
                                    while (objCH.EnvError >= 0x10000)
						            {
                                        objCH.Envxx += (127 << 24) / 256;
                                        objCH.EnvError -= 0x10000;
						            }
                                    objCH.EnvX = objCH.Envxx >> 24;
					            }
					            else
					            {
                                    objCH.EnvError &= 0xffff;
                                    objCH.EnvX += intStep << 1;
                                    objCH.Envxx = objCH.EnvX << 24;
					            }
                                if (objCH.EnvX >= 126)
					            {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 5;
                                    objCH.Mode = 5;
                                    SetEnvelopeRate(intJ, 0, -1, 0);
					            }
					            break;
				            case 8:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX -= intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;
                                if (objCH.EnvX <= 0)
					            {
                                    SetEndOfSample(intJ, objCH);
                                    return;
					            }
					            break;
				            case 9:
                                while (objCH.EnvError >= 0x10000)
					            {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
					            }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
					            {
                                    SetEndOfSample(intJ, objCH);
                                    return;
					            }
					            break;
				            case 5:
                                SetEnvelopeRate(intJ, 0, -1, 0);
					            break;
		                }
                        objCH.LeftVolLevel = (Int16)((objCH.EnvX * objCH.VolumeLeft) / 128);
                        intV = (objCH.Sample * objCH.LeftVolLevel) / 128;
		            }
		
		            objCH.Count += (Int32)lngFreq;
                    if (objCH.Count >= 0x10000)
		            {
                        intV = objCH.Count >> 16;
                        objCH.SamplePointer += intV;
                        objCH.Count &= 0xffff;

                        objCH.Sample = objCH.NextSample;
                        if (objCH.SamplePointer >= 16)
                        {
                            if (objCH.SamplePointer >= 0xffffff)
                            {
                                SetEndOfSample(intJ, objCH);
                                return;
                            }
                            do
                            {
                                objCH.SamplePointer -= 16;
                                if (objCH.LastBlock)
                                {
                                    if (!objCH.Loop)
                                    {
                                        objCH.SamplePointer = 0xffffff;
                                        objCH.NextSample = objCH.Sample;
                                        break;
                                    }
                                    else
                                    {
                                        objCH.LastBlock = false;
                                        Int32 intAddr = (((_arrRegisters[0x5d] << 8) + (objCH.SampleNumber << 2)) & 0xffff);
                                        objCH.BlockPointer = arrRam[intAddr + 2] | arrRam[intAddr + 3] << 8;
                                        arrRam[0x7c] |= (Byte)(1 << intJ);
                                    }
                                }
                                DecodeBlock(objCH, ref arrRam);
                            } while (objCH.SamplePointer >= 16);
                            if (objCH.SamplePointer < 0xffffff)
                            {
                                objCH.NextSample = objCH.Block[objCH.SamplePointer];
                            }
                        }
                        else
                        {
                            objCH.NextSample = objCH.Block[objCH.SamplePointer];
                        }
			            if (objCH.Type == 0)
			            {
					        objCH.Interpolate = 0;
			            }
			            else
			            {
                            for (; intV > 0; intV--)
                            {
                                if (((_intNoiseGen <<= 1) & 0x80000000L) > 0)
                                {
                                    _intNoiseGen ^= (Int32)0x0040001L;
                                }
                            }
                            objCH.Sample = (Int16)((_intNoiseGen << 17) >> 17);
                            objCH.Interpolate = 0;
			            }
                        intV = (objCH.Sample * objCH.LeftVolLevel) / 128;
		            }
		            else
		            {
                        if (objCH.Interpolate>0)
			            {
                            Int32 intS = (Int32)objCH.Sample + objCH.Interpolate;
				            intS = intCLIP16(intS);
                            objCH.Sample = (Int16)intS;
                            intV = (objCH.Sample * objCH.LeftVolLevel) / 128;
			            }
		            }
		            _arrMixBuffer[intI] += intV;
		            objCH.EchoBufPtr[intI] += intV;
                    if ((intPitchMod & (1 << (intJ + 1)))>0)
                    {
                        arrWave[intI] = objCH.Sample * objCH.EnvX;
                    }
                }
            }
        }

        /// <summary>
        /// mix stereo
        /// </summary>
        /// <param name="intSampleCount"></param>
        /// <param name="arrRam"></param>
        private void MixStereo(Int32 intSampleCount, ref Byte[] arrRam)
        {
            Int32[] arrWave = new Int32[16384];
            Int32 intPitchMod = _intPitchMod & ~_arrRegisters[0x3d];

            for (Int32 intJ = 0; intJ < 8; intJ++)
            {
                Int32 intVL, intVR;
                Channel objCH = _arrChannels[intJ];
                Int64 lngFreq0 = objCH.Frequency;
                if (objCH.State == 0 || !((_intSoundSwitch & (1 << intJ)) > 0))
                {
                    continue;
                }
                lngFreq0 = (Int64)((Double)lngFreq0 * 0.985);//uncommented by jonathan gevaryahu, as it is necessary for most cards in linux
                Boolean blnMod = (_intPitchMod & (1 << intJ)) > 0;

                if (objCH.NeedsDecode)
                {
                    DecodeBlock(objCH, ref arrRam);
                    objCH.NeedsDecode = false;
                    objCH.Sample = objCH.Block[0];
                    objCH.SamplePointer = (Int32) (lngFreq0 >> 16);
                    if (objCH.SamplePointer == 0)
                    {
                        objCH.SamplePointer = 1;
                    }
                    if (objCH.SamplePointer > 16)
                    {
                        objCH.SamplePointer = 15;
                    }
                    objCH.NextSample = objCH.Block[objCH.SamplePointer];
                    objCH.Interpolate = 0;
                }
                intVL = (objCH.Sample * objCH.LeftVolLevel) / 128;
                intVR = (objCH.Sample * objCH.RightVolLevel) / 128;
                for (Int32 intI = 0; intI < intSampleCount; intI += 2)
                {
                    Int64 lngFreq = lngFreq0;
                    if (blnMod)
                    {
                        lngFreq = (lngFreq * (((arrWave[intI / 2] + 0x800000) >> 16) >> 7));
                    }
                    objCH.EnvError += objCH.ERate;
                    if (objCH.EnvError >= 0x10000)
                    {
                        Int32 intStep = (Int32) (objCH.EnvError >> 16);

                        switch (objCH.State)
                        {
                            case 1:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX += intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;

                                if (objCH.EnvX >= 126)
                                {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 2;
                                    if (objCH.SustainLevel != 8)
                                    {
                                        SetEnvelopeRate(intJ, objCH.DecayRate, -1, (Int32) ((127 * objCH.SustainLevel) >> 3));
                                        break;
                                    }
                                    objCH.State = 3;
                                    SetEnvelopeRate(intJ, objCH.SustainRate, -1, 0);
                                }
                                break;
                            case 2:
                                while (objCH.EnvError >= 0x10000)
                                {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
                                }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= objCH.EnvxTarget)
                                {
                                    if (objCH.EnvX <= 0)
                                    {
                                        SetEndOfSample(intJ, objCH);
                                        return;
                                    }
                                    objCH.State = 3;
                                    SetEnvelopeRate(intJ, objCH.SustainRate, -1, 0);
                                }
                                break;

                            case 3:
                                while (objCH.EnvError >= 0x10000)
                                {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
                                }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
                                {
                                    SetEndOfSample(intJ, objCH);
                                    return;
                                }
                                break;
                            case 4:
                                while (objCH.EnvError >= 0x10000)
                                {
                                    objCH.Envxx -= (127 << 24) / 256;
                                    objCH.EnvError -= 0x10000;
                                }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
                                {
                                    SetEndOfSample(intJ, objCH);
                                    return;
                                }
                                break;
                            case 6:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX += intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;

                                if (objCH.EnvX >= 126)
                                {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 5;
                                    objCH.Mode = 5;
                                    SetEnvelopeRate(intJ, 0, -1, 0);
                                }
                                break;
                            case 7:
                                if (objCH.EnvX >= (127 * 3) / 4)
                                {
                                    while (objCH.EnvError >= 0x10000)
                                    {
                                        objCH.Envxx += (127 << 24) / 256;
                                        objCH.EnvError -= 0x10000;
                                    }
                                    objCH.EnvX = objCH.Envxx >> 24;
                                }
                                else
                                {
                                    objCH.EnvError &= 0xffff;
                                    objCH.EnvX += intStep << 1;
                                    objCH.Envxx = objCH.EnvX << 24;
                                }
                                if (objCH.EnvX >= 126)
                                {
                                    objCH.EnvX = 127;
                                    objCH.Envxx = 127 << 24;
                                    objCH.State = 5;
                                    objCH.Mode = 5;
                                    SetEnvelopeRate(intJ, 0, -1, 0);
                                }
                                break;
                            case 8:
                                objCH.EnvError &= 0xffff;
                                objCH.EnvX -= intStep << 1;
                                objCH.Envxx = objCH.EnvX << 24;
                                if (objCH.EnvX <= 0)
                                {
                                    SetEndOfSample(intJ, objCH);
                                    return;
                                }
                                break;
                            case 9:
                                while (objCH.EnvError >= 0x10000)
                                {
                                    objCH.Envxx = (objCH.Envxx >> 8) * 255;
                                    objCH.EnvError -= 0x10000;
                                }
                                objCH.EnvX = objCH.Envxx >> 24;
                                if (objCH.EnvX <= 0)
                                {
                                    SetEndOfSample(intJ, objCH);
                                    return;
                                }
                                break;
                            case 5:
                                SetEnvelopeRate(intJ, 0, -1, 0);
                                break;
                        }
                        objCH.LeftVolLevel = (Int16)((objCH.EnvX * objCH.VolumeLeft) / 128);
                        objCH.RightVolLevel = (Int16)((objCH.EnvX * objCH.VolumeRight) / 128);
                        intVL = (objCH.Sample * objCH.LeftVolLevel) / 128;
                        intVR = (objCH.Sample * objCH.RightVolLevel) / 128;
                    }
                    objCH.Count += (Int32) lngFreq;
                    if (objCH.Count >= 0x10000)
                    {
                        intVL = objCH.Count >> 16;
                        objCH.SamplePointer += intVL;
                        objCH.Count &= 0xffff;

                        objCH.Sample = objCH.NextSample;
                        if (objCH.SamplePointer >= 16)
                        {
                            if (objCH.SamplePointer >= 0xffffff)
                            {
                                SetEndOfSample(intJ, objCH);
                                return;
                            }
                            do
                            {
                                objCH.SamplePointer -= 16;
                                if (objCH.LastBlock)
                                {
                                    if (!objCH.Loop)
                                    {
                                        objCH.SamplePointer = 0xffffff;
                                        objCH.NextSample = objCH.Sample;
                                        break;
                                    }
                                    else
                                    {
                                        arrRam[0x7c] |= (Byte)(1 << intJ);
                                        objCH.LastBlock = false;
                                        Int32 intAddr = (((_arrRegisters[0x5d] << 8) + (objCH.SampleNumber << 2)) & 0xffff);
                                        objCH.BlockPointer = arrRam[intAddr + 2] | arrRam[intAddr + 3] << 8;
                                    }
                                }
                                DecodeBlock(objCH, ref arrRam);
                            } while (objCH.SamplePointer >= 16);
                            if (objCH.SamplePointer < 0xffffff)
                            {
                                objCH.NextSample = objCH.Block[objCH.SamplePointer];
                            }
                        }
                        else
                        {
                            objCH.NextSample = objCH.Block[objCH.SamplePointer];
                        }
                        if (objCH.Type == 0)
                        {
                            objCH.Interpolate = 0;
                        }
                        else
                        {
                            for (; intVL > 0; intVL--)
                            {
                                if (((_intNoiseGen <<= 1) & 0x80000000L) > 0)
                                {
                                    _intNoiseGen ^= (Int32)0x0040001L;
                                }
                            }
                            objCH.Sample = (Int16)((_intNoiseGen << 17) >> 17);
                            objCH.Interpolate = 0;
                        }
                        intVL = (objCH.Sample * objCH.LeftVolLevel) / 128;
                        intVR = (objCH.Sample * objCH.RightVolLevel) / 128;
                    }
                    else if (objCH.Interpolate > 0)
                    {
                        Int32 intS = objCH.Sample + objCH.Interpolate;
                        intS = intCLIP16(intS);
                        objCH.Sample = (Int16)intS;
                        intVL = (objCH.Sample * objCH.LeftVolLevel) / 128;
                        intVR = (objCH.Sample * objCH.RightVolLevel) / 128;
                    }
                    if ((_intPitchMod & (1 << (intJ + 1))) > 0)
                    {
                        arrWave[intI / 2] = objCH.Sample * objCH.EnvX;
                    }
                    _arrMixBuffer[intI] += intVL;
                    _arrMixBuffer[intI + 1] += intVR;
                    objCH.EchoBufPtr[intI] += intVL;
                    objCH.EchoBufPtr[intI + 1] += intVR;
                }
            }
        }

        /// <summary>
        /// decode block
        /// </summary>
        /// <param name="objCH"></param>
        /// <param name="arrRam"></param>
        private void DecodeBlock(Channel objCH, ref Byte[] arrRam)
        {
            Int32 intOut;
            Byte bteFilter;
            Byte bteShift;
            Byte bteSample1, bteSample2;
            Boolean blnInvalidHeader;
            Int32 intIndex;
	
            if (objCH.BlockPointer > 0x10000 - 9)
            {
		        objCH.LastBlock = true;
		        objCH.Loop = false;
		        objCH.Block = objCH.Decoded;
		        return;
            }
		    for (Int32 intI=0; intI<9; intI++) {
			    //report_memread(objCH.BlockPointer + intI);
		    }
            intIndex = 0;
            bteFilter = arrRam[objCH.BlockPointer+intIndex];
            if ((objCH.LastBlock = (bteFilter & 1)>0))
            {
		        objCH.Loop = (bteFilter & 2) != 0;
            }
            intIndex++;
	        bteFilter = arrRam[objCH.BlockPointer+intIndex];
	        //signed short *raw = objCH.Block = objCH.Decoded;
            objCH.Block = objCH.Decoded;
	
	        // Seperate out the header parts used for decoding
	        bteShift = (Byte)(bteFilter >> 4);
	        // Header validity check: if range(shift) is over 12, ignore
	        // all bits of the data for that block except for the sign bit of each
	        blnInvalidHeader = !(bteShift < 0xD);
            bteFilter = (Byte)(bteFilter & 0x0c);

	        Int32 intPrev0 = objCH.Previous[0];
	        Int32 intPrev1 = objCH.Previous[1];
	        for (Int32 intI = 8; intI != 0; intI--)
	        {
                intIndex++;
		        bteSample1 = arrRam[objCH.BlockPointer+intIndex];
                bteSample2 = (Byte)(bteSample1 << 4);
		        //Sample 2 = Bottom Nibble, Sign Extended.
		        bteSample2 >>= 4;
		        //Sample 1 = Top Nibble, shifted down and Sign Extended.
		        bteSample1 >>= 4;
			    if (blnInvalidHeader) { bteSample1>>=3; bteSample2>>=3; }
		
		        for (Int32 nybblesmp = 0; nybblesmp<2; nybblesmp++){
			        intOut=(((nybblesmp>0) ? bteSample2 : bteSample1) << bteShift);
			        intOut >>= 1;
			
			        switch(bteFilter)
			        {
				        case 0x00:
					        // Method0 - [Smp]
					        break;
				        case 0x04:
					        // Method1 - [Delta]+[Smp-1](15/16)
					        intOut+=(intPrev0>>1)+((-intPrev0)>>5);
					        break;	
				        case 0x08:
					        // Method2 - [Delta]+[Smp-1](61/32)-[Smp-2](15/16)
					        intOut+=(intPrev0)+((-(intPrev0 +(intPrev0>>1)))>>5)-(intPrev1>>1)+(intPrev1>>5);
					        break;
				        default:
					        // Method3 - [Delta]+[Smp-1](115/64)-[Smp-2](13/16)
					        intOut+=(intPrev0)+((-(intPrev0 + (intPrev0<<2) + (intPrev0<<3)))>>7)-(intPrev1>>1)+((intPrev1+(intPrev1>>1))>>4);
					        break;
			        }
			        intOut = intCLIP16(intOut);
				    //    *raw++ = (Int16)(intOut<<1);
			        intPrev1=(Int16)intPrev0;
			        intPrev0=(Int16)(intOut<<1);
		        }
	        }
	        objCH.Previous[0] = intPrev0;
	        objCH.Previous[1] = intPrev1;
            objCH.BlockPointer += 9;
        }

        /// <summary>
        /// clip
        /// </summary>
        /// <param name="intOut"></param>
        /// <returns></returns>
        private Int32 intCLIP16(Int32 intOut)
        {
            if (intOut < -32768)
            {
                return -32768;
            }
	        if (intOut > 32767) 
            {
                intOut = 32767;
            }
            return intOut;
        }

        /// <summary>
        /// simple conversion of tangens to angle
        /// </summary>
        /// <param name="dblX">tangens value</param>
        /// <returns>estimation of angle</returns>
        private Double Atan(Double dblX)
        {
            if ((dblX >= 1) || (dblX <= 1))
            {
                return (dblX / (1 + 0.28 * dblX * dblX));
            }
            else
            {
                return (Math.PI / 2 - Atan(1 / dblX));
            }
        }

        /// <summary>
        /// fix envelope
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="bteGain"></param>
        /// <param name="bteAdsr1"></param>
        /// <param name="bteAdsr2"></param>
        private void FixEnvelope(Int32 intChannel, Byte bteGain, Byte bteAdsr1, Byte bteAdsr2)
        {
            if ((bteAdsr1 & 0x80) > 0)
            {
		        if (blnSetSoundMode(intChannel, 1))
		        {
			        Int32 intAttack = _arrAttackRate[bteAdsr1 & 0xf];
			        SetSoundADSR(intChannel, intAttack, _arrDecayRate[(bteAdsr1 >> 4) & 7], _arrSustainRate[bteAdsr2 & 0x1f], (bteAdsr2 >> 5) & 7, 8);
		        }
            }
            else
            {
		        // Gain mode
		        if ((bteGain & 0x80) == 0)
		        {
			        if (blnSetSoundMode(intChannel, 5))
			        {
				        SetEnvelopeRate(intChannel, 0, 0, (Int32)(bteGain & 0x7f));
				        SetEnvelopeHeight(intChannel, (Int32)(bteGain & 0x7f));
			        }
		        }
		        else
		        {
                    if ((bteGain & 0x40) > 0)
                    {
                        // Increase mode
                        if (blnSetSoundMode(intChannel, ((Byte)(bteGain & 0x20) > 0) ? 7 : 6))
                        {
                            SetEnvelopeRate(intChannel, _arrIncreaseRate[bteGain & 0x1f], 1, 127);
                        }
                    }
                    else
                    {
                        Int32 intRate = ((Byte)(bteGain & 0x20) >0) ? _arrDecreaseRateExp[bteGain & 0x1f] / 2 : _arrIncreaseRate[bteGain & 0x1f];
                        Int32 intMode = ((Byte)(bteGain & 0x20) > 0) ? 9 : 8;
                        if (blnSetSoundMode(intChannel, intMode))
                        {
                            SetEnvelopeRate(intChannel, intRate, -1, 0);
                        }
                    }
		        }
            }
        }

        /// <summary>
        /// set sound volumne
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intVolumeLeft"></param>
        /// <param name="intVolumeRight"></param>
        private void SetSoundVolume(Int32 intChannel, Int16 intVolumeLeft, Int16 intVolumeRight)
        {
            Channel objChannel = _arrChannels[intChannel];
            if (_intStereo == 0)
            {
		        intVolumeLeft = (Int16) ((Math.Abs(intVolumeRight) + Math.Abs(intVolumeLeft)) / 2);
            }	
            objChannel.VolumeLeft = intVolumeLeft;
            objChannel.VolumeRight = intVolumeRight;
            objChannel.LeftVolLevel = (Int16) ((objChannel.EnvX * intVolumeLeft) / 128);
            objChannel.RightVolLevel = (Int16) ((objChannel.EnvX * intVolumeRight) / 128);
        }

        /// <summary>
        /// set master volume
        /// </summary>
        /// <param name="volume_left"></param>
        /// <param name="volume_right"></param>
        private void SetMasterVolume(Int16 intVolumeLeft, Int16 intVolumeRight)
        {
            if (_intStereo == 0)
            {
                intVolumeLeft = (Int16) ((Math.Abs(intVolumeRight) + Math.Abs(intVolumeLeft)) / 2);
            }
            _shrMasterVolumeLeft = intVolumeLeft;
            _shrMasterVolumeRight = intVolumeRight;
            _arrMasterVolume[0] = intVolumeLeft;
            _arrMasterVolume[1] = intVolumeRight;
        }

        /// <summary>
        /// set echo volumne
        /// </summary>
        /// <param name="intVolumeLeft"></param>
        /// <param name="intVolumeRight"></param>
        private void SetEchoVolume(Int16 intVolumeLeft, Int16 intVolumeRight)
        {
            if (_intStereo == 0)
            {
                intVolumeLeft = (Int16)((Math.Abs(intVolumeRight) + Math.Abs(intVolumeLeft)) / 2);
            }
            _shrEchoVolumeLeft = intVolumeLeft;
            _shrEchoVolumeRight = intVolumeRight;
            _arrEchoVolume[0] = intVolumeLeft;
            _arrEchoVolume[1] = intVolumeRight;
        }

        /// <summary>
        /// set echo enable
        /// </summary>
        /// <param name="objByte"></param>
        private void SetEchoEnable(Byte objByte)
        {
            _intEchoChannelEnable = objByte;
            if (_intEchoWriteEnabled == 0)
            {
                objByte = 0;
            }
            if (objByte > 0 && _intEchoEnable==0)
            {
                _arrEcho = new Int32[24000];
                _arrLoop = new Int32[16];
            }
            _intEchoEnable = objByte;
            for (Int32 intI = 0; intI < 8; intI++)
            {
		        if ((objByte & (Byte)(1 << intI)) > 0)
                {
			        _arrChannels[intI].EchoBufPtr = _arrEchoBuffer;
                }
		        else
                {
                    _arrChannels[intI].EchoBufPtr = _arrDummyEchoBuffer;
                }
            }
        }

        /// <summary>
        /// set echo feedback
        /// </summary>
        /// <param name="intFeedback"></param>
        private void SetEchoFeedback(Int32 intFeedback)
        {
            if (intFeedback < -128) 
            {
                intFeedback = -128; 
            } else if (intFeedback > 127)
            {
                intFeedback = 127;
            }
            _intEchoFeedback = intFeedback;
        }

        /// <summary>
        /// set echo delay
        /// </summary>
        /// <param name="intDelay"></param>
        private void SetEchoDelay(Int32 intDelay)
        {
            _intEchoBufferSize = (512 * intDelay * _intPlaybackRate) / 32000;
            if (_intStereo>0)
            {
                _intEchoBufferSize <<= 1;
            }
            if (_intEchoBufferSize > 0)
            {
                _intEchoPtr %= _intEchoBufferSize;
            }
            else
            {
                _intEchoPtr = 0;
            }
            SetEchoEnable(_arrRegisters[0x4d]);
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="objByte"></param>
        private void SetFrequencyModulationEnable(Byte objByte)
        {
            _intPitchMod = objByte & ~1;
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="intChannel"></param>
        private void SetSoundKeyOff(Int32 intChannel)
        {
            Channel objCh = _arrChannels[intChannel];
            if (objCh.State != 0)
            {
                objCh.State = 4;
                objCh.Mode = 4;
                SetEnvelopeRate(intChannel, 8, -1, 0);
            }
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="blnTrue"></param>
        private void SetEchoWriteEnable(Boolean blnTrue)
        {
            _intEchoWriteEnabled = blnTrue ? 1 : 0;
            SetEchoDelay(_arrRegisters[0x7d] & 15);
        }

        /// <summary>
        /// set filter coefficient
        /// </summary>
        /// <param name="intTap"></param>
        /// <param name="intValue"></param>
        private void SetFilterCoefficient(Int32 intTap, Int32 intValue)
        {
            _arrFilterTaps[intTap & 7] = intValue;
            _blnNoFilter = (_arrFilterTaps[0] == 127 || _arrFilterTaps[0] == 0) && _arrFilterTaps[1] == 0 && _arrFilterTaps[2] == 0 &&
                _arrFilterTaps[3] == 0 && _arrFilterTaps[4] == 0 && _arrFilterTaps[5] == 0 && _arrFilterTaps[6] == 0 && _arrFilterTaps[7] == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        private void FixSoundAfterSnapshotLoad()
        {
            _intEchoWriteEnabled = (Byte)(_arrRegisters[0x6c] & 0x20);
            _intEchoChannelEnable = _arrRegisters[0x4c];
            SetEchoDelay(_arrRegisters[0x7d] & 0xf);
            SetEchoFeedback(_arrRegisters[0x0d]);
	
            SetFilterCoefficient(0, _arrRegisters[0x0f]);
            SetFilterCoefficient(1, _arrRegisters[0x1f]);
            SetFilterCoefficient(2, _arrRegisters[0x2f]);
            SetFilterCoefficient(3, _arrRegisters[0x3f]);
            SetFilterCoefficient(4, _arrRegisters[0x4f]);
            SetFilterCoefficient(5, _arrRegisters[0x5f]);
            SetFilterCoefficient(6, _arrRegisters[0x6f]);
            SetFilterCoefficient(7, _arrRegisters[0x7f]);
            for (Int32 intI = 0; intI < 8; intI++)
            {
		        _arrChannels[intI].NeedsDecode = true;
                SetSoundFrequency(intI, _arrChannels[intI].Hertz);
                _arrChannels[intI].Envxx = _arrChannels[intI].EnvX << 24;
                _arrChannels[intI].NextSample = 0;
                _arrChannels[intI].Interpolate = 0;
                _arrChannels[intI].Previous[0] = (Int32)_arrChannels[intI].Previous16[0];
                _arrChannels[intI].Previous[1] = (Int32)_arrChannels[intI].Previous16[1];
            }
            _arrMasterVolume[0] = _shrMasterVolumeLeft;
            _arrMasterVolume[1] = _shrMasterVolumeRight;
            _arrEchoVolume[0] = _shrEchoVolumeLeft;
            _arrEchoVolume[1] = _shrEchoVolumeRight;
            _intScanline = 0;
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intAttackRate"></param>
        /// <param name="intDecayRate"></param>
        /// <param name="intSustainRate"></param>
        /// <param name="intSustainLevel"></param>
        /// <param name="intReleaseRate"></param>
        private void SetSoundADSR (Int32 intChannel, Int32 intAttackRate, Int32 intDecayRate, Int32 intSustainRate, Int32 intSustainLevel, Int32 intReleaseRate)
        {
            _arrChannels[intChannel].AttackRate = intAttackRate;
            _arrChannels[intChannel].DecayRate = intDecayRate;
            _arrChannels[intChannel].SustainRate = intSustainRate;
            _arrChannels[intChannel].ReleaseRate = intReleaseRate;
            _arrChannels[intChannel].SustainLevel = intSustainLevel + 1;
	
            switch (_arrChannels[intChannel].State)
            {
                case 1:
		            SetEnvelopeRate(intChannel, intAttackRate, 1, 127);
		            break;
                case 2:
		            SetEnvelopeRate(intChannel, intDecayRate, -1, (127 * (intSustainLevel + 1)) >> 3);
		            break;
                case 3:
		            SetEnvelopeRate(intChannel, intSustainRate, -1, 0);
		            break;
            }
        }

        /// <summary>
        /// set enveloppe height
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intLevel"></param>
        private void SetEnvelopeHeight(Int32 intChannel, Int32 intLevel)
        {
            Channel objCh = _arrChannels[intChannel];
            objCh.EnvX = intLevel;
            objCh.Envxx = intLevel << 24;
            objCh.LeftVolLevel = (Int16)((intLevel * objCh.VolumeLeft) / 128);
            objCh.RightVolLevel = (Int16)((intLevel * objCh.VolumeRight) / 128);
            if (objCh.EnvX == 0 && objCh.State != 0 && objCh.State != 5)
            {
                SetEndOfSample(intChannel, objCh);
            }
        }

        /// <summary>
        /// get height of envelope
        /// </summary>
        /// <param name="intChannel">channel</param>
        private Int32 intGetEnvelopeHeight(Int32 intChannel)
        {
            if (_arrChannels[intChannel].State != 0 && _arrChannels[intChannel].State != 5)
            {
                return _arrChannels[intChannel].EnvX;
            }
            return 0;
        }

        /// <summary>
        /// set sound
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intHertz"></param>
        private void SetSoundFrequency(Int32 intChannel, Int32 intHertz)
        {
            if (_intPlaybackRate>0)
            {
                if (_arrChannels[intChannel].Type == 1)
                {
                    intHertz = _arrNoiseFreq[_arrRegisters[0x6c] & 0x1f];
                }
                _arrChannels[intChannel].Frequency = (Int32)(((Int64)intHertz * 0x10000) / _intPlaybackRate);
            }
        }

        /// <summary>
        /// set sound hertz
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intHertz"></param>
        private void SetSoundHertz(Int32 intChannel, Int32 intHertz)
        {
            _arrChannels[intChannel].Hertz = intHertz;
            SetSoundFrequency(intChannel, intHertz);
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intType"></param>
        private void SetSoundType(Int32 intChannel, Int32 intType)
        {
            _arrChannels[intChannel].Type = intType;
        }

        /// <summary>
        /// set sound
        /// </summary>
        /// <param name="blnMute"></param>
        /// <returns></returns>
        private Boolean blnSetSoundMute(Boolean blnMute)
        {
            Boolean blnOld = _blnMuteSound;
            _blnMuteSound = blnMute;
            return blnOld;
        }

        /// <summary>
        /// set sound control
        /// </summary>
        /// <param name="intSoundSwitch"></param>
        private void SetSoundControl(Int32 intSoundSwitch)
        {
            _intSoundSwitch = intSoundSwitch;
        }

        /// <summary>
        /// set sound mode
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="intMode"></param>
        private Boolean blnSetSoundMode(Int32 intChannel, Int32 intMode)
        {
            Channel objCH = _arrChannels[intChannel];
            switch (intMode)
            {
                case 4:
                    if (objCH.Mode != 0)
		            {
                        objCH.Mode = 4;
			            return true;
		            }
		            break;
                case 8:
                case 9:
                case 5:
                    if (objCH.Mode != 4)
		            {
                        objCH.Mode = intMode;
                        if (objCH.State != 0)
                        {
                            objCH.State = intMode;
                        }
			            return true;
		            }
		            break;
                case 6:
                case 7:
                    if (objCH.Mode != 4)
		            {
                        objCH.Mode = intMode;
                        if (objCH.State != 0)
                        {
                            objCH.State = intMode;
                        }
                        return true;
		            }
		            break;
                case 1:
                    if (objCH.Mode == 0 || objCH.Mode == 1)
		            {
                        objCH.Mode = intMode;
			            return true;
		            }
                    break;
            }
            return false;
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="intPlaybackRate"></param>
        private void SetPlaybackRate(Int32 intPlaybackRate)
        {
            _intPlaybackRate = intPlaybackRate;
            _intErrRate = (Int32)(63.49e-6 * 0x10000 / (1.0 / (Double)_intPlaybackRate));
            SetEchoDelay(_arrRegisters[0x7d] & 0xf);
            for (Int32 intI = 0; intI < 8; intI++)
            {
		        SetSoundFrequency(intI, _arrChannels[intI].Hertz);
            }
        }

        /// <summary>
        /// end of sample
        /// </summary>
        /// <param name="intI"></param>
        /// <param name="objCH"></param>
        private void SetEndOfSample(Int32 intI, Channel objCH)
        {
            objCH.State = 0;
            objCH.Mode = 0;
            _arrRegisters[0x7c] |= (Byte)(1 << intI);
            _arrRegisters[0x4c] &= (Byte)(~(1 << intI));
            _arrRegisters[0x5c] &= (Byte)(~(1 << intI));
            _bteKeyedChannels &= (Byte)(~(1 << intI));
        }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="intChannel"></param>
        /// <param name="lngRate"></param>
        /// <param name="intDirection"></param>
        /// <param name="intTarget"></param>
        private void SetEnvelopeRate(Int32 intChannel, Int64 lngRate, Int32 intDirection, Int32 intTarget)
        {
            Channel objCH = _arrChannels[intChannel];
            objCH.EnvxTarget = (Int16)intTarget;
            if (lngRate == ~0L)
            {
		        objCH.Direction = 0;
		        lngRate = 0;
            }
            else
            {
		        objCH.Direction = intDirection;
            }
            Int32[] arrSteps = new Int32[] { 0, 64, 619, 619, 128, 1, 64, 55, 64, 619 };
	
            if (lngRate == 0 || _intPlaybackRate == 0)
            {
		        objCH.ERate = 0;
            }
            else
            {
		        objCH.ERate = (Int64)(((Int64) 0x10000 * 1000 * arrSteps[objCH.State]) / (lngRate * _intPlaybackRate));
            }
        }

        /// <summary>
        /// reset sound
        /// </summary>
        /// <param name="blnFull"></param>
        private void ResetSound(Boolean blnFull)
        {
            for (Int32 intI = 0; intI < 8; intI++)
            {
		        _arrChannels[intI].State = 0;
                _arrChannels[intI].Mode = 0;
                _arrChannels[intI].Type = 0;
                _arrChannels[intI].VolumeLeft = 0;
                _arrChannels[intI].VolumeRight = 0;
                _arrChannels[intI].Hertz = 0;
                _arrChannels[intI].Count = 0;
                _arrChannels[intI].Loop = false;
                _arrChannels[intI].EnvxTarget = 0;
                _arrChannels[intI].EnvError = 0;
                _arrChannels[intI].ERate = 0;
                _arrChannels[intI].EnvX = 0;
                _arrChannels[intI].Envxx = 0;
                _arrChannels[intI].LeftVolLevel = 0;
                _arrChannels[intI].RightVolLevel = 0;
                _arrChannels[intI].Direction = 0;
                _arrChannels[intI].AttackRate = 0;
                _arrChannels[intI].DecayRate = 0;
                _arrChannels[intI].SustainRate = 0;
                _arrChannels[intI].ReleaseRate = 0;
                _arrChannels[intI].SustainLevel = 0;
		        _intEchoPtr = 0;
		        _intEchoFeedback = 0;
		        _intEchoBufferSize = 1;
            }
            _arrFilterTaps[0] = 127;
            _arrFilterTaps[1] = 0;
            _arrFilterTaps[2] = 0;
            _arrFilterTaps[3] = 0;
            _arrFilterTaps[4] = 0;
            _arrFilterTaps[5] = 0;
            _arrFilterTaps[6] = 0;
            _arrFilterTaps[7] = 0;
            _blnMuteSound = true;
            _intNoiseGen = 1;
            _intSoundSwitch = 255;
            _intSamplesMixedSoFar = 0;
            _intPlayPosition = 0;
            _intErrCounter = 0;
            if (blnFull)
            {
		        _shrMasterVolumeLeft = 0;
                _shrMasterVolumeRight = 0;
		        _shrEchoVolumeLeft = 0;
                _shrEchoVolumeRight = 0;
		        _intEchoEnable = 0;
		        _intEchoWriteEnabled = 0;
		        _intEchoChannelEnable = 0;
		        _intPitchMod = 0;
		        _arrDummy[0] = 0;
                _arrDummy[1] = 0;
                _arrDummy[2] = 0;
		        _arrMasterVolume[0] = 0;
                _arrMasterVolume[1] = 0;
                _arrEchoVolume[0] = 0;
                _arrEchoVolume[1] = 0;
		        _intNoiseHertz = 0;
            }

            _shrMasterVolumeLeft = 127;
            _shrMasterVolumeRight = 127;
            _arrMasterVolume[0] = _arrMasterVolume[1] = 127;
            if (_intPlaybackRate > 0)
            {
                _intErrRate = (Int32)(0x10000 * 63.49e-6 / (1.0 / _intPlaybackRate));
            }
            else
            {
                _intErrRate = 0;
            }
            _blnNoFilter = true;
        }

        /// <summary>
        /// play sample
        /// </summary>
        /// <param name="intChannel"></param>
        private void PlaySample(Int32 intChannel, ref Byte[] arrRam)
        {
            Channel objCH = _arrChannels[intChannel];
            objCH.State = 0;
            objCH.Mode = 0;
            objCH.EnvX = 0;
            objCH.Envxx = 0;
            FixEnvelope(intChannel, _arrRegisters[0x07 + (intChannel << 4)], _arrRegisters[0x05 + (intChannel << 4)], _arrRegisters[0x06 + (intChannel << 4)]);
            objCH.SampleNumber = _arrRegisters[0x04 + intChannel * 0x10];
            if ((_arrRegisters[0x3d] & (1 << intChannel)) > 0)
            {
                objCH.Type = 1;
            }
            else
            {
                objCH.Type = 0;
            }
            SetSoundFrequency(intChannel, objCH.Hertz);
            objCH.Loop = false;
            objCH.NeedsDecode = true;
            objCH.LastBlock = false;
            objCH.Previous[0] = objCH.Previous[1] = 0;
            Int32 intAddr = (((_arrRegisters[0x5d] << 8) + (objCH.SampleNumber << 2)) & 0xffff);
            objCH.BlockPointer = arrRam[intAddr + 2] | arrRam[intAddr + 3] << 8;
            objCH.SamplePointer = 0;
            objCH.EnvError = 0;
            objCH.NextSample = 0;
            objCH.Interpolate = 0;
            switch (objCH.Mode)
            {
                case 1:
		            if (objCH.AttackRate == 0)
		            {
			            if (objCH.DecayRate == 0 || objCH.SustainLevel == 8)
			            {
				            objCH.State = 3;
				            objCH.EnvX = (Int32)((127 * objCH.SustainLevel) >> 3);
                            SetEnvelopeRate(intChannel, objCH.SustainRate, -1, 0);
			            }
			            else
			            {
				            objCH.State = 2;
				            objCH.EnvX = 127;
                            SetEnvelopeRate(intChannel, objCH.DecayRate, -1, (Int32)((127 * objCH.SustainLevel) >> 3));
			            }
			            objCH.LeftVolLevel = (Int16)((objCH.EnvX * objCH.VolumeLeft) / 128);
			            objCH.RightVolLevel = (Int16)((objCH.EnvX *  objCH.VolumeRight) / 128);
		            }
		            else
		            {
			            objCH.State = 1;
			            objCH.EnvX = 0;
			            objCH.LeftVolLevel = 0;
			            objCH.RightVolLevel = 0;
                        SetEnvelopeRate(intChannel, objCH.AttackRate, 1, 127);
		            }
		            objCH.Envxx = objCH.Envxx << 24;
		            break;
                case 5:
		            objCH.State = 5;
		            break;
                case 6:
		            objCH.State = 6;
		            break;
                case 7:
		            objCH.State = 7;
		            break;
                case 8:
		            objCH.State = 8;
		            break;
                case 9:
		            objCH.State = 9;
		            break;
                default:
		            break;
            }
            FixEnvelope(intChannel, _arrRegisters[0x07 + (intChannel << 4)], _arrRegisters[0x05 + (intChannel << 4)], _arrRegisters[0x06 + (intChannel << 4)]);
        }

        #endregion
    }
}
