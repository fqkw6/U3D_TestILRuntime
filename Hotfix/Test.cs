using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Hotfix
{
    public class Test : IUIInterface
    {
        // 不带参
        public static String GetMsg()
        {
            Debug.Log("call static GetMsg");
            return "Test Hotfix, static";
        }
		// 带参
        public static String GetMsg1(int num)
        {
            Debug.Log("call static GetMsg1, num = " + num);
            return "Test Hotfix, static, num = " + num;
        }
        // 非静态
        public String GetMsg2(){
            Debug.Log("call GetMsg2");
            return "Test Hotfix, no static";
        }

        public void Hide()
        {
            Debug.Log("TestUI Hide");
        }

        public void Show()
        {
            Debug.Log("TestUI Show");
            Button btn = GameObject.Find("Canvas/Button").GetComponent<Button>();
            btn.onClick.AddListener(OnClick);
        }

        void OnClick(){
            Debug.Log("OnClick Btn");
        }

        public string GetStr(){
            return "Test GetStr";
        }
    }
}
