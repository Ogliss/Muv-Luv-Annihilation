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
    /*
    [HarmonyPatch(typeof(PawnGenOption), "get_Cost")]
    public static class MLB_PawnGenOption_get_Cost_BETA_Patch
    {
        public static void Postfix(PawnGenOption __instance, ref float __result)
        {
            if (__instance.kind.RaceProps.FleshType.defName == "BETAFlesh")
            {
                float yearsingame = ((float)Find.TickManager.TicksAbs) / ((float)360000);

                Log.Message(yearsingame+" years passed "+__instance.kind.LabelCap + " cost: "+ __result);
            }
        }
    }
    */
}