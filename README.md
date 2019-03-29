# Proven-Unity-Timer

---

**Proven Unity Timer** 是 [**GitHub.akbiggs.UnityTimer**][1] 的实用优化简化版本。

---

**akbiggs** 实现的 [**UnityTimer**][1] 是优秀的Unity计时器

---

***两个版本的主要区别 ：***

 - 本版本去掉了一些API，比如暂停、取消所有Timer的调用
 - 获得Timer，不再反回Timer对象的引用，而是反回每次使用分配的UseId，UseID每次使用会递增，保持唯一；使用UseId取消Timer，好处是在做GC优化使用对象池时，一个Timer对象不会被多个外部调用引用持有，如果被持有，会被错误取消，造成BUG
 - 使用对象池管理Timer对象，避免频繁分配实例化
 - 增加了流逝时间回调
 - 增加了倒计时回调
 - 增加了每帧回调
 - 优化了List删除Timer时，不发生List数组向前Copy数据操作
 - 测试代码中添加了：中途取消Timer、监听GameObject删除取消Timer、重连时取消、销毁时取消等容易发生错误的测试代码


---

**一些测试代码**

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
    
        public void WhenDeInt()
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


  [1]: https://github.com/akbiggs/UnityTimer