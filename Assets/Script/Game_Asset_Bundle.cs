using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class BundleLoader : MonoBehaviour {
    public string Part;
    public string PartDetail;
    public static BundleLoader Instance;
    private Dictionary<string, AssetBundle> bundles = new();
    private readonly Dictionary<string, GameObject> prefabs = new();

    private void Awake() {
        Instance = this;
    }
    public GameObject LoadPrefab(BundleData data) {
        string key = $"{data.BundleName}/{data.AssetName}";
        if (prefabs.TryGetValue(key, out var prefab))
            return prefab;
        //
        AssetBundle bundle = LoadBundle(data.BundleName);
        if (bundle == null) return null;
        prefab = bundle.LoadAsset<GameObject>(data.AssetName);
        if (prefab == null) {
            Debug.LogError($"Asset '{data.AssetName}' tidak ditemukan di '{data.BundleName}'");
            return null;
        }
        prefabs.Add(key, prefab);
        return prefab;
    }
    public AssetBundle LoadBundle(string bundleName) {
        // Kalau tersedia cache
        if (bundles.TryGetValue(bundleName, out var bundle))
            return bundle;
        // kalau tidak tersedia cache
        string path = Path.Combine(Application.streamingAssetsPath, bundleName);
        bundle = AssetBundle.LoadFromFile(path);
        // kalau bundle kosong
        if (bundle == null) {
            Debug.LogError($"Bundle Not Found : {path}");
            return null;
        }
        bundles.Add(bundleName, bundle);
        return bundle;
    }
    //
    public IEnumerator DownloadBundle(string url, string bundleName) {
        using var req = UnityWebRequestAssetBundle.GetAssetBundle(url); 
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success) {
            Debug.LogError(req.error);
            yield break;
        }
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
        if (bundle == null) {
            Debug.LogError("Bundle download gagal.");
            yield break;
        }
        bundles[bundleName] = bundle;
        Debug.Log($"Bundle Downloaded : {bundleName}");
    }
    public IEnumerator DownloadPatch(string url, string savePath) {
        using var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success) {
            Debug.LogError(req.error);
            yield break;
        }
        File.WriteAllBytes(savePath, req.downloadHandler.data);
        Debug.Log("Download Complete");
    }
    //
    public void UnloadBundle(string bundleName, bool UnloadObjects = false) {
        if (!bundles.TryGetValue(bundleName, out var bundle))
            return;
        bundle.Unload(UnloadObjects);
        bundles.Remove(bundleName);
    }
    public void UnloadAll(bool UnloadObjects = false) {
        foreach (var bundle in bundles.Values) {
            bundle.Unload(UnloadObjects);
        }
        bundles.Clear();
        prefabs.Clear();
    }
}
[System.Serializable]
public class BundleData {
    public string ID;
    [Header("Prefab part")]
    public string BundleName;
    public string AssetName;
}
public class BuildAssetBundles {
    [MenuItem("Tools/Build AssetBundles")]
    public static void Build()
    {
        string path = "AssetBundles";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        Debug.Log("AssetBundle Build Complete");
    }
}
/// Contoh UI
/// BundleData Data
/// var ComponentData = bundleLoader.instance.Instantiate(Data, transformRoot);

///  

