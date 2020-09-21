using RimWorld;
using System.Linq;
using Verse;

namespace MuvLuvAnnihilation
{
	public class MechStation : Building
	{
        public MechSuit assignedSuit;
        public override void Tick()
        {
            base.Tick();
            if (assignedSuit != null && assignedSuit.Position == this.InteractionCell)
            {
                Log.Message("TEST - " + assignedSuit, true);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<MechSuit>(ref assignedSuit, "assignedSuit");
        }
    }
}
