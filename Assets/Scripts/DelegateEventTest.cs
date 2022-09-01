using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateEventTest : MonoBehaviour
{
    string testStrig = "zengyuqing";
    //测试委托和事件的应用，和event Action差不多
    public delegate void DeleteModel(string  modelInfo);
    public event DeleteModel DelegateOnDeleteModelEvent;

    //先注册事件，在Start()中，然后最后在需要的地方调用OnDelegateOnDeleteModelEvent方法即可
    public virtual void OnDelegateOnDeleteModelEvent(string modelInfo)
    {
        if (DelegateOnDeleteModelEvent != null)
        {
            DelegateOnDeleteModelEvent(modelInfo);//事件被触发
        }
        TestEvent();
        Debug.Log("触发："+testStrig);
    }
    void Start()
    {
        //测试注册事件
        DelegateOnDeleteModelEvent += (string s) => {
            //这里委托具体方法
            Debug.Log("注册："+s);
            TestEvent();
        };

        //不运行这个函数就无法触发事件
        OnDelegateOnDeleteModelEvent(testStrig);
    }

    private void TestEvent()
    {
        Debug.Log("=====注册成功，这是具体委托的方法执行=====");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
