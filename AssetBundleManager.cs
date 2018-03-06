using System;
using UnityEngine;
using System.Collections.Generic;

public class AssetBundleManager
{
	private Main Main {get;set;}
    public GameObject CartGo;
    public GameObject SideCrossBeamGo;
	private readonly Main _main;
	private AssetBundle assetBundle;
	public AssetBundleManager (Main main)
	{
		_main = main;
		var dsc = System.IO.Path.DirectorySeparatorChar;
		assetBundle = AssetBundle.LoadFromFile( _main.Path + dsc + "assetbundle" + dsc + "reel");

        CartGo =  assetBundle.LoadAsset<GameObject> ("Cart");
        SideCrossBeamGo =  assetBundle.LoadAsset<GameObject> ("SideCrossBeams");
    }

}


