using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using System;

namespace MuvLuvBeta.HarmonyInstance
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
	public static class FloatMenuMakerMap_AddHumanlikeOrders_CompReloadableDual_Patch
	{
        public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
        {

			IntVec3 c = IntVec3.FromVector3(clickPos);
			foreach (Pair<CompReloadableDual, Thing> pair in ReloadableDualUtility.FindPotentiallyReloadableGear(pawn, c.GetThingList(pawn.Map)))
			{
				CompReloadableDual comp = pair.First;
				Thing second = pair.Second;
				string text4 = "Reload".Translate(comp.parent.Named("GEAR"), comp.AmmoDefSecondry.Named("AMMO")) + " (" + comp.LabelRemainingSecondry + ")";
				List<Thing> chosenAmmo;
				if (!pawn.CanReach(second, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
				{
					opts.Add(new FloatMenuOption(text4 + ": " + "NoPath".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				else if (!comp.NeedsReload(true))
				{
					opts.Add(new FloatMenuOption(text4 + ": " + "ReloadFull".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				else if ((chosenAmmo = ReloadableDualUtility.FindEnoughAmmo(pawn, second.Position, comp, true)) == null)
				{
					opts.Add(new FloatMenuOption(text4 + ": " + "ReloadNotEnough".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
				}
				else
				{
					Action action4 = delegate ()
					{
						pawn.jobs.TryTakeOrderedJob(JobGiver_Reload.MakeReloadJob(comp, chosenAmmo), JobTag.Misc);
					};
					opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text4, action4, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, second, "ReservedBy"));
				}
			}
		}
    }

}
