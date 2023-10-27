namespace DuckGame
{
    public class ConditionalMessage : NetMessage
    {
        /// <summary>
        /// Return true when the message is ready to be activated.
        /// </summary>
        /// <returns></returns>
        public virtual bool Update() => true;
    }
}
