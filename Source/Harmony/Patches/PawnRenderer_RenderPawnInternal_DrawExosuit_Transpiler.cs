using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using HarmonyLib;
using Verse.Sound;
using System.Reflection.Emit;
using UnityEngine;
using System.Reflection;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnInternal", new Type[] { typeof(Vector3), typeof(float), typeof(bool), typeof(Rot4), typeof(Rot4), typeof(RotDrawMode), typeof(bool), typeof(bool), typeof(bool) }), HarmonyPriority(Priority.Last)]
    public static class PawnRenderer_RenderPawnInternal_DrawExosuit_Transpiler
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            List<CodeInstruction> instructionList = instructions.ToList();

            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[index: i];
                if (i > 1 && instructionList[index: i - 1].OperandIs(AccessTools.Method(type: typeof(Graphics), name: nameof(Graphics.DrawMesh), parameters: new[] { typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(Int32) })) && (i + 1) < instructionList.Count /* && instructionList[index: i + 1].opcode == OpCodes.Brtrue_S*/)
                {
                    yield return instruction; // portrait
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_1);
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                    yield return new CodeInstruction(opcode: OpCodes.Ldfld, operand: AccessTools.Field(type: typeof(PawnRenderer), name: "pawn"));
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_0);             // quat
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 4); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 9); //invisible
                    yield return new CodeInstruction(opcode: OpCodes.Ldloc_1);             // Mesh
                    yield return new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 5); // bodyfacing
                    yield return new CodeInstruction(opcode: OpCodes.Call, operand: typeof(PawnRenderer_RenderPawnInternal_DrawExosuit_Transpiler).GetMethod("DrawAddons"));

                    instruction = new CodeInstruction(opcode: OpCodes.Ldarg_S, operand: 7);
                }

                yield return instruction;
            }
        }

        public static void DrawAddons(bool portrait, Vector3 vector, Pawn pawn, Quaternion quat, Rot4 bodyFacing, bool invisible, Mesh mesh, Rot4 headfacing)
        {
            if (invisible) return;
            Vector2 size = mesh?.bounds.size ?? (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
            {
                foreach (var apparel in pawn.apparel.WornApparel)
                {
                    CompExosuitDrawer ExtraDrawer = apparel.TryGetComp<CompExosuitDrawer>();
                    if (ExtraDrawer != null)
                    {
                        foreach (CompExosuitDrawer Extas in apparel.AllComps.Where(x => x.GetType() == typeof(CompExosuitDrawer)))
                        {
                            Vector3 drawAt = vector;
                            if (!Extas.Props.ExtrasEntries.NullOrEmpty())
                            {
                                if (Extas.ShouldDrawExtra(pawn, apparel, bodyFacing, out Material extraMat))
                                {
                                    if (Extas.onHead)
                                    {
                                        drawAt = vector + quat * pawn.Drawer.renderer.BaseHeadOffsetAt(headfacing);
                                    }
                                    drawAt += quat * Extas.GetAltitudeOffset(bodyFacing, Extas.ExtraPartEntry);
                                    Matrix4x4 matrix = default(Matrix4x4);
                                    matrix.SetTRS(drawAt, quat, new Vector3(Extas.ExtraPartEntry.drawSize.x, 0, Extas.ExtraPartEntry.drawSize.y));
                                    Graphics.DrawMesh(mesh, matrix, extraMat, 1);
                                    //    vector.y += CompApparelExtaDrawer.MinClippingDistance;
                                }
                            }
                        }
                    }
                    /*
                    Comp_TurretGun compTurret = apparel.TryGetComp<Comp_TurretGun>();
                    if (compTurret != null)
                    {
                        foreach (Comp_TurretGun Extas in apparel.AllComps.Where(x => x.GetType() == typeof(Comp_TurretGun)))
                        {
                            Extas.PostDraw();
                        }
                    }
                    */
                }
            }

        }

        // Token: 0x06000F45 RID: 3909 RVA: 0x00057D14 File Offset: 0x00055F14
        private static Material OverrideMaterialIfNeeded(Material original, Pawn pawn)
        {
            Material baseMat = pawn.IsInvisible() ? InvisibilityMatPool.GetInvisibleMat(original) : original;
            return pawn.Drawer.renderer.graphics.flasher.GetDamagedMat(baseMat);
        }
        /*
        static void AlienRacesPatch(Pawn pawn, Rot4 bodyFacing, out Vector2 size, bool portrait)
        {
            AlienRace.ThingDef_AlienRace alienDef = pawn.def as AlienRace.ThingDef_AlienRace;
            Vector3 d;
            if (alienDef != null)
            {
                AlienRace.AlienPartGenerator.AlienComp comp = pawn.TryGetComp<AlienRace.AlienPartGenerator.AlienComp>();
                if (comp != null)
                {
                    d = (portrait ? comp.alienPortraitGraphics.bodySet.MeshAt(bodyFacing).bounds.size : comp.alienGraphics.bodySet.MeshAt(bodyFacing).bounds.size);

                    size = new Vector2(d.x, d.z);
                    return;
                }
            }
            d = (portrait ? MeshPool.humanlikeBodySet.MeshAt(bodyFacing).bounds.size : pawn.Drawer.renderer.graphics.nakedGraphic.MeshAt(bodyFacing).bounds.size);
            size = new Vector2(d.x*1.5f, d.z * 1.5f);
            return;
        }
        */

    }
}
