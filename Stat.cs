internal class Stat : IEquatable<Stat>
{
    public Stat()
    {
        _symbol = 0x0;
        _count = 0;
    }

    public Stat(byte symbol, UInt16 count)
    {
        _symbol = symbol;
        _count = count;
    }

    public void Increment()
    {
        _count++;
    }

    public void Flush()
    {
        _count = (UInt16)(_count / 2);
    }

    public override bool Equals(object? obj) => Equals(obj as Stat);

    public bool Equals(Stat? other)
    {
        return other?.Symbol == _symbol;
    }

    public override int GetHashCode() => _symbol;

    public byte Symbol => _symbol;

    public UInt16 Count => _count;

    private byte _symbol;
    private UInt16 _count;
}