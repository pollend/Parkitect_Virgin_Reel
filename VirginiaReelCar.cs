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

using TrackedRiderUtility;
using UnityEngine;

namespace VirginiaReel
{
    public class VirginiaReelCar : BaseCar
    {
        private const float radius = .4f;
        private const float timeSpentRotating = 3f;
        private readonly float maxRotation = 70f;

        private float rotational_speed;


        protected override void Awake()
        {
            carRotationAxis = transform.Find("rotator");
            frontAxis = transform.Find("frontAxis");
            backAxis = transform.Find("backAxis");
            base.Awake();
        }


        public override void reposition(float deltaTime, float position, int lane, Car previousCar)
        {
            if (train != null)
            {
                var tangentAxis = this.track.getTangentPoint(currentTrackPosition);
                var nextTangentAxis = this.track.getTangentPoint(currentBackTrackPosition);

                var normalAxis = this.track.getNormalPoint(currentTrackPosition);
                var binormal = Vector3.Cross(normalAxis, tangentAxis).normalized;


                var track = this.track.trackSegments[(int) currentTrackPosition];
                if (!(track is Station))
                {
                    var angle = MathHelper.AngleSigned(tangentAxis, nextTangentAxis, normalAxis);
                    rotational_speed -=
                        Mathf.Sign(angle) * Mathf.Sin(Mathf.Abs(angle)) * train.velocity * Time.deltaTime * 2f /
                        (.2f * Mathf.PI) * Mathf.Rad2Deg * Time.deltaTime;
                    if (Mathf.Abs(angle) < .2f)
                        rotational_speed -= rotational_speed * .6f * Time.deltaTime;

                    if (Mathf.Abs(rotational_speed) > maxRotation)
                        rotational_speed = maxRotation * Mathf.Sign(rotational_speed);

                    var additionalRotation =
                        Mathf.Sign(angle) * Mathf.Sin(Mathf.Abs(angle)) * train.velocity / (.2f * Mathf.PI) *
                        Mathf.Rad2Deg *
                        Time.deltaTime;

                    carRotationAxis.localRotation *=
                        Quaternion.AngleAxis(additionalRotation + rotational_speed, Vector3.up);
                }
                else
                {
                    rotational_speed -= rotational_speed * .6f * Time.deltaTime;

                    if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f)
                        carRotationAxis.localRotation *=
                            Quaternion.AngleAxis(rotational_speed + Time.deltaTime * 40f, Vector3.up);
                }
            }

            base.reposition(deltaTime, position, lane, previousCar);
        }


        public override bool isReadyForLettingGuestsInAndOut()
        {
            if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f) return false;
            return base.isReadyForLettingGuestsInAndOut();
        }
    }
}