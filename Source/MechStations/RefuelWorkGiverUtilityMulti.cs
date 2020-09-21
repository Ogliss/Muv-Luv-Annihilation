using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	public static class RefuelWorkGiverUtilityMulti
	{
		public static bool CanRefuel(Pawn pawn, Thing t, bool forced = false)
        {
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableSteel>(), forced))
            {
				return true;
            }
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableChemfuel>(), forced))
			{
				return true;
			}
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableAmmoFirst>(), forced))
			{
				return true;
			}
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableAmmoSecond>(), forced))
			{
				return true;
			}
			return false;
        }
		public static bool CheckIfCanRefuel(Pawn pawn, Thing t, CompRefuelableMulti compRefuelable, bool forced = false)
		{
			if (compRefuelable == null || compRefuelable.IsFull || (!forced && !compRefuelable.allowAutoRefuel))
			{
				return false;
			}
			if (!forced && !compRefuelable.ShouldAutoRefuelNow)
			{
				return false;
			}
			if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 1, -1, null, forced))
			{
				return false;
			}
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			if (FindBestFuel(pawn, compRefuelable.FuelFilter) == null)
			{
				JobFailReason.Is("NoFuelToRefuel".Translate(compRefuelable.FuelFilter.Summary));
				return false;
			}
			if (t.TryGetComp<CompRefuelableMulti>().Props.atomicFueling && FindAllFuel(pawn, t, compRefuelable) == null)
			{
				JobFailReason.Is("NoFuelToRefuel".Translate(compRefuelable.FuelFilter.Summary));
				return false;
			}
			return true;
		}

		public static Job RefuelJob(Pawn pawn, Thing t, bool forced = false, JobDef customRefuelJob = null, JobDef customAtomicRefuelJob = null)
        {
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableSteel>(), forced))
			{
				return GiveRefuelJob(pawn, t, t.TryGetComp<CompRefuelableSteel>(), forced, customRefuelJob, customAtomicRefuelJob);
			}
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableChemfuel>(), forced))
			{
				return GiveRefuelJob(pawn, t, t.TryGetComp<CompRefuelableChemfuel>(), forced, customRefuelJob, customAtomicRefuelJob);
			}
			if (CheckIfCanRefuel(pawn, t, t.TryGetComp<CompRefuelableAmmoFirst>(), forced))
			{
				return GiveRefuelJob(pawn, t, t.TryGetComp<CompRefuelableAmmoSecond>(), forced, customRefuelJob, customAtomicRefuelJob);
			}
			return null;
		}
		public static Job GiveRefuelJob(Pawn pawn, Thing t, CompRefuelableMulti refuelable, bool forced = false, JobDef customRefuelJob = null, JobDef customAtomicRefuelJob = null)
		{
			if (!refuelable.Props.atomicFueling)
			{
				Thing t2 = FindBestFuel(pawn, refuelable.FuelFilter);
				return JobMaker.MakeJob(customRefuelJob ?? BETADefOf.BETA_RefuelChargingStation, t, t2);
			}
			List<Thing> source = FindAllFuel(pawn, t, refuelable);
			Job job = JobMaker.MakeJob(customAtomicRefuelJob ?? BETADefOf.BETA_RefuelAtomicChargingStation, t);
			job.targetQueueB = source.Select((Thing f) => new LocalTargetInfo(f)).ToList();
			return job;
		}

		private static Thing FindBestFuel(Pawn pawn, ThingFilter filter)
		{
			Predicate<Thing> validator = delegate (Thing x)
			{
				if (x.IsForbidden(pawn) || !pawn.CanReserve(x))
				{
					return false;
				}
				return filter.Allows(x) ? true : false;
			};
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, filter.BestThingRequest, PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, validator);
		}

		private static List<Thing> FindAllFuel(Pawn pawn, Thing refuelable, CompRefuelableMulti refuelableComp)
		{
			int fuelCountToFullyRefuel = refuelableComp.GetFuelCountToFullyRefuel();
			ThingFilter filter = refuelableComp.FuelFilter;
			return FindEnoughReservableThings(pawn, refuelable.Position, new IntRange(fuelCountToFullyRefuel, fuelCountToFullyRefuel), (Thing t) => filter.Allows(t));
		}

		public static List<Thing> FindEnoughReservableThings(Pawn pawn, IntVec3 rootCell, IntRange desiredQuantity, Predicate<Thing> validThing)
		{
			Predicate<Thing> validator = delegate (Thing x)
			{
				if (x.IsForbidden(pawn) || !pawn.CanReserve(x))
				{
					return false;
				}
				return validThing(x) ? true : false;
			};
			Region region2 = rootCell.GetRegion(pawn.Map);
			TraverseParms traverseParams = TraverseParms.For(pawn);
			RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParams, isDestination: false);
			List<Thing> chosenThings = new List<Thing>();
			int accumulatedQuantity = 0;
			ThingListProcessor(rootCell.GetThingList(region2.Map), region2);
			if (accumulatedQuantity < desiredQuantity.max)
			{
				RegionTraverser.BreadthFirstTraverse(region2, entryCondition, RegionProcessor, 99999);
			}
			if (accumulatedQuantity >= desiredQuantity.min)
			{
				return chosenThings;
			}
			return null;
			bool RegionProcessor(Region r)
			{
				List<Thing> things2 = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
				return ThingListProcessor(things2, r);
			}
			bool ThingListProcessor(List<Thing> things, Region region)
			{
				for (int i = 0; i < things.Count; i++)
				{
					Thing thing = things[i];
					if (validator(thing) && !chosenThings.Contains(thing) && ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, region, PathEndMode.ClosestTouch, pawn))
					{
						chosenThings.Add(thing);
						accumulatedQuantity += thing.stackCount;
						if (accumulatedQuantity >= desiredQuantity.max)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
	}
}
