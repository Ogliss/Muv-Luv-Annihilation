using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(ThingListGroupHelper), "Includes")]
    internal static class ThingListGroupHelper_Includes_CompBETAProjectileInterceptor_Patch
    {
        static void Postfix(ThingRequestGroup group, ThingDef def, ref bool __result)
        {
            if (group == ThingRequestGroup.ProjectileInterceptor && !__result)
            {
                __result = def.HasComp(typeof(CompBETAProjectileInterceptor));
            }
        }

    }
}
