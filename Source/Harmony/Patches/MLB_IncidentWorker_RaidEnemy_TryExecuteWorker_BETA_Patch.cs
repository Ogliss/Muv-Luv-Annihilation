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
    public static class MLB_IncidentWorker_RaidEnemy_TryExecuteWorker_BETA_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def.defName.Contains("MuvLuv_BETA")))
                {
                    if ((parms.target is Map map))
                    {
                        float mult = 1f;
                        if (parms.faction.def.defName.Contains("2P"))
                        {
                            mult = 1.2f;
                        }
                        if (parms.faction.def.defName.Contains("3P"))
                        {
                            mult = 1.4f;
                        }
                        if (parms.faction.def.defName.Contains("4P"))
                        {
                            mult = 1.6f;
                        }
                        if (parms.faction.def.defName.Contains("5P"))
                        {
                            mult = 1.8f;
                        }
                        if (parms.faction.def.defName.Contains("6P"))
                        {
                            mult = 2f;
                        }
                	//        Log.Message("IncidentWorker_RaidEnemy points: " + parms.points + " Mult: " + mult + " Result: " + parms.points * mult);
                        parms.points = parms.points * mult;

                        if (Rand.Chance(0.25f))
                        {
                            int strikedelay = Find.TickManager.TicksGame + MLB_IncidentWorker_RaidEnemy_TryExecuteWorker_BETA_Patch.RaidDelay.RandomInRange;
                            parms.points = parms.points / 2;
                            float hivepoints = parms.points;
                            if (Rand.Chance(0.25f))
                            {
                                hivepoints = hivepoints / 2;
                        	//        Log.Message("Tripple Threat strikes in " + strikedelay + " "+ hivepoints + " points each strikes in " + strikedelay);
                                int @int = Rand.Int;
                                IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
                                raidParms.forced = true;
                                raidParms.faction = parms.faction;
                                raidParms.points = hivepoints;
                                raidParms.pawnGroupMakerSeed = new int?(@int);

                                IncidentDef incidentDef1 = IncidentDefOf.BETAInfestationInterior;
                                IncidentDef incidentDef2 = IncidentDefOf.BETAInfestationExterior;
                                QueuedIncident qi1 = new QueuedIncident(new FiringIncident(incidentDef1, null, raidParms), strikedelay, 0);
                                strikedelay += 60;
                                QueuedIncident qi2 = new QueuedIncident(new FiringIncident(incidentDef2, null, raidParms), strikedelay, 0);
                                Find.Storyteller.incidentQueue.Add(qi1);
                                Find.Storyteller.incidentQueue.Add(qi2);
                            }
                            else
                            {

                                IncidentDef incidentDef = Rand.Chance(0.5f) ? IncidentDefOf.BETAInfestationInterior : IncidentDefOf.BETAInfestationExterior;
                        	//        Log.Message("Double Threat " + incidentDef.defName +" strikes in "+ strikedelay);
                                int @int = Rand.Int;
                                IncidentParms raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
                                raidParms.forced = true;
                                raidParms.faction = parms.faction;
                                raidParms.points = hivepoints;
                                raidParms.pawnGroupMakerSeed = new int?(@int);

                                QueuedIncident qi = new QueuedIncident(new FiringIncident(incidentDef, null, raidParms), strikedelay, 0);
                                Find.Storyteller.incidentQueue.Add(qi);
                            }

                        }

                    }
                }
            }
        }
        private static readonly IntRange RaidDelay = new IntRange(1000, 2000);
    }
}