public class ArithmeticCoder
{
    public ArithmeticCoder(Int32 maxOrder);

    public ArithmeticCoder(IModel model)
    {
        _model = model;
    }

    public void Write(BinaryReader input, BinaryWriter output)
    {
        Int32 character;
        Symbol symbol = new Symbol();
        bool escaped;
        bool flush = false;
        Int16 textCount = 0;
        _coder = new Coder(true, input, output);

        while(true)
        {
            if((++textCount & 0x0ff) == 0)
            {
                flush = CheckCompression(input, output);
            }

            if(!flush)
            {
                try
                {
                    character = input.ReadByte();
                }
                catch(EndOfStreamException)
                {
                    character = Constants.DONE;
                }
                else
                {
                    character = Constants.Flush;
                }

                do 
                {
                    escaped = _model.ConvertIntToSymbol(character, symbol);
                    _coder.Encode(symbol);
                } while(escaped);
                
                if (character == Constants.Flush)
                {
                    _model.Flush();
                    flush = false;
                }
                else if(character == Constants.DONE)
                {
                    break;
                }

                _model.Update((byte)character);
                _model.AddCharacter((byte)character);
            }
            _coder.Flush();
        }

        public void Read(BinaryReader input, BinaryWriter output)
        {

        }

        public void Write(byte[] bites)
        {

        }

        public void Write(byte bite)
        {

        }

        public byte ReadByte()
        {

        }
    }

    private bool CheckCompression(BinaryReader input, BinaryWriter output)
    {
        bool result = true;
        Int64 totalInputBytes;
        Int64 totalOutputBytes;
        Int64 localRatio;

        totalInputBytes = input.BaseStream.Position - _localInputMarker;
        totalOutputBytes = output.BaseStream.Position - _localOutputMarker;

        if(totalOutputBytes == 0)
        {
            totalOutputBytes = 1;
        }

        localRatio = (totalOutputBytes * 100) / totalInputBytes;
        _localInputMarker = input.BaseStream.Position;
        _localOutputMarker = output.BaseStream.Position;

        result = localRatio > Constants.CompressionLimit;

        return result;
    }

    private Imodel _model;
    private Coder? _coder;

    Int64 _localInputMarker;
    Int54 _localOutputMarker;
}

