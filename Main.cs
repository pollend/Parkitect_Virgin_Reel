/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
using System.Linq;
using TrackedRiderUtility;
using UnityEngine;

namespace VirginiaReel
{
    public class Main : IMod
    {
        private TrackRiderBinder binder;
        private GameObject hider;


        public string Path
        {
            get { return ModManager.Instance.getModEntries().First(x => x.mod == this).path; }
        }
        
        private GameObject ProxyObject(GameObject gameObject)
        {
            gameObject.transform.SetParent(hider.transform);
            return gameObject;
        }
        
        public void onEnabled()
        {
            hider = new GameObject();
            hider.SetActive(false);
            
            AssetBundleManager assetBundleManager = new AssetBundleManager(this);

            binder = new TrackRiderBinder("ed7f0bf864bee459f34bc3e1b426c04e");
            var trackedRide =
                binder.RegisterTrackedRide<TrackedRide>("Wooden Coaster", "VirginiaReelCoaster", "Virginia Reel Coaster");
            var trackGenerator =
                binder.RegisterMeshGenerator<VirginiaReelTrackGenerator>(trackedRide);
            TrackRideHelper.PassMeshGeneratorProperties(TrackRideHelper.GetTrackedRide("Wooden Coaster").meshGenerator,
                trackedRide.meshGenerator);

            trackGenerator.crossBeamGO = GameObjectHelper.SetUV(ProxyObject(Object.Instantiate(assetBundleManager.SideCrossBeamGo)), 15, 14);

            trackedRide.price = 1200;
            trackedRide.maxBankingAngle = 0;
            trackedRide.min90CurveSize = 1;
            trackedRide.carTypes = new CoasterCarInstantiator[] { };
            trackedRide.meshGenerator.customColors = new[]
            {
                new Color(63f / 255f, 46f / 255f, 37f / 255f, 1), new Color(43f / 255f, 35f / 255f, 35f / 255f, 1),
                new Color(90f / 255f, 90f / 255f, 90f / 255f, 1)
            };
            trackedRide.canChangeCarRotation = true;

            var coasterCarInstantiator =
                binder.RegisterCoasterCarInstaniator<CoasterCarInstantiator>(trackedRide, "VirginiaReelInstantiator",
                    "Virginia Reel Car", 1, 1, 1);
            var virginiaReelCar = binder.RegisterCar<VirginiaReelCar>(ProxyObject(Object.Instantiate(assetBundleManager.CartGo)),
                "VirginiaReelCar", .3f, .1f, true, new[]
                {
                    new Color(71f / 255, 71f / 255, 71f / 255),
                    new Color(176f / 255, 7f / 255, 7f / 255),
                    new Color(26f / 255, 26f / 255, 26f / 255)
                }
            );
            coasterCarInstantiator.vehicleGO = virginiaReelCar;
            coasterCarInstantiator.vehicleGO.gameObject.AddComponent<RestraintRotationController>().closedAngles =
                new Vector3(0, 0, 120);

            binder.Apply();

            //deprecatedMappings
            var oldHash = "asdfasdfabeawefawefv";

            GameObjectHelper.RegisterDeprecatedMapping("virginia_reel_go" + oldHash, trackedRide.name);
            GameObjectHelper.RegisterDeprecatedMapping("Reel_Car@CoasterCarInstantiator" + oldHash,
                coasterCarInstantiator.name);
            GameObjectHelper.RegisterDeprecatedMapping("Reel_Car_Front" + oldHash, virginiaReelCar.name);
        }

        public void onDisabled()
        {
            binder.Unload();
        }

        public string Name => "Virginia Reel";

        public string Description =>
            "The Virginia reel  uses side friction like tracks. The tubs, have inward facing seating which spin freely on a chassis. The tubs are spun as they contacted the edges of the trough.";

        string IMod.Identifier => "VirginiaReel";
    }
}