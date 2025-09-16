using RimWorld;
using Verse;

namespace MuvLuvAnnihilation
{
	public class ThoughtWorker_EatingInMech : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.GetMechSuit() != null && p.CurJobDef == JobDefOf.Ingest;
		}
	}
}
