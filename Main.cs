public static void Main(string[] args)
{
    ContextKey key1 = new ContextKey(0x79);
    ContestKey key2 = new ContextKey(keys, 0x84);

    Console.WriteLine("Key1:\t{0}", key1.ToString());
    Console.WriteLine("Key2:\t{0}", key2.ToString());
}