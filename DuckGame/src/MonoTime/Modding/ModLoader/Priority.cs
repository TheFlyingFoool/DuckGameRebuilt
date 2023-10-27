namespace DuckGame
{
    /// <summary>
    /// A priority listing for measuring a priority compared to something else.
    /// Higher priorities take priority over lower ones (are executed last, basically)
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// Has no side-effects and will not conflict with other content
        /// </summary>
        Inconsequential,
        /// <summary>Lowest</summary>
        Lowest,
        /// <summary>Lower</summary>
        Lower,
        /// <summary>Low</summary>
        Low,
        /// <summary>Normal</summary>
        Normal,
        /// <summary>High</summary>
        High,
        /// <summary>Higher</summary>
        Higher,
        /// <summary>Highest</summary>
        Highest,
        /// <summary>
        /// Requires everything else to be done first before this one
        /// </summary>
        Monitor,
        HatPack,
        Reskin,
        MapPack,
    }
}
