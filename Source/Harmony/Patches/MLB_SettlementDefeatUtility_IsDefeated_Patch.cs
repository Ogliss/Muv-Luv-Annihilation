using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;

namespace MuvLuvBeta.HarmonyInstance
{
    // SettlementDefeatUtility.IsDefeated
    [HarmonyPatch(typeof(SettlementDefeatUtility), "IsDefeated")]
    public static class MLB_SettlementDefeatUtility_IsDefeated_Patch
    {
        [HarmonyPostfix]
        public static void IsDefeatedPostfix(Map map, Faction faction, ref bool __result)
        {
            if (__result)
            {
                if (faction.def.pawnGroupMakers.Any(x => x.options.Any(y => !y.kind.RaceProps.Humanlike)))
                {
                    List<Pawn> list = map.mapPawns.SpawnedPawnsInFaction(faction);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Pawn pawn = list[i];
                        if (GenHostility.IsActiveThreatToPlayer(pawn))
                        {
                            __result = false;
                        }
                    }
                }
                if (faction.def.HasModExtension<ExtraHives.HiveFactionExtension>())
                {
                    __result = map.listerBuildings.allBuildingsNonColonist.Any(x => x.def.HasModExtension<ExtraHives.HiveDefExtension>());
                }
            }
        }
    }
}