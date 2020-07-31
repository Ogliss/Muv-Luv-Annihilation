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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class MLB_IncidentWorker_RaidEnemy_Xenomorph_TryExecuteWorker_Patch
    {
        [HarmonyPrefix]
        public static bool PreExecute(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def.defName.Contains("MuvLuv_BETA")))
                {
                    if ((parms.target is Map map))
                    {
                        if (parms.raidArrivalMode.minTechLevel >= TechLevel.Industrial)
                        {
                            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
                        }
                        if (parms.faction.def.defName.Contains("2P"))
                        {
                            parms.points = parms.points * 1.03f;
                        }
                        if (parms.faction.def.defName.Contains("3P"))
                        {
                            parms.points = parms.points * 1.06f;
                        }
                        if (parms.faction.def.defName.Contains("4P"))
                        {
                            parms.points = parms.points * 1.09f;
                        }
                        if (parms.faction.def.defName.Contains("5P"))
                        {
                            parms.points = parms.points * 1.12f;
                        }
                        if (parms.faction.def.defName.Contains("6P"))
                        {
                            parms.points = parms.points * 1.15f;
                        }

                    }
                }
            }
            return true;
        }
    }
}