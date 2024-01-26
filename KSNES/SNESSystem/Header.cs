namespace KSNES.SNESSystem;

public class Header
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Name;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public int Type;
    public int Speed;
    public int Chips;
    public int RomSize;
    public int RamSize;
}