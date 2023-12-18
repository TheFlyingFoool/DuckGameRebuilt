namespace DuckGame
{
    internal static class Factory<T> where T : new()
    {
        private static int kMaxObjects = 1024;
        private static T[] _objects = new T[kMaxObjects];
        private static int _lastActiveObject = 0;

        static Factory()
        {
            for (int index = 0; index < kMaxObjects; ++index)
                _objects[index] = new T();
        }

        public static T New()
        {
            T obj = _objects[_lastActiveObject];
            _lastActiveObject = (_lastActiveObject + 1) % kMaxObjects;
            return obj;
        }
    }
}
