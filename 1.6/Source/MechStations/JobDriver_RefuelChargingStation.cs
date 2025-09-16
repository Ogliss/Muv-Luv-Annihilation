using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	public class JobDriver_RefuelChargingStation : JobDriver
	{
		private const TargetIndex RefuelableInd = TargetIndex.A;

		private const TargetIndex FuelInd = TargetIndex.B;

		private const int RefuelingDuration = 240;

		protected Thing Refuelable => job.GetTarget(TargetIndex.A).Thing;

		private CompRefuelableMulti refuelableComp = null;
		protected CompRefuelableMulti RefuelableComp
		{
			get
			{
				if (refuelableComp == null)
                {
					if (Refuelable.TryGetComp<CompRefuelableSteel>().FuelFilter.Allows(Fuel) && !Refuelable.TryGetComp<CompRefuelableSteel>().IsFull)
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableSteel>();
					}
					else if (Refuelable.TryGetComp<CompRefuelableChemfuel>().FuelFilter.Allows(Fuel) && !Refuelable.TryGetComp<CompRefuelableChemfuel>().IsFull)
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableChemfuel>();
					}
					else if (Refuelable.TryGetComp<CompRefuelableAmmoFirst>().FuelFilter.Allows(Fuel) && !Refuelable.TryGetComp<CompRefuelableAmmoFirst>().IsFull)
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableAmmoFirst>();
					}
					else if (Refuelable.TryGetComp<CompRefuelableAmmoSecond>().FuelFilter.Allows(Fuel) && !Refuelable.TryGetComp<CompRefuelableAmmoSecond>().IsFull)
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableAmmoSecond>();
					}
				}
				return refuelableComp;
			}
		}

		protected Thing Fuel => job.GetTarget(TargetIndex.B).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(Refuelable, job, 1, -1, null, errorOnFailed))
			{
				return pawn.Reserve(Fuel, job, 1, -1, null, errorOnFailed);
			}
			return false;
		}

        public override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			AddEndCondition(() => (!RefuelableComp.IsFull) ? JobCondition.Ongoing : JobCondition.Succeeded);
			AddFailCondition(() => !job.playerForced && !RefuelableComp.ShouldAutoRefuelNowIgnoringFuelPct);
			AddFailCondition(() => !RefuelableComp.allowAutoRefuel && !job.playerForced);
			yield return Toils_General.DoAtomic(delegate
			{
				job.count = RefuelableComp.GetFuelCountToFullyRefuel();
			});
			Toil reserveFuel = Toils_Reserve.Reserve(TargetIndex.B);
			yield return reserveFuel;
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Haul.CheckForGetOpportunityDuplicate(reserveFuel, TargetIndex.B, TargetIndex.None, takeFromValidStorage: true);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden(TargetIndex.B).FailOnDestroyedNullOrForbidden(TargetIndex.A)
				.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
				.WithProgressBarToilDelay(TargetIndex.A);
			yield return FinalizeRefueling(TargetIndex.A, TargetIndex.B, RefuelableComp);
		}

		public static Toil FinalizeRefueling(TargetIndex refuelableInd, TargetIndex fuelInd, CompRefuelableMulti RefuelableComp)
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Job curJob = toil.actor.CurJob;
				Thing thing = curJob.GetTarget(refuelableInd).Thing;
				if (toil.actor.CurJob.placedThings.NullOrEmpty())
				{
					RefuelableComp.Refuel(new List<Thing>
										{
												curJob.GetTarget(fuelInd).Thing
										});
				}
				else
				{
					RefuelableComp.Refuel(toil.actor.CurJob.placedThings.Select((ThingCountClass p) => p.thing).ToList());
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			return toil;
		}
	}
}
