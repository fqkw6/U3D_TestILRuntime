using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Utils{
    public static async Task CopyFileAsync(string src, string dst, bool isWWWRead = false) {
        if (File.Exists(dst))
            File.Delete(dst);

        string dirPath = System.IO.Path.GetDirectoryName(dst);
        if (!Directory.Exists(dirPath))
            CreateDirectory(dirPath);
        if (isWWWRead) {
            byte[] content = await LoadFileBytesAsync(PrefixPath(src));
            await WriteFileAsync(dst, content);
        } else {
            File.Copy(src, dst, true);
        }
    }

    public static async Task<byte[]> LoadFileBytesAsync(string filePath){
        byte[] fileBytes;
        if(!File.Exists(filePath)){
            return null;
        }
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
        {
            fileBytes = new byte[fileStream.Length];
            await fileStream.ReadAsync(fileBytes, 0, (int) fileStream.Length);
        }
        return fileBytes;
    }

    public static async Task WriteFileAsync(string path, byte[] content) {
        string dirPath = System.IO.Path.GetDirectoryName(path);
        if (!IsDirectoryExist(dirPath)) CreateDirectory(dirPath);

        FileStream fs = new FileStream(path, FileMode.Create);
        await fs.WriteAsync(content, 0, content.Length);
        fs.Close();
    }

    public static bool IsDirectoryExist(string path) {
        return System.IO.Directory.Exists(path);
    }

    public static bool CreateDirectory(string path) {
        bool result = true;
        try {
            System.IO.Directory.CreateDirectory(path);
        } catch (Exception exp) {
            Debug.LogErrorFormat("create directory fail {0}", exp.ToString());
            result = false;
        }
        return result;
    }


    public static string PrefixPath(string path){
        switch (Application.platform) {
            case RuntimePlatform.Android:
                return path;
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.OSXEditor:
                return "file://" + path;
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "file:///" + path; 
            default:
                Debug.LogError("[PrefixPath] 不支持的平台:" + Application.platform.ToString());
                return "";
        }
    }

    public static string GetStreamAssetsPath(){
#if UNITY_ANDROID
        return "file:///" + Application.streamingAssetsPath
#else
        return Application.streamingAssetsPath;
#endif
    }
}