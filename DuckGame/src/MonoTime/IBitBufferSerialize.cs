namespace DuckGame
{
    public interface IBitBufferSerialize
    {
        object BitBuffer_Read(BitBuffer pBuffer);

        void BitBuffer_Write(BitBuffer pBuffer);
    }
}
