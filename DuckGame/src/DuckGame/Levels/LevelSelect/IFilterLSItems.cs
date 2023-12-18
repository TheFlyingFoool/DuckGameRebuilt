namespace DuckGame
{
    /// <summary>
    /// Represents an interface for filtering level select items from the list based on
    /// conditions.
    /// </summary>
    public interface IFilterLSItems
    {
        /// <summary>Filters the specified level.</summary>
        /// <param name="level">The level.</param>
        /// <param name="location">The location.</param>
        /// <returns>true to keep, false to remove</returns>
        bool Filter(string level, LevelLocation location = LevelLocation.Any);
    }
}
