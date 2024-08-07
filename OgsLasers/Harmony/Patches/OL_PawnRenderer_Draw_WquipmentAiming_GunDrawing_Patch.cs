﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace OgsLasers
{

    [HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming"), StaticConstructorOnStartup]
    public static class OL_PawnRenderer_Draw_WquipmentAiming_GunDrawing_Patch
    {
        [HarmonyPrefix, HarmonyPriority(Priority.First)]
        static void Prefix(Thing eq, ref Vector3 drawLoc, ref float aimAngle)
        {
            var ___pawn = (eq.ParentHolder as Pawn_EquipmentTracker)?.pawn;
            if (___pawn == null) return;

            IDrawnWeaponWithRotation gun = eq as IDrawnWeaponWithRotation;
            if (gun == null) return;

            Stance_Busy stance_Busy = ___pawn.stances.curStance as Stance_Busy;
            if (stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid)
            {
                drawLoc -= new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
                aimAngle = (aimAngle + gun.RotationOffset) % 360;
                drawLoc += new Vector3(0f, 0f, 0.4f).RotatedBy(aimAngle);
            }
        }
    }
}
