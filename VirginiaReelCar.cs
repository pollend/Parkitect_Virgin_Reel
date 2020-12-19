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

        private float rotational_speed = 0;

        private float previousPosition = 0;

        protected override void Awake()
        {
            carRotationAxis = transform.Find("rotator");
            base.Awake();
        }


        public override void reposition(float deltaTime, float position, int lane, Car previousCar)
        {
            base.reposition(deltaTime, position, lane, previousCar);
            if (train != null)
            {
                var previousTangent = this.track.getTangentPoint(previousPosition);
                var currentTangent = this.track.getTangentPoint(position);
                previousPosition = position;

                var normalAxis = this.track.getNormalPoint(currentTrackPosition);
                var track = this.track.trackSegments[(int) currentTrackPosition];
                if (!(track is Station))
                {
                    var deltaAngle = MathHelper.AngleSigned(currentTangent,previousTangent, normalAxis);

                    rotational_speed -= ((Mathf.Sign(deltaAngle) * Mathf.Sin(Mathf.Abs(deltaAngle)) * train.velocity) /
                                        (.4f * Mathf.PI)) * deltaTime;
                    rotational_speed -= rotational_speed * .9f * deltaTime;

                    if (Mathf.Abs(rotational_speed) > maxRotation)
                        rotational_speed = maxRotation * Mathf.Sign(rotational_speed);

                    var additionalRotation =
                        ((Mathf.Sign(deltaAngle) * Mathf.Sin(Mathf.Abs(deltaAngle)) * train.velocity) / (.4f * Mathf.PI)) * deltaTime;

                    carRotationAxis.localRotation *=
                        Quaternion.AngleAxis((additionalRotation + rotational_speed) * Mathf.Rad2Deg, Vector3.up);
                }
                else
                {
                    rotational_speed -= rotational_speed * .9f * deltaTime;
                    if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f)
                        carRotationAxis.localRotation *=
                            Quaternion.AngleAxis(rotational_speed + Time.deltaTime * 40f, Vector3.up);
                }
            }

        }



        public override bool isReadyForLettingGuestsInAndOut()
        {
            if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f) return false;
            return base.isReadyForLettingGuestsInAndOut();
        }
    }
}
