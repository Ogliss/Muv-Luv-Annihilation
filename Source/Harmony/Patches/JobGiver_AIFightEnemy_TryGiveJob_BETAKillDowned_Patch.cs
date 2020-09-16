using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(JobGiver_AIFightEnemy), "TryGiveJob")]
    internal static class JobGiver_AIFightEnemy_TryGiveJob_BETAKillDowned_Patch
    {
        static void Postfix(ref JobGiver_AIFightEnemy __instance, ref Job __result, Pawn pawn)
        {
            if (pawn.RaceProps.FleshType.defName.Contains("BETAFlesh") && __result !=null)
            {
                if (__result.def == JobDefOf.AttackMelee)
                {
                    __result.killIncappedTarget = true;
                }
            }
        }

    }
}
