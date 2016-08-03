using System;
using UnityEngine;
using TrackedRiderUtility;


public class VirginiaReelCar : BaseCar
{

	private const float radius = .4f;
	private const float timeSpentRotating = 3f;

	private float rotational_speed = 0f;
	private float maxRotation = 70f;


	protected override void Awake ()
	{
        this.carRotationAxis = transform.Find ("rotator");
		frontAxis = transform.Find ("frontAxis");
		backAxis = transform.Find ("backAxis");
		base.Awake ();
	}

	protected override void onRepositionAxis (float frontAxisPosition, float backAxisPosition)
	{
		if (this.train != null) {
			Vector3 tangent_axis = this.track.getTangentPoint (backAxisPosition);
			Vector3 next_tangent_axis = this.track.getTangentPoint (frontAxisPosition);

			Vector3 normal_axis = this.track.getNormalPoint (backAxisPosition);
			Vector3 binormal = Vector3.Cross (normal_axis, tangent_axis).normalized;


			TrackSegment4 track = this.track.trackSegments [(int)backAxisPosition];
			if (!(track is Station)) {
				float angle = MathHelper.AngleSigned (tangent_axis, next_tangent_axis, normal_axis);
				rotational_speed += ((Mathf.Sign (angle) * Mathf.Sin (Mathf.Abs (angle)) * this.train.velocity * Time.deltaTime * 2f) / (.2f * Mathf.PI)) * Mathf.Rad2Deg * Time.deltaTime;
                if(Mathf.Abs (angle) < .2f)
                rotational_speed -= this.rotational_speed * .6f * Time.deltaTime;

                if (Mathf.Abs(this.rotational_speed) > maxRotation )
                    this.rotational_speed = maxRotation * Mathf.Sign(this.rotational_speed);

				float additional_rotation = ((Mathf.Sign (angle) * Mathf.Sin (Mathf.Abs (angle)) * this.train.velocity) / (.2f * Mathf.PI)) * Mathf.Rad2Deg * Time.deltaTime;

                this.carRotationAxis.localRotation *= Quaternion.AngleAxis (additional_rotation+rotational_speed, Vector3.up);
			} else {
				rotational_speed -= this.rotational_speed * .6f * Time.deltaTime;

                if (Quaternion.Angle (Quaternion.identity, this.carRotationAxis.localRotation) > 5f) {
                    this.carRotationAxis.localRotation *= Quaternion.AngleAxis (rotational_speed + Time.deltaTime * 40f, Vector3.up);
				}
			}
		}


		base.onRepositionAxis (frontAxisPosition, backAxisPosition);


	}

	public override bool isReadyForLettingGuestsInAndOut ()
	{
        if (Quaternion.Angle (Quaternion.identity, this.carRotationAxis.localRotation) > 5f) {
			return false;
		}
		return base.isReadyForLettingGuestsInAndOut ();
	}


}
