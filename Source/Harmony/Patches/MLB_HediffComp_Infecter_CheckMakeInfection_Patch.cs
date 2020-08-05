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
    // Protects Cocooned Pawns from wound infections
    [HarmonyPatch(typeof(HediffComp_Infecter), "CheckMakeInfection")]
    public static class MLB_HediffComp_Infecter_CheckMakeInfection_Patch
    {
        [HarmonyPrefix]
        public static bool preCheckMakeInfection(HediffComp_Infecter __instance)
        {
            if (__instance.Pawn.RaceProps.FleshType.defName==("BETAFlesh"))
            {
                return false;
            }
            return true;
        }
    }

}