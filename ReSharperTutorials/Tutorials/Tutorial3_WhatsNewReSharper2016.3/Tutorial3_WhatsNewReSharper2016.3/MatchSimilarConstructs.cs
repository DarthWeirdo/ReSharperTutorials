namespace ReSharper20163
{
    class MatchSimilarConstructs
    {
        public int First => 42 * Second;
        public int Second => 42 * Third;
        public int Third => 0x2A * First;
    }   
}