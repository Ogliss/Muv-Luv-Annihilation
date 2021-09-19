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

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(IncidentWorker_Raid), "ResolveRaidArriveMode")]
    public static class IncidentWorker_Raid_ResolveRaidArriveMode_BETA_Patch
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
                                parms.raidArrivalMode = tunnelchance ? OgsOld_ExtraHives.PawnsArrivalModeDefOf.CenterTunnelIn_OgsOld_ExtraHives : RimWorld.PawnsArrivalModeDefOf.EdgeWalkIn;
                            }
                            if (parms.raidArrivalMode == RimWorld.PawnsArrivalModeDefOf.EdgeDrop)
                            {
                                parms.raidArrivalMode = tunnelchance ? OgsOld_ExtraHives.PawnsArrivalModeDefOf.EdgeTunnelIn_OgsOld_ExtraHives : RimWorld.PawnsArrivalModeDefOf.EdgeWalkIn;
                            }
                            if (parms.raidArrivalMode == OgsOld_ExtraHives.PawnsArrivalModeDefOf.EdgeDropGroups)
                            {
                                parms.raidArrivalMode = tunnelchance ? OgsOld_ExtraHives.PawnsArrivalModeDefOf.EdgeTunnelInGroups_OgsOld_ExtraHives : OgsOld_ExtraHives.PawnsArrivalModeDefOf.EdgeWalkInGroups;
                            }
                            if (parms.raidArrivalMode == RimWorld.PawnsArrivalModeDefOf.RandomDrop)
                            {
                                parms.raidArrivalMode = tunnelchance ? OgsOld_ExtraHives.PawnsArrivalModeDefOf.RandomTunnelIn_OgsOld_ExtraHives : OgsOld_ExtraHives.PawnsArrivalModeDefOf.EdgeWalkInGroups;
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