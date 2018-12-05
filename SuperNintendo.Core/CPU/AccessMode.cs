namespace SuperNintendo.Core.CPU
{
    public enum AccessMode
    {
        NONE = 0,
        READ = 1,
        WRITE = 2,
        MODIFY = 3,
        JUMP = 5,
        JSR = 8
    }
}