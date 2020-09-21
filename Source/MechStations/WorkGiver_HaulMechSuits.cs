using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace MuvLuvAnnihilation
{
	public class WorkGiver_HaulMechSuits : WorkGiver_Haul
	{
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return MechUtils.MechSuits(pawn.Map);
		}

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return MechUtils.MechSuits(pawn.Map).Count<Thing>() == 0;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!HaulAIUtility.PawnCanAutomaticallyHaulFast(pawn, t, forced) && t is MechSuit mechSuit && mechSuit.AssignedStation != null)
			{
				return null;
			}
			return HaulAIUtility.HaulToStorageJob(pawn, t);
		}
	}
}
