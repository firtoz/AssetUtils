#if UNITY_EDITOR

using System;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class AssetUtils {
    private static readonly string DirectorySeparator = Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
    private readonly string localFolderName;
    private readonly string folderName;

    public AssetUtils(params string[] baseDirectory)
    {
        folderName = CreatePath(baseDirectory);
        localFolderName = CreatePath("Assets", folderName);
    }


    private static string CreatePath(params string[] elements)
    {
        return String.Join(DirectorySeparator, elements);
    }

    public static T GetProjectAsset<T>(string assetPath) where T : Object
    {
        Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));

        if (asset == null)
            return null;

        return (T)asset;
    }

    public T GetAsset<T>(string fileName) where T : Object
    {
        string assetPath = CreatePath(localFolderName, fileName);
        return GetProjectAsset<T>(assetPath);
    }

    public T GetOrCreateAsset<T>(string fileName) where T : ScriptableObject {
        T asset = GetAsset<T>(fileName) ?? CreateAsset(ScriptableObject.CreateInstance<T>(), fileName);
        return asset;
    }

    public T CreateAsset<T>(T asset, string fileName) where T : Object
    {
        string fullAssetPath = CreatePath(Application.dataPath, folderName, fileName);
        string directoryName = Path.GetDirectoryName(fullAssetPath);
        string assetPath = CreatePath(localFolderName, fileName);
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
}
#endif