using System.Collections.Generic;
using Assets;
using JetBrains.Annotations;
using UnityEngine;

public class TimerManager : Singleton<TimerManager>
{
    private readonly List<Timer> _timers = new List<Timer>();
    private readonly ObjectPool<Timer> _objectPool = new ObjectPool<Timer>(10);

    public Timer Get()
    {
        var tiemr = _objectPool.Get();
        _timers.Add(tiemr);

        return tiemr;
    }
    

    public void OnUpdate(float deltaTime)
    {
        for (var i = _timers.Count - 1; i >= 0; i--)
        {
            _timers[i].OnUpdate(deltaTime);
        }

        for (var i = _timers.Count - 1; i >= 0; i--)
        {
            var timer = _timers[i];
            if (timer.IsCancelled)
            {
                timer.Reset();
                _timers.RemoveAt(i);
                _objectPool.Release(timer);
            }
        }
    }

    public bool TryGet(long useId, ref Timer timer)
    {
        for (var i = _timers.Count - 1; i >= 0; i--)
        {
            if (_timers[i].UseId != useId) continue;

            timer = _timers[i];
            return true;
        }

        return false;
    }
}