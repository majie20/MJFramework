using System.IO;
using UnityEditor;
public class AssetBundleTest {

    [MenuItem("Tools/Build AssetBundle")]
    public static void CreateAssetBundle () {
        string path = "AssetBundleRes";
        if (!Directory.Exists (path)) {
            Directory.CreateDirectory (path);
        }

        BuildPipeline.BuildAssetBundles (path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}