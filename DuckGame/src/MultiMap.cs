using System.Collections.Generic;

namespace DuckGame
{
    /// <summary>Type alias for MultiMap</summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class MultiMap<TKey, TElement> : MultiMap<TKey, TElement, List<TElement>>
    {
    }
}
