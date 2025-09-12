public class ContextKey : IEquatable<ContextKey>
{
    public ContextKey(Int32 maxLength)
    {
        _maxLength = maxLength;
        _key = new List<byte>();
    }

    public ContextKey(Int32 maxLength, byte key)
    {
        _maxLength = maxLength;
        _key = new List<byte>();
        _key.Add(key);
    }

    public ContexKey(ContextKey conKey)
    {
        _maxLength = conKey.MaxLength;
        _key = new List<byte>();
        foreach(byte bite in conKey.Key)
        {
            _key.Add(bite);
        }
    }

    public ContextKey(ContextKey conKey, byte symbol)
    {
        _maxLength = conKey.MaxLength;
        _key = new List<byte>();
        foreach(byte bite in conKey.Key)
        {
            _key.Add(bite);
        }
        _key.Add(symbol);
        while(_key.Length > _maxLength)
        {
            _key.RemoveAt(0);
        }
    }

    public ContextKey GetLesser()
    {
        ContextKey result = new ContextKey(this);
        result.RemoveAt(0);
        return result;
    }

    public bool Empty()
    {
        return _key.Length == 0;
    }

    public Int32 MaxLength
    {
        get => _maxLength;
        set
        {
            _maxLength = value;
        }
    }

    public List<byte> Key
    {
        get
        {
            return _key;
        }
    }

    public override bool Equals(object? obj) => Equals(obj as ContexKey);

    public bool Equals(ContexKey? other)
    {
        bool result = false;
        bool allElementsCompareFail = false;

        if(_key.Length == other.Key.Length)
        {
            for(int i = 0; i < _key.Length; i++)
            {
                if(_key[i] != other.Key[i])
                {
                    allElementsCompareFail = true;
                }
            }
            result = !allElementsCompareFail;
        }

        return result;
    }

    public override int GetHashCode()
    {
        int result = 0;
        int shift = 0;
        int mask = 0x0FFFFFFF;
        if(_key.Length <= 4)
        {
            foreach(byte bite in _key)
            {
                result = (result << shift) | bite;
                shift += 8;
            }
        }
        else
        {
            for(int i = 0; i < 4; i++)
            {
                result = (result << shift) | _key[i];
                shift += 8;
            }
            for(int i = 4; i < _key.Length; i++)
            {
                result += _key[i];
            }
            result = (result & mask) | (_key.Length << 28);
        }

        return result;
    }

    public override string ToString()
    {
        string result = "";

        foreach(byte bite in _key)
        {
            result += String.Format("{0:x2} ", bite);
        }

        return result;
    }

    private List<byte> _key;
    private _maxKeyLength;

}