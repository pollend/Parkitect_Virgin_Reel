using UnityEngine;
using System.Collections.Generic;

public class Main : IMod
{
    public string Identifier { get; set; }
	public static AssetBundleManager AssetBundleManager = null;
    public static Configuration Configeration = null;
    public static string HASH = "asdfasdfabeawefawefv";

    private List<UnityEngine.Object> registeredObjects = new List<UnityEngine.Object>();

    GameObject hider;
    public void onEnabled()
    {
        hider = new GameObject ();

		if (Main.AssetBundleManager == null) {

			AssetBundleManager = new AssetBundleManager (this);
		}

        TrackedRide selected = null;
        foreach (Attraction t in AssetManager.Instance.getAttractionObjects ()) {
            if (t.getUnlocalizedName() == "Wooden Coaster") {
                selected = (TrackedRide)t;
                break;     
            }
        }

        TrackedRide trackRider = UnityEngine.Object.Instantiate (selected);

        trackRider.canChangeCarRotation = false;
        trackRider.meshGenerator =  ScriptableObject.CreateInstance<VirginiaReelTrackGenerator> ();
        trackRider.meshGenerator.stationPlatformGO = selected.meshGenerator.stationPlatformGO;
        trackRider.meshGenerator.material = selected.meshGenerator.material;
        trackRider.meshGenerator.liftMaterial = selected.meshGenerator.liftMaterial;
        trackRider.meshGenerator.frictionWheelsGO = selected.meshGenerator.frictionWheelsGO;
        trackRider.meshGenerator.supportInstantiator =selected.meshGenerator.supportInstantiator;
        trackRider.meshGenerator.crossBeamGO = this.SetUV(Main.AssetBundleManager.SideCrossBeamGo,15,14);
        

        Color[] colors = new Color[] { new Color(63f / 255f, 46f / 255f, 37f / 255f, 1), new Color(43f / 255f, 35f / 255f, 35f / 255f, 1), new Color(90f / 255f, 90f / 255f, 90f / 255f, 1) };
        trackRider.meshGenerator.customColors = colors;
        trackRider.meshGenerator.customColors = colors;
        trackRider.setDisplayName("Virginia Reel");
        trackRider.price = 1200;
        trackRider.name = "virginia_reel_go" + HASH ;
        trackRider.maxBankingAngle = 0;
        trackRider.min90CurveSize = 1;



        AssetManager.Instance.registerObject (trackRider);
        registeredObjects.Add (trackRider);


        List<CoasterCarInstantiator> trains = new List<CoasterCarInstantiator> ();
       

        trains.Add (this.AddCar(
            Main.AssetBundleManager.CartGo,
            "Reel",
            "Reel_Car",
            new Color[] { new Color(71f / 255, 71f / 255, 71f / 255), new Color(176f / 255, 7f / 255, 7f / 255), new Color(26f / 255, 26f / 255, 26f / 255)},
            .3f,.1f));

        

        trackRider.carTypes = trains.ToArray();

        hider.SetActive (false);

	}

    private CoasterCarInstantiator AddCar(GameObject model,string display,string name,Color[] colors,float frontOffset, float backOffset)
    {
        //get car
        GameObject frontcarGo = UnityEngine.GameObject.Instantiate(model);
        Rigidbody frontcarRigid = frontcarGo.AddComponent<Rigidbody> ();
        frontcarRigid.isKinematic = true;
        frontcarGo.AddComponent<BoxCollider> ();

        //add Component
        VirginiaReelCar frontCar = frontcarGo.AddComponent<VirginiaReelCar> ();
        MakeRecolorble(frontcarGo, "CustomColorsDiffuse", colors);
        frontCar.name = name + "_Front" + HASH;

        frontCar.offsetFront = frontOffset;
        frontCar.offsetBack = backOffset;

        CoasterCarInstantiator coasterCarInstantiator = ScriptableObject.CreateInstance<CoasterCarInstantiator> ();

        coasterCarInstantiator.name = name+"@CoasterCarInstantiator" + HASH;
        coasterCarInstantiator.defaultTrainLength = 1;
        coasterCarInstantiator.maxTrainLength = 1;
        coasterCarInstantiator.minTrainLength = 1;
        coasterCarInstantiator.frontCarGO = frontcarGo;
        coasterCarInstantiator.displayName = display;
       


        //Restraints
        RestraintRotationController controllerFront = frontcarGo.AddComponent<RestraintRotationController>();
        controllerFront.closedAngles = new Vector3(0, 0, 120);
        frontcarGo.transform.parent = hider.transform;

        AssetManager.Instance.registerObject (coasterCarInstantiator);
        registeredObjects.Add (coasterCarInstantiator);


        //register cars
        AssetManager.Instance.registerObject (frontCar);
        registeredObjects.Add (frontCar);

        return coasterCarInstantiator;

    }

    private void MakeRecolorble(GameObject GO, string shader, Color[] colors)
    {
        CustomColors cc = GO.AddComponent<CustomColors>();
        cc.setColors(colors);

        foreach (Material material in AssetManager.Instance.objectMaterials)
        {
            if (material.name == shader)
            {
                SetMaterial(GO, material);
                break;
            }
        }

    }

    private void SetMaterial(GameObject go, Material material)
    {
        // Go through all child objects and recolor     
        Renderer[] renderCollection;
        renderCollection = go.GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renderCollection)
        {
            render.sharedMaterial = material;
        }
    }

    public void onDisabled()
    {
        foreach(UnityEngine.Object o in registeredObjects)
        {
            AssetManager.Instance.unregisterObject (o);
        }
        UnityEngine.GameObject.DestroyImmediate (hider);
	}
    GameObject SetUV(GameObject GO, int gridX, int gridY)
    {
        Mesh mesh = GO.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(0.0625f * ((float)gridX + 0.5f), 1f - 0.0625f * ((float)gridY + 0.5f));
        }
        mesh.uv = uvs;
        return GO;
    }
    public string Name
    {
        get { return "Virginia Reel"; }
    }

    public string Description
    {
        get { return "The Virginia reel  uses side friction like tracks. The tubs, have inward facing seating which spin freely on a chassis. The tubs are spun as they contacted the edges of the trough."; }
    }


	public string Path { get; set; }

}

