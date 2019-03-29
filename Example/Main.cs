using UnityEngine;

public class Main : MonoBehaviour
{
    public void Update()
    {
        TimerManager.Self.OnUpdate(Time.deltaTime);
    }
    
    private GameObject _obj;
    public long _countDownTimerId = 0;
    public long _printTipTimerId = 0;

    public void OnGUI()
    {
        if (GUILayout.Button("等待5秒 打印 Hello"))
        {
            Timer.Wait(
                5, 
                () => Debug.Log("Hello"));
        }


        if (GUILayout.Button("5秒倒计时 打印每秒更新"))
        {
            Timer.CountDown(
                5,
                () => Debug.Log("倒计时结束"),
                second => { Debug.Log(second); });
        }


        if (GUILayout.Button("测试A 5秒倒计时 开始"))
        {
            _countDownTimerId = Timer.CountDown(
                5,
                () => Debug.Log("倒计时结束"),
                second => { Debug.Log(second); });
        }

        if (GUILayout.Button("测试A 5秒倒计时 中途取消"))
        {
            Timer.Cancel(_countDownTimerId);
        }
        

        if (GUILayout.Button("测试A 5秒倒计时 重新开始倒计时"))
        {
            _countDownTimerId = Timer.CountDown(
                5,
                () => Debug.Log("倒计时结束"),
                second => { Debug.Log(second); });
        }

        
        if (GUILayout.Button("测试B Run 开始 创建关注的GameObject"))
        {
            _countDownTimerId = Timer.RunBySecond(
                deltaTime =>{ Debug.Log(deltaTime); },
                _obj = new GameObject("_obj"));
        }


        if (GUILayout.Button("测试B 删除关注的GameObject"))
        {
            DestroyImmediate(_obj);
        }


        if (GUILayout.Button("测试C 循环调用"))
        {
            PrintTip();
        }

        if (GUILayout.Button("测试C 主动取消 循环调用"))
        {
            Timer.Cancel(_printTipTimerId);
        }
    }

    public void PrintTip()
    {
        Timer.Cancel(_printTipTimerId);
        Debug.Log("Tip: HI......");

        _printTipTimerId = Timer.Wait(3, PrintTip);
    }

    public void WhenDeInit()
    {
        Timer.Cancel(_countDownTimerId);
        Timer.Cancel(_printTipTimerId);
    }

    public void WhenReLink()
    {
        Timer.Cancel(_countDownTimerId);
        Timer.Cancel(_printTipTimerId);
    }
}