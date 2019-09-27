using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ILTools{
    private const string dllName = "Hotfix.dll";
    private const string pdbName = "Hotfix.pdb";
    private const string msbuildExe = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe";
    private const string ILBindingPath = "Assets/Code/ILRT/ILBinding";

    [MenuItem("Tools/ILRuntime/Generate CLR Binding Code by Analysis")]
    public async static void GenerateCLRBindingByAnalysis(){
        GenerateCLRBinding();
	    
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        string dstPath = Utils.GetStreamAssetsPath();
        byte[] dllBytes = await Utils.LoadFileBytesAsync(dstPath + "/Hotfix.dll");
        using (System.IO.MemoryStream fs = new MemoryStream(dllBytes))
        {
	        domain.LoadAssembly(fs);
	        //Crossbind Adapter is needed to generate the correct binding code
	        ILHelper.Initialize(domain);
	        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, ILBindingPath);
	        AssetDatabase.Refresh();
        }
        UnityEngine.Debug.Log("ILBinding Code Generated");
    }
    
    static void GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(long));
        types.Add(typeof(object));
        types.Add(typeof(string));
        types.Add(typeof(Array));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(RectTransform));
        types.Add(typeof(Time));
        types.Add(typeof(UnityEngine.Debug));
        //所有DLL内的类型的真实C#类型都是ILTypeInstance
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, ILBindingPath);
		AssetDatabase.Refresh();
    }

    [MenuItem("Tools/ILRuntime/Build Hotfix(Debug)")]
    static void BuildHotfixDebug(){
        BuildHotfix("Debug");
    }

    [MenuItem("Tools/ILRuntime/Build Hotfix(Release)")]
    static void BuildHotfixRelease(){
        BuildHotfix("Release");
    }

    static void BuildHotfix(string _c){
        if(!File.Exists(msbuildExe)){
            UnityEngine.Debug.LogError("找不到 MSBuild 工具");
            return;
        }
        System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(Application.dataPath);
        string projectPath = parent.ToString();
        ProcessCommand(msbuildExe, projectPath + "/Hotfix/Hotfix.csproj /t:Rebuild /p:Configuration=" + _c);
        UnityEngine.Debug.LogFormat("Hotfix {0} 编译完成", _c);
    }


    [MenuItem("Tools/ILRuntime/Copy Hotfix Files(Debug)")]
    public static void CopyHotfixFilesDebug(){
        CopyHotfixFilesAsync("Debug");
    }

    [MenuItem("Tools/ILRuntime/Copy Hotfix Files(Release)")]
    public static void CopyHotfixFilesRelease(){
        CopyHotfixFilesAsync("Release", false);
    }

    async static void CopyHotfixFilesAsync(string _c, bool copyPDB = true){
        System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(Application.dataPath);
        string projectPath = parent.ToString();
        string srcPath = projectPath + "/Hotfix/bin/" + _c + "/";
        string dstPath = Utils.GetStreamAssetsPath();
        await Utils.CopyFileAsync(srcPath + dllName, dstPath + "/" + dllName);
        if(copyPDB){
            await Utils.CopyFileAsync(srcPath + pdbName, dstPath + "/" + pdbName);
            UnityEngine.Debug.LogFormat("Copy {0} and {1} from {2} to {3}", dllName, pdbName, srcPath, dstPath);
        }
        UnityEngine.Debug.LogFormat("Copy {0} from {1} to {2}", dllName, srcPath, dstPath);
    }

    public static void ProcessCommand(string command, string argument) {
        ProcessStartInfo start = new ProcessStartInfo(command);
        start.Arguments = argument;
        start.CreateNoWindow = true;
        start.ErrorDialog = true;
        start.UseShellExecute = true; 
        if (start.UseShellExecute) {
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;
            start.RedirectStandardInput = false;
        } else {
            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;
            start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        } 
        Process p = Process.Start(start); 
        if (!start.UseShellExecute) {
            // UnityEngine.Debug.LogFormat("--- output:{0}", p.StandardOutput.ToString());
            // printOutPut(p.StandardOutput);
            // printOutPut(p.StandardError);
        } 
        p.WaitForExit();
        p.Close();
    }
}