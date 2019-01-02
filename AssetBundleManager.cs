using UnityEngine;
using VirginiaReel;

public class AssetBundleManager
{
    private readonly Main _main;
    private readonly AssetBundle assetBundle;
    public GameObject CartGo;
    public GameObject SideCrossBeamGo;

    public AssetBundleManager(Main main)
    {
        _main = main;
        var dsc = System.IO.Path.DirectorySeparatorChar;
        assetBundle = AssetBundle.LoadFromFile(_main.Path + dsc + "assetbundle" + dsc + "reel");

        CartGo = assetBundle.LoadAsset<GameObject>("Cart");
        SideCrossBeamGo = assetBundle.LoadAsset<GameObject>("SideCrossBeams");

        assetBundle.Unload(false);
    }

    private Main Main { get; set; }
}