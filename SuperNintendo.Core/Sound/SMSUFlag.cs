namespace SuperNintendo.Core.Sound
{
    public enum SMSUFlag
    {
        Revision = 0x07,	// bitmask, not the actual version number
	    AudioError = 0x08,
	    AudioPlaying = 0x10,
	    AudioRepeating = 0x20,
	    AudioBusy = 0x40,
	    DataBusy = 0x80
    }
}