using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 点击按钮，背景色改变，用代码控制，这样点击了按钮后，再点击场景按钮还是点击后的颜色，而不是原来色
/// 把按钮的控制的颜色变化关闭
/// </summary>
public class ChangeButtonImage : MonoBehaviour
{
    public Button button1, button2, button3;
    void Start()
    {
        
    }

    void ChangeButton_image(Button button)
    {
        button1.GetComponent<Image>().color = button == button1 ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        button2.GetComponent<Image>().color = button == button2 ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
        button3.GetComponent<Image>().color = button == button3 ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
