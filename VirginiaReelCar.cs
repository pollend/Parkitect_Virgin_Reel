using System;
using UnityEngine;


public class VirginiaReelCar : Car
{

    private const float radius = .4f;
    private const float timeSpentRotating = 3f;

    private Transform rotator;
    private float rotational_speed = 0f;
    private Vector3 previous_dir = Vector3.zero;
    private float maxRotation = 70f;

    public void Decorate(bool isFront)
    {
        backAxis = transform.Find ("backAxis");
        if (isFront)
            frontAxis = transform.Find ("frontAxis");
        

    }

    protected override void Awake ()
    {
        rotator = transform.Find ("rotator");
    
        base.Awake ();
    }

    protected override void onRepositionAxis (float frontAxisPosition, float backAxisPosition)
    {
        Vector3 tangent_axis = this.track.getTangentPoint (backAxisPosition);
        Vector3 next_tangent_axis = this.track.getTangentPoint (frontAxisPosition);

        Vector3 normal_axis = this.track.getNormalPoint (backAxisPosition);
        Vector3 binormal = Vector3.Cross (normal_axis, tangent_axis).normalized;


        TrackSegment4 track = this.track.trackSegments [(int)backAxisPosition];

        if (!(track is Station)) {    
            float angle = MathHelper.AngleSigned (tangent_axis, next_tangent_axis, normal_axis);
            rotational_speed += ((Mathf.Sign (angle) * Mathf.Sin (Mathf.Abs (angle)) * this.train.velocity* Time.deltaTime * 2f) / (.2f * Mathf.PI)) * Mathf.Rad2Deg * Time.deltaTime;
            rotational_speed -= this.rotational_speed * .6f * Time.deltaTime;
            
            if (this.rotational_speed > maxRotation * Time.deltaTime)
                this.rotational_speed = maxRotation * Time.deltaTime;
            this.rotator.localRotation *= Quaternion.AngleAxis (rotational_speed, Vector3.up);
       } 
        else
        {
            rotational_speed -= this.rotational_speed * .6f * Time.deltaTime;

            if (Quaternion.Angle (Quaternion.identity, this.rotator.localRotation) > 5f) {
                this.rotator.localRotation *= Quaternion.AngleAxis (rotational_speed + Time.deltaTime * 40f, Vector3.up);
            }
                // this.rotator.localRotation = Quaternion.Lerp (this.rotator.localRotation, Quaternion.identity, (Time.time - startTime) * 1.0f);
        }


        base.onRepositionAxis (frontAxisPosition, backAxisPosition);

        previous_dir = tangent_axis;

  
    }

    public override bool isReadyForLettingGuestsInAndOut ()
    {
        if (Quaternion.Angle (Quaternion.identity, this.rotator.localRotation) > 5f) {
            return false;
        }
        return base.isReadyForLettingGuestsInAndOut ();
    }


}
