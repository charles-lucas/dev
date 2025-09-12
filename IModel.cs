public interface IModel
{
    public bool ConvertIntToSymbol(Int32 character, Symbol symbol);
    public void Update(byte bite);
    public void AddSymbol(byte bite);
    public void Flush();
}