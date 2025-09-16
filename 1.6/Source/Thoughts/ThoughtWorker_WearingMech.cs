using RimWorld;
using Verse;

namespace MuvLuvAnnihilation
{
	public class ThoughtWorker_WearingMech : ThoughtWorker
	{
        public override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.GetMechSuit() != null;
		}
	}
}
