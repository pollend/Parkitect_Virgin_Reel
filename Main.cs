using UnityEngine;
using System.Collections.Generic;

public class Main : IMod
{
    public string Identifier { get; set; }
	public static AssetBundleManager AssetBundleManager = null;
    public static Configuration Configeration = null;
    public static string HASH = "asdfawujeba8whe9jnimpiasnd";

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
            if (t.getUnlocalizedName() == "Ghost Mansion Ride") {
                selected = (TrackedRide)t;
                break;
                    
            }
        }

        TrackedRide trackRider = UnityEngine.Object.Instantiate (selected);

        trackRider.canChangeCarRotation = false;
        trackRider.meshGenerator = selected.meshGenerator;// ScriptableObject.CreateInstance<MinetrainTrackGenerator> ();
        trackRider.meshGenerator.stationPlatformGO = selected.meshGenerator.stationPlatformGO;
        trackRider.meshGenerator.material = selected.meshGenerator.material;
        trackRider.meshGenerator.liftMaterial = selected.meshGenerator.liftMaterial;
        trackRider.meshGenerator.frictionWheelsGO = selected.meshGenerator.frictionWheelsGO;
        trackRider.meshGenerator.supportInstantiator =selected.meshGenerator.supportInstantiator;
        trackRider.meshGenerator.crossBeamGO = selected.meshGenerator.crossBeamGO;
        trackRider.targetVelocity = 7f;
        trackRider.maximumVelocity = 7f;

        Color[] colors = new Color[] { new Color(63f / 255f, 46f / 255f, 37f / 255f, 1), new Color(43f / 255f, 35f / 255f, 35f / 255f, 1), new Color(90f / 255f, 90f / 255f, 90f / 255f, 1) };
        trackRider.meshGenerator.customColors = colors;
        trackRider.meshGenerator.customColors = colors;
        trackRider.setDisplayName("Car Ride");
        trackRider.price = 1200;
        trackRider.name = "car_ride_coaster_GO" + HASH ;
        AssetManager.Instance.registerObject (trackRider);
        registeredObjects.Add (trackRider);


        List<CoasterCarInstantiator> trains = new List<CoasterCarInstantiator> ();
       

        trains.Add (this.AddCar(
            Main.AssetBundleManager.MouseCarGo,
            "Mouse Car",
            "Mouse_Car",
            new Color[] { new Color(71f / 255, 71f / 255, 71f / 255), new Color(176f / 255, 7f / 255, 7f / 255), new Color(26f / 255, 26f / 255, 26f / 255),new Color(31f / 255, 31f / 255, 31f / 255)},
            .3f,.1f));

        trains.Add (this.AddCar(
            Main.AssetBundleManager.TruckGo,
            "Truck",
            "Truck_Car",
            new Color[] { new Color(68f / 255, 58f / 255, 50f / 255), new Color(176f / 255, 7f / 255, 7f / 255), new Color(55f / 255, 32f / 255, 12f / 255),new Color(61f / 255, 40f / 255, 19f / 255)},
            .3f,.35f));

        trains.Add (this.AddCar(
            Main.AssetBundleManager.SportsCarGo,
            "Sports Car",
            "Sports_Car",
            new Color[] { new Color(.949f, .141f, .145f), new Color(.925f, .937f, .231f), new Color(.754f, .754f, .754f),new Color(.788f,.788f, .788f)},
            .3f,.35f));
        

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
        CarCar frontCar = frontcarGo.AddComponent<CarCar> ();
        MakeRecolorble(frontcarGo, "CustomColorsDiffuse", colors);
        frontCar.name = name + "_Front" + HASH;

        frontCar.offsetFront = frontOffset;
        frontCar.offsetBack = backOffset;

        frontCar.Decorate (true);

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

    public string Name
    {
        get { return "Car Ride"; }
    }

    public string Description
    {
        get { return "A gental ride that follows a central guide rail. The cars are self powered and follow the main guide rail."; }
    }


	public string Path { get; set; }

}

