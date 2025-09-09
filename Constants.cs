static class Constants
{
    public const UInt32 MAXIMUM_SCALE = 16383;
    public const Int16 ESCAPE = 256;
    public const Int16 DONE = -1
    public const Int16 FLUSH = -2;
    public const Int16 EOF = -1;
}

public class enum Order
{   
    Control = -2,
    AllSymbols = -1,
    Model = 0;
}