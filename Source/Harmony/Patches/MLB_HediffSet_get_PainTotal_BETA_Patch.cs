using System;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(HediffSet), "get_PainTotal")]
    public static class MLB_HediffSet_get_PainTotal_BETA_Patch
    {
        [HarmonyPostfix]
        public static void get_PainTotal_ComPainKiller_Postfix(HediffSet __instance, ref float __result)
        {
            if (__instance.pawn != null)
            {
                if (__instance.pawn.RaceProps.FleshType.defName == ("BETAFlesh"))
                {
                    __result = 0f;
                }
            }
        }
    }

}
