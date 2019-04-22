using UnityEngine;
using System;

public class Timer
{
    private float _duration;
    private Action _onComplete;
    private Action<float> _onPerFrame;
    private Action<float> _onPerSecond;
    private Action<float> _onCountDown;
    private bool _isLoop;
    private bool _usesRealTime;
    private GameObject _autoDestroyOwner;

    public long UseId { get;  set; }
    public bool IsCancelled { get;  set; }
    private bool _hasAutoDestroyOwner;
    
    private static long _allocUseId = 1;
    public static long AllocUseId
    {
        get { return ++_allocUseId; }
    }

    private float _startTime = 0f;
    private float _elapsedTimeTemp = 0f;
    private float _elapsedSecond = 0f;
    

    public static long CountDown(float duration,
        Action onComplete, 
        Action<float> onCountDown)
    {
        return Register(duration, onComplete, null, null, onCountDown);
    }


    public static long Wait(float duration,
        Action onComplete)
    {
        return Register(duration, onComplete);
    }


    public static long Run(Action<float> onPerFrame)
    {
        return Register(-1, null, onPerFrame);
    }


    public static long RunByFrame(Action<float> onPerFrame, GameObject autoDestroyOwner)
    {
        return Register(-1, 
            null, 
            onPerFrame, 
            null, 
            null, 
            false, 
            false, 
            autoDestroyOwner);
    }


    public static long RunBySecond(Action<float> onPerSecond, GameObject autoDestroyOwner)
    {
        return Register(-1,
            null,
            null,
            onPerSecond,
            null,
            false,
            false,
            autoDestroyOwner);
    }


    public static long Register(float duration = 0,
        Action onComplete = null,
        Action<float> onPerFrame = null,
        Action<float> onPerSecond = null,
        Action<float> onCountDown = null,
        bool isLoop = false,
        bool useRealTime = false,
        GameObject autoDestroyOwner = null
    )
    {
        var timer = TimerManager.Self.Get();
        timer.Reset();
        
        timer.UseId = AllocUseId;
        timer._duration = duration;
        timer._onComplete = onComplete;
        timer._onPerFrame = onPerFrame;
        timer._onPerSecond = onPerSecond;
        timer._onCountDown = onCountDown;
        timer._isLoop = isLoop;
        timer._usesRealTime = useRealTime;
        timer._hasAutoDestroyOwner = autoDestroyOwner != null;
        timer._autoDestroyOwner = autoDestroyOwner;
        timer._startTime = timer.GetWorldTime();
        timer._elapsedTimeTemp = 0f;
        timer._elapsedSecond = 0f;

        return timer.UseId;
    }


    private float GetWorldTime()
    {
        return _usesRealTime ? Time.realtimeSinceStartup : Time.time;
    }


    private float GetFireTime()
    {
        return _startTime + _duration;
    }
    

    public static void Cancel(long useId)
    {
        if (useId <= 0)
        {
            return;
        }

        Timer timer = null;
        if (TimerManager.Self.TryGet(useId, ref timer))
        {
            timer.IsCancelled = true;
        }
    }
    

    public Timer()
    {
        Reset();
    }


    public void Reset()
    {
        UseId = 0;
        _duration = 0f;
        _onComplete = null;
        _onPerFrame = null;
        _onPerSecond = null;
        _onCountDown = null;
        _isLoop = false;
        _usesRealTime = false;
        _elapsedTimeTemp = 0f;
        _elapsedSecond = 0f;
        IsCancelled = false;
    }


    public void OnUpdate(float deltaTime)
    {
        if(IsCancelled) return;
        if (_hasAutoDestroyOwner && _autoDestroyOwner == null)
        {
            Cancel(UseId);
            return;
        }

        var worldTime = GetWorldTime();
        var fireTime = GetFireTime();

        if (_duration >= 0 && worldTime >= fireTime)
        {
            if (_onComplete != null) _onComplete();

            if (_isLoop)
            {
                _startTime = worldTime;
            }
            else
            {
                Cancel(UseId);
                return;
            }
        }

        if (_onPerFrame != null) _onPerFrame(deltaTime);

        _elapsedTimeTemp += deltaTime;
        if (_elapsedTimeTemp >= 1f)
        {
            _elapsedTimeTemp -= 1;
            _elapsedSecond++;
            if (_onPerSecond != null) _onPerSecond(_elapsedSecond);

            if (_duration >= 0)
            {
                var countDown = _duration - _elapsedSecond;
                if (_onCountDown != null) _onCountDown(countDown >= 1f ? countDown : 0f);
            }
        }
    }
}
