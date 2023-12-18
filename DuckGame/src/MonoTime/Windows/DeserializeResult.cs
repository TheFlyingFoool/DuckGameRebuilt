namespace DuckGame
{
    public enum DeserializeResult
    {
        NotDeserialized,
        HeaderDeserialized,
        Success,
        InvalidMagicNumber,
        FileVersionTooOld,
        FileVersionTooNew,
        Failure,
        NoData,
        ExceptionThrown,
        AlreadyDeserialized,
    }
}
