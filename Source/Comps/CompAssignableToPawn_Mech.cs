using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation
{
	public class CompAssignableToPawn_Mech : CompAssignableToPawn
	{
		public override IEnumerable<Pawn> AssigningCandidates
		{
			get
			{
				if (!parent.Spawned)
				{
					return Enumerable.Empty<Pawn>();
				}
				return from p in parent.Map.mapPawns.FreeColonists orderby CanAssignTo(p).Accepted descending select p;
			}
		}

		protected override string GetAssignmentGizmoDesc()
		{
			return "CommandMechSetOwnerDesc".Translate();
		}

		public override string CompInspectStringExtra()
		{
			if (base.AssignedPawnsForReading.Count == 0)
			{
				return "Owner".Translate() + ": " + "Nobody".Translate();
			}
			if (base.AssignedPawnsForReading.Count == 1)
			{
				return "Owner".Translate() + ": " + base.AssignedPawnsForReading[0].Label;
			}
			return "";
		}

        public override bool AssignedAnything(Pawn pawn)
        {
			if (Current.Game.GetComponent<MechOwnership>().mechOwnerships != null)
            {
				return Current.Game.GetComponent<MechOwnership>().mechOwnerships.ContainsKey(pawn);
            }
			return false;
		}

        protected override bool ShouldShowAssignmentGizmo()
        {
            return true;
        }
        public override void TryAssignPawn(Pawn pawn)
		{
			pawn.ClaimMech((MechSuit)parent);
		}

		public override void TryUnassignPawn(Pawn pawn, bool sort = true)
		{
			pawn.UnclaimMech((MechSuit)parent);
		}
	}
}
