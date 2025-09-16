using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using System;

namespace MuvLuvAnnihilation.HarmonyInstance
{
	public class FloatMenuMakerMap_AddHumanlikeOrders_CompReloadableDual_Patch : FloatMenuOptionProvider
	{
        public override bool Drafted => true;

        public override bool Undrafted => true;

        public override bool Multiselect => false;

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            var pawn = context.FirstSelectedPawn;
            foreach (Pair<CompReloadableDual, Thing> pair in ReloadableDualUtility.FindPotentiallyReloadableGear(pawn, new List<Thing>{ clickedThing }))
            {
                CompReloadableDual comp = pair.First;
                Thing second = pair.Second;
                string text4 = "Reload".Translate(comp.parent.Named("GEAR"), comp.AmmoDefSecondry.Named("AMMO")) + " (" + comp.LabelRemainingSecondry + ")";
                List<Thing> chosenAmmo;
                if (!pawn.CanReach(second, PathEndMode.ClosestTouch, Danger.Deadly, false))
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
            return opts;
        }
    }
}
