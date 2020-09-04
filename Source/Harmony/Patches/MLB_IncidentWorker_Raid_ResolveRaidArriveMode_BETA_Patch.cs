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
    [HarmonyPatch(typeof(IncidentWorker_Raid), "ResolveRaidArriveMode")]
    public static class MLB_IncidentWorker_Raid_ResolveRaidArriveMode_BETA_Patch
    {
        [HarmonyPostfix] 
        public static void Postfix(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def.defName.Contains("MuvLuv_BETA")))
                {
                    parms.raidArrivalMode = RimWorld.PawnsArrivalModeDefOf.EdgeWalkIn;
                    return;
                    /*
                    if ((parms.target is Map map))
                    {
                	//        Log.Message("raidArrivalMode " + parms.raidArrivalMode);
                        if (parms.raidArrivalMode?.minTechLevel >= TechLevel.Industrial)
                        {
                            bool tunnelchance = Rand.Chance(0.5f);
                            if (tunnelchance)
                            {
                               
                            }
                            if (parms.raidArrivalMode == RimWorld.PawnsArrivalModeDefOf.CenterDrop)
                            {
                                parms.raidArrivalMode = tunnelchance ? ExtraHives.PawnsArrivalModeDefOf.CenterTunnelIn_ExtraHives : RimWorld.PawnsArrivalModeDefOf.EdgeWalkIn;
                            }
                            if (parms.raidArrivalMode == RimWorld.PawnsArrivalModeDefOf.EdgeDrop)
                            {
                                parms.raidArrivalMode = tunnelchance ? ExtraHives.PawnsArrivalModeDefOf.EdgeTunnelIn_ExtraHives : RimWorld.PawnsArrivalModeDefOf.EdgeWalkIn;
                            }
                            if (parms.raidArrivalMode == ExtraHives.PawnsArrivalModeDefOf.EdgeDropGroups)
                            {
                                parms.raidArrivalMode = tunnelchance ? ExtraHives.PawnsArrivalModeDefOf.EdgeTunnelInGroups_ExtraHives : ExtraHives.PawnsArrivalModeDefOf.EdgeWalkInGroups;
                            }
                            if (parms.raidArrivalMode == RimWorld.PawnsArrivalModeDefOf.RandomDrop)
                            {
                                parms.raidArrivalMode = tunnelchance ? ExtraHives.PawnsArrivalModeDefOf.RandomTunnelIn_ExtraHives : ExtraHives.PawnsArrivalModeDefOf.EdgeWalkInGroups;
                            }
                            if (parms.raidArrivalMode.minTechLevel >= TechLevel.Industrial)
                            {
                                parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                            }
                    	//        Log.Message("raidArrivalMode now " + parms.raidArrivalMode);
                        }
                    }
                    */
                }
            }
        }
    }
}