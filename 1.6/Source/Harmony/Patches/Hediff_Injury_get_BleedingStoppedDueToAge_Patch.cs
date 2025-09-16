using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation.HarmonyInstance
{

    [HarmonyPatch(typeof(Hediff_Injury), "get_BleedingStoppedDueToAge")]
    public static class Hediff_Injury_get_BleedingStoppedDueToAge_Patch
    {
        [HarmonyPrefix]
        static void Prefix(ref Hediff_Injury __instance, ref bool __result)
        {
            if (__instance.pawn.def.race.FleshType.defName == "BETAFlesh")
            {
                __result =__instance.ageTicks >= AgeTicksToStopBleeding(__instance) / 3;
            }
        }

        private static int AgeTicksToStopBleeding(Hediff_Injury __instance)
        {
            int num = 90000;
            float t = Mathf.Clamp(Mathf.InverseLerp(1f, 30f, __instance.Severity), 0f, 1f);
            return num + Mathf.RoundToInt(Mathf.Lerp(0f, 90000f, t));
        }
    }
}
