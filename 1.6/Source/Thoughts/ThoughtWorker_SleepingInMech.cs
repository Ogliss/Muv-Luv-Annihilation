using RimWorld;
using Verse;

namespace MuvLuvAnnihilation
{
	public class ThoughtWorker_SleepingInMech : ThoughtWorker
	{
        public override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.GetMechSuit() != null && p.Awake() != true;
		}
	}
}
