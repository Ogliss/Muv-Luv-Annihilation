using ExtraHives;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(ExtraHives.TunnelHiveSpawner), "MakeLord")]
    public static class MLB_TunnelRaidSpawner_MakeLord_BETA_Patch
    {
        static void Postfix(ExtraHives.TunnelHiveSpawner __instance, Type lordJobType, List<Pawn> list)
        {
            if (__instance.SpawnedFaction != null)
            {
                if (__instance.SpawnedFaction.def.defName.Contains("MuvLuv_BETA"))
                {
                //    Log.Message("BETA from " + __instance.SpawnedFaction.Name + " spawning with "+ lordJobType.Name+" Lord");
                }
            }
        }
    }
}
