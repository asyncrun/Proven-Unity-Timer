using System.Collections.Generic;
using UnityEngine.XR.WSA.Input;
using UnityEngine.XR.WSA.Persistence;

namespace Assets
{
    public class ObjectPool<T> where T : class, new()
    {
        private readonly Stack<T> _pool = new Stack<T>();
        
        public ObjectPool (int initCount)
        {
            for (var i = 0; i < initCount; i++)
            {
                _pool.Push(new T());
            }
        }

        public T Get()
        {
            return _pool.Count == 0 ? new T() : _pool.Pop();
        }

        public void Release(T obj)
        {
            _pool.Push(obj);
        }
    }
}