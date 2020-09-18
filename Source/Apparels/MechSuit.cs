using RimWorld;
using Verse;

namespace MuvLuvAnnihilation
{
	public class MechSuit : Apparel
	{
		public CompAssignableToPawn_Mech CompAssignableToPawn => GetComp<CompAssignableToPawn_Mech>();
		public Pawn AssignedPawn
		{
			get
			{
				if (CompAssignableToPawn == null || !CompAssignableToPawn.AssignedPawnsForReading.Any())
				{
					return null;
				}
				return CompAssignableToPawn.AssignedPawnsForReading[0];
			}
		}
	}
}
