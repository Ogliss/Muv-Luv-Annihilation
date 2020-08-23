using System;
using System.Linq;
using Verse;
using HarmonyLib;
using UnityEngine;
using RimWorld;
using System.Reflection;
using System.Globalization;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) })]
    public static class MLB_PawnRenderer_RenderPawnInternal_DrawExtras_Patch
    {
        [HarmonyPostfix]
        public static void PawnRenderer_RenderPawnInternal_Postfix(ref PawnRenderer __instance, Pawn ___pawn, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            if (!__instance.graphics.AllResolved)
            {
                __instance.graphics.ResolveAllGraphics();
            }
            Mesh mesh = null;
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);
            /*
            if (AvPIntergrationUtil.enabled_AlienRaces)
            {
                AvP_PawnRenderer_RenderPawnInternal_DrawExtras_Patch.AlienRacesPatch(ref __instance, ___pawn, rootLoc, angle, renderBody, bodyFacing, headFacing, out mesh, bodyDrawType, portrait, headStump, invisible);
            }
            else
            */
            {
                if (___pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
            if (renderBody)
            {
                Vector3 vector = rootLoc;

                if (___pawn.apparel != null && ___pawn.apparel.WornApparelCount > 0)
                {
                    for (int k = 0; k < ___pawn.apparel.WornApparel.Count; k++)
                    {
                        if (___pawn.apparel.WornApparel[k].TryGetComp<MuvLuvBeta.CompApparelDrawExtras>() != null)
                        {
                            foreach (MuvLuvBeta.CompApparelDrawExtras Extas in ___pawn.apparel.WornApparel[k].AllComps.Where(x => x.GetType() == typeof(MuvLuvBeta.CompApparelDrawExtras)))
                            {
                                if (!Extas.pprops.ExtrasEntries.NullOrEmpty())
                                {
                                    if (Extas.ShouldDrawExtra(___pawn, ___pawn.apparel.WornApparel[k], bodyFacing, out Material extraMat))
                                    {
                                        Vector3 drawAt = vector;
                                        if (Extas.onHead)
                                        {
                                            drawAt = vector + __instance.BaseHeadOffsetAt(headFacing);
                                        }
                                        drawAt.y += Extas.GetAltitudeOffset(bodyFacing, Extas.ExtraPartEntry);
                                        extraMat = (Material)Traverse.Create(__instance).Method("OverrideMaterialIfNeeded", new object[] { extraMat, ___pawn }).GetValue();
                                        GenDraw.DrawMeshNowOrLater(mesh, drawAt, quat, extraMat, portrait);
                                        //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                    }
                                }
                            }
                        }
                        //ApparelGraphicRecord apparelGraphicRecord = __instance.graphics.apparelGraphics[k];
                        //if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell)
                        //{
                        //           Material material2 = apparelGraphicRecord.graphic.MatAt(bodyFacing, null);
                        //           material2 = __instance.graphics.flasher.GetDamagedMat(material2);
                        //           GenDraw.DrawMeshNowOrLater(mesh, vector, quat, material2, portrait);

                        //}
                    }
                }
            }
        }
        /*
        static void AlienRacesPatch(ref PawnRenderer __instance, Pawn pawn, Vector3 rootLoc, float angle, bool renderBody, Rot4 bodyFacing, Rot4 headFacing, out Mesh mesh, RotDrawMode bodyDrawType, bool portrait, bool headStump, bool invisible)
        {
            mesh = null;
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            if (alienDef != null)
            {
                Mesh mesh2;
                if (bodyDrawType == RotDrawMode.Rotting)
                {
                    if (__instance.graphics.dessicatedGraphic.ShouldDrawRotated)
                    {
                        mesh2 = MeshPool.GridPlane(portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize);
                    }
                    else
                    {
                        Vector2 size = portrait ? alienDef.alienRace.generalSettings.alienPartGenerator.customPortraitDrawSize : alienDef.alienRace.generalSettings.alienPartGenerator.customDrawSize;
                        if (bodyFacing.IsHorizontal)
                        {
                            size = size.Rotated();
                        }
                        if (bodyFacing == Rot4.West && (__instance.graphics.dessicatedGraphic.data == null || __instance.graphics.dessicatedGraphic.data.allowFlip))
                        {
                            mesh = MeshPool.GridPlaneFlip(size);
                        }
                        mesh = MeshPool.GridPlane(size);
                    }
                }
                else
                {
                    AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                    if (comp != null)
                    {
                        mesh = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing) : comp.alienGraphics.bodySet.MeshAt(bodyFacing));
                    }
                }
            }
            else
            {

                if (pawn.RaceProps.Humanlike)
                {
                    mesh = MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
                }
                else
                {
                    mesh = __instance.graphics.nakedGraphic.MeshAt(bodyFacing);
                }
            }
        }
    */
    }

}
