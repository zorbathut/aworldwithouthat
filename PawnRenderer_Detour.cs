using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AWorldWithoutHat
{
    public static class PawnRenderer_Detour
    {
        public static void RenderPawnInternal(this PawnRenderer renderer, Vector3 rootLoc, Quaternion quat, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType = RotDrawMode.Fresh, bool portrait = false)
        {
            Pawn rendererPawn = renderer.GetFieldViaReflection<Pawn>("pawn");

            if (!renderer.graphics.AllResolved)
            {
                renderer.graphics.ResolveAllGraphics();
            }
            Mesh mesh = null;
            if (renderBody)
            {
                Vector3 loc = rootLoc;
                loc.y += 0.005f;
                if (bodyDrawType == RotDrawMode.Dessicated && !rendererPawn.RaceProps.Humanlike && renderer.graphics.dessicatedGraphic != null && !portrait)
                {
                    renderer.graphics.dessicatedGraphic.Draw(loc, bodyFacing, rendererPawn);
                }
                else
                {
                    if (rendererPawn.RaceProps.Humanlike)
                    {
                        mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                    }
                    else
                    {
                        mesh = renderer.graphics.nakedGraphic.MeshAt(bodyFacing);
                    }
                    List<Material> list = renderer.graphics.MatsBodyBaseAt(bodyFacing, bodyDrawType);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Material damagedMat = renderer.graphics.flasher.GetDamagedMat(list[i]);
                        GenDraw.DrawMeshNowOrLater(mesh, loc, quat, damagedMat, portrait);
                        loc.y += 0.005f;
                    }
                    if (bodyDrawType == RotDrawMode.Fresh)
                    {
                        Vector3 drawLoc = rootLoc;
                        drawLoc.y += 0.02f;
                        renderer.GetFieldViaReflection<PawnWoundDrawer>("woundOverlays").RenderOverBody(drawLoc, mesh, quat, portrait);
                    }
                }
            }
            Vector3 loc2 = rootLoc;
            Vector3 a = rootLoc;
            if (bodyFacing != Rot4.North)
            {
                a.y += 0.03f;
                loc2.y += 0.0249999985f;
            }
            else
            {
                a.y += 0.0249999985f;
                loc2.y += 0.03f;
            }
            if (renderer.graphics.headGraphic != null)
            {
                Vector3 b = quat * renderer.BaseHeadOffsetAt(headFacing);
                Mesh mesh2 = MeshPool.humanlikeHeadSet.MeshAt(headFacing);
                Material mat = renderer.graphics.HeadMatAt(headFacing, bodyDrawType);
                GenDraw.DrawMeshNowOrLater(mesh2, a + b, quat, mat, portrait);
                Vector3 loc3 = rootLoc + b;
                loc3.y += 0.035f;
                bool flag = false;
                if (!portrait)
                {
                    Mesh mesh3 = renderer.graphics.HairMeshSet.MeshAt(headFacing);
                    List<ApparelGraphicRecord> apparelGraphics = renderer.graphics.apparelGraphics;
                    for (int j = 0; j < apparelGraphics.Count; j++)
                    {
                        if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayer.Overhead)
                        {
                            flag = true;
                            Material material = apparelGraphics[j].graphic.MatAt(bodyFacing, null);
                            material = renderer.graphics.flasher.GetDamagedMat(material);
                            GenDraw.DrawMeshNowOrLater(mesh3, loc3, quat, material, portrait);
                        }
                    }
                }
                if (!flag && bodyDrawType != RotDrawMode.Dessicated)
                {
                    Mesh mesh4 = renderer.graphics.HairMeshSet.MeshAt(headFacing);
                    Material mat2 = renderer.graphics.HairMatAt(headFacing);
                    GenDraw.DrawMeshNowOrLater(mesh4, loc3, quat, mat2, portrait);
                }
            }
            if (renderBody)
            {
                for (int k = 0; k < renderer.graphics.apparelGraphics.Count; k++)
                {
                    ApparelGraphicRecord apparelGraphicRecord = renderer.graphics.apparelGraphics[k];
                    if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell)
                    {
                        Material material2 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                        material2 = renderer.graphics.flasher.GetDamagedMat(material2);
                        GenDraw.DrawMeshNowOrLater(mesh, loc2, quat, material2, portrait);
                    }
                }
            }
            if (!portrait && rendererPawn.RaceProps.Animal && rendererPawn.inventory != null && rendererPawn.inventory.innerContainer.Count > 0)
            {
                Graphics.DrawMesh(mesh, loc2, quat, renderer.graphics.packGraphic.MatAt(rendererPawn.Rotation, null), 0);
            }
            if (!portrait)
            {
                renderer.CallMethodViaReflection("DrawEquipment", rootLoc);
                if (rendererPawn.apparel != null)
                {
                    List<Apparel> wornApparel = rendererPawn.apparel.WornApparel;
                    for (int l = 0; l < wornApparel.Count; l++)
                    {
                        wornApparel[l].DrawWornExtras();
                    }
                }
                Vector3 bodyLoc = rootLoc;
                bodyLoc.y += 0.0449999981f;
                renderer.GetFieldViaReflection<PawnHeadOverlays>("statusOverlays").RenderStatusOverlays(bodyLoc, quat, MeshPool.humanlikeHeadSet.MeshAt(headFacing));
            }
        }
    }
}
