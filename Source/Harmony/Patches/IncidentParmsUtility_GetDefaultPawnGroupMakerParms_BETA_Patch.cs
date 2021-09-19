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
    [HarmonyPatch(typeof(IncidentParmsUtility), "GetDefaultPawnGroupMakerParms")]
    public static class IncidentParmsUtility_GetDefaultPawnGroupMakerParms_BETA_Patch
    {
        [HarmonyPrefix] 
        public static void Prefix(ref PawnGroupKindDef groupKind, ref IncidentParms parms)
        {
            if (parms.target is Map && (parms.target as Map).IsPlayerHome)
            {
                if (parms.faction != null && (parms.faction.def.defName.Contains("MuvLuv_BETA")))
                {
                    if ((parms.target is Map map))
                    {
                	 //   Log.Message("groupKind " + groupKind+" for "+parms.faction);
                        if (parms.raidArrivalMode.defName.Contains("TunnelIn"))
                        {
                            groupKind = OgsOld_ExtraHives.PawnGroupKindDefOf.Tunneler_OgsOld_ExtraHives;
                        //    Log.Message("groupKind now " + groupKind);
                        }
                    }
                }
            }
        }
    }
}