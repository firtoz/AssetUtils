#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetUtils {
    private const string DirectorySeparator = "/";
    private readonly string folderName;

    public AssetUtils(params string[] baseDirectory)
    {
        folderName = CreatePath(baseDirectory);
    }

    public static string CreatePath(params string[] elements)
    {
        List<String> allElements = new List<string>(elements);
        List<String> finalElements = new List<string>();

        foreach (string element in allElements) {
            finalElements.AddRange(element.Split('\\'));
        }
        return String.Join(DirectorySeparator, finalElements.ToArray());
    }

    public static T GetProjectAsset<T>(params string[] elements) where T : Object {
        string assetPath = CreatePath(elements);
        Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

        if (asset == null)
            return null;

        return (T)asset;
    }

    public T GetAsset<T>(string fileName) where T : Object
    {
        string assetPath = CreatePath(folderName, fileName);
        return GetProjectAsset<T>(assetPath);
    }

    public T GetOrCreateAsset<T>(string fileName) where T : ScriptableObject {
        T asset = GetAsset<T>(fileName) ?? CreateAsset(ScriptableObject.CreateInstance<T>(), fileName);
        return asset;
    }

    public static bool IsAssetPath(string fullPath)
    {
        Uri dataPathUri = new Uri(new DirectoryInfo(Application.dataPath).FullName);

        DirectoryInfo directoryInfo = new DirectoryInfo(fullPath).Parent;

        bool isChild = false;
        while (directoryInfo != null)
        {
            Uri childUri = new Uri(directoryInfo.FullName);
            if (childUri.Equals(dataPathUri))
            {
                isChild = true;
                break;
            }
            directoryInfo = directoryInfo.Parent;
        }

        return isChild;
    }

    public T CreateAsset<T>(T asset, string fileName) where T : Object
    {
        string fullAssetPath = CreatePath(Application.dataPath, folderName, fileName);
        string directoryName = Path.GetDirectoryName(fullAssetPath);
        string assetPath = CreatePath(folderName, fileName);
        if (directoryName != null)
        {
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            AssetDatabase.CreateAsset(asset, assetPath);
        }
        else
        {
            Debug.LogError("Asset creation failed because of an invalid directory name: " + fullAssetPath);
        }
        return asset;
    }

    public static string GetRelativePath(string path) {
        Uri baseUri = new Uri(Application.dataPath);
        Uri destinationUri  = new Uri(new DirectoryInfo(path).FullName);
        Uri relativePath = baseUri.MakeRelativeUri(destinationUri);
        return Uri.UnescapeDataString(relativePath.ToString());
    }
}
#endif