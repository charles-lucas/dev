public class ModelOrder0 : public ModelInterface
{
    public Model0()
    {
        Initialize();
    }

    public void override Encode(Stream input, Stream output)
    {
        Int32 character;
        Symbol symbol = new Symbol();
        byte bite = 0x0;
        bool escaped;
        bool flush = false;
        Int16 text_count = 0;

        while(true)
        {
            character = input.ReadByte();
            if ( ( ++text_count & 0x0ff ) == 0 )
            {
                flush = CheckCompression( input, output );
            }
        if ( !flush )
        {
            character = input.ReadByte();
        }
        else
        {
            character = Constants.FLUSH;
        }
        if ( character == Constants.EOF )
        {
            character = Constants.DONE;
        }
        do
        {
            escaped = ConvertIntToSymbol( character, symbol);
            EncodeSymbol( output, symbol );
        } while ( escaped );
        if ( character == DONE )
        {
            break;
        }
        if ( character == FLUSH )
        {
            Flush();
            flush = 0;
        }
        Update( character );
        AddCharacter( character );
    }

    public void override Decode(Stream input, Stream output)
    {

    }

    private void Initialize()
    {
        _scoreboard = new byte[256];
        _totals = new UInt16[16];
        _contexts = new Dictionary<ContextKey, Context>();
        InitializeAllSysmbolsContext();
        InitializeControlContext();
        _order = Order.Model;
        _contextKey = new ContexKey();
        _order = Order.AllSymbols;
    }

    private void InitializeAllSysmbolsContext()
    {
        _allsymbols = new Context();
        for(byte bite = 0x0; bite <= 0xFF; bite++ )
        {
            _allsymbols.Update(new Stat(bite, 0));
        }
    }

    private void InitializeControlContext()
    {
        _controls.Add(-Constants.FLUSH, 1);
        _controls.Add(-Constants.DONE, 1);
    }

    private bool ConvertIntToSymbol(Int32 character, Symbol symbol)
    {
        int i;
        Context table;
        Uint16 totals[];
        
        if(_order == Order.Model)
        {
            table = _context[ _contextKey ];
        }
        else if(_oder == Order.AllSymbols)
        {
            table = _allsymbols;
        }
        else
        {
            table = _controls;
        }

        //TotalizeTable( table );
        totals = table.Totalize(_scoreboard);

        symbol.Scale = totals[ 0 ];

        if ( _order == Order.Control )
        {
            character = -character;
        }
        //for ( i = 0 ; i <= table->max_index ; i++ )
        foreach(Stats stat in table.Stats)
        {
            if ( character == (int) stat.Symbol )
            {
                if ( stat.Count == 0 )
                {
                    break;
                }
                symbol.LowCount = totals[ i+2 ];
                symbol.HighCount = totals[ i+1 ];
                return false;
            }
        }
        symbol.LowCount = totals[ 1 ];
        symbol.HighCount= totals[ 0 ];

        DecrementOrder();

        return true;
    }

    private Order DecrementOrder()
    {
        switch(_order)
        {
            case Order.AllSymbols:
                _order = Order.Control;
                break;
            case Order.Model:
                _order = Order.AllSymbols;
                break;
        }
    }

    private void EncodeSymbol(Stream output, Symbol symbol )
    {
        Int32 range;
        /*
         * These three lines rescale high and low for the new symbol.
        */
        range = (Int32) ( high-low ) + 1;
        high = low + (UInt16)(( range * symbol.HighCount ) / symbol.Scale -1 );
        low = low + (UInt16)(( range * symbol.LowCount ) / symbol.Scale );
        /*
         * This loop turns out new bits until high and low are far enough
         * apart to have stabilized.
        */
        for ( ; ; )
        {
            /*
             * If this test passes, it means that the MSDigits match, and can
             * be sent to the output stream.
            */
            if ( ( high & 0x8000 ) == ( low & 0x8000 ) )
            {
                OutputBit( stream, high & 0x8000 );
                while ( underflow_bits > 0 )
                {
                    OutputBit( stream, ~high & 0x8000 );
                    underflow_bits--;
                }
            }
            /*
             * If this test passes, the numbers are in danger of underflow, because
             * the MSDigits don't match, and the 2nd digits are just one apart.
            */
            else if ( ( low & 0x4000 ) && !( high & 0x4000 ))
            {
                underflow_bits += 1;
                low &= 0x3fff;
                high |= 0x4000;
            }
            else
            {
                return ;
            }
            low <<= 1;
            high <<= 1;
            high |= 1;
        }
    }

    private Dictionary<ContextKey, Context> _contexts;
    private Context _allsymbols;
    private Context _controls;
    private byte[] _scoreboard;
    private UInt16[] _totals;
    private Order _order;
    private ContexKey _contextKey;
    private UInt16 low;
    private UInt16 high;
}