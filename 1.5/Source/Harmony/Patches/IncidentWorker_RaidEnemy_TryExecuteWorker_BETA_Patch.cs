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
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy), "TryExecuteWorker")]
    public static class IncidentWorker_RaidEnemy_TryExecuteWorker_BETA_Patch
    {
        [HarmonyPrefix,HarmonyPriority(Priority.Last)]
        public static void Prefix(ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def.defName.Contains("MuvLuv_BETA")))
                {
                    if ((parms.target is Map map))
                    {
                        if (Rand.Chance(0.25f))
                        {
                            int strikedelay = Find.TickManager.TicksGame + IncidentWorker_RaidEnemy_TryExecuteWorker_BETA_Patch.RaidDelay.RandomInRange;
                         //   parms.points = parms.points / 2;
                            float hivepoints = parms.points;
                            if (Rand.Chance(0.25f))
                            {
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