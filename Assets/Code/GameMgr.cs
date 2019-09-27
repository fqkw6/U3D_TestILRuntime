using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ILRuntime.Runtime.Generated;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    List<Action> DllUIUpdateList = new List<Action>();
    // Start is called before the first frame update
    void Start()
    {
        LoadHotfix();
    }

    async void LoadHotfix(){
        string root = Utils.GetStreamAssetsPath();
        byte [] dllBytes = await Utils.LoadFileBytesAsync(root + "/Hotfix.dll");
        byte [] pdbBytes = await Utils.LoadFileBytesAsync(root + "/Hotfix.pdb");

        if(dllBytes != null && pdbBytes != null){
            Debug.Log("Load Hotfix.dll and Hotfix.pdb success");
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
            {
#if RELEASE
                appdomain.LoadAssembly(fs, null, null);
#else
                using (System.IO.MemoryStream p = new MemoryStream(pdbBytes))
                {
                    appdomain.LoadAssembly(fs, null, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
#endif
            }
            ILHelper.Initialize(appdomain);
            ILRuntimeTest();
        }else{
            if(dllBytes == null){
                Debug.Log("Load Hotfix.dll fail");
            }
            if(pdbBytes == null){
                Debug.Log("Load Hotfix.pdb fail");
            }
        }
    }

    void ILRuntimeTest(){
        Debug.Log(appdomain.Invoke("Hotfix.Test", "GetMsg", null, null));
        object[] param = new object[1];
        param[0] = 666;
        Debug.Log(appdomain.Invoke("Hotfix.Test", "GetMsg1", null, param));
        var testInst = appdomain.Instantiate<IUIInterface>("Hotfix.Test");
        Debug.Log(appdomain.Invoke("Hotfix.Test", "GetMsg2", testInst, null));
        testInst.Show();
        Debug.Log(testInst.GetStr());
        // var testUIInst = appdomain.Instantiate<IUIInterface>("Hotfix.TestUI");
        // testUIInst.Show();
        // Types();
    }

    void Types(){
         List<Type> allTypes = new List<Type>();
        var values = appdomain.LoadedTypes.Values.ToList();
        foreach (var v in values)
        {
            allTypes.Add(v.ReflectionType);
        }
        //去重
        allTypes = allTypes.Distinct().ToList();
 
        DllUIUpdateList.Clear();
        foreach (var v in allTypes)
        {
            Debug.LogFormat("v: isClass:{0}, v.GetInterface('Adaptor'):{1} , FullName:{2}", v.IsClass,v.GetInterface("Adaptor"), v.FullName);
            //找到实现IUI接口的类 Adaptor 前面写的适配器IUI的类
            if (v.IsClass && v.GetInterface("Adaptor") != null)
            {
                Debug.Log("-----------v:" + v.FullName);
                //生成实例
                var gs = appdomain.Instantiate<IUIInterface>(v.FullName);
 
                //调用接口方法
                gs.Show();
            }
        }
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
