using TrackedRiderUtility;
using UnityEngine;

public class VirginiaReelCar : BaseCar
{

	private const float radius = .4f;
	private const float timeSpentRotating = 3f;

	private float rotational_speed;
	private float maxRotation = 70f;


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
			Vector3 tangentAxis = this.track.getTangentPoint(currentTrackPosition);
			Vector3 nextTangentAxis = this.track.getTangentPoint(currentBackTrackPosition);

			Vector3 normalAxis = this.track.getNormalPoint(currentTrackPosition);
			Vector3 binormal = Vector3.Cross(normalAxis, tangentAxis).normalized;


			TrackSegment4 track = this.track.trackSegments[(int) currentTrackPosition];
			if (!(track is Station))
			{
				float angle = MathHelper.AngleSigned(tangentAxis, nextTangentAxis, normalAxis);
				rotational_speed +=
				((Mathf.Sign(angle) * Mathf.Sin(Mathf.Abs(angle)) * train.velocity * Time.deltaTime * 2f) /
				 (.2f * Mathf.PI)) * Mathf.Rad2Deg * Time.deltaTime;
				if (Mathf.Abs(angle) < .2f)
					rotational_speed -= rotational_speed * .6f * Time.deltaTime;

				if (Mathf.Abs(rotational_speed) > maxRotation)
					rotational_speed = maxRotation * Mathf.Sign(rotational_speed);

				float additionalRotation =
					((Mathf.Sign(angle) * Mathf.Sin(Mathf.Abs(angle)) * train.velocity) / (.2f * Mathf.PI)) * Mathf.Rad2Deg *
					Time.deltaTime;

				carRotationAxis.localRotation *= Quaternion.AngleAxis(additionalRotation + rotational_speed, Vector3.up);
			}
			else
			{
				rotational_speed -= rotational_speed * .6f * Time.deltaTime;

				if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f)
				{
					carRotationAxis.localRotation *= Quaternion.AngleAxis(rotational_speed + Time.deltaTime * 40f, Vector3.up);
				}
			}
		}
		base.reposition(deltaTime,position,lane,previousCar);
	}



	public override bool isReadyForLettingGuestsInAndOut()
	{
		if (Quaternion.Angle(Quaternion.identity, carRotationAxis.localRotation) > 5f)
		{
			return false;
		}
		return base.isReadyForLettingGuestsInAndOut();
	}

}
