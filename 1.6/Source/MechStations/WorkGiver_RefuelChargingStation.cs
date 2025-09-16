using RimWorld;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	public class WorkGiver_RefuelChargingStation : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(BETADefOf.MLB_ChargingStation);

		public override PathEndMode PathEndMode => PathEndMode.Touch;

		public virtual JobDef JobStandard => BETADefOf.BETA_RefuelChargingStation;

		public virtual JobDef JobAtomic => BETADefOf.BETA_RefuelAtomicChargingStation;

		public virtual bool CanRefuelThing(Thing t)
		{
			return !(t is Building_Turret);
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (CanRefuelThing(t))
			{
				return RefuelWorkGiverUtilityMulti.CanRefuel(pawn, t, forced);
			}
			return false;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job job = RefuelWorkGiverUtilityMulti.RefuelJob(pawn, t, forced, JobStandard, JobAtomic);
			return job;
		}
	}
}
