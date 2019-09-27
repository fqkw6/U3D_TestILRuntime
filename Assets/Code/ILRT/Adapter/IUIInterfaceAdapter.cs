using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IUIInterfaceAdapter : CrossBindingAdaptor
{
    public override Type BaseCLRType{
        get { return typeof (IUIInterface); }
    }
    public override Type AdaptorType{
        get { return typeof (Adaptor); }
    }
    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance){
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : IUIInterface, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        IMethod mHide;
        IMethod mGetStr;
        IMethod mShow;

        public Adaptor(){}

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }
        public ILTypeInstance ILInstance {get { return instance; } }

        public void Hide()
        {
            if(mHide == null){
                mHide = instance.Type.GetMethod("Hide", 0);
            }
            if(mHide != null){
                this.appdomain.Invoke(this.mHide, instance, null);
            }
        }

        public void Show()
        {
            if(mShow == null){
                mShow = instance.Type.GetMethod("Show", 0);
            }
            if(mShow != null){
                this.appdomain.Invoke(this.mShow, instance, null);
            }
        }

        public string GetStr(){
            if(mGetStr == null){
                mGetStr = instance.Type.GetMethod("GetStr", 0);
            }
            if(mGetStr != null){
                return this.appdomain.Invoke(this.mGetStr, instance, null).ToString();
            }
            return "";
        }
    }
}