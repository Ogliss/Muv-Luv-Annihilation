using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MuvLuvBeta
{
	// Token: 0x02000DC9 RID: 3529
	public static class CompTurretReloadableUtility
	{
		// Token: 0x06005676 RID: 22134 RVA: 0x001CE77C File Offset: 0x001CC97C
		public static Comp_Turret FindSomeReloadableComponent(Pawn pawn, bool allowForcedReload)
		{
			if (pawn.apparel == null)
			{
				return null;
			}
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				for (int i2 = 0; i2 < wornApparel[i].AllComps.Count; i2++)
				{
					Comp_Turret compReloadable = wornApparel[i].AllComps[i2] as Comp_Turret;
					if (compReloadable != null && compReloadable.NeedsReload(allowForcedReload))
					{
						return compReloadable;
					}
				}
			}
			return null;
		}
		
		public static IEnumerable<Comp_Turret> FindSomeReloadableComponents(Pawn pawn, bool allowForcedReload)
		{
			if (pawn.apparel == null)
			{
				yield break;
			}
			List<Apparel> wornApparel = pawn.apparel.WornApparel;
			for (int i = 0; i < wornApparel.Count; i++)
			{
				for (int i2 = 0; i2 < wornApparel[i].AllComps.Count; i2++)
				{
					Comp_Turret compReloadable = wornApparel[i].AllComps[i2] as Comp_Turret;
					if (compReloadable != null && compReloadable.NeedsReload(allowForcedReload))
					{
						yield return compReloadable;
					}
				}
			}
		}

		// Token: 0x06005677 RID: 22135 RVA: 0x001CE7CC File Offset: 0x001CC9CC
		public static List<Thing> FindEnoughAmmo(Pawn pawn, IntVec3 rootCell, Comp_Turret comp, bool forceReload)
		{
			if (comp == null)
			{
				return null;
			}
			IntRange desiredQuantity = new IntRange(comp.MinAmmoNeeded(forceReload), comp.MaxAmmoNeeded(forceReload));
			return RefuelWorkGiverUtility.FindEnoughReservableThings(pawn, rootCell, desiredQuantity, (Thing t) => t.def == comp.AmmoDef);
		}

		// Token: 0x06005678 RID: 22136 RVA: 0x001CE823 File Offset: 0x001CCA23
		public static IEnumerable<Pair<Comp_Turret, Thing>> FindPotentiallyReloadableGear(Pawn pawn, List<Thing> potentialAmmo)
		{
			if (pawn.apparel == null)
			{
				yield break;
			}
			List<Apparel> worn = pawn.apparel.WornApparel;
			int num;
			for (int i = 0; i < worn.Count; i = num + 1)
			{
				Comp_Turret comp = worn[i].TryGetComp<Comp_Turret>();
				Comp_Turret compReloadable = comp;
				if (((compReloadable != null) ? compReloadable.AmmoDef : null) != null)
				{
					for (int i2 = 0; i2 < worn[i].AllComps.Count; i2++)
					{
						compReloadable = worn[i].AllComps[i2] as Comp_Turret;
						if (((compReloadable != null) ? compReloadable.AmmoDef : null) != null)
						{
							for (int j = 0; j < potentialAmmo.Count; j = num + 1)
							{
								Thing thing = potentialAmmo[j];
								if (thing.def == compReloadable.Props.ammoDef)
								{
									yield return new Pair<Comp_Turret, Thing>(compReloadable, thing);
								}
								num = j;
							}
						}

					}
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x06005679 RID: 22137 RVA: 0x001CE83C File Offset: 0x001CCA3C
		public static Pawn WearerOf(Comp_Turret comp)
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
			Comp_Turret compReloadable = gear.TryGetComp<Comp_Turret>();
			int num = 0;
			if (compReloadable != null && pawn != null)
			{
				foreach (Job job in pawn.jobs.AllJobs())
				{
					Verb_ShootCompMounted verbToUse = job.verbToUse as Verb_ShootCompMounted;
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
			Comp_Turret compReloadable = gear.TryGetComp<Comp_Turret>();
			if (compReloadable == null)
			{
				return true;
			}
			string text = null;
			if (!Event.current.shift)
			{
				if (!compReloadable.CanBeUsed)
				{
					text = compReloadable.DisabledReason(compReloadable.MinAmmoNeeded(false), compReloadable.MaxAmmoNeeded(false));
				}
			}
			else if (CompTurretReloadableUtility.TotalChargesFromQueuedJobs(pawn, gear) + 1 > compReloadable.RemainingCharges)
			{
				text = compReloadable.DisabledReason(compReloadable.MaxAmmoAmount(), compReloadable.MaxAmmoAmount());
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
