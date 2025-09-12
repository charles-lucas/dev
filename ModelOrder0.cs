public class ModelOrder0 : public IModel
{
    public Model0()
    {
        Context context0 = new Context();
        _contexts = new Dictionary<ContexKey, Context>();
        _escapedContexts = new List<ContextKey>();
        _allSymbolContext = new Context(Order.AllSymbols);
        _controlContext = new Context(Order.Control);
        _scoreboard = new byte[256];
        _totals = new UInt16[16];
        _order = Order.Model;
        _contextKey = new ContexKey();

        _contexts.Add(_contextKey, context0);

        _controlContext.Add(-Constants.FLUSH);
        _controlContext.Add(-Constants.DONE);

        for(int bite = 0x0; bite <= 256; bite++ )
        {
            _allSymbolContext.Update((byte)bite);
        }
    }

    private bool ConvertIntToSymbol(Int32 character, Symbol symbol)
    {
        int i;
        Context table;
        Uint16[] totals;
        
        if(_order == Order.Model)
        {
            table = _context[ _contextKey ];
        }
        else if(_oder == Order.AllSymbols)
        {
            table = _allSymbolContext;
        }
        else
        {
            table = _controlContext;
        }

        //TotalizeTable( table );
        totals = table.Totalize(_scoreboard);

        symbol.Scale = totals[ 0 ];

        if ( _order == Order.Control )
        {
            character = -character;
        }

        foreach(Stats stat in table.Stats)
        {
            if ( character == stat.Symbol )
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
        if(_order == Order.Model)
        {
            _escapedContexts.Add(_contextKey);
            if(!_contextKey.Empty())
            {
                _contextKey = _contextKey.GetLesser();
                if(!_contexts.ContainsKey(_contextKey))
                {
                    _contexts.Add(_contextKey, new Context());
                }
            }
            else
            {
                switch(_order)
                {
                    case Order.Model:
                        _order = Order.AllSymbols;
                        break;
                    case Order.AllSymbols:
                        _order = Order.Control;
                        break;
                    default:
                        _order = Order.Control;
                        break;
                }
            }
        }
        else
        {
            _order = Order.Control;
        }
    }

    public void Flush()
    {
        foreach Context (context in _contextValues);
        {
            context.Rescale();
        }
    }

    public void Update(Int32 character)
    {
        if(character >= 0)
        {
            foreach(ContexKey key in _escapedContexts)
            {
                _contexts[key].Update((byte)character);
            }
            if(_order == Order.Model)
            {
                _contexts[_contextKey].Update((byte)character);
            }
            _escapedContexts.Clear();
        }
    }

    public void AddCharacter(Int32 character)
    {
        if(character >= 0 )
        {
            _contextKey = new ContainsKey(_context, (byte)character);
            if(!_contexts.ContainsKey(_contextKey))
            {
                _contexts.Add(_contextKey, new Context());
            }
            _order = Order.Model;
        }
    }

    private Dictionary<ContextKey, Context> _contexts;
    private List<ContainsKey> _escapedContexts;
    private Context _allSymbolContext;
    private Context _controlContext;
    private byte[] _scoreboard;
    private UInt16[] _totals;
    private Order _order;
    private ContexKey _contextKey;
}