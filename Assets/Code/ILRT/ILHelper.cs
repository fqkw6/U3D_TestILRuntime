using System;
using ILRuntime.Runtime.Generated;

public class ILHelper{

    public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain appdomain){
        // 注册委托
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });

        // CLR 绑定
        CLRBindings.Initialize(appdomain);

        // 注册适配器
        appdomain.RegisterCrossBindingAdaptor(new IUIInterfaceAdapter());
    }
}