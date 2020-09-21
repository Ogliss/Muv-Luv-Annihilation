using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	public class JobDriver_RefuelChargingStationAtomic : JobDriver
	{
		private const TargetIndex RefuelableInd = TargetIndex.A;

		private const TargetIndex FuelInd = TargetIndex.B;

		private const TargetIndex FuelPlaceCellInd = TargetIndex.C;

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
			pawn.ReserveAsManyAsPossible(job.GetTargetQueue(TargetIndex.B), job);
			return pawn.Reserve(Refuelable, job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			AddEndCondition(() => (!RefuelableComp.IsFull) ? JobCondition.Ongoing : JobCondition.Succeeded);
			AddFailCondition(() => (!job.playerForced && !RefuelableComp.ShouldAutoRefuelNowIgnoringFuelPct) || !RefuelableComp.allowAutoRefuel);
			AddFailCondition(() => !RefuelableComp.allowAutoRefuel && !job.playerForced);
			yield return Toils_General.DoAtomic(delegate
			{
				job.count = RefuelableComp.GetFuelCountToFullyRefuel();
			});
			Toil getNextIngredient = Toils_General.Label();
			yield return getNextIngredient;
			yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.B).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, putRemainderInQueue: false, subtractNumTakenFromJobCount: true).FailOnDestroyedNullOrForbidden(TargetIndex.B);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil findPlaceTarget = Toils_JobTransforms.SetTargetToIngredientPlaceCell(TargetIndex.A, TargetIndex.B, TargetIndex.C);
			yield return findPlaceTarget;
			yield return Toils_Haul.PlaceHauledThingInCell(TargetIndex.C, findPlaceTarget, storageMode: false);
			yield return Toils_Jump.JumpIf(getNextIngredient, () => !job.GetTargetQueue(TargetIndex.B).NullOrEmpty());
			yield return Toils_General.Wait(240).FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
				.WithProgressBarToilDelay(TargetIndex.A);
			yield return FinalizeRefueling(TargetIndex.A, TargetIndex.None, RefuelableComp);
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
