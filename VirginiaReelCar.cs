using System;
using UnityEngine;


public class VirginiaReelCar : Car
{

    private const float radius = .4f;


    private Transform rotator;
    private float rotational_speed = 0f;
    private Vector3 previous_dir = Vector3.zero;
    private Vector3 force = Vector3.zero;

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
        Vector3 normal_axis = this.track.getNormalPoint (backAxisPosition);
        Vector3 binormal = Vector3.Cross(normal_axis, tangent_axis).normalized;

        float angle = MathHelper.AngleSigned (previous_dir,tangent_axis, normal_axis) ;
        if (Mathf.Abs (angle) > .01f) {
            rotational_speed = ((Mathf.Sign (angle) * this.train.velocity * Time.deltaTime) / (.4f * Mathf.PI)) * Mathf.Rad2Deg;
        }
        else
        {
            rotational_speed -= this.rotational_speed * .99f * Time.deltaTime;
        }
        this.rotator.localRotation *= Quaternion.AngleAxis (rotational_speed, Vector3.up);
        base.onRepositionAxis (frontAxisPosition, backAxisPosition);

        previous_dir = tangent_axis;

  
    }


}
