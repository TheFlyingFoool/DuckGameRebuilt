namespace DuckGame
{
    /// <summary>
    /// Defines an object which contain, or may contain, an object of a specific type
    /// </summary>
    public interface IContainAThing
    {
        System.Type contains { get; }
    }
}
