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
					if (Refuelable.TryGetComp<CompRefuelableSteel>().Props.fuelFilter.Allows(Fuel))
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableSteel>();
					}
					if (Refuelable.TryGetComp<CompRefuelableChemfuel>().Props.fuelFilter.Allows(Fuel))
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableChemfuel>();
					}
					if (Refuelable.TryGetComp<CompRefuelableAmmo>().Props.fuelFilter.Allows(Fuel))
					{
						refuelableComp = Refuelable.TryGetComp<CompRefuelableAmmo>();
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

		protected override IEnumerable<Toil> MakeNewToils()
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
			Log.Message(" - FinalizeRefueling - Toil toil = new Toil(); - 1", true);
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				Log.Message(" - FinalizeRefueling - Job curJob = toil.actor.CurJob; - 2", true);
				Job curJob = toil.actor.CurJob;
				Log.Message(" - FinalizeRefueling - Thing thing = curJob.GetTarget(refuelableInd).Thing; - 3", true);
				Thing thing = curJob.GetTarget(refuelableInd).Thing;
				Log.Message(" - FinalizeRefueling - if (toil.actor.CurJob.placedThings.NullOrEmpty()) - 4", true);
				if (toil.actor.CurJob.placedThings.NullOrEmpty())
				{
					Log.Message(" - FinalizeRefueling - RefuelableComp.Refuel(toil.actor.CurJob.placedThings.Select((ThingCountClass p) => p.thing).ToList()); - 5", true);

					RefuelableComp.Refuel(new List<Thing>
										{
												curJob.GetTarget(fuelInd).Thing
										});
				}
				else
				{
					Log.Message(" - FinalizeRefueling - RefuelableComp.Refuel(toil.actor.CurJob.placedThings.Select((ThingCountClass p) => p.thing).ToList()); - 6", true);
					RefuelableComp.Refuel(toil.actor.CurJob.placedThings.Select((ThingCountClass p) => p.thing).ToList());
				}
			};
			Log.Message(" - FinalizeRefueling - toil.defaultCompleteMode = ToilCompleteMode.Instant; - 8", true);
			toil.defaultCompleteMode = ToilCompleteMode.Instant;
			Log.Message(" - FinalizeRefueling - return toil; - 9", true);
			return toil;
		}
	}
}
