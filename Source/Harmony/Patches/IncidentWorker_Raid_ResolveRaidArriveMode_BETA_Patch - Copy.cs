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
	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public static class Patch_GetGizmos
	{
        public static void Postfix(Pawn __instance, ref IEnumerable<Gizmo> __result)
        {
            var comp = Current.Game.GetComponent<MechOwnership>();
            Log.Message("comp.mechOwnerships: " + comp.mechOwnerships.Count, true);
            foreach (var p in comp.mechOwnerships)
            {
                Log.Message(p.Key + " - " + p.Value, true);
            }
            if (comp.mechOwnerships != null && comp.mechOwnerships.ContainsKey(__instance) && comp.mechOwnerships[__instance] != null)
            {
                Log.Message(" - Postfix - if (comp.mechOwnerships != null && comp.mechOwnerships.ContainsKey(__instance) && comp.mechOwnerships[__instance] != null) - 2", true);

                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "MUV_GoToMech".Translate(),
                    defaultDesc = "MUV_GoToMechDesc".Translate(),
                    action = delegate ()
                    {
                        Log.Message("comp.mechOwnerships[__instance]:" + comp.mechOwnerships[__instance], true);
                        var job = JobMaker.MakeJob(JobDefOf.Wear, comp.mechOwnerships[__instance]);
                        __instance.jobs.TryTakeOrderedJob(job);
                    },
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/LaunchShip", true)
                };
                Log.Message(" - Postfix - 4", true);

                __result = new List<Gizmo>(__result)
                                {
                                        command_Action
                                };
            }
        }
	}
}