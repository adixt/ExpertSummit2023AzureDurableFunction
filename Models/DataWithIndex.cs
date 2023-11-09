using System.Collections.Generic;

namespace FunctionApp1.Models
{
    public class DataWithIndex
    {
        public IEnumerable<string> Names { get; init; }
        public int Idx { get; init; }

        public DataWithIndex(IEnumerable<string> names, int idx)
        {
            (Names, Idx) = (names, idx);
        }
    }
}