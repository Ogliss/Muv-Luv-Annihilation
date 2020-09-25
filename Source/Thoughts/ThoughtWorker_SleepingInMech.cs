using RimWorld;
using Verse;

namespace MuvLuvAnnihilation
{
	public class ThoughtWorker_SleepingInMech : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			return p.GetMechSuit() != null && p.Awake() != true;
		}
	}
}
