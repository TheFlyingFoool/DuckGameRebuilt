using System.Collections.Generic;

namespace DuckGame
{
    public interface IContainPossibleThings
    {
        List<TypeProbPair> possible { get; }

        void PreparePossibilities();
    }
}
