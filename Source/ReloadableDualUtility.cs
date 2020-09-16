using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	// Token: 0x02000DC9 RID: 3529
	public static class ReloadableDualUtility
	{
		// Token: 0x06005676 RID: 22134 RVA: 0x001CE77C File Offset: 0x001CC97C
		public static CompReloadableDual FindSomeReloadableComponent(Pawn pawn, bool allowForcedReload)
		{
			if (pawn.apparel == null)
			{
				return null;
			}
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				CompReloadableDual compReloadable = wornApparel[i].TryGetComp<CompReloadableDual>();
				if (compReloadable != null && compReloadable.NeedsReload(allowForcedReload))
				{
					return compReloadable;
				}
			}
			return null;
		}

		// Token: 0x06005677 RID: 22135 RVA: 0x001CE7CC File Offset: 0x001CC9CC
		public static List<Thing> FindEnoughAmmo(Pawn pawn, IntVec3 rootCell, CompReloadableDual comp, bool forceReload)
		{
			if (comp == null)
			{
				return null;
			}
			IntRange desiredQuantity = new IntRange(comp.MinAmmoNeededSecondry(forceReload), comp.MaxAmmoNeededSecondry(forceReload));
			return RefuelWorkGiverUtility.FindEnoughReservableThings(pawn, rootCell, desiredQuantity, (Thing t) => t.def == comp.AmmoDefSecondry);
		}

		// Token: 0x06005678 RID: 22136 RVA: 0x001CE823 File Offset: 0x001CCA23
		public static IEnumerable<Pair<CompReloadableDual, Thing>> FindPotentiallyReloadableGear(Pawn pawn, List<Thing> potentialAmmo)
		{
			if (pawn.apparel == null)
			{
				yield break;
			}
			List<Apparel> worn = pawn.apparel.WornApparel;
			int num;
			for (int i = 0; i < worn.Count; i = num + 1)
			{
				CompReloadableDual comp = worn[i].TryGetComp<CompReloadableDual>();
				CompReloadableDual compReloadable = comp;
				if (((compReloadable != null) ? compReloadable.AmmoDefSecondry : null) != null)
				{
					for (int j = 0; j < potentialAmmo.Count; j = num + 1)
					{
						Thing thing = potentialAmmo[j];
						if (thing.def == comp.Props.ammoDefSecondry)
						{
							yield return new Pair<CompReloadableDual, Thing>(comp, thing);
						}
						num = j;
					}
					comp = null;
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06005679 RID: 22137 RVA: 0x001CE83C File Offset: 0x001CCA3C
		public static Pawn WearerOf(CompReloadableDual comp)
		{
			Pawn_ApparelTracker pawn_ApparelTracker = comp.ParentHolder as Pawn_ApparelTracker;
			if (pawn_ApparelTracker != null)
			{
				return pawn_ApparelTracker.pawn;
			}
			return null;
		}

		// Token: 0x0600567A RID: 22138 RVA: 0x001CE860 File Offset: 0x001CCA60
		public static int TotalChargesFromQueuedJobs(Pawn pawn, ThingWithComps gear)
		{
			CompReloadableDual compReloadable = gear.TryGetComp<CompReloadableDual>();
			int num = 0;
			if (compReloadable != null && pawn != null)
			{
				foreach (Job job in pawn.jobs.AllJobs())
				{
					Verb verbToUse = job.verbToUse;
					if (verbToUse != null && compReloadable == verbToUse.ReloadableCompSource)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600567B RID: 22139 RVA: 0x001CE8D0 File Offset: 0x001CCAD0
		public static bool CanUseConsideringQueuedJobs(Pawn pawn, ThingWithComps gear, bool showMessage = true)
		{
			CompReloadableDual compReloadable = gear.TryGetComp<CompReloadableDual>();
			if (compReloadable == null)
			{
				return true;
			}
			string text = null;
			if (!Event.current.shift)
			{
				if (!compReloadable.CanBeUsed)
				{
					text = compReloadable.DisabledReason(compReloadable.MinAmmoNeededSecondry(false), compReloadable.MaxAmmoNeededSecondry(false));
				}
			}
			else if (ReloadableDualUtility.TotalChargesFromQueuedJobs(pawn, gear) + 1 > compReloadable.RemainingCharges)
			{
				text = compReloadable.DisabledReason(compReloadable.MaxAmmoAmountSecondry(), compReloadable.MaxAmmoAmountSecondry());
			}
			if (text != null)
			{
				if (showMessage)
				{
					Messages.Message(text, pawn, MessageTypeDefOf.RejectInput, false);
				}
				return false;
			}
			return true;
		}
	}
}
