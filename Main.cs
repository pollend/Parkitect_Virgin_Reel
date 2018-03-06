using System.Linq;
using TrackedRiderUtility;
using UnityEngine;

public class Main : IMod
{
	public static AssetBundleManager AssetBundleManager;
  
    private TrackRiderBinder binder;

    public void onEnabled()
    {
        
		if (AssetBundleManager == null) {

			AssetBundleManager = new AssetBundleManager (this);
		}

        binder = new TrackRiderBinder ("ed7f0bf864bee459f34bc3e1b426c04e");
        TrackedRide trackedRide = binder.RegisterTrackedRide<TrackedRide> ("Wooden Coaster","VirginiaReelCoaster", "Virginia Reel Coaster");
        VirginiaReelTrackGenerator trackGenerator = binder.RegisterMeshGenerator<VirginiaReelTrackGenerator> (trackedRide);
        TrackRideHelper.PassMeshGeneratorProperties (TrackRideHelper.GetTrackedRide ("Wooden Coaster").meshGenerator,trackedRide.meshGenerator);

        trackGenerator.crossBeamGO = GameObjectHelper.SetUV (AssetBundleManager.SideCrossBeamGo, 15, 14);

        trackedRide.price = 1200;
        trackedRide.maxBankingAngle = 0;
        trackedRide.min90CurveSize = 1;
        trackedRide.carTypes = new CoasterCarInstantiator[]{ };
        trackedRide.meshGenerator.customColors = new[] { new Color(63f / 255f, 46f / 255f, 37f / 255f, 1), new Color(43f / 255f, 35f / 255f, 35f / 255f, 1), new Color(90f / 255f, 90f / 255f, 90f / 255f, 1) };
        trackedRide.canChangeCarRotation = true;

        CoasterCarInstantiator coasterCarInstantiator = binder.RegisterCoasterCarInstaniator<CoasterCarInstantiator> (trackedRide, "VirginiaReelInstantiator", "Virginia Reel Car", 1, 1, 1);
        VirginiaReelCar virginiaReelCar =  binder.RegisterCar<VirginiaReelCar> ( AssetBundleManager.CartGo,"VirginiaReelCar", .3f,.1f,true, new[] { 
            new Color(71f / 255, 71f / 255, 71f / 255), 
            new Color(176f / 255, 7f / 255, 7f / 255), 
            new Color(26f / 255, 26f / 255, 26f / 255)}
        );
        coasterCarInstantiator.vehicleGO = virginiaReelCar;
        coasterCarInstantiator.vehicleGO.gameObject.AddComponent<RestraintRotationController>().closedAngles = new Vector3(0,0,120);

        binder.Apply ();

        //deprecatedMappings
        string oldHash = "asdfasdfabeawefawefv";

        GameObjectHelper.RegisterDeprecatedMapping ("virginia_reel_go" + oldHash,trackedRide.name);
        GameObjectHelper.RegisterDeprecatedMapping ("Reel_Car@CoasterCarInstantiator" + oldHash, coasterCarInstantiator.name);
        GameObjectHelper.RegisterDeprecatedMapping ("Reel_Car_Front"+ oldHash, virginiaReelCar.name);

	}

   



    public void onDisabled()
    {
        binder.Unload ();
	}

    public string Name => "Virginia Reel";

    public string Description => "The Virginia reel  uses side friction like tracks. The tubs, have inward facing seating which spin freely on a chassis. The tubs are spun as they contacted the edges of the trough.";

	string IMod.Identifier => "VirginiaReel";
	
	
	public string Path
	{
		get
		{
			return ModManager.Instance.getModEntries().First(x => x.mod == this).path;
		}
	}
}

