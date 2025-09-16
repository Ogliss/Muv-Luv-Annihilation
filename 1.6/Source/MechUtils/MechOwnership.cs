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

		public override void LoadedGame()
		{
			base.LoadedGame();
            var faction = Find.FactionManager.FirstFactionOfDef(BETADefOf.MuvLuv_BETA);
            if (faction != null)
            {
                faction.hidden = true;
            }
        }

		public override void StartedNewGame()
		{
			base.StartedNewGame();
			var faction = Find.FactionManager.FirstFactionOfDef(BETADefOf.MuvLuv_BETA);
			if (faction != null)
			{
                faction.hidden = true;
            }
		}
		public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Collections.Look<Pawn, MechSuit>(ref mechOwnerships, "mechOwnerships", LookMode.Reference, LookMode.Reference);
		}

		public Dictionary<Pawn, MechSuit> mechOwnerships = new Dictionary<Pawn, MechSuit>();
    }
}
