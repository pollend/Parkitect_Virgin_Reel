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
using UnityEngine;

namespace VirginiaReel
{
    public class VirginiaReelTrackGenerator : MeshGenerator
    {
        private const float railOffset = .28f;
        private BoxExtruder collisionMeshExtruder;

        private BoxExtruder leftRail;

        private BoxExtruder leftRailInner;
        private BoxExtruder rightRail;
        private BoxExtruder rightRailInner;
        private BoxExtruder trackBase;

        protected override void Initialize()
        {
            base.Initialize();
            trackWidth = 0.27f * 2.0f;
            crossBeamSpacing = 0.3f;
        }


        public override void prepare(TrackSegment4 trackSegment, GameObject putMeshOnGO)
        {
            base.prepare(trackSegment, putMeshOnGO);
            putMeshOnGO.GetComponent<Renderer>().sharedMaterial = material;

            trackBase = new BoxExtruder(.7172f, .0292f);
            trackBase.setUV(15, 15);

            leftRail = new BoxExtruder(.06718f, .05529f);
            rightRail = new BoxExtruder(.06718f, .05529f);
            leftRail.setUV(14, 15);
            rightRail.setUV(14, 15);

            leftRailInner = new BoxExtruder(.005535f, .04158f);
            rightRailInner = new BoxExtruder(.005535f, .04158f);
            leftRailInner.setUV(15, 14);
            rightRailInner.setUV(15, 14);


            collisionMeshExtruder = new BoxExtruder(trackWidth, 0.022835f);
            buildVolumeMeshExtruder = new BoxExtruder(trackWidth, 0.7f);
            buildVolumeMeshExtruder.closeEnds = true;
        }

        public override void sampleAt(TrackSegment4 trackSegment, float t)
        {
            base.sampleAt(trackSegment, t);
            var normal = trackSegment.getNormal(t);
            var trackPivot = getTrackPivot(trackSegment.getPoint(t, 0), normal);
            var tangentPoint = trackSegment.getTangentPoint(t);
            var binormal = Vector3.Cross(normal, tangentPoint).normalized;

            var midPoint = trackPivot - normal * getCenterPointOffsetY();

            trackBase.extrude(trackPivot, tangentPoint, normal);

            leftRail.extrude(
                trackPivot - normal * (leftRail.height / 2.0f + trackBase.height / 2.0f) + binormal * railOffset,
                tangentPoint, normal);
            rightRail.extrude(
                trackPivot - normal * (rightRail.height / 2.0f + trackBase.height / 2.0f) - binormal * railOffset,
                tangentPoint, normal);

            leftRailInner.extrude(
                trackPivot - normal * (leftRailInner.height / 2.0f + trackBase.height / 2.0f) +
                binormal * (railOffset - (leftRail.width / 2.0f + leftRailInner.width / 2.0f)), tangentPoint, normal);
            rightRailInner.extrude(
                trackPivot - normal * (leftRailInner.height / 2.0f + trackBase.height / 2.0f) -
                binormal * (railOffset - (rightRail.width / 2.0f + leftRailInner.width / 2.0f)), tangentPoint, normal);

            collisionMeshExtruder.extrude(trackPivot, tangentPoint, normal);
            if (liftExtruder != null) liftExtruder.extrude(midPoint, tangentPoint, normal);
        }

        public override Mesh getMesh(GameObject putMeshOnGO)
        {
            return MeshCombiner.start().add(trackBase, leftRail, rightRail, leftRailInner, rightRailInner)
                .end(putMeshOnGO.transform.worldToLocalMatrix);
        }

        public override Mesh getCollisionMesh(GameObject putMeshOnGO)
        {
            return collisionMeshExtruder.getMesh(putMeshOnGO.transform.worldToLocalMatrix);
        }

        public override Extruder getBuildVolumeMeshExtruder()
        {
            return buildVolumeMeshExtruder;
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

        public override float getTunnelWidth(TrackSegment4 trackSegment, float t)
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

        public override float getCenterPointOffsetY()
        {
            return .0592f;
        }
    }
}