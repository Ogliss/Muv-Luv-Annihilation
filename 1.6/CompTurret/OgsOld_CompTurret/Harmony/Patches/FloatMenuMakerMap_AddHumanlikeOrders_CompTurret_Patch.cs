using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using System;

namespace OgsOld_CompTurret.HarmonyInstance 
{
	public class FloatMenuMakerMap_AddHumanlikeOrders_OgsOld_CompTurret_Patch : FloatMenuOptionProvider
	{
        public override bool Drafted => true;

        public override bool Undrafted => true;

        public override bool Multiselect => false;

        public override IEnumerable<FloatMenuOption> GetOptionsFor(Thing clickedThing, FloatMenuContext context)
        {
            List<FloatMenuOption> opts = new List<FloatMenuOption>();
            var pawn = context.FirstSelectedPawn;
            foreach (Pair<OgsOld_CompTurret, Thing> pair in OgsOld_CompTurretReloadableUtility.FindPotentiallyReloadableGear(pawn, new List<Thing> { clickedThing }))
            {
                OgsOld_CompTurretGun comp = pair.First as OgsOld_CompTurretGun;
                Thing second = pair.Second;
                string text4 = "Reload".Translate(comp.parent.Named("GEAR"), comp.AmmoDef.Named("AMMO")) + " (" + comp.LabelRemaining + ")";
                List<Thing> chosenAmmo;
                if (!pawn.CanReach(second, PathEndMode.ClosestTouch, Danger.Deadly, false, false, TraverseMode.ByPawn))
                {
                    opts.Add(new FloatMenuOption(text4 + ": " + "NoPath".Translate().CapitalizeFirst(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                else if (!comp.NeedsReload(true))
                {
                    opts.Add(new FloatMenuOption(text4 + ": " + "ReloadFull".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                else if ((chosenAmmo = OgsOld_CompTurretReloadableUtility.FindEnoughAmmo(pawn, second.Position, comp, true)) == null)
                {
                    opts.Add(new FloatMenuOption(text4 + ": " + "ReloadNotEnough".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null));
                }
                else
                {
                    Action action4 = delegate ()
                    {
                        pawn.jobs.TryTakeOrderedJob(JobGiver_ReloadOgsOld_CompTurret.MakeReloadJob(comp, chosenAmmo), JobTag.Misc);
                    };
                    opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text4, action4, MenuOptionPriority.Default, null, null, 0f, null, null), pawn, second, "ReservedBy"));
                }
            }
            return opts;
        }
    }
}
