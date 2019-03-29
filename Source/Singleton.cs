
    public class Singleton<T> where T : new()
    {
        public static T Self { get; } = new T();
    }