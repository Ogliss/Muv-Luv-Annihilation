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
    
    [HarmonyPatch(typeof(Hediff), "CauseDeathNow")]
    public static class MLB_Hediff_CauseDeathNow_BETA_Patch
    {
        public static void Postfix(Hediff __instance, ref bool __result)
        {
            if (__instance.pawn.RaceProps.FleshType.defName == "BETAFlesh")
            {
                __result = false;
            }
        }
    }
    
}