using System;
using UnityEngine;

public class VirginiaReelTrackGenerator: MeshGenerator
{
    private BoxExtruder trackBase;
    private BoxExtruder collisionMeshExtruder;

    private BoxExtruder leftRail;
    private BoxExtruder rightRail;

    private BoxExtruder leftRailInner;
    private BoxExtruder rightRailInner;

    private const float railOffset  = .28f;

    protected override void Initialize()
    {
        base.Initialize();
        base.trackWidth = 0.27f * 2.0f;
        this.crossBeamSpacing = 0.3f;
    }


    public override void prepare(TrackSegment4 trackSegment, GameObject putMeshOnGO)
    {
        base.prepare(trackSegment, putMeshOnGO);
        putMeshOnGO.GetComponent<Renderer>().sharedMaterial = this.material;

        trackBase = new BoxExtruder (.7172f, .0292f);
        trackBase.setUV (15, 15);

        leftRail = new BoxExtruder (.06718f, .05529f);
        rightRail = new BoxExtruder (.06718f, .05529f);
        leftRail.setUV (14, 15);
        rightRail.setUV (14, 15);

        leftRailInner = new BoxExtruder (.005535f, .04158f);
        rightRailInner = new BoxExtruder (.005535f, .04158f);
        leftRailInner.setUV (15, 14);
        rightRailInner.setUV (15, 14);



        this.collisionMeshExtruder = new BoxExtruder(base.trackWidth, 0.022835f);
        this.buildVolumeMeshExtruder = new BoxExtruder(base.trackWidth, 0.7f);
        this.buildVolumeMeshExtruder.closeEnds = true;
    }

    public override void sampleAt(TrackSegment4 trackSegment, float t)
    {
        base.sampleAt(trackSegment, t);
        Vector3 normal = trackSegment.getNormal(t);
        Vector3 trackPivot = base.getTrackPivot(trackSegment.getPoint(t), normal);
        Vector3 tangentPoint = trackSegment.getTangentPoint(t);
        Vector3 binormal = Vector3.Cross(normal, tangentPoint).normalized;

        Vector3 midPoint = trackPivot - normal * this.getCenterPointOffsetY();

        trackBase.extrude (trackPivot, tangentPoint , normal);

        leftRail.extrude (trackPivot - normal*(leftRail.height/2.0f + trackBase.height/2.0f) + binormal * railOffset, tangentPoint, normal);
        rightRail.extrude (trackPivot - normal*(rightRail.height/2.0f + trackBase.height/2.0f) - binormal * railOffset, tangentPoint, normal);

        leftRailInner.extrude (trackPivot - normal*(leftRailInner.height/2.0f + trackBase.height/2.0f) + binormal * (railOffset-(leftRail.width/2.0f + leftRailInner.width/2.0f)), tangentPoint, normal);
        rightRailInner.extrude (trackPivot - normal*(leftRailInner.height/2.0f + trackBase.height/2.0f) - binormal * (railOffset-(rightRail.width/2.0f + leftRailInner.width/2.0f)), tangentPoint, normal);

        this.collisionMeshExtruder.extrude(trackPivot, tangentPoint, normal);
        if (this.liftExtruder != null)
        {
            this.liftExtruder.extrude(midPoint, tangentPoint, normal);
        }
    }

    public override void afterExtrusion (TrackSegment4 trackSegment, GameObject putMeshOnGO)
    {
        
        base.afterExtrusion (trackSegment, putMeshOnGO);
    }

    public override Mesh getMesh (GameObject putMeshOnGO)
    {
        return default(MeshCombiner).start().add(new Extruder[]
            {
                trackBase,
                leftRail,
                rightRail,
                leftRailInner,
                rightRailInner
            }).end(putMeshOnGO.transform.worldToLocalMatrix);
    }

    public override Mesh getCollisionMesh(GameObject putMeshOnGO)
    {
        return this.collisionMeshExtruder.getMesh(putMeshOnGO.transform.worldToLocalMatrix);
    }

    public override Extruder getBuildVolumeMeshExtruder()
    {
        return this.buildVolumeMeshExtruder;
    }

    public override float trackOffsetY()
    {
        return 0.2225f;
    }

    public override float getSupportOffsetY()
    {
        return 0.05f;
    }

    public override float getTunnelOffsetY()
    {
        return 0.15f;
    }

    public override float getTunnelWidth()
    {
        return 0.7f;
    }

    public override float getTunnelHeight()
    {
        return 0.95f;
    }

    protected override float railHalfHeight()
    {
        return 0.022835f;
    }
    public override float getCenterPointOffsetY ()
    {
        return .0592f;
    }
}
