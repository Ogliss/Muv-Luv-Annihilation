using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;
using System;
using Verse.AI;
using System.Text;
using System.Linq;
using Verse.AI.Group;
using RimWorld.Planet;
using UnityEngine;

namespace MuvLuvBeta.HarmonyInstance
{
    
    // Gets Gizmos from Apparel's Comps
    [HarmonyPatch(typeof(Pawn), "Tick")]
    public static class MLB_Apparel_Tick_ApparelTurret_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn __instance)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.apparel == null)
            {
                return;
            }
            if (__instance.apparel.WornApparel.NullOrEmpty())
            {
                return;
            }
            for (int i = 0; i < __instance.apparel.WornApparel.Count; i++)
            {
                Apparel apparel = __instance.apparel.WornApparel[i];
                foreach (Comp_TurretGun comp in apparel.GetComps<Comp_TurretGun>())
                {
                    comp.CompTick();
                }
            }
        }
    }
    
}