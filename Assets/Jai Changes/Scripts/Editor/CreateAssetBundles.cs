using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Build/Build Asset Bundles")]
    static void BuildAssetBundles()
    {
        string assetBundleDir = "Assets/AssetBundles";
        string assetBundleDir2 = Path.Combine(Application.streamingAssetsPath, "DLC"); //for testing

        if (!Directory.Exists(assetBundleDir))
        {
            Directory.CreateDirectory(assetBundleDir);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        BuildPipeline.BuildAssetBundles(assetBundleDir2, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);//for testing
    }
}
