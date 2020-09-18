using System.Collections.Generic;
using Verse;

namespace MuvLuvAnnihilation
{
	public class MechOwnership : GameComponent
	{
		public MechOwnership(Game game)
		{
			mechOwnerships = new Dictionary<Pawn, MechSuit>();
		}
		public MechOwnership()
		{
			mechOwnerships = new Dictionary<Pawn, MechSuit>();
		}

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Collections.Look<Pawn, MechSuit>(ref mechOwnerships, "mechOwnerships", LookMode.Reference, LookMode.Reference);
		}

		public Dictionary<Pawn, MechSuit> mechOwnerships = new Dictionary<Pawn, MechSuit>();
    }
}
